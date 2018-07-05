using WindowsInput;
using WindowsInput.Native;

namespace ZombiesAutosplitter
{
    public static class LivesplitHelper
    {
        static VirtualKeyCode resetKey = VirtualKeyCode.NUMPAD2;
        static VirtualKeyCode splitKey = VirtualKeyCode.NUMPAD1;
        static VirtualKeyCode pauseKey = VirtualKeyCode.NUMPAD3;

        public static void ResetAndStart()
        {
            InputSimulator s = new InputSimulator();
            s.Keyboard.KeyPress(resetKey);
            s.Keyboard.KeyPress(splitKey); 
        }

        public static void Reset()
        {
            InputSimulator s = new InputSimulator();
            s.Keyboard.KeyPress(resetKey);
        }

        public static void TogglePause()
        {
            InputSimulator s = new InputSimulator();
            s.Keyboard.KeyPress(pauseKey);
        }
    }
}
