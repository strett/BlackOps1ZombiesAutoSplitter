using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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

            processFindThread.Start();
            logAppendThread.Start();

            AskForRunLevel();

            checkStateThread = new Thread(new ThreadStart(CheckGameState));
            checkStateThread.Start();

            
            while (true)
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

        private static void AskForRunLevel()
        {
            Logger.Log("This bot will split at round [2,3,4,5,10,15,30,50,70,100], make sure you dont have any other splits in your timer");

            string[] acceptedInput = new string[6] { "5", "15", "30", "50", "70", "100" };

            string input = "";
            while (!acceptedInput.Any(e => e == input))
            {
                Logger.Log("What round are you running? [5,15,30,50,70,100]");
                Logger.Log("Round: ");
                input = Console.ReadLine();
            }

            window.SetRunLevel(int.Parse(input));
        }

        private static void FindWindow()
        {
            window = GameWindow.Attach();

            while (!window.Active && ProgramActive)
            {
                try
                {
                    window = GameWindow.Attach();
                    Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    Logger.Log(ex.Message, LogType.ERROR);
                }
            }
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
                catch (Exception ex)
                {
                    Logger.Log(ex.Message, LogType.ERROR);
                }
            }
        }
    }
}
