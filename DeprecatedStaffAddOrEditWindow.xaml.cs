using PlanningScheduleApp.Models;
using PlanningScheduleApp.Pages;
using System.Windows;

namespace PlanningScheduleApp
{
    public partial class DeprecatedStaffAddOrEditWindow : Window
    {
        StaffModel SelectedStaff { get; set; }
        DepModel SelectedDep { get; set; }
        string Mode;

        public DeprecatedStaffAddOrEditWindow(StaffModel selectedStaff, string mode, DepModel selectedDep)
        {
            InitializeComponent();
            SelectedStaff = selectedStaff;
            SelectedDep = selectedDep;
            Mode = mode;

            if (Mode == "Add")
            {
                MainFrame.Navigate(new DeprecatedStaffAddPage(SelectedDep));
            }
            else if (Mode == "Edit")
            {
                MainFrame.Navigate(new DeprecatedStaffEditPage(SelectedStaff)); 
            }
        }
    }
}
