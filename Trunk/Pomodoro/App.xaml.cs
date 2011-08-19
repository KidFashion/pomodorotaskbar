using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Shell;

namespace Pomodoro
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {
        public static string Command;

        [STAThread]
        private static void Main(string[] args)
        {
            if (SingleInstance<App>.InitializeAsFirstInstance("Pomodoro.App"))
            {
                if (args.Length > 0)
                    Command = args[0].ToLowerInvariant();
                
                var app = new App();
                app.InitializeComponent();
                app.Run();
            }
        }

        bool ISingleInstanceApp.SignalExternalCommandLineArgs(IList<string> args)
        {
            return args.Count <= 1 || ((MainWindow)MainWindow).ProcessCommandLineArgs(args[1].ToLowerInvariant());
        }
    }
}
