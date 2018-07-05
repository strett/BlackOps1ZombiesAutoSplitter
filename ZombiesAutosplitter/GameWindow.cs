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

        public bool GamePaused { get; set; } = false;

        public int Width { get; set; }
        public int Height { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        private readonly string BO1ProcessName = "BlackOps";
        private Process _process = null;

        private GameWindow() { }

        private GameState _gameState;

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

            if (_gameState == GameState.INGAME_ZOMBIES && state != GameState.INGAME_ZOMBIES)
                LivesplitHelper.Reset();

                _gameState = state;

            return state;
        }

        int _currentLevel = 0;
        int _lastValue = 0;
        DateTime _levelEndStamp;
        bool _timestampSet = false;
        bool _levelIncremented = false;
        public void CheckLevel()
        {
            //const int waitSpanMS = 10000;
            
            //// 0x01809A34 = some sort of timer that start at 0 when reset
            //// 0x01809A34 is a few offsets added together
            //byte[] valBuffer = new byte[4];
            //IntPtr baseAddress = _process.MainModule.BaseAddress;
            //IntPtr valAddr = IntPtr.Add(baseAddress, 0x01809A34);

            //User32Helper.ReadProcessMemory(_process.Handle, valAddr, valBuffer, 4, out IntPtr numberOfBytesRead);

            //int valValue = BitConverter.ToInt32(valBuffer, 0);

            //var ms = (DateTime.Now - _levelEndStamp).TotalMilliseconds;

            //// entered once when timer increases
            //if (valValue > _lastValue && _lastValue > 0 && !_timestampSet)
            //{
            //    Logger.Log("entered");
            //    if (!_timestampSet)
            //    {
            //        _levelEndStamp = DateTime.Now;
            //        _timestampSet = true;
            //        _levelIncremented = false;
            //    }
            //}
            //else
            //{
            //    if (_currentLevel == 0)
            //    {
            //        _currentLevel++;
            //        Logger.Log("LEVEL: " + _currentLevel.ToString());
            //    }
            //}

            //if (ms > waitSpanMS && _timestampSet && !_levelIncremented)
            //{
            //    _currentLevel++;
            //    Logger.Log("LEVEL: " + _currentLevel.ToString());
            //    _levelIncremented = true;
            //}

            //if (ms > waitSpanMS && _timestampSet && _levelIncremented)
            //{
            //    Logger.Log("ready for next level");
            //    _timestampSet = false;
            //}

            //_lastValue = valValue;
        }

        private bool hasBeenAboveResetLine = true;
        public bool CheckIsReset()
        {
            if (_gameState != GameState.INGAME_ZOMBIES) return false;

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
                //Logger.Log("Reset, going to reset..");
            }
            else if (timerValue > resetLine && !hasBeenAboveResetLine)
            {
                hasBeenAboveResetLine = true;
                Logger.Log("Reset");
                _lastValue = 0;
                _currentLevel = 0;
                return true;
            }

            return false;
        }

        public PauseState GetPauseState()
        {
            if (_gameState != GameState.INGAME_ZOMBIES) return PauseState.NO_CHANGE;

            // 2EE7F2C = 0 if paused, 1065353216 if playing
            byte[] pauseBuffer = new byte[4];
            IntPtr baseAddress = _process.MainModule.BaseAddress;
            IntPtr pauseAddr = IntPtr.Add(baseAddress, 0x2EE7F2C);
            User32Helper.ReadProcessMemory(_process.Handle, pauseAddr, pauseBuffer, 4, out IntPtr numberOfBytesRead);

            int pauseValue = BitConverter.ToInt32(pauseBuffer, 0);

            if (GamePaused && pauseValue == 1065353216)
            {
                GamePaused = false;
                // game resumed
                return PauseState.RESUMED;
            }
            else if (!GamePaused && pauseValue == 0)
            {
                GamePaused = true;
                return PauseState.PAUSED;
            }

            return PauseState.NO_CHANGE;
        }
    }

    public enum PauseState
    {
        RESUMED,
        PAUSED,
        NO_CHANGE
    }

    public enum GameState : int
    {
        INTRO_SCREEN = 1,
        INGAME_ZOMBIES = 0,
        MAIN_MENU = 8,
        CAMPAIGN_OPTIONS = 7,
        ZOMBIES_SOLO_MULTIPLAYER = 5,
        POPUP = 9,  // multiplayer select, options and quit menu
        MAP_SELECT_FIND_MATCH = 6,
        LOADING_MAP = 18568,
    }
}
