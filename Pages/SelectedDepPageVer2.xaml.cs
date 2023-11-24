using PlanningScheduleApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using Application = System.Windows.Application;
using Clipboard = System.Windows.Clipboard;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;

namespace PlanningScheduleApp.Pages
{
    public partial class SelectedDepPageVer2 : System.Windows.Controls.Page
    {
        private BindingSource staffBindingSource = new BindingSource();
        private BindingList<StaffModel> StaffList = new BindingList<StaffModel>();
        private DepModel SelectedDep { get; set; }

        private StaffModel SelectedRow { get; set; }

        //List<string> columnsToShow = new List<string> { "STAFF_ID", "TABEL_ID", "SHORT_FIO", "WorkTime", "LunchTime", "DTA", "WorkingHours", };

        public string connectionString = "Persist Security Info=False;User ID=sa; Password=server_esa;Initial Catalog=dsl_sp;Server=sql";

        List<StaffModel> StaffListInPosition = new List<StaffModel>();

        List<ScheduleTemplateModel> ScheduleTemplateList = new List<ScheduleTemplateModel>();

        List<AbsenceModel> CauseList = new List<AbsenceModel>();

        StaffModel SelectedStaffInDG { get; set; }
        StaffModel SelectedStaff { get; set; }
        ScheduleTemplateModel SelectedTemplate { get; set; }
        AbsenceModel SelectedCause { get; set; }

        public double WorkingHours, CalculatedLunchTime, LunchTime;

        public SelectedDepPageVer2(DepModel selectedDep)
        {
            InitializeComponent();

            SetDoubleBuffered(StaffDGV, true);
            StaffDGV.SendToBack();

            SelectedDep = selectedDep;

            AssignCMB();
            UpdateTemplatesList();

            Application.Current.Dispatcher.Invoke(() =>
            {
                StaffListInPosition = Odb.db.Database.SqlQuery<StaffModel>("select distinct b.SHORT_FIO, b.TABEL_ID, b.ID_STAFF as STAFF_ID from perco...staff_ref as a left join perco...staff as b on a.STAFF_ID = b.ID_STAFF left join perco...subdiv_ref as c on a.SUBDIV_ID = c.ID_REF where c.DISPLAY_NAME = @padrazd", new SqlParameter("padrazd", SelectedDep.Position)).OrderBy(s => s.SHORT_FIO).ToList();
                StaffLV.ItemsSource = StaffListInPosition;
                CauseList = Odb.db.Database.SqlQuery<AbsenceModel>("select distinct * from Zarplats.dbo.AbsenceRef").ToList();
                CauseLV.ItemsSource = CauseList;

                AbsenceStartDP.SelectedDate = DateTime.Now;
                AbsenceFinishDP.SelectedDate = DateTime.Now;
            });

            Task.Run(() => InitializeAsync());
            StaffDGV.SelectionChanged += StaffDGV_SelectionChanged;

            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                if (mainWindow.TopFrame.Content is ChooseDepPage chooseDepPage)
                {
                    chooseDepPage.DepVisibilityChanged += ChooseDepPage_DepVisibilityChanged;
                }
            }
        }

        private void ChooseDepPage_DepVisibilityChanged(object sender, bool isVisible)
        {
            if (isVisible)
            {
                DGVBorder.VerticalAlignment = VerticalAlignment.Bottom;
                DGVBorder.MinHeight = 400;
            } 
            else
            {
                DGVBorder.VerticalAlignment = VerticalAlignment.Stretch;
                DGVBorder.MinHeight = 490;
            }
                
        }

        private async void InitializeAsync()
        {
            try
            {
                await UpdateDGV();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when UpdateDGV in InitializeAsync: {ex.Message}");
            }

        }

