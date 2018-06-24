using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;

namespace BO1ZombiesAutosplitter
{
    /// <summary>
    /// Automatically split at round 1,2,3,4,5,15,30,50,70
    /// Note that the Black ops 1 window has to be on top for this to work
    /// Change below key codes to your livesplit bindings
    /// 
    /// I have only tested this on resolution 1600x900, this is probably not going to
    /// work on any other resolutions atm
    /// 
    /// TESTED SCENARIOS:
    /// Kino der toten 1-70
    /// </summary>
    class Program
    {
        static readonly VirtualKeyCode split_key = VirtualKeyCode.END;
        static readonly VirtualKeyCode reset_key = VirtualKeyCode.NUMPAD3;

        static readonly string BO1ProcessName = "BlackOps";

        static Process BO1Process;
        static Thread levelReadThread;

        static resolution[] resolutions;
        static resolution selectedResolution;

        static void Main(string[] args)
        {
            if (!GetProcess())
                ExitProgram();

            Points.InitializePoints(ref resolutions);

            if (!SelectResolution())
                Log("Your resolution is not supported", LogType.INFO);
            else
                Log("Found fitting resolution: " + selectedResolution.window_width + "x" + selectedResolution.window_height, LogType.INFO);

            levelReadThread = new Thread(new ThreadStart(ReadLevel));
            levelReadThread.Start();

            //var bmp = CaptureSceenshot(true);
            //bmp.Dispose();

            Console.Read();
        }

        static bool SelectResolution()
        {
            var rect = new User32.Rect();
            User32.GetWindowRect(BO1Process.MainWindowHandle, ref rect);

            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;

            for (int i = 0; i < resolutions.Length; i++)
            {
                if (resolutions[i].window_width == width && resolutions[i].window_height == height)
                {
                    selectedResolution = resolutions[i];
                    return true;
                }
            }

            return false;
        }

        static int currentLevel = 0;
        static int resetDelayTick = 0;
        static bool firstReset = true;
        private static void ReadLevel()
        {
            while (!BO1Process.HasExited)
            {
                resetDelayTick++;

                var bmpReset = CaptureSceenshotReset();
                if (CheckIfReset(bmpReset) && (resetDelayTick > 100 || firstReset))
                {
                    firstReset = false;
                    resetDelayTick = 0;
                    currentLevel = 1;
                    Log("RESET", LogType.INFO);
                    Log("ROUND: " + currentLevel.ToString(), LogType.INFO);

                    // reset here
                    InputSimulator s = new InputSimulator();
                    s.Keyboard.KeyPress(reset_key);
                    s.Keyboard.KeyPress(split_key);
                }
                bmpReset.Dispose();

                var bmp = CaptureSceenshot();
                int level = ReadLevelFromBmp(bmp);

                if (currentLevel < level && level != -1)
                {
                    currentLevel = level;
                    Log("ROUND: " + currentLevel.ToString(), LogType.INFO);

                    // split here
                    InputSimulator s = new InputSimulator();
                    s.Keyboard.KeyPress(split_key);
                }

                bmp.Dispose();

                Thread.Sleep(20);
            }

            Log("Black ops 1 has exited", LogType.WARNING);
        }

        private static bool CheckIfReset(Bitmap bmp)
        {
            Color clr = Color.FromArgb(214, 214, 214);

            bool matches = true;
            for (int i = 0; i < selectedResolution.reset_points.Length; i++)
            {
                var point = selectedResolution.reset_points[i];
                var pixel = bmp.GetPixel(point.X, point.Y);

                if (ColorsAreClose(pixel, clr, 25))
                {
                    if (!point.ShouldMatch)
                        matches = false;
                }
                else
                {
                    if (point.ShouldMatch)
                        matches = false;
                }
            }

            return matches;
        }

        private static int ReadLevelFromBmp(Bitmap bmp)
        {
            // #360F0C
            Color redColor = Color.FromArgb(174, 174, 174);
            //Color redColor = Color.FromArgb(54, 0, 0);

            for (int l = 0; l < selectedResolution.levels.Length; l++)
            {
                bool matches = true;
                for (int c = 0; c < selectedResolution.levels[l].pointsToMatch.Length; c++)
                {
                    var point = selectedResolution.levels[l].pointsToMatch[c];
                    var pixel = bmp.GetPixel(point.X, point.Y);

                    // change to 55 if not working correctly
                    if (!ColorsAreClose(redColor, pixel, 55))
                    {
                        if (point.ShouldMatch)
                            matches = false;
                    }
                    else
                    {
                        // if it matches but it shouldnt
                        if (!point.ShouldMatch)
                            matches = false;
                    }
                }

                if (matches)
                {
                    return selectedResolution.levels[l].lvl;
                }
            }

            return -1;
        }

        static bool ColorsAreClose(Color a, Color z, int threshold = 50)
        {
            int r = (int)a.R - z.R,
                g = (int)a.G - z.G,
                b = (int)a.B - z.B;
            return (r * r + g * g + b * b) <= threshold * threshold;
        }

        static bool GetProcess()
        {
            var processes = Process.GetProcessesByName(BO1ProcessName);

            if (processes.Length == 0)
            {
                Log("Black ops 1 is not running", LogType.ERROR);
                return false;
            }

            BO1Process = processes[0];
            Log("Succesfully found Black ops 1 process", LogType.SUCCESS);

            return true;
        }

        static void ExitProgram()
        {
            Console.Read();
        }

        static Bitmap CaptureSceenshot(bool save = false)
        {
            var rect = new User32.Rect();
            User32.GetWindowRect(BO1Process.MainWindowHandle, ref rect);

            int width = 250;
            int height = 120;

            var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(bmp);
            graphics.CopyFromScreen(rect.left, rect.bottom - height, 0, 0, new Size(width, height - 20), CopyPixelOperation.SourceCopy);

            graphics.Dispose();

            if (save)
            {
                bmp.Save(AppDomain.CurrentDomain.BaseDirectory + "tmp.png", ImageFormat.Png);
                Process.Start(AppDomain.CurrentDomain.BaseDirectory + "tmp.png");
            }

            return bmp;
        }

        static Bitmap CaptureSceenshotReset(bool save = false)
        {
            //723 341
            var rect = new User32.Rect();
            User32.GetWindowRect(BO1Process.MainWindowHandle, ref rect);

            int width = 160;
            int height = 60;

            var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(bmp);
            graphics.CopyFromScreen(rect.left + 723, rect.top + 341, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);

            graphics.Dispose();

            if (save)
            {
                bmp.Save(AppDomain.CurrentDomain.BaseDirectory + "tmp.png", ImageFormat.Png);
                Process.Start(AppDomain.CurrentDomain.BaseDirectory + "tmp.png");
            }

            return bmp;
        }

        public static void Log(string message, LogType type)
        {
            string date = "[" + DateTime.Now.ToLongTimeString() + "] ";
            Console.WriteLine(date + type.ToString() + " : " + message);
        }

        public enum LogType
        {
            SUCCESS,
            INFO,
            WARNING,
            ERROR,
        }

        private class User32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct Rect
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }

            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);
        }
    }

    public struct level
    {
        public int lvl;
        public point[] pointsToMatch;
    }

    public struct point
    {
        public int X;
        public int Y;
        public bool ShouldMatch;

        public point(int x, int y, bool shouldMatch = true)
        {
            this.X = x;
            this.Y = y;
            this.ShouldMatch = shouldMatch;
        }
    }

    public struct resolution
    {
        public level[] levels;
        public point[] reset_points;

        public int window_width;
        public int window_height;
    }
}
