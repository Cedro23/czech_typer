using CzechTyper.Models;
using CzechTyper.ViewModels;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;

namespace CzechTyper
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public readonly MainVM VM = new MainVM();
        
        public MainWindow()
        {
            InitializeComponent();
            DataContext = VM;
            WindowState = WindowState.Minimized;
            Hide();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Messenger.SendMessage(MessageType.Closing);
            base.OnClosing(e);
        }
    }
}
