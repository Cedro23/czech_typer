using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;

namespace CzechTyper.Keyboard
{
    internal class KeyboardHook
    {
        // Import SetWindowsHookEx from user32.dll
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        // Import UnhookWindowsHookEx from user32.dll
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        // Import CallNextHookEx from user32.dll
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern short GetKeyState(int keyCode);

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_SYSKEYDOWN = 0x0104;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;
        private static InputSimulator _inputSimulator = new InputSimulator();

        public static void SetHook()
        {
            _hookID = SetHook(_proc);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        public static void Unhook()
        {
            UnhookWindowsHookEx(_hookID);
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_SYSKEYDOWN)
            {
                // Alt is pressed, handle the key combination here
                // For example, if 'S' is pressed, simulate typing 'š'
                if (IsAltKeyDown())
                {
                    Keys key = (Keys)Marshal.ReadInt32(lParam);

                    switch (key)
                    {
                        case Keys.A:
                            HandleKey('á', 'Á');
                            return (IntPtr)1; // Block the default behavior
                        case Keys.C:
                            HandleKey('č', 'Č');
                            return (IntPtr)1; // Block the default behavior
                        case Keys.D:
                            HandleKey('ď', 'Ď');
                            return (IntPtr)1; // Block the default behavior
                        case Keys.E:
                            _inputSimulator.Keyboard.TextEntry(IsCapital() ? 'Ě' : 'ě');
                            StateMachine.Reset();
                            return (IntPtr)1; // Block the default behavior
                        case Keys.I:
                            HandleKey('í', 'Í');
                            return (IntPtr)1; // Block the default behavior
                        case Keys.N:
                            HandleKey('ň', 'Ň');
                            return (IntPtr)1; // Block the default behavior
                        case Keys.O:
                            HandleKey('ó', 'Ó');
                            return (IntPtr)1; // Block the default behavior
                        case Keys.R:
                            HandleKey('ř', 'Ř');
                            return (IntPtr)1; // Block the default behavior
                        case Keys.S:
                            HandleKey('š', 'Š');
                            return (IntPtr)1; // Block the default behavior
                        case Keys.T:
                            HandleKey('ť', 'Ť');
                            return (IntPtr)1; // Block the default behavior
                        case Keys.U:
                            StateMachine.Update(key);
                            if (StateMachine.IsDoublePress(key))
                            {
                                if (StateMachine.GetLastCharacterWritten() == "ú")
                                {
                                    _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.BACK);
                                }
                                _inputSimulator.Keyboard.TextEntry(IsCapital() ? 'Ů' : 'ů');
                                StateMachine.SetLastCharacterWritten("ů");
                                StateMachine.Reset();
                            }
                            else
                            {
                                _inputSimulator.Keyboard.TextEntry(IsCapital() ? 'Ú' : 'ú');
                                StateMachine.SetLastCharacterWritten("ú");
                            }
                            return (IntPtr)1; // Block the default behavior
                        case Keys.Y:
                            HandleKey('ý', 'Ý');
                            return (IntPtr)1; // Block the default behavior
                        case Keys.Z:
                            HandleKey('ž', 'Ž');
                            return (IntPtr)1; // Block the default behavior
                    }
                }
                else
                {
                    StateMachine.Reset();
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private static void HandleKey(char lowerCaseChar, char upperCaseChar)
        {
            char character = IsCapital() ? upperCaseChar : lowerCaseChar;
            _inputSimulator.Keyboard.TextEntry(character);
            StateMachine.Reset();
        }

        private static bool IsAltKeyDown()
        {
            return _inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.MENU);
        }

        private static bool IsCapital()
        {
            return _inputSimulator.InputDeviceState.IsTogglingKeyInEffect(VirtualKeyCode.CAPITAL) || _inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.SHIFT);
        }
    }
}
