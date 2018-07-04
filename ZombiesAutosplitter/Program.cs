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
                    }

                }

                Thread.Sleep(50);
            }
        }
    }
}
