using PlanningScheduleApp.Models;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;

namespace PlanningScheduleApp
{
    /// <summary>
    /// Логика взаимодействия для InfoCustomWindow.xaml
    /// </summary>
    public partial class InfoCustomWindow : Window
    {
        string WhichInfo;
        StaffModel SelectedRow { get;set; }
        DateTime Date;

        StaffModel Absence { get; set; }
        StaffModel ScheduleInfo { get; set; }

        public InfoCustomWindow(string whichInfo, StaffModel selectedRow, DateTime date)
        {
            InitializeComponent();

            WhichInfo = whichInfo;
            SelectedRow = selectedRow;
            Date = date;

            Absence = Odb.db.Database.SqlQuery<StaffModel>("select a.*, b.Cause as CauseAbsence from Zarplats.dbo.Schedule_Absence as a left join Zarplats.dbo.AbsenceRef as b on a.AbsenceRef_ID = b.ID_AbsenceRef where id_Staff = @idstaff and DateBegin <= @date and DateEnd >= @date", new SqlParameter("idstaff", SelectedRow.STAFF_ID), new SqlParameter("date", Date)).FirstOrDefault();
            ScheduleInfo = Odb.db.Database.SqlQuery<StaffModel>("select distinct * from Zarplats.dbo.Staff_Schedule where STAFF_ID = @idstaff and DTA = @date", new SqlParameter("idstaff", SelectedRow.STAFF_ID), new SqlParameter("date", Date)).FirstOrDefault();
            int hasAbsenceFullDay = Odb.db.Database.SqlQuery<int>("select count(*) from Zarplats.dbo.Schedule_Absence where id_Staff = @idstaff and DateBegin <= @date and DateEnd >= @date and TimeBegin is null and TimeEnd is null", new SqlParameter("date", Date), new SqlParameter("idstaff", SelectedRow.STAFF_ID)).FirstOrDefault();

            string absenceDynamicStroke;
            string scheduleDynamicStroke;

            if (Absence == null)
            {
                AbsenceTI.Visibility = Visibility.Collapsed;
            }
            if (Absence != null && ScheduleInfo == null)    // если есть отсутствие, но нет информации о рабочем дне (напр. отсутсвие на выходной день)
            {
                InfoTC.SelectedIndex = 1;
                WorkingDayTI.Visibility = Visibility.Collapsed;

                if (Absence.TimeBegin != null && Absence.TimeEnd != null)
                    absenceDynamicStroke = $"Сотрудник: {selectedRow.SHORT_FIO}" + Environment.NewLine +
                                    $"Дата отсутствия: {date.Date}" + Environment.NewLine +
                                    $"Время отсутствия: {Absence.TimeBegin} - {Absence.TimeEnd}" + Environment.NewLine +
                                    $"Причина: {Absence.CauseAbsence} ({Absence.DateBegin.GetValueOrDefault().ToString("dd.MM.yyyy")} - {Absence.DateEnd.GetValueOrDefault().ToString("dd.MM.yyyy")})";
                else
                    absenceDynamicStroke = $"Сотрудник: {selectedRow.SHORT_FIO}" + Environment.NewLine +
                                    $"Дата отсутствия: {date.Date}" + Environment.NewLine +
                                    $"Время отсутствия: весь день" + Environment.NewLine +
                                    $"Причина: {Absence.CauseAbsence} ({Absence.DateBegin.GetValueOrDefault().ToString("dd.MM.yyyy")} - {Absence.DateEnd.GetValueOrDefault().ToString("dd.MM.yyyy")})";

                AbsenceInfoTBX.Text = absenceDynamicStroke;
            }
            else if (Absence == null && ScheduleInfo != null)   // если есть информация о рабочем дне, но нет отсутсвия
            {
                AbsenceTI.Visibility = Visibility.Collapsed;

                scheduleDynamicStroke = $"Сотрудник: {selectedRow.SHORT_FIO}" + Environment.NewLine +
                                $"Дата: {ScheduleInfo.DTA.ToShortDateString()}" + Environment.NewLine +
                                $"Время работы: {ScheduleInfo.WorkTime}" + Environment.NewLine +
                                $"Обед: {ScheduleInfo.LunchTime}" + Environment.NewLine +
                                $"Рабочие часы: {ScheduleInfo.WorkingHours}";
                WorkingDayInfoTBX.Text = scheduleDynamicStroke;
            }
            else if (Absence != null && ScheduleInfo != null && hasAbsenceFullDay <= 0)   // если есть и отсутствие и информация о рабочем дне, но отсутствие не на весь день
            {
                if (Absence.TimeBegin != null && Absence.TimeEnd != null)
                    absenceDynamicStroke = $"Сотрудник: {selectedRow.SHORT_FIO}" + Environment.NewLine +
                                    $"Дата отсутствия: {date.Date}" + Environment.NewLine +
                                    $"Время отсутствия: {Absence.TimeBegin} - {Absence.TimeEnd}" + Environment.NewLine +
                                    $"Причина: {Absence.CauseAbsence} ({Absence.DateBegin.GetValueOrDefault().ToString("dd.MM.yyyy")} - {Absence.DateEnd.GetValueOrDefault().ToString("dd.MM.yyyy")})";
                else
                    absenceDynamicStroke = $"Сотрудник: {selectedRow.SHORT_FIO}" + Environment.NewLine +
                                    $"Дата отсутствия: {date.Date}" + Environment.NewLine +
                                    $"Время отсутствия: весь день" + Environment.NewLine +
                                    $"Причина: {Absence.CauseAbsence} ({Absence.DateBegin.GetValueOrDefault().ToString("dd.MM.yyyy")} - {Absence.DateEnd.GetValueOrDefault().ToString("dd.MM.yyyy")})";
                scheduleDynamicStroke = $"Сотрудник: {selectedRow.SHORT_FIO}" + Environment.NewLine +
                                $"Дата: {ScheduleInfo.DTA.ToShortDateString()}" + Environment.NewLine +
                                $"Время работы: {ScheduleInfo.WorkTime}" + Environment.NewLine +
                                $"Обед: {ScheduleInfo.LunchTime}" + Environment.NewLine +
                                $"Рабочие часы: {ScheduleInfo.WorkingHours}";

                AbsenceInfoTBX.Text = absenceDynamicStroke;
                WorkingDayInfoTBX.Text = scheduleDynamicStroke;
            }
            else if (Absence != null && hasAbsenceFullDay > 0)   // если есть отсутствие и оно на полный день
            {
                InfoTC.SelectedIndex = 1;
                WorkingDayTI.Visibility = Visibility.Collapsed;

                if (Absence.TimeBegin != null && Absence.TimeEnd != null)
                    absenceDynamicStroke = $"Сотрудник: {selectedRow.SHORT_FIO}" + Environment.NewLine +
                                    $"Дата отсутствия: {date.Date}" + Environment.NewLine +
                                    $"Время отсутствия: {Absence.TimeBegin} - {Absence.TimeEnd}" + Environment.NewLine +
                                    $"Причина: {Absence.CauseAbsence} ({Absence.DateBegin.GetValueOrDefault().ToString("dd.MM.yyyy")} - {Absence.DateEnd.GetValueOrDefault().ToString("dd.MM.yyyy")})";
                else
                    absenceDynamicStroke = $"Сотрудник: {selectedRow.SHORT_FIO}" + Environment.NewLine +
                                    $"Дата отсутствия: {date.Date}" + Environment.NewLine +
                                    $"Время отсутствия: весь день" + Environment.NewLine +
                                    $"Причина: {Absence.CauseAbsence} ({Absence.DateBegin.GetValueOrDefault().ToString("dd.MM.yyyy")} - {Absence.DateEnd.GetValueOrDefault().ToString("dd.MM.yyyy")})";

                AbsenceInfoTBX.Text = absenceDynamicStroke;
            }
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void InfoTC_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (InfoTC.SelectedIndex == 0)
                this.Height = 245;
            else if (InfoTC.SelectedIndex == 1)
                this.Height = 220;
        }
    }
}
