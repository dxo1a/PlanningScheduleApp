using System.Windows;

namespace PlanningScheduleApp
{
    public partial class LoadingWindow : Window
    {
        public bool CanClose { get; set; }

        public LoadingWindow()
        {
            InitializeComponent();
            ResizeMode = ResizeMode.NoResize;
            App.DisableAllWindowsExcept(this);
            CanClose = false;
            Closing += LoadingWindow_Closing;
        }

        private void LoadingWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!CanClose)
            {
                e.Cancel = true;
                
            } else if (CanClose)
            {
                e.Cancel = false;
                App.EnableAllWindows();
            }
            
        }
    }
}
