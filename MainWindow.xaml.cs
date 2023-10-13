using PlanningScheduleApp.Pages;
using System.Windows;

namespace PlanningScheduleApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Odb.db = new System.Data.Entity.DbContext("Persist Security Info=False;User ID=sa; Password=server_esa;Initial Catalog=dsl_sp;Server=sql");

            FrameApp.SetCurrentMainFrame(MainFrame);
            FrameApp.SetCurrentTopFrame(TopFrame);

            FrameApp.FrameTop.Navigate(new ChooseDepPage());
        }
    }
}
