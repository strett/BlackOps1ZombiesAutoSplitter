using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ZombiesAutosplitter
{
    static class Program
    {
        static volatile GameWindow window;

        // threads
        static Thread processFindThread;
        static Thread logAppendThread;
        static Thread checkStateThread;

        static void Main()
        {
            PrintNotice();

            processFindThread = new Thread(new ThreadStart(FindWindow));
            logAppendThread = new Thread(new ThreadStart(AppendLogs));
            checkStateThread = new Thread(new ThreadStart(CheckGameState));

            logAppendThread.Start();
            processFindThread.Start();
            checkStateThread.Start();

            
            Console.Read();

            ProgramActive = false;
        }

        public static bool ProgramActive { get; set; } = true;

        private static void PrintNotice()
        {
            Logger.Log("=======================================================================================");
            Logger.Log(@"Split key = numpad1");
            Logger.Log(@"Reset key = numpad2");
            Logger.Log(@"Pause key = numpad3");
            Logger.Log(@"These key bindings are currently fixed. It is imporant that the keybindings are numpad");
            Logger.Log(@"keys else we wont be able to split when shift key is down.");
            Logger.Log(@"---------------------------------------------------------------------------------------");
            Logger.Log(@"This program has te be launched before starting a run or it won't work properly.");
            Logger.Log(@"https://github.com/aldrikboy/BlackOps1ZombiesAutoSplitter");
            Logger.Log("=======================================================================================");
        }

        private static void FindWindow()
        {
            window = GameWindow.Attach();

            while (!window.Active && ProgramActive)
            {
                window = GameWindow.Attach();
                Thread.Sleep(100);
            }

            Logger.Log("Window found");
        }

        private static void AppendLogs()
        {
            while (ProgramActive)
            {
                Logger.AppendLogs();
            }
        }

        static private GameState _lastState;
        private static void CheckGameState()
        {
            while (ProgramActive)
            {
                try
                {
                    if (window != null && window.Active)
                    {
                        var state = window.GetMenuState();

                        if (_lastState != state)
                        {
                            Logger.Log("STATE: " + state.ToString());
                            _lastState = state;
                        }

                        if (window.CheckIsReset())
                        {
                            // press reset key here
                            LivesplitHelper.ResetAndStart();
                        }

                        window.CheckLevel();

                        var pauseState = window.GetPauseState();
                        switch (pauseState)
                        {
                            case PauseState.PAUSED: LivesplitHelper.TogglePause(); break;
                            case PauseState.RESUMED: LivesplitHelper.TogglePause(); break;
                        }
                    }

                    Thread.Sleep(5);
                }
                catch { }
            }
        }
    }
}
