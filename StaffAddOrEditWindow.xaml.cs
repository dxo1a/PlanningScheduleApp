using PlanningScheduleApp.Models;
using PlanningScheduleApp.Pages;
using System.Windows;

namespace PlanningScheduleApp
{
    public partial class StaffAddOrEditWindow : Window
    {
        StaffModel SelectedStaff { get; set; }
        DepModel SelectedDep { get; set; }
        string Mode;

        public StaffAddOrEditWindow(StaffModel selectedStaff, string mode, DepModel selectedDep)
        {
            InitializeComponent();
            SelectedStaff = selectedStaff;
            SelectedDep = selectedDep;
            Mode = mode;

            if (Mode == "Add")
            {
                MainFrame.Navigate(new StaffAddPage(SelectedDep));
            }
            else if (Mode == "Edit")
            {
                MainFrame.Navigate(new StaffEditPage(SelectedStaff)); 
            }
        }
    }
}
