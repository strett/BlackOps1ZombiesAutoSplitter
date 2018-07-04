using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombiesAutosplitter
{
    public class GameWindow
    {
        public bool Active { get; set; } = false;

        public int Width { get; set; }
        public int Height { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        private readonly string BO1ProcessName = "BlackOps";
        private Process _process = null;

        private GameWindow() { }

        public static GameWindow Attach()
        {
            GameWindow gameWindow = new GameWindow();
            gameWindow.FindProcess();

            return gameWindow;
        }

        private void FindProcess()
        {
            Process[] processes = Process.GetProcessesByName(BO1ProcessName);

            if (processes.Length > 0)
            {
                Active = true;
                _process = processes[0];
                _process.Exited += delegate { Active = false; };

                GetBounds();
            }
            else
            {
                Active = false;
                _process = null;
            }
        }

        private void GetBounds()
        {
            User32Helper.Rect rect = new User32Helper.Rect();
            User32Helper.GetWindowRect(_process.MainWindowHandle, ref rect);

            this.Width = rect.right - rect.left;
            this.Height = rect.bottom - rect.top;

            this.X = rect.left;
            this.Y = rect.top;
        }

        public GameState GetMenuState()
        {
            // 0x4212FEC =  menu state, will be zero when in zombies match
            // 421EA98 = will be 1 when in zombies menu and in zombies match, 0 in any other state
            // 2E8C8AC = 18568 when loading zombies map

            byte[] menuStateBuffer = new byte[4];
            IntPtr baseAddress = _process.MainModule.BaseAddress;
            IntPtr menuStateAddr = IntPtr.Add(baseAddress, 0x4212FEC);
            User32Helper.ReadProcessMemory(_process.Handle, menuStateAddr, menuStateBuffer, 4, out IntPtr numberOfBytesRead);

            byte[] loadingStateBuffer = new byte[4];
            IntPtr loadingStateAddr = IntPtr.Add(baseAddress, 0x2E8C8AC);
            User32Helper.ReadProcessMemory(_process.Handle, loadingStateAddr, loadingStateBuffer, 4, out numberOfBytesRead);

            int numberState = BitConverter.ToInt32(menuStateBuffer, 0);
            GameState state = (GameState)numberState;

            int numberLoadingState = BitConverter.ToInt32(loadingStateBuffer, 0);
            if ((GameState)numberLoadingState == GameState.LOADING_MAP)
                state = GameState.LOADING_MAP;

            return state;
        }

        private bool hasBeenAboveResetLine = true;
        public bool CheckIsReset()
        {
            const int resetLine = 290;

            // 2F08A30 = some sort of timer that start at 0 when reset
            byte[] timerBuffer = new byte[4];
            IntPtr baseAddress = _process.MainModule.BaseAddress;
            IntPtr timerAddr = IntPtr.Add(baseAddress, 0x2F08A30);
            User32Helper.ReadProcessMemory(_process.Handle, timerAddr, timerBuffer, 4, out IntPtr numberOfBytesRead);

            int timerValue = BitConverter.ToInt32(timerBuffer, 0);
            if (hasBeenAboveResetLine && timerValue < resetLine)
            {
                hasBeenAboveResetLine = false;
                Logger.Log("Reset, going to reset..");
            }
            else if (timerValue > resetLine && !hasBeenAboveResetLine)
            {
                hasBeenAboveResetLine = true;
                Logger.Log("Reset");
                return true;
            }

            return false;
        }
    }

    public enum GameState : int
    {
        INGAME_ZOMBIES = 0,
        MAIN_MENU = 8,
        CAMPAIGN_OPTIONS = 7,
        ZOMBIES_SOLO_MULTIPLAYER = 5,
        POPUP = 9,  // multiplayer select, options and quit menu
        MAP_SELECT_FIND_MATCH = 6,
        LOADING_MAP = 18568,
    }
}
