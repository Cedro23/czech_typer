using CzechTyper.ComponentModel;
using CzechTyper.Keyboard;
using CzechTyper.Models;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace CzechTyper.ViewModels
{
    public class MainVM : ViewModelBase
    {
        private readonly Icon _onIcon = new Icon("C:\\Users\\CLR\\Documents\\Dev\\_Tests\\CzechTyper\\Resources\\Flag_of_the_Czech_Republic.ico");
        private readonly Icon _offIcon = new Icon("C:\\Users\\CLR\\Documents\\Dev\\_Tests\\CzechTyper\\Resources\\Flag_of_the_Czech_Republic_Grey.ico");
        private NotifyIcon _notifyIcon;

        private bool _isHooked = false;

        public bool IsHooked
        {
            get => _isHooked;
            set
            {
                _isHooked = value;
                OnPropertyChanged(nameof(IsHooked));
            }
        }

        public MainVM()
        {
            if (System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1)
            {
                MessageBox.Show("An instance is already running.", "Existing instance", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }

            InitializeTrayIcon();
            HandleHook(true);
            Messenger.MessageReceived += OnMessageReceived;
        }

        private void OnMessageReceived(MessageType message)
        {
            if (message == MessageType.Closing)
            {
                _notifyIcon.Click -= Toggle;
                _notifyIcon.Dispose();
            }
        }

        private void InitializeTrayIcon()
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = _onIcon,
                Text = "Czech typer",
                Visible = true
            };
            _notifyIcon.Click += Toggle;

            //BalloonTip
            _notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            _notifyIcon.BalloonTipText = "Click on the flag to turn the typer ON or OFF.";
            _notifyIcon.BalloonTipTitle = _notifyIcon.Text;


            // Optionally, you can add a context menu to the tray icon
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add("Exit", OnExit);
            _notifyIcon.ContextMenu = contextMenu;
        }

        public void HandleHook(bool isActive)
        {
            if (isActive)
                KeyboardHook.SetHook();
            else
                KeyboardHook.Unhook();

            IsHooked = isActive;
        }

        private void Toggle(object sender, EventArgs e)
        {
            HandleHook(!IsHooked);

            if (IsHooked)
                _notifyIcon.Icon = _onIcon;
            else
                _notifyIcon.Icon = _offIcon;
        }

        private void OnExit(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }

        public void Dispose()
        {
            Messenger.MessageReceived -= OnMessageReceived;
        }
    }
}
