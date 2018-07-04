using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace ZombiesAutosplitter
{
    public partial class FormMain : Form
    {
        public static bool ProgramActive { get; set; } = true;

        volatile GameWindow window;

        // threads
        Thread processFindThread;
        Thread logAppendThread;
        Thread checkStateThread;

        public FormMain()
        {
            InitializeComponent();
            
            processFindThread = new Thread(new ThreadStart(FindWindow));
            logAppendThread = new Thread(new ThreadStart(AppendLogs));
            checkStateThread = new Thread(new ThreadStart(CheckGameState));

            this.FormClosing += delegate { ProgramActive = false; };
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            logAppendThread.Start();
            processFindThread.Start();
            checkStateThread.Start();
        }

        private void FindWindow()
        {
            window = GameWindow.Attach();

            while (!window.Active && ProgramActive)
            {
                window = GameWindow.Attach();
                Thread.Sleep(100);
            }

            Logger.Log("Window found");
        }

        private void AppendLogs()
        {
            Action action = delegate () {
                Logger.AppendLogs(this.RTB_log);
            };

            this.RTB_log.HandleDestroyed += delegate { return; };

            try
            {
                while (ProgramActive)
                {
                    if (this.RTB_log.InvokeRequired)
                    {
                        this.RTB_log.Invoke(action);
                    }

                }
            }
            catch { }
        }

        private void CheckGameState()
        {
            while (ProgramActive)
            {
                if (window != null && window.Active)
                {
                    var state = window.GetMenuState();
                    DisplayState(state);

                    if (window.CheckIsReset())
                    {
                        // press reset key here
                    }

                }

                Thread.Sleep(50);
            }
        }

        private void DisplayState(GameState state)
        {
            try
            {
                Action action = delegate ()
                {
                    this.LB_MenuState.Text = "STATE: " + state.ToString();
                };

                if (this.LB_MenuState.InvokeRequired)
                {
                    this.LB_MenuState.Invoke(action);
                }
            }
            catch { }
        }
    }
}
