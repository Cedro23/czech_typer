using System.Windows.Forms;

namespace CzechTyper.Keyboard
{
    internal static class StateMachine
    {
        private static int _lastKeyPressedCount = 0;
        private static Keys _lastKeyPressed = Keys.None;
        private static string _lastCharacterWritten = "";

        public static void Reset()
        {
            _lastKeyPressedCount = 0;
            _lastKeyPressed = Keys.None;
            _lastCharacterWritten = "";
        }

        public static void Update(Keys key)
        {
            if (_lastKeyPressed == key)
            {
                _lastKeyPressedCount++;
            }
            else
            {
                Reset();
                _lastKeyPressed = key;
                _lastKeyPressedCount = 1;
            }
        }

        public static bool IsDoublePress(Keys key)
        {
            return _lastKeyPressed == key && _lastKeyPressedCount == 2;
        }

        public static void SetLastCharacterWritten(string character)
        {
            _lastCharacterWritten = character;
        }

        public static string GetLastCharacterWritten()
        {
            return _lastCharacterWritten;
        }
    }
}
