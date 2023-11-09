using PlanningScheduleApp.Models;
using System;
using System.Collections.Generic;
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
        List<ScheduleTemplateModel> scheduleTemplates { get; set; }
        ScheduleTemplateModel SelectedTemplate { get; set; }

        public ScheduleManageWindow()
        {
            InitializeComponent();
            scheduleTemplates = new List<ScheduleTemplateModel>();
            scheduleTemplates = Odb.db.Database.SqlQuery<ScheduleTemplateModel>("select ID_Template, TemplateName, isFlexible, RestingDaysCount from Zarplats.dbo.Schedule_Template").ToList();
            SchedulesDG.ItemsSource = scheduleTemplates;
        }

        private void ScheduleAdd_Click(object sender, RoutedEventArgs e)
        {
            ScheduleAddWindow scheduleAddWindow = new ScheduleAddWindow();
            scheduleAddWindow.ShowDialog();
        }

        private void SchedulesDG_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ScheduleEditWindow scheduleEditWindow = new ScheduleEditWindow(SelectedTemplate.ID_Template);
            scheduleEditWindow.ShowDialog();
        }

        private void SchedulesDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedTemplate = (ScheduleTemplateModel)SchedulesDG.SelectedItem;
        }
    }
}
