using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        static Thread inputThread;

        static List<Resolution> resolutions;
        static Resolution selectedResolution;
        static Map selectedMap = null;

        static string data_dir;

        static bool LookForCurrentMap = false;

        [STAThread]
        static void Main(string[] args)
        {
            if (!GetProcess())
                ExitProgram();

            data_dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "debug");
            if (!Directory.Exists(data_dir))
                Directory.CreateDirectory(data_dir);

            resolutions = Points.InitializePoints();

            if (!SelectResolution())
            {
                Log("Your resolution is not supported", LogType.INFO);
                ExitProgram();
            }
            else
                Log("Found fitting resolution: " + selectedResolution.window_width + "x" + selectedResolution.window_height, LogType.INFO);

            levelReadThread = new Thread(new ThreadStart(StartSplitter));
            levelReadThread.Start();

            inputThread = new Thread(new ThreadStart(ReadInput));
            inputThread.Start();
        }

        private static void ReadInput()
        {
            while (true)
            {
                string input = Console.ReadLine();

                string[] command = input.Split();

                try
                {
                    switch (command[0])
                    {
                        case "quit": ExitProgram(); break;
                        case "reload": resolutions = Points.InitializePoints(); Log("Reloaded data", LogType.INFO); break;
                        case "gen_reset": selectedMap.reset_points = PointGenerator.GenerateResetPoints(data_dir, selectedResolution); break;
                        case "lvl": currentLevel = int.Parse(command[1]); break;
                        case "save": SaveData(); break;
                        case "reset_rec": selectedMap.reset_rec = new Rectangle(int.Parse(command[1]), int.Parse(command[2]), int.Parse(command[3]), int.Parse(command[4])); break;
                        case "pic_reset": saveNext = true; break;
                        case "level_rec": selectedMap.level_rec = new Rectangle(int.Parse(command[1]), int.Parse(command[2]), int.Parse(command[3]), int.Parse(command[4])); break;
                        case "pic_level": saveNextLevel = true; break;
                        case "gen_level":
                            selectedMap.levels.Where(e => e.lvl == int.Parse(command[1])).First().pointsToMatch =
                            PointGenerator.GeterateLevelPoints(data_dir, selectedResolution, int.Parse(command[1])).ToArray(); break;
                    }
                }
                catch (Exception ex)
                {
                    Log(ex.Message, LogType.ERROR);
                }
            }
        }

        static void SaveData()
        {
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "data.json", JsonConvert.SerializeObject(resolutions, Formatting.Indented));
        }

        static bool SelectResolution()
        {
            var rect = new User32.Rect();
            User32.GetWindowRect(BO1Process.MainWindowHandle, ref rect);

            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;

            Log("Current resolution is: " + width.ToString() + "x" + height.ToString(), LogType.INFO);

            for (int i = 0; i < resolutions.Count; i++)
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
        static DebugForm level_form;
        static DebugForm reset_form;
        private static void StartSplitter()
        {
            while (!BO1Process.HasExited)
            {
                //try
                //{
                    resetDelayTick++;

                    if (selectedMap != null)
                    {
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

                        var bmp = CaptureLevelSceenshot();
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
                    }

                    if (IsInLoadingScreen() && !LookForCurrentMap)
                    {
                        LookForCurrentMap = true;
                        Log("Currently loading new map..", LogType.INFO);
                    }

                    if (LookForCurrentMap)
                    {
                        if (GetCurrentMap())
                        {
                            LookForCurrentMap = false;

                            Log($"Found map {selectedMap.map_name}", LogType.SUCCESS);

                            #if DEBUG
                            CreateDebugForms();
                            #endif
                        }
                        else
                        {
                            Log("Current map not supported for resolution", LogType.WARNING);
                        }
                    }

#if DEBUG
                    SetDebugWindowPosition();
#endif

                    Thread.Sleep(2);
                //}
                //catch (Exception ex)
                //{
                //    Log(ex.Message, LogType.ERROR);
                //}
            }

            Log("Black ops 1 has exited", LogType.WARNING);
        }

        private static bool GetCurrentMap()
        {
            //throw new NotImplementedException();
            return false;
        }

        private static bool IsInLoadingScreen()
        {
            var bmp = CaptureLoadingSceenshot();
            var colorToMatch = Color.Black;
            bool found = true;
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    var pixel = bmp.GetPixel(x, y);
                    if (!Utils.ColorsAreClose(pixel, colorToMatch, 10))
                    {
                        found = false;
                    }
                }
            }

            bmp.Dispose();

            return found;
        }

        private static void CreateDebugForms()
        {
            var rect = new User32.Rect();
            User32.GetWindowRect(BO1Process.MainWindowHandle, ref rect);

            Application.EnableVisualStyles();
            level_form = new DebugForm();

            level_form.Location = new Point(
                selectedMap.level_rec.X + rect.left,
                selectedMap.level_rec.Y + rect.top);

            level_form.Size = new Size(
                selectedMap.level_rec.Width,
                selectedMap.level_rec.Height);

            level_form.Show();

            reset_form = new DebugForm();

            reset_form.Location = new Point(
                selectedMap.reset_rec.X + rect.left,
                selectedMap.reset_rec.Y + rect.top);

            reset_form.Size = new Size(
                selectedMap.reset_rec.Width,
                selectedMap.reset_rec.Height);

            reset_form.Show();
        }

        private static void SetDebugWindowPosition()
        {
            if (level_form == null || reset_form == null) return;

            var rect = new User32.Rect();
            User32.GetWindowRect(BO1Process.MainWindowHandle, ref rect);

            level_form.Location = new Point(
             selectedMap.level_rec.X + rect.left,
             selectedMap.level_rec.Y + rect.top);

            level_form.Size = new Size(
                selectedMap.level_rec.Width,
                selectedMap.level_rec.Height);

            reset_form.Location = new Point(
                selectedMap.reset_rec.X + rect.left,
                selectedMap.reset_rec.Y + rect.top);

            reset_form.Size = new Size(
                selectedMap.reset_rec.Width,
                selectedMap.reset_rec.Height);

            Graphics g = level_form.CreateGraphics();
            level_form.Invalidate();
            g.DrawRectangle(new Pen(Color.Red, 2), 0, 0, level_form.Width, level_form.Height);

            Graphics gg = reset_form.CreateGraphics();
            reset_form.Invalidate();
            gg.DrawRectangle(new Pen(Color.Red, 2), 0, 0, reset_form.Width, reset_form.Height);

            var brushMatch = new SolidBrush(Color.Red);
            var brushNMatch = new SolidBrush(Color.Black);
            foreach (var point in selectedMap.reset_points)
            {
                if (point.ShouldMatch)
                    gg.FillRectangle(brushMatch, point.X, point.Y, 1, 1);
                else
                    gg.FillRectangle(brushNMatch, point.X, point.Y, 1, 1);
            }

            if (currentLevel != 0)
            {
                var crntLvl = selectedMap.levels.Where(e => e.lvl == currentLevel).First();
                foreach (var point in crntLvl.pointsToMatch)
                {
                    if (point.ShouldMatch)
                        g.FillRectangle(brushMatch, point.X, point.Y, 1, 1);
                    else
                        g.FillRectangle(brushNMatch, point.X, point.Y, 1, 1);
                }
            }

            brushMatch.Dispose();
            brushNMatch.Dispose();
            gg.Dispose();
            g.Dispose();
        }

        private static bool CheckIfReset(Bitmap bmp)
        {
            Color clr = Color.FromArgb(214, 214, 214);

            if (selectedMap.reset_points == null) return false;

            int goodMatches = 0;
            for (int i = 0; i < selectedMap.reset_points.Count; i++)
            {
                var point = selectedMap.reset_points[i];
                var pixel = bmp.GetPixel(point.X, point.Y);

                if (Utils.ColorsAreClose(pixel, clr, 25))
                {
                    if (point.ShouldMatch)
                        goodMatches++;
                }
                else
                {
                    if (!point.ShouldMatch)
                        goodMatches++;
                }
            }

            bool matches = (((float)goodMatches / (float)selectedMap.reset_points.Count) > 0.93f);

            return matches;
        }

        private static int ReadLevelFromBmp(Bitmap bmp)
        {
            // #360F0C
            Color redColor = Color.FromArgb(174, 174, 174);
            //Color redColor = Color.FromArgb(54, 0, 0);

            if (selectedMap.levels == null) return -1;

            for (int l = 0; l < selectedMap.levels.Count; l++)
            {
                bool matches = true;
                for (int c = 0; c < selectedMap.levels[l].pointsToMatch.Length; c++)
                {
                    var point = selectedMap.levels[l].pointsToMatch[c];
                    var pixel = bmp.GetPixel(point.X, point.Y);

                    // change to 55 if not working correctly
                    if (!Utils.ColorsAreClose(redColor, pixel, 55))
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
                    return selectedMap.levels[l].lvl;
                }
            }

            return -1;
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
            if (levelReadThread != null)
                levelReadThread.Abort();
            if (inputThread != null)
                inputThread.Abort();
            Console.Read();
            Environment.Exit(-1);
        }

        static bool saveNextLevel = false;
        static Bitmap CaptureLevelSceenshot(bool save = false)
        {
            HideLevelDebugForm();

            var rect = new User32.Rect();
            User32.GetWindowRect(BO1Process.MainWindowHandle, ref rect);

            int width = selectedMap.level_rec.Width;
            int height = selectedMap.level_rec.Height;

            var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(bmp);
            graphics.CopyFromScreen(
                rect.left + selectedMap.level_rec.X,
                rect.top + selectedMap.level_rec.Y,
                0, 0,
                new Size(width, height), CopyPixelOperation.SourceCopy);

            graphics.Dispose();

            if (save || saveNextLevel)
            {
                bmp.Save(Path.Combine(data_dir, "level_" + selectedResolution.window_width + "x" + selectedResolution.window_height + ".png"), ImageFormat.Png);

                if (saveNextLevel)
                    saveNextLevel = false;
            }

            ShowLevelDebugForm();

            return bmp;
        }

        static Bitmap CaptureLoadingSceenshot()
        {
            var rect = new User32.Rect();
            User32.GetWindowRect(BO1Process.MainWindowHandle, ref rect);

            int width = 300;
            int height = 300;

            var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(bmp);
            graphics.CopyFromScreen(
                rect.left + 100,
                rect.top + 100,
                0, 0,
                new Size(width, height), CopyPixelOperation.SourceCopy);

            graphics.Dispose();

            return bmp;
        }

        static void HideResetDebugForm()
        {
#if DEBUG
            if (reset_form != null)
            {
                reset_form.TopMost = false;
                reset_form.SendToBack();
            }
#endif
        }

        static void ShowResetDebugForm()
        {
#if DEBUG
            if (reset_form != null)
            {
                reset_form.TopMost = true;
                reset_form.BringToFront();
            }
#endif
        }

        static void HideLevelDebugForm()
        {
#if DEBUG
            if (level_form != null)
            {
                level_form.TopMost = false;
                level_form.SendToBack();
            }
#endif
        }

        static void ShowLevelDebugForm()
        {
#if DEBUG
            if (level_form != null)
            {
                level_form.TopMost = true;
                level_form.BringToFront();
            }
#endif
        }

        static bool saveNext = false;
        static Bitmap CaptureSceenshotReset(bool save = false)
        {
            HideResetDebugForm();

            var rect = new User32.Rect();
            User32.GetWindowRect(BO1Process.MainWindowHandle, ref rect);

            int width = selectedMap.reset_rec.Width;
            int height = selectedMap.reset_rec.Height;

            var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(bmp);
            graphics.CopyFromScreen(rect.left + selectedMap.reset_rec.X, rect.top + selectedMap.reset_rec.Y, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);

            graphics.Dispose();

            if (save || saveNext)
            {
                string path = Path.Combine(data_dir, "reset_" + selectedResolution.window_width + "x" + selectedResolution.window_height + ".png");
                bmp.Save(path, ImageFormat.Png);
                Process.Start(path);

                if (saveNext)
                    saveNext = false;
            }

            ShowResetDebugForm();

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

    public class level
    {
        public int lvl;
        public point[] pointsToMatch;
    }

    public class point
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

    public class Map
    {
        public string map_name;
        public List<level> levels;
        public List<point> reset_points;

        public Rectangle level_rec;
        public Rectangle reset_rec;
    }

    public class Resolution
    {
        public int window_width;
        public int window_height;

        public List<Map> Maps { get; set; }
    }
}