        private async Task UpdateDGV()
        {
            try
            {
                List<StaffModel> tempList = await Task.Run(() =>
                {
                    return Odb.db.Database.SqlQuery<StaffModel>("SELECT DISTINCT a.ID_Schedule, a.STAFF_ID, LTRIM(e.TABEL_ID) as TABEL_ID, e.SHORT_FIO, a.WorkBegin, a.WorkEnd, a.DTA, a.LunchTimeBegin, a.LunchTimeEnd, a.WorkingHours, b.ID_Absence, c.Cause as CauseAbsence, b.DateBegin, b.DateEnd, b.TimeBegin, b.TimeEnd FROM [Zarplats].[dbo].[Staff_Schedule] as a left join Zarplats.dbo.Schedule_Absence as b on a.STAFF_ID = b.id_Staff and a.DTA between b.DateBegin and b.DateEnd left join Zarplats.dbo.AbsenceRef as c on b.AbsenceRef_ID = c.ID_AbsenceRef left join perco...staff as e on a.STAFF_ID = e.ID_STAFF left join Zarplats.dbo.StaffView as f on a.STAFF_ID = f.STAFF_ID where f.Position = @podrazd order by a.DTA",
                        new SqlParameter("podrazd", SelectedDep.Position)).ToList();
                });

                var groupedData = tempList
                    .GroupBy(x => x.STAFF_ID)
                    .Select(g => new StaffModel
                    {
                        STAFF_ID = g.Key,
                        SHORT_FIO = g.First().SHORT_FIO,
                        // Другие свойства, которые не зависят от даты

                        // Сохраняем все даты и ID_Schedule в списке для каждого сотрудника
                        DatesAndSchedules = g.Select(item => new DateAndSchedule
                        {
                            DTA = item.DTA,
                            ID_Schedule = item.ID_Schedule
                        }).ToList()
                    })
                    .ToList();

                Dispatcher.Invoke(() =>
                {
                    StaffList.Clear();
                    foreach (var staff in groupedData)
                    {
                        StaffList.Add(staff);
                    }

                    staffBindingSource.DataSource = StaffList;

                    DateTime currentDate = DateTime.Now;
                    int daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

                    StaffDGV.Columns.Clear();

                    DataGridViewTextBoxColumn staffIdColumn = new DataGridViewTextBoxColumn();
                    staffIdColumn.HeaderText = "ID";
                    staffIdColumn.DataPropertyName = "STAFF_ID";
                    staffIdColumn.MinimumWidth = 50;
                    StaffDGV.Columns.Add(staffIdColumn);

                    DataGridViewTextBoxColumn staffFIOColumn = new DataGridViewTextBoxColumn();
                    staffFIOColumn.HeaderText = "ФИО";
                    staffFIOColumn.DataPropertyName = "SHORT_FIO";
                    staffFIOColumn.MinimumWidth = 110;
                    StaffDGV.Columns.Add(staffFIOColumn);

                    for (int day = 1; day <= daysInMonth; day++)
                    {
                        DateTime currentDateForColumn = new DateTime(currentDate.Year, currentDate.Month, day);
                        string columnHeader = currentDateForColumn.ToString("dd.MM.yyyy");

                        DataGridViewTextBoxColumn dateColumn = new DataGridViewTextBoxColumn();
                        dateColumn.HeaderText = columnHeader;
                        dateColumn.DataPropertyName = $"Day_{day}";
                        dateColumn.MinimumWidth = 70;
                        dateColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        StaffDGV.Columns.Add(dateColumn);
                    }

                    StaffDGV.DataSource = staffBindingSource;

                    for (int day = 1; day <= daysInMonth; day++)
                    {
                        DateTime currentDateForCell = new DateTime(currentDate.Year, currentDate.Month, day);

                        for (int rowIndex = 0; rowIndex < StaffList.Count; rowIndex++)
                        {
                            StaffModel staff = StaffList[rowIndex];

                            bool isRecordExists = CheckRecordExists(staff.STAFF_ID, currentDateForCell);
                            int hasAbsenceFullDay = Odb.db.Database.SqlQuery<int>("select count(*) from Zarplats.dbo.Schedule_Absence where id_Staff = @idstaff and DateBegin <= @date and DateEnd >= @date and TimeBegin is null and TimeEnd is null", new SqlParameter("date", currentDateForCell.Date), new SqlParameter("idstaff", staff.STAFF_ID)).FirstOrDefault();

                            int? idSchedule = Odb.db.Database.SqlQuery<int?>("select ID_Schedule from Zarplats.dbo.Staff_Schedule where STAFF_ID = @idstaff and DTA = @date",
                                new SqlParameter("idstaff", staff.STAFF_ID), new SqlParameter("date", currentDateForCell.Date)).FirstOrDefault();
                            int? idAbsence = Odb.db.Database.SqlQuery<int?>("select b.ID_Absence from Zarplats.dbo.Staff_Schedule as a left join Zarplats.dbo.Schedule_Absence as b on a.STAFF_ID = b.id_Staff and a.DTA between b.DateBegin and b.DateEnd where STAFF_ID = @idstaff and DTA = @date",
                                new SqlParameter("idstaff", staff.STAFF_ID), new SqlParameter("date", currentDateForCell.Date)).FirstOrDefault();

                            DataGridViewCell cell = StaffDGV.Rows[rowIndex].Cells[day + 1];

                            cell.Value = new DateAndSchedule
                            {
                                ID_Schedule = idSchedule ?? 0,
                                ID_Absence = idAbsence ?? 0,
                                DTA = currentDateForCell,
                                cellText = "Н"
                            };

                            if (isRecordExists)
                            {
                                if (hasAbsenceFullDay > 0)
                                {
                                    cell.Value = new DateAndSchedule
                                    {
                                        ID_Schedule = idSchedule ?? 0,
                                        ID_Absence = idAbsence ?? 0,
                                        DTA = currentDateForCell,
                                        cellText = "Н"
                                    };
                                }
                                else
                                {
                                    cell.Value = new DateAndSchedule
                                    {
                                        ID_Schedule = idSchedule ?? 0,
                                        ID_Absence = idAbsence ?? 0,
                                        DTA = currentDateForCell,
                                        cellText = "Р"
                                    };
                                }
                            }
                        }
                    }

                    StaffDGV.CellFormatting += (s, e) =>
                    {
                        if (e.RowIndex >= 0 && e.RowIndex < StaffDGV.RowCount)
                        {
                            int columnIndex = e.ColumnIndex;
                            int day = columnIndex - 1;

                            if (day > 0)
                            {
                                DataGridViewRow row = StaffDGV.Rows[e.RowIndex];
                                StaffModel staff = (StaffModel)row.DataBoundItem;

                                if (staff != null)
                                {
                                    DateTime currentDateForCell = new DateTime(currentDate.Year, currentDate.Month, day);
                                    bool isRecordExists = CheckRecordExists(staff.STAFF_ID, currentDateForCell);
                                    bool hasAbsence = HasAbsence(staff.STAFF_ID, currentDateForCell);

                                    if (hasAbsence && isRecordExists)
                                        e.CellStyle.BackColor = System.Drawing.Color.Orange;
                                    else
                                        e.CellStyle.BackColor = isRecordExists ? System.Drawing.Color.LightGreen : System.Drawing.Color.LightBlue;
                                }
                            }
                        }
                    };

                    StaffDGV.CellMouseEnter += (s, e) =>
                    {
                        if (e.RowIndex >= 0 && e.ColumnIndex > 1)
                        {
                            DataGridViewCell cell = StaffDGV.Rows[e.RowIndex].Cells[e.ColumnIndex];

                            if (cell.Value is DateAndSchedule dateAndSchedule)
                            {
                                if (dateAndSchedule.ToString() == "Р")
                                    StaffDGV.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "Работает";
                                else
                                    StaffDGV.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "Не работает";
                            }
                        }
                    };
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching data: " + ex.Message);
            }
        }

        private bool CheckRecordExists(int staffId, DateTime date)
        {
            int checkExisting = Odb.db.Database.SqlQuery<int>("select count(*) from Zarplats.dbo.Staff_Schedule where STAFF_ID = @staffId and DTA = @dta", new SqlParameter("staffId", staffId), new SqlParameter("dta", date)).FirstOrDefault();
            if (checkExisting > 0)
                return true;
            else
                return false;
        }

        private void SetDoubleBuffered(System.Windows.Forms.Control c, bool value)
        {
            var property = typeof(System.Windows.Forms.Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            property.SetValue(c, value, null);
        }

        private void StaffDGV_SelectionChanged(object sender, EventArgs e)
        {
            if (StaffDGV.CurrentRow != null)
            {
                SelectedRow = (StaffModel)staffBindingSource.Current;
                if (SelectedRow != null)
                    StaffRemoveBtn.IsEnabled = true;
                else
                    StaffRemoveBtn.IsEnabled = false;
            }
        }

        int doubleClickCounter = 0;
        private void StaffDGV_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex > 1) // Проверяем, что клик был по ячейке с данными
            {
                DataGridViewCell cell = StaffDGV.Rows[e.RowIndex].Cells[e.ColumnIndex];

                if (cell.Value is DateAndSchedule dateAndSchedule)
                {
                    if (dateAndSchedule.ID_Schedule != null && dateAndSchedule.DTA != null)
                    {
                        bool hasAbsence = HasAbsence(SelectedRow.STAFF_ID, dateAndSchedule.DTA.Date);
                        bool isRecordExists = CheckRecordExists(SelectedRow.STAFF_ID, dateAndSchedule.DTA.Date);
                        if (hasAbsence)
                        {
                            InfoCustomWindow infoCustomWindow = new InfoCustomWindow("Отсутствие", SelectedRow, dateAndSchedule.DTA.Date);
                            infoCustomWindow.ShowDialog();
                        }
                        else if (!hasAbsence && isRecordExists)
                        {
                            InfoCustomWindow infoCustomWindow = new InfoCustomWindow("Информация о рабочем дне", SelectedRow, dateAndSchedule.DTA.Date);
                            infoCustomWindow.ShowDialog();
                        } 

                        Console.WriteLine($"[] SelectedRow Info:\n{SelectedRow.STAFF_ID}, {dateAndSchedule.DTA.Date}, {dateAndSchedule.ID_Schedule}, {dateAndSchedule.ID_Absence}");
                    }
                    else
                    {
                        Console.WriteLine($"[] SelectedRow Info:\n{SelectedRow.STAFF_ID}, No information available");
                    }
                }
                else
                {
                    Console.WriteLine($"[] SelectedRow Info:\n{SelectedRow.STAFF_ID}, No information available");
                }
            }
        }

        private async void AddScheduleBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CheckWhatToAdd() == "1100")
            {
                if (SelectedTemplate.isFlexible)
                    FillFlexibleSchedule();
                else if (!SelectedTemplate.isFlexible)
                    FillStaticSchedule();
            }
            else if (CheckWhatToAdd() == "1011")
            {
                int checkExistingSchedule = Odb.db.Database.SqlQuery<int>("select count(*) from Zarplats.dbo.Staff_Schedule where DTA between @DateBegin and @DateEnd", new SqlParameter("DateBegin", AbsenceStartDP.SelectedDate), new SqlParameter("DateEnd", AbsenceFinishDP.SelectedDate)).SingleOrDefault();
                if (checkExistingSchedule > 0)
                    await AddAbsence();
                else
                    MessageBox.Show("Для указанного диапазона не найдено рабочих дней!");
            }
            else if (CheckWhatToAdd() == "1111")
            {
                int checkExistingSchedule = Odb.db.Database.SqlQuery<int>("select count(*) from Zarplats.dbo.Staff_Schedule where DTA between @DateBegin and @DateEnd", new SqlParameter("DateBegin", ScheduleStartDP.SelectedDate), new SqlParameter("DateEnd", ScheduleEndDP.SelectedDate)).SingleOrDefault();
                if (checkExistingSchedule <= 0)
                {
                    if (SelectedTemplate.isFlexible)
                    {
                        FillFlexibleSchedule();
                        await AddAbsence();
                    }
                    else if (!SelectedTemplate.isFlexible)
                    {
                        FillStaticSchedule();
                        await AddAbsence();
                    }
                }
                else
                {
                    MessageBox.Show("График уже существует, добавляем только отсутствие.");
                    await AddAbsence();
                }
            }
            else if (CheckWhatToAdd() == "##0#")
            {
                MessageBox.Show("Укажите причину отсутствия.");
                CauseLV.Focus();
            }
            else if (CheckWhatToAdd() == "1608")
                MessageBox.Show("Ошибка 1608. Обратитесь к разработчику.");
            else if (CheckWhatToAdd() == "0###")
            {
                MessageBox.Show("Выберите сотрудника.");
                StaffLV.Focus();
            }
            else if (CheckWhatToAdd() == "#0##")
                MessageBox.Show("Не все поля графика заполнены.");
            else if (CheckWhatToAdd() == "###0")
                MessageBox.Show($"Не все поля отсутствия заполнены.\nTimeStart: {AbsenceTimeBeginMTBX.Text}, TimeEnd: {AbsenceTimeEndMTBX.Text}");
        }

        public async void DeleteRow()
        {
            if (StaffDGV.SelectedCells.Count > 0)
            {
                var result = MessageBox.Show("Удалить выбранные записи?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    foreach (DataGridViewCell selectedCell in StaffDGV.SelectedCells)
                    {
                        // получаем индекс строки и индекс колонки (для пересечения)
                        int rowIndex = selectedCell.RowIndex;
                        int columnIndex = selectedCell.ColumnIndex;

                        if (rowIndex >= 0 && rowIndex < StaffDGV.Rows.Count && columnIndex > 1)
                        {
                            // получаем экземпляр модели
                            var selectedModel = (StaffModel)StaffDGV.Rows[rowIndex].DataBoundItem;

                            // получаем дату из колонки
                            DateTime currentDateForCell = new DateTime(DateTime.Now.Year, DateTime.Now.Month, columnIndex - 1);

                            // получаем idScheduleToDelete по дате в ячейке
                            int? idScheduleToDelete = selectedModel.DatesAndSchedules
                                .FirstOrDefault(ds => ds.DTA == currentDateForCell)?.ID_Schedule;
                            int? idAbsence = Odb.db.Database.SqlQuery<int?>("select b.ID_Absence from Zarplats.dbo.Staff_Schedule as a left join Zarplats.dbo.Schedule_Absence as b on a.STAFF_ID = b.id_Staff and a.DTA between b.DateBegin and b.DateEnd where STAFF_ID = @idstaff and DTA = @date",
                                new SqlParameter("idstaff", selectedModel.STAFF_ID), new SqlParameter("date", currentDateForCell.Date)).FirstOrDefault();

                            if (idScheduleToDelete.HasValue && idScheduleToDelete.Value != 0)
                            {
                                Console.WriteLine($"Row ID_Schedule in Deleting: {idScheduleToDelete}");

                                Odb.db.Database.ExecuteSqlCommand("DELETE FROM Zarplats.dbo.Staff_Schedule WHERE ID_Schedule = @idschedule", new SqlParameter("idschedule", idScheduleToDelete));
                                if (idAbsence.HasValue && idAbsence != 0)
                                {
                                    var confirmResult = MessageBox.Show($"День {currentDateForCell:dd.MM.yyyy} имеет отсутствие. Удалить отсутствие?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
                                    if (confirmResult == MessageBoxResult.Yes)
                                        Odb.db.Database.ExecuteSqlCommand("DELETE FROM Zarplats.dbo.Schedule_Absence WHERE ID_Absence = @idabsence", new SqlParameter("idabsence", idAbsence));
                                }

                                var dateAndScheduleToDelete = selectedModel.DatesAndSchedules.FirstOrDefault(ds => ds.ID_Schedule == idScheduleToDelete);
                                if (dateAndScheduleToDelete != null)
                                {
                                    

                                    // обновляем объект DateAndSchedule после удаления (надо ли?)
                                    dateAndScheduleToDelete.ID_Schedule = 0; // или другое значение, которое не будет использоваться
                                    dateAndScheduleToDelete.ID_Absence = 0;
                                    dateAndScheduleToDelete.DTA = DateTime.MinValue;
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Выбран некорректный день");
                        }
                    }

                    await UpdateDGV();
                }
            }
            else
            {
                MessageBox.Show("Выберите строки для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void DeleteAbsenceMI_Click(object sender, RoutedEventArgs e)
        {
            List<StaffModel> selectedItems = StaffDGV.SelectedRows.Cast<StaffModel>().ToList();
            if (selectedItems.Count > 0)
            {
                var result = MessageBox.Show("Удалить отсутствия для выбранных записей?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    foreach (StaffModel selectedRow in selectedItems)
                    {
                        await DeleteAbsence(selectedRow);
                    }

                    await UpdateDGV();
                }
            }
            else
            {
                MessageBox.Show("Выберите записи для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #region UI
        private void ExcelBtn_Click(object sender, RoutedEventArgs e)
        {
            ExportToExcelFilterWindow exportToExcelFilterWindow = new ExportToExcelFilterWindow();
            exportToExcelFilterWindow.ShowDialog();
            //DataGridTestWindow dataGridTestWindow = new DataGridTestWindow(SelectedDep);
            //dataGridTestWindow.ShowDialog();
        }
        private void StaffDG_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                DeleteRow();
            }
        }

        private void StaffTBX_GotFocus(object sender, RoutedEventArgs e)
        {
            StaffLV.Visibility = Visibility.Visible;
        }

        private void StaffTBX_LostFocus(object sender, RoutedEventArgs e)
        {
            StaffLV.Visibility = Visibility.Collapsed;
        }

        private void StaffTBX_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SearchStaff();
        }

        private void StaffLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedStaff = (StaffModel)StaffLV.SelectedItem;
            if (SelectedStaff != null)
            {
                StaffTBX.Text = $"{SelectedStaff.SHORT_FIO} ({SelectedStaff.TABEL_ID.Trim()})";
            }
        }

        private void TemplateCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedTemplate = (ScheduleTemplateModel)TemplateCB.SelectedItem;
        }

        private void TemplateCB_DropDownOpened(object sender, EventArgs e)
        {
            UpdateTemplatesList();
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            StaffTBX.Clear();
            StaffLV.SelectedItem = null;
            SelectedStaff = null;
        }

        private void ManageScheduleBtn_Click(object sender, RoutedEventArgs e)
        {
            ScheduleManageWindow scheduleManageWindow = new ScheduleManageWindow();
            scheduleManageWindow.TemplateCreated += ScheduleManageWindow_TemplateCreated;
            scheduleManageWindow.TemplateDeleted += ScheduleManageWindow_TemplateDeleted;
            scheduleManageWindow.ShowDialog();
        }

        private void ScheduleManageWindow_TemplateCreated(object sender, EventArgs e)
        {
            UpdateTemplatesList();
        }

        private void ScheduleManageWindow_TemplateDeleted(object sender, EventArgs e)
        {
            UpdateTemplatesList();
        }

        private void StaffRemoveBtn_Click(object sender, RoutedEventArgs e) => DeleteRow();

        private async void StaffRefreshBtn_Click(object sender, RoutedEventArgs e) => await UpdateDGV();

        #region Search Functionality
        #region Search In DataGrid
        private void SearchTBX_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) => SearchInDG();

        private void SearchInDG()
        {
            List<StaffModel> staff = new List<StaffModel>();
            string txt = SearchTBX.Text;
            if (txt.Length == 0)
                staff = StaffList.ToList();

            switch (filterCMB.SelectedIndex)
            {
                case 0:
                    staff = StaffList.Where(u => u.SHORT_FIO.ToString().ToLower().Contains(txt.ToLower())).ToList();
                    break;
                case 1:
                    staff = StaffList.Where(u => u.TABEL_ID.ToString().ToLower().Contains(txt.ToLower())).ToList();
                    break;
                case 2:
                    staff = StaffList.Where(u => u.DTA.ToString().ToLower().Contains(txt.ToLower())).ToList();
                    break;
                case 3:
                    staff = StaffList.Where(u => u.WorkingHours.ToString().ToLower().Contains(txt.ToLower())).ToList();
                    break;
                default:
                    staff = StaffList.Where(u => u.StaffForSearch.ToLower().Contains(txt.ToLower())).ToList();
                    break;

            };
            StaffDGV.DataSource = staff;
        }

        private void filterCMB_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) => SearchTBX.Clear();

        public void AssignCMB()
        {
            filterCMB.ItemsSource = new filterCMB[]
            {
                new filterCMB { id = 0, filterName = "ФИО" },
                new filterCMB { id = 1, filterName = "табельному номеру" },
                new filterCMB { id = 2, filterName = "дате" },
                new filterCMB { id = 3, filterName = "рабочим часам" }
            };
            filterCMB.SelectedIndex = 0;
        }
        #endregion

        #region Search Staff
        private void SearchStaff()
        {
            List<StaffModel> staff = new List<StaffModel>();
            string txt = StaffTBX.Text;
            if (txt.Length == 0)
                staff = StaffList.ToList();
            staff = StaffListInPosition.Where(u => u.StaffForSearch.ToLower().Contains(txt.ToLower())).ToList();
            StaffLV.ItemsSource = staff;
        }
        #endregion
        #endregion

        private void CauseTBX_GotFocus(object sender, RoutedEventArgs e)
        {
            CauseLV.Visibility = Visibility.Visible;
        }

        private void CauseTBX_LostFocus(object sender, RoutedEventArgs e)
        {
            CauseLV.Visibility = Visibility.Collapsed;
        }

        private void CauseLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedCause = (AbsenceModel)CauseLV.SelectedItem;
            if (SelectedCause != null)
                CauseTBX.Text = SelectedCause.Cause;
        }

        private void ClearAbsenceBtn_Click(object sender, RoutedEventArgs e)
        {
            CauseLV.SelectedItem = null;
            CauseTBX.Clear();
            AbsenceStartDP.SelectedDate = null;
            AbsenceFinishDP.SelectedDate = null;
            AbsenceTimeBeginMTBX.Clear();
            AbsenceTimeEndMTBX.Clear();
        }

        private string CheckWhatToAdd() // # - сотрудник, # - поля графика, # - отсутствие, # - поля отсутствия
        {
            if (StaffLV.SelectedItem != null)
            {
                if (FieldIsFilled("staff") && CauseLV.SelectedItem == null)   // если поля сотрудника заполнены, но отсутствие не выбрано, то добавляем график
                    return "1100";
                else if (FieldIsFilled("staff") == false && CauseLV.SelectedItem != null && FieldIsFilled("absence"))  // если выбран сотрудник, но его поля графика не заполнены и поля отсутствия заполнены, то добавляем только отсутствие
                    return "1011";
                else if (FieldIsFilled("staff") && CauseLV.SelectedItem != null && FieldIsFilled("absence"))  // если поля сотрудника и отсутствия заполнены
                    return "1111";
                else if (CauseLV.SelectedItem == null)
                    return "##0#";   // причина отсутствия не выбрана
                else if (!FieldIsFilled("staff"))
                    return "#0##";
                else if (!FieldIsFilled("absence"))
                    return "###0";
                else
                    return "1608";     // сотрудник выбран, но неизвестная ошибка - отладить код
            }
            else return "0###";  // сотрудник не выбран
        }

        private bool FieldIsFilled(string whichFields)
        {
            if (whichFields == "staff")
            {
                if (TemplateCB.SelectedItem == null || ScheduleStartDP.SelectedDate == null || ScheduleEndDP.SelectedDate == null)
                    return false;
                return true;
            }
            else if (whichFields == "absence")
            {
                if (AbsenceStartDP.SelectedDate == null || AbsenceFinishDP.SelectedDate == null)
                    return false;
                return true;
            }
            else return false;
        }

        private void DP_LostFocus(object sender, RoutedEventArgs e)
        {
            DatePicker datePicker = sender as DatePicker;
            if (datePicker != null)
            {
                if (DateTime.TryParse(datePicker.Text, out DateTime selectedDate))
                    datePicker.SelectedDate = selectedDate;
            }
        }

        private void MTBX_KeyDown(object sender, KeyEventArgs e)
        {
            MaskedTextBox maskedTextBox = sender as MaskedTextBox;

            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.C)
            {
                string textToCopy = maskedTextBox.Text;
                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        Clipboard.SetText(textToCopy);
                        return;
                    }
                    catch { }
                    System.Threading.Thread.Sleep(10);
                }
            }
        }

        private void StaffDG_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SmenZadaniaWindow smenZadaniaWindow = new SmenZadaniaWindow(SelectedStaffInDG);
            smenZadaniaWindow.ShowDialog();
        }

        private bool HasAbsence(int staffId, DateTime date)
        {
            StaffModel absence = Odb.db.Database.SqlQuery<StaffModel>("select * from Zarplats.dbo.Schedule_Absence where id_Staff = @idstaff and DateBegin <= @date and DateEnd >= @date", new SqlParameter("date", date), new SqlParameter("idstaff", staffId)).FirstOrDefault();
            if (absence != null)
                return true;
            else
                return false;
        }

        private void AbsenceStartDP_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            AbsenceFinishDP.IsEnabled = true;
            AbsenceFinishDP.DisplayDateStart = AbsenceStartDP.SelectedDate;
        }

        private void ScheduleStartDP_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ScheduleEndDP.IsEnabled = true;
            ScheduleEndDP.DisplayDateStart = ScheduleStartDP.SelectedDate;
        }
        #endregion

        public void UpdateTemplatesList()
        {
            ScheduleTemplateList = Odb.db.Database.SqlQuery<ScheduleTemplateModel>("select distinct ID_Template, TemplateName, isFlexible, RestingDaysCount, WorkingDaysCount from Zarplats.dbo.Schedule_Template as a").OrderBy(u => u.TemplateName).ToList();
            if (ScheduleTemplateList.Count <= 0)
            {
                TemplateCB.SelectedItem = null;
                TemplateCB.IsEnabled = false;
            }
            else
            {
                TemplateCB.SelectedIndex = 0;
                TemplateCB.IsEnabled = true;
                TemplateCB.ItemsSource = ScheduleTemplateList;
            }
        }

        #region Заполнение графиков
        private async void FillFlexibleSchedule()
        {
            List<ScheduleTemplateModel> flexibleDays = Odb.db.Database.SqlQuery<ScheduleTemplateModel>("select distinct * from Zarplats.dbo.Schedule_FlexibleDays where Template_ID = @templateid", new SqlParameter("templateid", SelectedTemplate.ID_Template)).ToList();

            DateTime selectedStartDate = ScheduleStartDP.SelectedDate ?? DateTime.Now;
            DateTime selectedFinishDate = ScheduleEndDP.SelectedDate ?? DateTime.Now;
            DateTime current = selectedStartDate;

            int flexibleDaysIndex = 0;

            while (current <= selectedFinishDate)
            {
                if (flexibleDaysIndex >= flexibleDays.Count)
                {
                    flexibleDaysIndex = 0;
                    current = current.AddDays(SelectedTemplate.RestingDaysCount);
                }

                var flexibleDay = flexibleDays[flexibleDaysIndex];

                DateTime workBegin = ConvertToDateTime(current, flexibleDay.WorkBegin);
                DateTime workEnd = ConvertToDateTime(current, flexibleDay.WorkEnd);
                DateTime lunchTimeBegin = ConvertToDateTime(current, flexibleDay.LunchTimeBegin);
                DateTime lunchTimeEnd = ConvertToDateTime(current, flexibleDay.LunchTimeEnd);

                var absenceInfo = await CalculateAbsenceHoursForEachDay(SelectedStaff.STAFF_ID, current);
                double totalAbsenceTime = absenceInfo.Item1;
                DateTime absenceStart = absenceInfo.Item2;
                DateTime absenceEnd = absenceInfo.Item3;

                double intersectionTime = CalculateIntersectionTime(lunchTimeBegin, lunchTimeEnd, absenceStart, absenceEnd);
                if (intersectionTime > 0)
                {
                    DateTime intersectionEnd = lunchTimeEnd > absenceEnd ? absenceEnd : lunchTimeEnd;
                    intersectionTime = (intersectionEnd - lunchTimeBegin).TotalHours;
                }
                double workingHours = CalculateWorkingHours(workBegin, workEnd, lunchTimeBegin, lunchTimeEnd, current) - (intersectionTime > 0 ? intersectionTime : totalAbsenceTime);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // проверка существования записи в графике
                    using (SqlCommand checkCommand = new SqlCommand("SELECT COUNT(*) FROM Zarplats.dbo.Staff_Schedule WHERE STAFF_ID = @staffId AND DTA = @date", connection))
                    {
                        checkCommand.Parameters.AddWithValue("@staffId", SelectedStaff.STAFF_ID);
                        checkCommand.Parameters.AddWithValue("@date", current.Date);

                        int existingCount = (int)checkCommand.ExecuteScalar();

                        if (existingCount > 0)
                        {
                            // запись существует, обновление
                            using (SqlCommand updateCommand = new SqlCommand("UPDATE Zarplats.dbo.Staff_Schedule SET WorkBegin = @workBegin, WorkEnd = @workEnd, LunchTimeBegin = @lunchTimeBegin, LunchTimeEnd = @lunchTimeEnd, WorkingHours = @workingHours WHERE STAFF_ID = @staffId AND DTA = @date", connection))
                            {
                                updateCommand.Parameters.AddWithValue("@workBegin", flexibleDay.WorkBegin);
                                updateCommand.Parameters.AddWithValue("@workEnd", flexibleDay.WorkEnd);
                                updateCommand.Parameters.AddWithValue("@lunchTimeBegin", flexibleDay.LunchTimeBegin);
                                updateCommand.Parameters.AddWithValue("@lunchTimeEnd", flexibleDay.LunchTimeEnd);
                                updateCommand.Parameters.AddWithValue("@workingHours", workingHours);
                                updateCommand.Parameters.AddWithValue("@staffId", SelectedStaff.STAFF_ID);
                                updateCommand.Parameters.AddWithValue("@date", current.Date);

                                updateCommand.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // записи нет, добавление новой записи
                            using (SqlCommand insertCommand = new SqlCommand("INSERT INTO Zarplats.dbo.Staff_Schedule (WorkBegin, WorkEnd, DTA, STAFF_ID, LunchTimeBegin, LunchTimeEnd, WorkingHours) VALUES (@workBegin, @workEnd, @date, @staffId, @lunchTimeBegin, @lunchTimeEnd, @workingHours)", connection))
                            {
                                insertCommand.Parameters.AddWithValue("@workBegin", flexibleDay.WorkBegin);
                                insertCommand.Parameters.AddWithValue("@workEnd", flexibleDay.WorkEnd);
                                insertCommand.Parameters.AddWithValue("@date", current.Date);
                                insertCommand.Parameters.AddWithValue("@staffId", SelectedStaff.STAFF_ID);
                                insertCommand.Parameters.AddWithValue("@lunchTimeBegin", flexibleDay.LunchTimeBegin);
                                insertCommand.Parameters.AddWithValue("@lunchTimeEnd", flexibleDay.LunchTimeEnd);
                                insertCommand.Parameters.AddWithValue("@workingHours", workingHours);

                                insertCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }

                flexibleDaysIndex++;
                current = current.AddDays(1);
            }
            MessageBox.Show($"График заполнен!");
            await UpdateDGV();
        }

        private async void FillStaticSchedule()
        {
            DateTime selectedStartDate = ScheduleStartDP.SelectedDate ?? DateTime.Now;
            DateTime selectedFinishDate = ScheduleEndDP.SelectedDate ?? DateTime.Now;
            DateTime current = selectedStartDate;

            List<ScheduleTemplateModel> Days = GetDaysInfo(SelectedTemplate.ID_Template);

            DayOfWeek startDayOfWeek = selectedStartDate.DayOfWeek;
            var currentDay = Days.FirstOrDefault(d => d.Day == startDayOfWeek.ToString());

            while (current <= selectedFinishDate)
            {
                if (currentDay != null && !currentDay.isRestingDay)
                {
                    DateTime workBegin = ConvertToDateTime(current, currentDay.WorkBegin);
                    DateTime workEnd = ConvertToDateTime(current, currentDay.WorkEnd);
                    DateTime lunchTimeBegin = ConvertToDateTime(current, currentDay.LunchTimeBegin);
                    DateTime lunchTimeEnd = ConvertToDateTime(current, currentDay.LunchTimeEnd);

                    var absenceInfo = await CalculateAbsenceHoursForEachDay(SelectedStaff.STAFF_ID, current);
                    double totalAbsenceTime = absenceInfo.Item1;
                    DateTime absenceStart = absenceInfo.Item2;
                    DateTime absenceEnd = absenceInfo.Item3;

                    double intersectionTime = CalculateIntersectionTime(lunchTimeBegin, lunchTimeEnd, absenceStart, absenceEnd);
                    if (intersectionTime > 0)
                    {
                        DateTime intersectionEnd = lunchTimeEnd > absenceEnd ? absenceEnd : lunchTimeEnd;
                        intersectionTime = (intersectionEnd - lunchTimeBegin).TotalHours;
                    }
                    double workingHours = CalculateWorkingHours(workBegin, workEnd, lunchTimeBegin, lunchTimeEnd, current) - (intersectionTime > 0 ? intersectionTime : totalAbsenceTime);

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        // проверка существования записи в графике
                        using (SqlCommand checkCommand = new SqlCommand("SELECT COUNT(*) FROM Zarplats.dbo.Staff_Schedule WHERE STAFF_ID = @staffId AND DTA = @date", connection))
                        {
                            checkCommand.Parameters.AddWithValue("@staffId", SelectedStaff.STAFF_ID);
                            checkCommand.Parameters.AddWithValue("@date", current.Date);

                            int existingCount = (int)checkCommand.ExecuteScalar();

                            if (existingCount > 0)
                            {
                                // запись существует, обновление
                                using (SqlCommand updateCommand = new SqlCommand("UPDATE Zarplats.dbo.Staff_Schedule SET WorkBegin = @workBegin, WorkEnd = @workEnd, LunchTimeBegin = @lunchTimeBegin, LunchTimeEnd = @lunchTimeEnd, WorkingHours = @workingHours WHERE STAFF_ID = @staffId AND DTA = @date", connection))
                                {
                                    updateCommand.Parameters.AddWithValue("@workBegin", currentDay.WorkBegin);
                                    updateCommand.Parameters.AddWithValue("@workEnd", currentDay.WorkEnd);
                                    updateCommand.Parameters.AddWithValue("@lunchTimeBegin", currentDay.LunchTimeBegin);
                                    updateCommand.Parameters.AddWithValue("@lunchTimeEnd", currentDay.LunchTimeEnd);
                                    updateCommand.Parameters.AddWithValue("@workingHours", workingHours);
                                    updateCommand.Parameters.AddWithValue("@staffId", SelectedStaff.STAFF_ID);
                                    updateCommand.Parameters.AddWithValue("@date", current.Date);

                                    updateCommand.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                // записи нет, добавление новой записи
                                using (SqlCommand insertCommand = new SqlCommand("INSERT INTO Zarplats.dbo.Staff_Schedule (WorkBegin, WorkEnd, DTA, STAFF_ID, LunchTimeBegin, LunchTimeEnd, WorkingHours) VALUES (@workBegin, @workEnd, @date, @staffId, @lunchTimeBegin, @lunchTimeEnd, @workingHours)", connection))
                                {
                                    insertCommand.Parameters.AddWithValue("@workBegin", currentDay.WorkBegin);
                                    insertCommand.Parameters.AddWithValue("@workEnd", currentDay.WorkEnd);
                                    insertCommand.Parameters.AddWithValue("@date", current.Date);
                                    insertCommand.Parameters.AddWithValue("@staffId", SelectedStaff.STAFF_ID);
                                    insertCommand.Parameters.AddWithValue("@lunchTimeBegin", currentDay.LunchTimeBegin);
                                    insertCommand.Parameters.AddWithValue("@lunchTimeEnd", currentDay.LunchTimeEnd);
                                    insertCommand.Parameters.AddWithValue("@workingHours", workingHours);

                                    insertCommand.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }

                current = current.AddDays(1);
                currentDay = Days.FirstOrDefault(d => d.Day == current.DayOfWeek.ToString());   // переход к следующему дню недели в записях из базы данных
            }
            MessageBox.Show($"График заполнен!");
            await UpdateDGV();
        }

        public List<ScheduleTemplateModel> GetDaysInfo(int templateid) // информация о каждом дне в статик таблице
        {
            List<ScheduleTemplateModel> staticDaysList = new List<ScheduleTemplateModel>();
            staticDaysList = Odb.db.Database.SqlQuery<ScheduleTemplateModel>("select distinct * from Zarplats.dbo.Schedule_StaticDays where Template_ID = @templateid", new SqlParameter("templateid", templateid)).ToList();
            return staticDaysList;
        }

        public DateTime ConvertToDateTime(DateTime date, string time)
        {
            DateTime dateTime;
            if (DateTime.TryParseExact(time, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
            {
                return new DateTime(date.Year, date.Month, date.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
            }
            return DateTime.MinValue;
        }
        #endregion

        #region Работа с отсутствиями
        private async Task AddAbsence()
        {
            string timeBeginValue = AbsenceTimeBeginMTBX.Text.Any(char.IsDigit) ? AbsenceTimeBeginMTBX.Text : string.Empty;
            string timeEndValue = AbsenceTimeEndMTBX.Text.Any(char.IsDigit) ? AbsenceTimeEndMTBX.Text : string.Empty;

            int checkExistingAbsence = Odb.db.Database.SqlQuery<int>("IF EXISTS (SELECT * FROM Zarplats.dbo.Schedule_Absence WHERE DateBegin <= @DateEnd AND DateEnd >= @DateBegin AND id_Staff = @staffid) SELECT 1 ELSE SELECT 0", new SqlParameter("DateBegin", AbsenceStartDP.SelectedDate), new SqlParameter("DateEnd", AbsenceFinishDP.SelectedDate), new SqlParameter("staffid", SelectedStaff.STAFF_ID)).SingleOrDefault();
            if (!Convert.ToBoolean(checkExistingAbsence))
            {
                Odb.db.Database.ExecuteSqlCommand("INSERT INTO Zarplats.dbo.Schedule_Absence (AbsenceRef_ID, id_Staff, DateBegin, DateEnd, TimeBegin, TimeEnd) VALUES (@AbsenceRef_ID, @staffid, @DateBegin, @DateEnd, @TimeBegin, @TimeEnd)",
                    new SqlParameter("AbsenceRef_ID", SelectedCause.ID_AbsenceRef), new SqlParameter("staffid", SelectedStaff.STAFF_ID), new SqlParameter("DateBegin", AbsenceStartDP.SelectedDate), new SqlParameter("DateEnd", AbsenceFinishDP.SelectedDate), new SqlParameter("TimeBegin", timeBeginValue), new SqlParameter("TimeEnd", timeEndValue));

                DateTime absenceStart = AbsenceStartDP.SelectedDate ?? DateTime.Now;
                DateTime absenceEnd = AbsenceFinishDP.SelectedDate ?? DateTime.Now;

                // обновление рабочих часов
                await UpdateWorkingHours(new List<StaffModel> { SelectedStaff }, absenceStart, absenceEnd);
                MessageBox.Show("Отсутствие добавлено!");

                await UpdateDGV();
            }
            else
            {
                MessageBox.Show("В указанном периоде уже существует отсутствие!");
            }
        }

        private async Task DeleteAbsence(StaffModel selectedRow)
        {
            // период отсутствия
            DateTime absenceStart = selectedRow.DateBegin ?? DateTime.Now;
            DateTime absenceEnd = selectedRow.DateEnd ?? DateTime.Now;

            // удаление отсутствия
            await Task.Run(() => { Odb.db.Database.ExecuteSqlCommand("DELETE FROM Zarplats.dbo.Schedule_Absence WHERE ID_Absence = @ID_Absence", new SqlParameter("ID_Absence", selectedRow.ID_Absence)); });

            // обновление рабочих часов для всех дней, затронутых удаленным отсутствием
            Console.WriteLine("Начали обновлять рабочие часы для периода");
            await UpdateWorkingHours(StaffDGV.SelectedRows.Cast<StaffModel>().ToList(), absenceStart, absenceEnd);
            Console.WriteLine("Закончили обновлять рабочие часы для периода");
        }


        #endregion

        #region Вычисления рабочих часов
        private async Task UpdateWorkingHours(List<StaffModel> selectedStaffList, DateTime startDate, DateTime endDate)
        {
            Console.WriteLine("Получаем список затронутых строк");
            List<StaffModel> affectedRows = await GetAffectedRowsAsync(startDate, endDate);
            Console.WriteLine("Закончили получать список затронутых строк");

            foreach (StaffModel selectedStaff in selectedStaffList)
            {
                foreach (StaffModel row in affectedRows)
                {
                    DateTime workBegin = ConvertToDateTime(row.DTA, row.WorkBegin);
                    DateTime workEnd = ConvertToDateTime(row.DTA, row.WorkEnd);
                    DateTime lunchTimeBegin = ConvertToDateTime(row.DTA, row.LunchTimeBegin);
                    DateTime lunchTimeEnd = ConvertToDateTime(row.DTA, row.LunchTimeEnd);

                    var absenceInfo = await CalculateAbsenceHoursForEachDay(row.STAFF_ID, row.DTA);
                    double totalAbsenceTime = absenceInfo.Item1;
                    DateTime absenceStart = absenceInfo.Item2;
                    DateTime absenceEnd = absenceInfo.Item3;

                    double intersectionTime = CalculateIntersectionTime(lunchTimeBegin, lunchTimeEnd, absenceStart, absenceEnd);
                    if (intersectionTime > 0)
                    {
                        DateTime intersectionEnd = lunchTimeEnd > absenceEnd ? absenceEnd : lunchTimeEnd;
                        intersectionTime = (intersectionEnd - lunchTimeBegin).TotalHours;
                    }
                    double workingHours = CalculateWorkingHours(workBegin, workEnd, lunchTimeBegin, lunchTimeEnd, row.DTA) - (intersectionTime > 0 ? intersectionTime : totalAbsenceTime);

                    await Odb.db.Database.ExecuteSqlCommandAsync("update Zarplats.dbo.Staff_Schedule set WorkingHours = @workingHours where ID_Schedule = @id", new SqlParameter("workingHours", workingHours), new SqlParameter("id", row.ID_Schedule));
                }
            };
        }

        private async Task<List<StaffModel>> GetAffectedRowsAsync(DateTime absenceBegin, DateTime absenceEnd)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                var query = $"SELECT DISTINCT a.ID_Schedule, a.STAFF_ID, LTRIM(e.TABEL_ID) as TABEL_ID, e.SHORT_FIO, a.WorkBegin, a.WorkEnd, a.DTA, a.LunchTimeBegin, a.LunchTimeEnd, a.WorkingHours, b.ID_Absence, c.Cause as CauseAbsence, b.DateBegin, b.DateEnd, b.TimeBegin, b.TimeEnd FROM [Zarplats].[dbo].[Staff_Schedule] as a left join Zarplats.dbo.Schedule_Absence as b on a.STAFF_ID = b.id_Staff and a.DTA between b.DateBegin and b.DateEnd left join Zarplats.dbo.AbsenceRef as c on b.AbsenceRef_ID = c.ID_AbsenceRef left join perco...staff as e on a.STAFF_ID = e.ID_STAFF left join Zarplats.dbo.StaffView as f on a.STAFF_ID = f.STAFF_ID where f.Position = @podrazd and a.DTA between @absenceBegin and @absenceEnd order by a.DTA";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@podrazd", SelectedDep.Position);
                    command.Parameters.AddWithValue("@absenceBegin", absenceBegin);
                    command.Parameters.AddWithValue("@absenceEnd", absenceEnd);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var affectedRows = new List<StaffModel>();

                        while (await reader.ReadAsync())
                        {
                            StaffModel staff = new StaffModel
                            {
                                ID_Schedule = reader.GetInt32(0),
                                STAFF_ID = reader.GetInt32(1),
                                TABEL_ID = reader.GetString(2),
                                SHORT_FIO = reader.GetString(3),
                                WorkBegin = reader.GetString(4),
                                WorkEnd = reader.GetString(5),
                                DTA = reader.GetDateTime(6),
                                LunchTimeBegin = reader.GetString(7),
                                LunchTimeEnd = reader.GetString(8),
                                WorkingHours = reader.GetDouble(9),
                                ID_Absence = reader.IsDBNull(10) ? 0 : reader.GetInt32(10),
                                CauseAbsence = reader.IsDBNull(11) ? string.Empty : reader.GetString(11),
                                DateBegin = reader.IsDBNull(12) ? (DateTime?)null : reader.GetDateTime(12),
                                DateEnd = reader.IsDBNull(13) ? (DateTime?)null : reader.GetDateTime(13),
                                TimeBegin = reader.IsDBNull(14) ? string.Empty : reader.GetString(14),
                                TimeEnd = reader.IsDBNull(15) ? string.Empty : reader.GetString(15)
                            };
                            affectedRows.Add(staff);
                        }

                        return affectedRows;
                    }
                }
            }
        }

        private double CalculateWorkingHours(DateTime workBegin, DateTime workEnd, DateTime lunchTimeBegin, DateTime lunchTimeEnd, DateTime date)
        {
            double totalWorkingHours = (workEnd - workBegin).TotalHours;
            double lunchTime = (lunchTimeEnd - lunchTimeBegin).TotalHours;
            //double absenceHours = CalculateAbsenceHours(SelectedStaff.STAFF_ID, date, date);

            return totalWorkingHours - lunchTime;
        }

        private double CalculateIntersectionTime(DateTime workLunchStart, DateTime workLunchEnd, DateTime absenceStart, DateTime absenceEnd)
        {
            // Находим максимальное начальное время и минимальное конечное время, чтобы определить пересечение
            DateTime intersectionStart = workLunchStart > absenceStart ? workLunchStart : absenceStart;
            DateTime intersectionEnd = workLunchEnd < absenceEnd ? workLunchEnd : absenceEnd;

            // Проверяем, есть ли пересечение
            if (intersectionStart < intersectionEnd)
            {
                // Рассчитываем продолжительность пересечения
                double intersectionHours = (intersectionEnd - intersectionStart).TotalHours;
                return intersectionHours;
            }

            // Если пересечения нет, возвращаем 0
            return 0;
        }

        private async Task<Tuple<double, DateTime, DateTime>> CalculateAbsenceHoursForEachDay(int staffId, DateTime currentDate)
        {
            double totalAbsenceTime = 0;
            DateTime absenceStart = DateTime.MinValue;
            DateTime absenceEnd = DateTime.MinValue;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT TimeBegin, TimeEnd FROM Zarplats.dbo.Schedule_Absence WHERE id_Staff = @staffId AND DateBegin <= @currentDate AND DateEnd >= @currentDate";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@staffId", staffId);
                    command.Parameters.AddWithValue("@currentDate", currentDate);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            DateTime currentAbsenceTimeBegin = ConvertToDateTime(currentDate, reader["TimeBegin"].ToString());
                            DateTime currentAbsenceTimeEnd = ConvertToDateTime(currentDate, reader["TimeEnd"].ToString());

                            // Обновление времени начала и окончания отсутствия
                            if (absenceStart == DateTime.MinValue || currentAbsenceTimeBegin < absenceStart)
                            {
                                absenceStart = currentAbsenceTimeBegin;
                            }

                            if (absenceEnd == DateTime.MinValue || currentAbsenceTimeEnd > absenceEnd)
                            {
                                absenceEnd = currentAbsenceTimeEnd;
                            }

                            totalAbsenceTime += (currentAbsenceTimeEnd - currentAbsenceTimeBegin).TotalHours;
                        }
                    }
                }
            }

            return new Tuple<double, DateTime, DateTime>(totalAbsenceTime, absenceStart, absenceEnd);
        }
        #endregion
    }
}
