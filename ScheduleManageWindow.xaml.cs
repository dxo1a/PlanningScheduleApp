using PlanningScheduleApp.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PlanningScheduleApp
{
    public partial class ScheduleManageWindow : Window
    {
        List<ScheduleTemplateModel> scheduleTemplates = new List<ScheduleTemplateModel>();
        ScheduleTemplateModel SelectedTemplate { get; set; }

        public ScheduleManageWindow()
        {
            InitializeComponent();
            UpdateGrid();
        }

        private void TemplateAdd_Click(object sender, RoutedEventArgs e)
        {
            ScheduleAddWindow scheduleAddWindow = new ScheduleAddWindow();
            scheduleAddWindow.ShowDialog();
        }

        private void TemplatesDG_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ScheduleEditWindow scheduleEditWindow = new ScheduleEditWindow(SelectedTemplate.ID_Template);
            scheduleEditWindow.ShowDialog();
        }

        private void TemplatesDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedTemplate = (ScheduleTemplateModel)TemplatesDG.SelectedItem;
        }

        private void TemplateDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Удалить шаблон?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Odb.db.Database.ExecuteSqlCommand("delete from Zarplats.dbo.Schedule_Template where ID_Template = @templateid; delete from Zarplats.dbo.Schedule_StaticDays where Template_ID = @templateid", new SqlParameter("templateid", SelectedTemplate.ID_Template));
                MessageBox.Show("Шаблон удалён!");
                UpdateGrid();
            }
            
        }

        private void UpdateGrid()
        {
            scheduleTemplates = Odb.db.Database.SqlQuery<ScheduleTemplateModel>("select ID_Template, TemplateName, isFlexible, RestingDaysCount from Zarplats.dbo.Schedule_Template").ToList();
            TemplatesDG.ItemsSource = scheduleTemplates;
        }
    }
}
