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
    /// I have tested round 1 up to 70 on Kino
    /// </summary>
    class Program
    {
        static readonly VirtualKeyCode split_key = VirtualKeyCode.END;
        static readonly VirtualKeyCode reset_key = VirtualKeyCode.NUMPAD3;

        static readonly string BO1ProcessName = "BlackOps";

        static Process BO1Process;

        static Thread levelReadThread;

        static List<level> Levels = new List<level>();

        static void Main(string[] args)
        {
            if (!GetProcess())
                ExitProgram();

            InitLevelPoints();

            levelReadThread = new Thread(new ThreadStart(ReadLevel));
            levelReadThread.Start();

            //var bmp = CaptureSceenshot(true);
            //bmp.Dispose();

            Console.Read();
        }

        static void InitLevelPoints()
        {
            Levels.Add(
                new level()
                {
                    lvl = 70,
                    pointsToMatch = new point[]
                    {
                        // number seven
                       new point(7,4),
                       new point(20,4),
                       new point(41,4),
                       new point(40,11),
                       new point(36,17),
                       new point(29,29),
                       new point(25,38),
                       new point(21,47),
                       new point(17,62),
                       new point(16,77),

                         // number zero
                        new point(64,42),
                        new point(100,10),
                        new point(84,2),
                        new point(68,36),
                        new point(103,31),
                        new point(104,45),
                        new point(97,56),
                        new point(101,65),
                        new point(94,74),
                        new point(70,70),
                }
            });

            Levels.Add(
                new level()
                {
                    lvl = 50,
                    pointsToMatch = new point[]
                    {
                        // number five
                       new point(41,3),
                       new point(10,4),
                       new point(26,3),
                       new point(8,13),
                       new point(8,26),
                       new point(8,36),
                       new point(27,29),
                       new point(37,34),
                       new point(42,42),
                       new point(43,53),
                       new point(40,65),

                       new point(34,72),
                       new point(25,75),
                       new point(11,72),
                       new point(7,64),

                        // number zero
                        new point(64,42),
                        new point(100,10),
                        new point(84,2),
                        new point(68,36),
                        new point(103,31),
                        new point(104,45),
                        new point(97,56),
                        new point(101,65),
                        new point(94,74),
                        new point(70,70),
                }
                });

            Levels.Add(
                new level()
                {
                    lvl = 30,
                    pointsToMatch = new point[]
                    {
                        // number three
                        new point(8,14),
                        new point(14,3),
                        new point(40,10),
                        new point(37,28),
                        new point(22,36),
                        new point(37,42),
                        new point(43,55),
                        new point(40,69),
                        new point(27,74),
                        new point(12,73),
                        new point(7,64),

                        // number zero
                        new point(64,42),
                        new point(100,10),
                        new point(84,2),
                        new point(68,36),
                        new point(103,31),
                        new point(104,45),
                        new point(97,56),
                        new point(101,65),
                        new point(94,74),
                        new point(70,70),
                    }
                });

            Levels.Add(
                new level()
                {
                    lvl = 15,
                    pointsToMatch = new point[]
                    {
                        // number one
                        new point(30,2),
                        new point(7,14),
                        new point(28,76),

                        // number five
                        new point(100,3),                      
                        new point(84,76),
                        new point(103,53),
                        new point(86,28),
                        new point(68,36),
                    }
                });

            Levels.Add(
                new level()
                {
                lvl = 10,
                pointsToMatch = new point[]
                {
                        new point(98,22),
                        new point(9,64),
                        new point(145,60),
                        new point(169,51),
                        new point(192,41),
                        new point(225,22),
                        new point(135,64),

                        new point(106,85, false),
                        new point(76,17, false),
                        new point(106,85, false),
                        new point(28,79, false),
                        new point(28,44, false),
                        new point(50,12, false),
                        new point(75,39, false),
                        new point(80,84, false),
                        new point(101,12, false),
                        new point(237,62, false),
                    }
                });

            Levels.Add(
                new level()
                {
                    lvl = 5,
                    pointsToMatch = new point[]
                    {
                        new point(98,22),
                        new point(9,64),
                        new point(145,60, false),
                        new point(169,51, false),
                        new point(192,41, false),

                        new point(106,85, false),
                        new point(76,17, false),
                        new point(106,85, false),
                        new point(28,79, false),
                        new point(28,44, false),
                        new point(50,12, false),
                        new point(75,39, false),
                        new point(80,84, false),
                        new point(101,12, false),
                    }
                });


            Levels.Add(
              new level()
              {
                  lvl = 4,
                  pointsToMatch = new point[]
                  {
                        new point(90,19),
                        new point(93,87),
                        new point(98,22, false),
                        new point(9,64, false),

                        new point(106,85, false),
                        new point(76,17, false),
                        new point(106,85, false),
                        new point(28,79, false),
                        new point(28,44, false),
                        new point(50,12, false),
                        new point(75,39, false),
                        new point(80,84, false),
                        new point(101,12, false),
                  }
              });

           Levels.Add(
               new level()
               {
                   lvl = 3,
                   pointsToMatch = new point[]
                   {
                       new point(15,12),
                       new point(37,15),
                       new point(63,16),
                       new point(93,88, false),
                       new point(98,22, false),
                       new point(9,64, false),

                       new point(106,85, false),
                       new point(76,17, false),
                       new point(106,85, false),
                       new point(28,79, false),
                       new point(28,44, false),
                       new point(50,12, false),
                       new point(75,39, false),
                       new point(80,84, false),
                       new point(101,12, false),
                   }
               });

            Levels.Add(
                new level()
                {
                    lvl = 2,
                    pointsToMatch = new point[]
                    {
                        new point(14,13),
                        new point(36,14),
                        new point(38,29),
                        new point(63,16, false),
                        new point(93,88, false),
                        new point(98,22, false),
                        new point(9,64, false),

                        new point(106,85, false),
                        new point(76,17, false),
                        new point(106,85, false),
                        new point(28,79, false),
                        new point(28,44, false),
                        new point(50,12, false),
                        new point(75,39, false),
                        new point(80,84, false),
                        new point(101,12, false),
                    }
                });

            Levels.Add(
                new level()
                {
                    lvl = 1,
                    pointsToMatch = new point[] 
                    {
                        new point(15,10),
                        new point(38,29, false),
                        new point(36,14, false),
                        new point(93,88, false),
                        new point(98,22, false),
                        new point(9,64, false),

                        new point(106,85, false),
                        new point(76,17, false),
                        new point(106,85, false),
                        new point(28,79, false),
                        new point(28,44, false),
                        new point(50,12, false),
                        new point(75,39, false),
                        new point(80,84, false),
                        new point(101,12, false),
                    }
                });
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

            var px1 = bmp.GetPixel(12, 34);
            var px2 = bmp.GetPixel(150, 40);

            var px3 = bmp.GetPixel(0, 0);
            var px4 = bmp.GetPixel(0, 59);
            var px5 = bmp.GetPixel(159, 0);
            var px6 = bmp.GetPixel(159, 59);

            var px7 = bmp.GetPixel(159, 29);
            var px8 = bmp.GetPixel(0, 29);
            var px9 = bmp.GetPixel(79, 0);
            var px10 = bmp.GetPixel(39, 0);
            var px11 = bmp.GetPixel(129, 0);

            if (ColorsAreClose(px1, clr, 25) && ColorsAreClose(px2, clr, 25))
            {
                // dont reset on nuke :)
                if (!ColorsAreClose(px3, clr, 25) &&
                    !ColorsAreClose(px4, clr, 25) &&
                    !ColorsAreClose(px5, clr, 25) &&
                    !ColorsAreClose(px6, clr, 25) &&
                    !ColorsAreClose(px7, clr, 25) &&
                    !ColorsAreClose(px8, clr, 25) &&
                    !ColorsAreClose(px9, clr, 25) &&
                    !ColorsAreClose(px10, clr, 25) &&
                    !ColorsAreClose(px11, clr, 25))
                return true;
            }

            return false;
        }

        private static int ReadLevelFromBmp(Bitmap bmp)
        {
            // #360F0C
            Color redColor = Color.FromArgb(174, 174, 174);
            //Color redColor = Color.FromArgb(54, 0, 0);

            for (int l = 0; l < Levels.Count; l++)
            {
                bool matches = true;
                for (int c = 0; c < Levels[l].pointsToMatch.Length; c++)
                {
                    var point = Levels[l].pointsToMatch[c];
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
                    return Levels[l].lvl;
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

        struct level
        {
            public int lvl;
            public point[] pointsToMatch;
        }

        struct point
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
    }
}
