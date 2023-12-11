using StyledWindow.WPF.Commands;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PlanningScheduleApp
{
    public partial class App : Application
    {
        public App()
        {
            //ThemeEx.ChangeCulture += Action<string>;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            //await ThemeEx.LoadThemeAsync(null);
            //var load_com = new LoadThemeCommand();
            //load_com.Execute(null);
            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await ThemeEx.SaveThemeAsync(null).ConfigureAwait(true);

            //var save_com = new SaveThemeCommand();
            //save_com.Execute(null);

            base.OnExit(e);
        }

        public static void DisableAllWindows()
        {
            foreach (Window window in Application.Current.Windows)
            {
                window.IsEnabled = false;
            }
        }

        public static void EnableAllWindows()
        {
            foreach (Window window in Application.Current.Windows)
            {
                window.IsEnabled = true;
            }
        }

        public static void DisableAllWindowsExcept(Window exceptWindow)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window != exceptWindow)
                {
                    window.IsEnabled = false;
                }
            }
        }
    }
}
