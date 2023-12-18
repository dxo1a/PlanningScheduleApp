using PlanningScheduleApp.MVVM.ViewModel;
using System.ComponentModel;
using System.Windows;

namespace PlanningScheduleApp.MVVM.View
{
    public partial class ProcessWindow : Window
    {
        public ProcessWindow()
        {
            InitializeComponent();
            Closing += ProcessWindow_Closing;
        }

        private void ProcessWindow_Closing(object sender, CancelEventArgs e)
        {
            var viewModel = DataContext as ProcessExportToExcelViewModel;
            if (viewModel != null && !viewModel.CanClose)
            {
                e.Cancel = true;
            }
        }
    }
}
