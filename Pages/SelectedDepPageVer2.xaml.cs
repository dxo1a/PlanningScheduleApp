using PlanningScheduleApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using Application = System.Windows.Application;
using Clipboard = System.Windows.Clipboard;
using ContextMenu = System.Windows.Forms.ContextMenu;
using Cursor = System.Windows.Input.Cursor;
using Cursors = System.Windows.Input.Cursors;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MaskedTextBox = Xceed.Wpf.Toolkit.MaskedTextBox;
using MenuItem = System.Windows.Forms.MenuItem;
using MessageBox = System.Windows.MessageBox;

namespace PlanningScheduleApp.Pages
{
    public partial class SelectedDepPageVer2 : System.Windows.Controls.Page
    {
        #region Переменные
        public string connectionString = "Persist Security Info=False;User ID=sa; Password=server_esa;Initial Catalog=dsl_sp;Server=sql";
        private BindingSource staffBindingSource = new BindingSource();

        #region StaffModels
        //надо нормально распределить значения по соответствующим моделям
        public BindingList<StaffModel> StaffList = new BindingList<StaffModel>();
        private StaffModel SelectedRow { get; set; }
        List<StaffModel> StaffListInPosition = new List<StaffModel>();
        List<StaffModel> tempList = new List<StaffModel>();
        StaffModel SelectedStaff { get; set; }
        StaffModel SelectedStaffForAbsence { get; set; }
        #endregion

        #region ScheduleTemplateModel
        List<ScheduleTemplateModel> ScheduleTemplateList = new List<ScheduleTemplateModel>();
        ScheduleTemplateModel SelectedTemplate { get; set; }
        #endregion

        #region AbsenceModel
        List<AbsenceModel> CauseList = new List<AbsenceModel>();
        AbsenceModel SelectedCause { get; set; }
        #endregion

        #region Vanilla
        public double WorkingHours, CalculatedLunchTime, LunchTime;
        private DateTime fromDate;
        #endregion

        private DepModel SelectedDep { get; set; }
        #endregion

        public SelectedDepPageVer2(DepModel selectedDep)
        {
            InitializeComponent();

            //SetDoubleBuffered(StaffDGV, true);

            SelectedDep = selectedDep;

            AssignCMB();
            UpdateTemplatesList();

            Application.Current.Dispatcher.Invoke(() =>
            {
                StaffListInPosition = Odb.db.Database.SqlQuery<StaffModel>("SELECT DISTINCT b.SHORT_FIO, b.TABEL_ID, b.ID_STAFF AS STAFF_ID FROM perco...staff_ref AS a LEFT JOIN perco...staff AS b ON a.STAFF_ID = b.ID_STAFF LEFT JOIN perco...subdiv_ref AS c ON a.SUBDIV_ID = c.ID_REF WHERE c.DISPLAY_NAME = @podrazd AND b.VALID = 1 UNION ALL SELECT DISTINCT b.SHORT_FIO, b.TABEL_ID, b.ID_STAFF AS STAFF_ID FROM perco...staff_ref AS a LEFT JOIN perco...staff AS b ON a.STAFF_ID = b.ID_STAFF LEFT JOIN perco...subdiv_ref AS c ON a.SUBDIV_ID = c.ID_REF WHERE b.ID_STAFF = 5985 AND @podrazd = 'Трансмаш - Отдел технической поддержки и администрирования';", new SqlParameter("podrazd", SelectedDep.Position)).OrderBy(s => s.SHORT_FIO).ToList();
                StaffLV.ItemsSource = StaffListInPosition;
                StaffAbsenceLV.ItemsSource = StaffListInPosition;
                UpdateCauseList();

                AbsenceStartDP.SelectedDate = DateTime.Now;
                AbsenceFinishDP.SelectedDate = DateTime.Now;

                StaffDGV.Font = new Font(new FontFamily("Segoe UI"), 9);
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
             await UpdateDGVAsync();
        }

        private async Task<List<StaffModel>> LoadDataAsync()
        {
            Console.WriteLine("Обновление данных...");
            Dispatcher.Invoke(() => StatusTB.Text = "Обновление данных...");

            tempList = new List<StaffModel>();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("SELECT DISTINCT a.ID_Schedule, a.STAFF_ID, LTRIM(e.TABEL_ID) as TABEL_ID, e.SHORT_FIO, a.WorkBegin, a.WorkEnd, a.DTA, a.LunchTimeBegin, a.LunchTimeEnd, a.WorkingHours, b.ID_Absence, c.Cause as CauseAbsence, b.DateBegin, b.DateEnd, b.TimeBegin, b.TimeEnd, f.Subdivision FROM [Zarplats].[dbo].[Staff_Schedule] as a left join Zarplats.dbo.Schedule_Absence as b on a.STAFF_ID = b.id_Staff and a.DTA between b.DateBegin and b.DateEnd left join Zarplats.dbo.AbsenceRef as c on b.AbsenceRef_ID = c.ID_AbsenceRef left join perco...staff as e on a.STAFF_ID = e.ID_STAFF left join Zarplats.dbo.StaffView as f on a.STAFF_ID = f.STAFF_ID where f.Position = @podrazd order by a.DTA", connection))
                    {
                        command.Parameters.AddWithValue("podrazd", SelectedDep.Position);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                StaffModel staffModel = new StaffModel
                                {
                                    ID_Schedule = reader.GetFieldValue<int>(0),
                                    STAFF_ID = reader.GetFieldValue<int>(1),
                                    TABEL_ID = reader.GetFieldValue<string>(2),
                                    SHORT_FIO = reader.GetFieldValue<string>(3),
                                    WorkBegin = reader.GetFieldValue<string>(4),
                                    WorkEnd = reader.GetFieldValue<string>(5),
                                    DTA = reader.GetFieldValue<DateTime>(6),
                                    LunchTimeBegin = reader.GetFieldValue<string>(7),
                                    LunchTimeEnd = reader.GetFieldValue<string>(8),
                                    WorkingHours = reader.GetFieldValue<double>(9),
                                    ID_Absence = reader.IsDBNull(10) ? (int?)null : reader.GetFieldValue<int>(10),
                                    CauseAbsence = reader.IsDBNull(11) ? null : reader.GetFieldValue<string>(11),
                                    DateBegin = reader.IsDBNull(12) ? (DateTime?)null : reader.GetFieldValue<DateTime>(12),
                                    DateEnd = reader.IsDBNull(13) ? (DateTime?)null : reader.GetFieldValue<DateTime>(13),
                                    TimeBegin = reader.IsDBNull(14) ? null : reader.GetFieldValue<string>(14),
                                    TimeEnd = reader.IsDBNull(15) ? null : reader.GetFieldValue<string>(15),
                                    Subdivision = reader.IsDBNull(16) ? null : reader.GetFieldValue<string>(16)
                                };

                                tempList.Add(staffModel);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
            Console.WriteLine($"Обновление данных завершено, tempListCount: {tempList.Count}");
            Dispatcher.Invoke(() => StatusTB.Text = "Обновление данных завершено.");
            return tempList;
        }

        private async Task<List<StaffModel>> LoadDataForAffectedCellsAsync(DateTime absenceBegin, DateTime absenceEnd)
        {
            Console.WriteLine($"Обновление данных для затронутых строк в периоде: {absenceBegin.Date.ToShortDateString()} - {absenceEnd.Date.ToShortDateString()}...");
            Dispatcher.Invoke(() => StatusTB.Text = $"Обновление данных...");
            tempList = new List<StaffModel>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("SELECT DISTINCT a.ID_Schedule, a.STAFF_ID, LTRIM(e.TABEL_ID) as TABEL_ID, e.SHORT_FIO, a.WorkBegin, a.WorkEnd, a.DTA, a.LunchTimeBegin, a.LunchTimeEnd, a.WorkingHours, b.ID_Absence, c.Cause as CauseAbsence, b.DateBegin, b.DateEnd, b.TimeBegin, b.TimeEnd, f.Subdivision FROM [Zarplats].[dbo].[Staff_Schedule] as a left join Zarplats.dbo.Schedule_Absence as b on a.STAFF_ID = b.id_Staff and a.DTA between b.DateBegin and b.DateEnd left join Zarplats.dbo.AbsenceRef as c on b.AbsenceRef_ID = c.ID_AbsenceRef left join perco...staff as e on a.STAFF_ID = e.ID_STAFF left join Zarplats.dbo.StaffView as f on a.STAFF_ID = f.STAFF_ID where f.Position = @podrazd and a.DTA between @absenceBegin and @absenceEnd order by a.DTA", connection))
                    {
                        command.Parameters.AddWithValue("podrazd", SelectedDep.Position);
                        command.Parameters.AddWithValue("absenceBegin", absenceBegin);
                        command.Parameters.AddWithValue("absenceEnd", absenceEnd);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                StaffModel staffModel = new StaffModel
                                {
                                    ID_Schedule = reader.GetFieldValue<int>(0),
                                    STAFF_ID = reader.GetFieldValue<int>(1),
                                    TABEL_ID = reader.GetFieldValue<string>(2),
                                    SHORT_FIO = reader.GetFieldValue<string>(3),
                                    WorkBegin = reader.GetFieldValue<string>(4),
                                    WorkEnd = reader.GetFieldValue<string>(5),
                                    DTA = reader.GetFieldValue<DateTime>(6),
                                    LunchTimeBegin = reader.GetFieldValue<string>(7),
                                    LunchTimeEnd = reader.GetFieldValue<string>(8),
                                    WorkingHours = reader.GetFieldValue<double>(9),
                                    ID_Absence = reader.IsDBNull(10) ? (int?)null : reader.GetFieldValue<int>(10),
                                    CauseAbsence = reader.IsDBNull(11) ? null : reader.GetFieldValue<string>(11),
                                    DateBegin = reader.IsDBNull(12) ? (DateTime?)null : reader.GetFieldValue<DateTime>(12),
                                    DateEnd = reader.IsDBNull(13) ? (DateTime?)null : reader.GetFieldValue<DateTime>(13),
                                    TimeBegin = reader.IsDBNull(14) ? null : reader.GetFieldValue<string>(14),
                                    TimeEnd = reader.IsDBNull(15) ? null : reader.GetFieldValue<string>(15),
                                    Subdivision = reader.IsDBNull(16) ? null : reader.GetFieldValue<string>(16)
                                };

                                tempList.Add(staffModel);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
            Console.WriteLine($"Обновление данных для затронутых строк в периоде завершено, tempListCount: {tempList.Count}");
            Dispatcher.Invoke(() => StatusTB.Text = "Обновление данных завершено.");
            return tempList;
        }

        private async Task UpdateDGVAsync()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    Mouse.OverrideCursor = Cursors.AppStarting;
                    StatusTB.Text = "Обновление таблицы...";
                    App.DisableAllWindows();
                });

                Console.WriteLine("Обновление таблицы...");

                typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.NonPublic |
                BindingFlags.Instance | BindingFlags.SetProperty, null,
                StaffDGV, new object[] { true });

                DateTime selectedMonth;

                await Dispatcher.InvokeAsync(() =>
                {
                    selectedMonth = DateTime.ParseExact(MonthCMB.SelectedItem.ToString(), "MMMM yyyy", CultureInfo.CurrentCulture);
                    fromDate = selectedMonth;
                });

                // Асинхронно загружаем данные
                tempList = await LoadDataAsync();

                List<StaffModel> groupedData = tempList
                    .AsParallel()
                    .GroupBy(x => x.STAFF_ID)
                    .Select(g => new StaffModel
                    {
                        STAFF_ID = g.Key,
                        SHORT_FIO = g.First().SHORT_FIO,
                        TABEL_ID = g.First().TABEL_ID,
                        Subdivision = g.First().Subdivision,

                        // то что зависит от даты
                        DatesAndSchedules = g.AsParallel().Select(item => new DateAndSchedule
                        {
                            DTA = item.DTA,
                            ID_Schedule = item.ID_Schedule,
                            WorkingHours = item.WorkingHours,
                            TimeBegin = item.TimeBegin,
                            TimeEnd = item.TimeEnd,
                            WorkBegin = item.WorkBegin,
                            WorkEnd = item.WorkEnd,
                            LunchTimeBegin = item.LunchTimeBegin,
                            LunchTimeEnd = item.LunchTimeEnd,
                            DateBegin = item.DateBegin ?? DateTime.Now,
                            DateEnd = item.DateEnd ?? DateTime.Now
                        }).ToList()
                    })
                    .Where(staff => staff.DatesAndSchedules.Any(ds => ds.DTA.Month == fromDate.Month))
                    .ToList();

                await Dispatcher.InvokeAsync(() =>
                {
                    StaffList.Clear();

                    foreach (var staff in groupedData)
                    {
                        StaffList.Add(staff);
                    }

                    #region Создание DataGridView
                    DateTime currentDate = fromDate;
                    int daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

                    StaffDGV.Columns.Clear();

                    DataGridViewTextBoxColumn staffTabelColumn = new DataGridViewTextBoxColumn();
                    staffTabelColumn.HeaderText = "Таб. номер";
                    staffTabelColumn.DataPropertyName = "TABEL_ID";
                    staffTabelColumn.MinimumWidth = 100;
                    StaffDGV.Columns.Add(staffTabelColumn);

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
                    #endregion

                    staffBindingSource.DataSource = StaffList;
                    StaffDGV.DataSource = staffBindingSource;

                    Task.Run(() => FillDGVCells());

                    #region Форматирование и обработчики
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
                        if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                        {
                            DataGridViewCell cell = StaffDGV.Rows[e.RowIndex].Cells[e.ColumnIndex];

                            if (e.ColumnIndex == 0 || e.ColumnIndex == 1)
                            {
                                DataGridViewRow row = StaffDGV.Rows[e.RowIndex];
                                StaffModel staff = (StaffModel)row.DataBoundItem;

                                if (staff != null)
                                {
                                    string toolTipText = $"{staff.SHORT_FIO}\nТабельный номер: {staff.TABEL_ID}\nДолжность: {staff.Subdivision}\n\n(Двойной клик чтобы выделить все дни)";

                                    StaffDGV.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = toolTipText;
                                }
                            }
                            else if (cell.Value is DateAndSchedule dateAndSchedule)
                            {
                                if (dateAndSchedule.ToString() == "Р")
                                    StaffDGV.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "Работает";
                                else
                                    StaffDGV.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "Не работает";
                            }
                        }
                    };

                    StaffDGV.CellMouseClick += (s, e) =>
                    {
                        if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex >= 0)
                        {
                            ContextMenu contextMenu = new ContextMenu();

                            contextMenu.MenuItems.Add(new MenuItem("Сменные задания", ContextMenuItem_Click));

                            Rectangle cellRectangle = StaffDGV.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);

                            contextMenu.Show(StaffDGV, new System.Drawing.Point(cellRectangle.Right, cellRectangle.Top));
                        }
                    };
                    #endregion
                    StatusTB.Text = "Таблица обновлена.";
                });
                Console.WriteLine("Обновление таблицы завершено.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении таблицы: {ex.Message}");
                Dispatcher.Invoke(() =>
                {
                    Mouse.OverrideCursor = null;
                    App.EnableAllWindows();
                });
            }
            finally
            {
                Dispatcher.Invoke(() =>
                {
                    Mouse.OverrideCursor = null;
                    App.EnableAllWindows();
                });
            }
        }

        private void SearchTBX_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string searchText = SearchTBX.Text.ToLower();

                var groupedData = tempList
                    .Where(x => (filterCMB.SelectedIndex == 0 && x.SHORT_FIO.ToLower().Contains(searchText)) ||
                          (filterCMB.SelectedIndex == 1 && x.TABEL_ID.ToLower().Contains(searchText)))
                    .GroupBy(x => x.STAFF_ID)
                    .Select(g => new StaffModel
                    {
                        STAFF_ID = g.Key,
                        SHORT_FIO = g.First().SHORT_FIO,
                        TABEL_ID = g.First().TABEL_ID,
                        Subdivision = g.First().Subdivision,

                        // то что зависит от даты
                        DatesAndSchedules = g.Select(item => new DateAndSchedule
                        {
                            DTA = item.DTA,
                            ID_Schedule = item.ID_Schedule,
                            WorkingHours = item.WorkingHours,
                            TimeBegin = item.TimeBegin,
                            TimeEnd = item.TimeEnd,
                            WorkBegin = item.WorkBegin,
                            WorkEnd = item.WorkEnd,
                            LunchTimeBegin = item.LunchTimeBegin,
                            LunchTimeEnd = item.LunchTimeEnd,
                            DateBegin = item.DateBegin ?? DateTime.Now,
                            DateEnd = item.DateEnd ?? DateTime.Now
                        }).ToList()
                    })
                    .Where(staff => staff.DatesAndSchedules.Any(ds => ds.DTA.Month == fromDate.Month))
                    .ToList();

                Dispatcher.Invoke(() =>
                {
                    StaffList.Clear();
                    foreach (var staff in groupedData)
                    {
                        StaffList.Add(staff);
                    }
                });

                staffBindingSource.DataSource = StaffList;

                Task.Run(() => FillDGVCells());
            }
        }

        public async Task UpdateCellAsync(int rowIndex, int columnIndex)
        {
            try
            {
                if (StaffList.Count > rowIndex && rowIndex >= 0)
                {
                    DateTime currentDate = fromDate;
                    DateTime currentDateForCell = new DateTime(currentDate.Year, currentDate.Month, columnIndex - 1);

                    StaffModel staff = StaffList[rowIndex];

                    bool isRecordExists = CheckRecordExists(staff.STAFF_ID, currentDateForCell);
                    int hasAbsenceFullDay;
                    int? idSchedule;
                    int? idAbsence;

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();

                        using (SqlCommand command1 = new SqlCommand("select count(*) from Zarplats.dbo.Schedule_Absence where id_Staff = @idstaff and DateBegin <= @date and DateEnd >= @date and TimeBegin is null and TimeEnd is null", connection))
                        {
                            command1.Parameters.AddWithValue("@date", currentDateForCell.Date);
                            command1.Parameters.AddWithValue("@idstaff", staff.STAFF_ID);
                            hasAbsenceFullDay = await command1.ExecuteScalarAsync() as int? ?? 0;
                        }

                        using (SqlCommand command2 = new SqlCommand("select ID_Schedule from Zarplats.dbo.Staff_Schedule where STAFF_ID = @idstaff and DTA = @date", connection))
                        {
                            command2.Parameters.AddWithValue("@idstaff", staff.STAFF_ID);
                            command2.Parameters.AddWithValue("@date", currentDateForCell.Date);
                            idSchedule = await command2.ExecuteScalarAsync() as int?;
                        }

                        using (SqlCommand command3 = new SqlCommand("select b.ID_Absence from Zarplats.dbo.Staff_Schedule as a left join Zarplats.dbo.Schedule_Absence as b on a.STAFF_ID = b.id_Staff and a.DTA between b.DateBegin and b.DateEnd where STAFF_ID = @idstaff and DTA = @date", connection))
                        {
                            command3.Parameters.AddWithValue("@idstaff", staff.STAFF_ID);
                            command3.Parameters.AddWithValue("@date", currentDateForCell.Date);
                            idAbsence = await command3.ExecuteScalarAsync() as int?;
                        }
                    }

                    DateAndSchedule cellValue;

                    if (isRecordExists)
                    {
                        cellValue = new DateAndSchedule
                        {
                            ID_Schedule = idSchedule ?? 0,
                            ID_Absence = idAbsence ?? 0,
                            DTA = currentDateForCell,
                            TimeBegin = staff.TimeBegin,
                            TimeEnd = staff.TimeEnd,
                            WorkBegin = staff.WorkBegin,
                            WorkEnd = staff.WorkEnd,
                            LunchTimeBegin = staff.LunchTimeBegin,
                            LunchTimeEnd = staff.LunchTimeEnd,
                            DateBegin = staff.DateBegin ?? DateTime.Now,
                            DateEnd = staff.DateEnd ?? DateTime.Now,
                            cellText = hasAbsenceFullDay > 0 ? "Н" : "Р"
                        };
                    }
                    else
                    {
                        cellValue = new DateAndSchedule
                        {
                            ID_Schedule = idSchedule ?? 0,
                            ID_Absence = idAbsence ?? 0,
                            DTA = currentDateForCell,
                            TimeBegin = staff.TimeBegin,
                            TimeEnd = staff.TimeEnd,
                            WorkBegin = staff.WorkBegin,
                            WorkEnd = staff.WorkEnd,
                            LunchTimeBegin = staff.LunchTimeBegin,
                            LunchTimeEnd = staff.LunchTimeEnd,
                            DateBegin = staff.DateBegin ?? DateTime.Now,
                            DateEnd = staff.DateEnd ?? DateTime.Now,
                            cellText = "Н"
                        };
                    }

                    Dispatcher.Invoke(() =>
                    {
                        if (rowIndex >= 0 && columnIndex >= 0)
                        {
                            DataGridViewCell cell = StaffDGV.Rows[rowIndex].Cells[columnIndex];
                            cell.Value = cellValue;
                        }
                    });
                }
                else
                {
                    Console.WriteLine($"Индекс {rowIndex} вне допустимого диапазона.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении ячейки: {ex.Message}");
            }
}

        // заполнение ячеек данными
        private async Task FillDGVCells()
        {
            try
            {
                DateTime currentDate = fromDate;
                int daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

                await Task.Run(async () =>
                {
                    for (int day = 1; day <= daysInMonth; day++)
                    {
                        for (int rowIndex = 0; rowIndex < StaffList.Count; rowIndex++)
                        {
                            await UpdateCellAsync(rowIndex, day + 1);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при заполнении ячеек данными: {ex.Message}");
            }
        }

        public async Task UpdateAffectedCellsAsync(DateTime dateBegin, DateTime dateEnd, int rowIndex)
        {
            try
            {
                await LoadDataForAffectedCellsAsync(dateBegin, dateEnd);
                int columnIndexBegin = (dateBegin - fromDate).Days + 2;
                int columnIndexEnd = (dateEnd - fromDate).Days + 2;

                for (int day = columnIndexBegin; day <= columnIndexEnd; day++)
                {
                    await UpdateCellAsync(rowIndex, day);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении затронутых ячеек: {ex.Message}");
            }
        }

        private SmenZadaniaWindow smenZadaniaWindow;
        private async void ContextMenuItem_Click(object sender, EventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                string selectedMenuItemText = menuItem.Text;
                if (selectedMenuItemText == "Сменные задания")
                {
                    StaffDGV.ContextMenu = null;
                    if (StaffDGV.SelectedCells.Count > 0)
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

                                List<SmenZadaniaModel> SmenZadaniaList = await LoadSmenZadaniaForStuffAsync(selectedModel.STAFF_ID, currentDateForCell.Date);
                                if (SmenZadaniaList.Any(s => s.Product != null && s.DetailName != null && s.PP != null && s.NUM != null))
                                {
                                    smenZadaniaWindow = new SmenZadaniaWindow(SmenZadaniaList);
                                    smenZadaniaWindow.Show();
                                }
                                else
                                    MessageBox.Show($"Сменные задания для сотрудника {selectedModel.SHORT_FIO} ({selectedModel.TABEL_ID}) [{currentDateForCell.Date.ToShortDateString()}] не найдены.", "Сменные задания", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                                MessageBox.Show("Выбран некорректный день");
                        }
                    }
                }
            }
        }

        private async Task<List<SmenZadaniaModel>> LoadSmenZadaniaForStuffAsync(int StaffID, DateTime Date)
        {
            List<SmenZadaniaModel> smenZadaniaList = new List<SmenZadaniaModel>();

            using (SqlConnection connection = new SqlConnection("Persist Security Info=False;User ID=sa; Password=server_esa;Initial Catalog=dsl_sp;Server=sql"))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand(@"
                    SELECT DISTINCT
                        c.SHORT_FIO,
                        sz.Product,
                        sz.Detail as DetailNum,
                        dv.НазваниеД as DetailName,
                        sz.NUM,
                        dv.Договор as PP,
                        CAST(ROUND(sz.Cost, 2) as FLOAT) as Cost,
                        sz.Count,
                        a.WorkingHours,
                        a.DTA,
                        CAST(ROUND((SELECT SUM(sz.Cost) FROM Zarplats.dbo.SmenZadView sz WHERE LTRIM(c.TABEL_ID) = sz.id_Tabel AND a.DTA = sz.DTE), 2) AS FLOAT) as TotalHours
                    FROM [Zarplats].[dbo].[Staff_Schedule] as a
                    LEFT JOIN PERCO...staff_ref as b on a.STAFF_ID = b.STAFF_ID
                    LEFT JOIN PERCO...staff AS c ON b.STAFF_ID = c.ID_STAFF
                    LEFT JOIN PERCO...subdiv_ref AS d ON b.SUBDIV_ID = d.ID_REF
                    LEFT JOIN PERCO...appoint_ref AS e ON b.APPOINT_ID = e.ID_REF
                    LEFT JOIN Zarplats.dbo.SmenZadView as sz on LTRIM(c.TABEL_ID) = sz.id_Tabel and a.DTA = sz.DTE
                    LEFT JOIN Cooperation.dbo.DetailsView as dv on sz.NUM = dv.ПрП and sz.Detail = dv.НомерД
                    WHERE b.STAFF_ID = @staffid and a.DTA = @dta
                ", connection))
                {
                    command.Parameters.AddWithValue("staffid", StaffID);
                    command.Parameters.AddWithValue("dta", Date);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            SmenZadaniaModel smenZadania = new SmenZadaniaModel
                            {
                                SHORT_FIO = reader["SHORT_FIO"] is DBNull ? null : reader["SHORT_FIO"].ToString(),
                                Product = reader["Product"] is DBNull ? null : reader["Product"].ToString(),
                                DetailNum = reader["DetailNum"] is DBNull ? null : reader["DetailNum"].ToString(),
                                DetailName = reader["DetailName"] is DBNull ? null : reader["DetailName"].ToString(),
                                NUM = reader["NUM"] is DBNull ? null : reader["NUM"].ToString(),
                                PP = reader["PP"] is DBNull ? null : reader["PP"].ToString(),
                                Cost = reader["Cost"] is DBNull ? 0.0 : Convert.ToDouble(reader["Cost"]),
                                Count = reader["Count"] is DBNull ? 0.0 : Convert.ToDouble(reader["Count"]),
                                WorkingHours = reader["WorkingHours"] is DBNull ? 0.0 : Convert.ToDouble(reader["WorkingHours"]),
                                DTA = reader["DTA"] is DBNull ? DateTime.MinValue : Convert.ToDateTime(reader["DTA"]),
                                TotalHours = reader["TotalHours"] is DBNull ? 0.0 : Convert.ToDouble(reader["TotalHours"])
                            };

                            smenZadaniaList.Add(smenZadania);
                        }
                    }
                }
            }

            return smenZadaniaList;
        }

        private bool CheckRecordExists(int staffId, DateTime date)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Zarplats.dbo.Staff_Schedule WHERE STAFF_ID = @staffId AND DTA = @dta", connection))
                    {
                        command.Parameters.AddWithValue("@staffId", staffId);
                        command.Parameters.AddWithValue("@dta", date);

                        int checkExisting = (int)command.ExecuteScalar();

                        return checkExisting > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при проверке существующих записей: " + ex.Message);
                return false;
            }
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

        private void StaffDGV_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex > 1)
            {
                DataGridViewCell cell = StaffDGV.Rows[e.RowIndex].Cells[e.ColumnIndex];

                if (cell.Value is DateAndSchedule dateAndSchedule)
                {
                    if (dateAndSchedule.DTA != null)
                    {
                        bool hasAbsence = HasAbsence(SelectedRow.STAFF_ID, dateAndSchedule.DTA.Date);
                        bool isRecordExists = CheckRecordExists(SelectedRow.STAFF_ID, dateAndSchedule.DTA.Date);
                        if (hasAbsence)
                        {
                            InfoCustomWindow infoCustomWindow = new InfoCustomWindow(SelectedRow, dateAndSchedule.DTA.Date, StaffList, this);
                            infoCustomWindow.AbsenceRemoved += OnAbsenceRemoved;
                            infoCustomWindow.ShowDialog();
                        }
                        else if (!hasAbsence && isRecordExists)
                        {
                            InfoCustomWindow infoCustomWindow = new InfoCustomWindow(SelectedRow, dateAndSchedule.DTA.Date, StaffList, this);
                            infoCustomWindow.ShowDialog();
                        }
                    }
                }
            }
            else if (e.RowIndex >= 0 && e.ColumnIndex <= 1)
            {
                StaffDGV.ClearSelection();
                for (int columnIndex = 2; columnIndex < StaffDGV.Columns.Count; columnIndex++)
                {
                    StaffDGV.Rows[e.RowIndex].Cells[columnIndex].Selected = true;
                }
            }
        }

        private void OnAbsenceRemoved(object sender, EventArgs e)
        {
            Task.Run(() => UpdateDGVAsync());
        }

        public async Task DeleteRow()
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
                            DateTime currentDateForCell = new DateTime(fromDate.Year, fromDate.Month, columnIndex - 1);

                            // получаем idScheduleToDelete по дате в ячейке
                            int? idScheduleToDelete;
                            using (SqlConnection connection = new SqlConnection(connectionString))
                            {
                                connection.Open();

                                using (SqlCommand command = new SqlCommand("select a.ID_Schedule from Zarplats.dbo.Staff_Schedule as a left join Zarplats.dbo.Schedule_Absence as b on a.STAFF_ID = b.id_Staff and a.DTA between b.DateBegin and b.DateEnd where STAFF_ID = @idstaff and DTA = @date", connection))
                                {
                                    command.Parameters.AddWithValue("idstaff", selectedModel.STAFF_ID);
                                    command.Parameters.AddWithValue("date", currentDateForCell.Date);

                                    object rslt = command.ExecuteScalar();
                                    idScheduleToDelete = (rslt == DBNull.Value) ? null : (int?)rslt;
                                }
                            }
                            int? idAbsence;
                            using (SqlConnection connection = new SqlConnection(connectionString))
                            {
                                connection.Open();

                                using (SqlCommand command = new SqlCommand("select b.ID_Absence from Zarplats.dbo.Staff_Schedule as a left join Zarplats.dbo.Schedule_Absence as b on a.STAFF_ID = b.id_Staff and a.DTA between b.DateBegin and b.DateEnd where STAFF_ID = @idstaff and DTA = @date", connection))
                                {
                                    command.Parameters.AddWithValue("idstaff", selectedModel.STAFF_ID);
                                    command.Parameters.AddWithValue("date", currentDateForCell.Date);

                                    object rslt = command.ExecuteScalar();
                                    idAbsence = (rslt == DBNull.Value) ? null : (int?)rslt;
                                }
                            }


                            if (idScheduleToDelete.HasValue && idScheduleToDelete.Value != 0)
                            {
                                Odb.db.Database.ExecuteSqlCommand("DELETE FROM Zarplats.dbo.Staff_Schedule WHERE ID_Schedule = @idschedule", new SqlParameter("idschedule", idScheduleToDelete));
                                if (idAbsence.HasValue && idAbsence != 0)
                                {
                                    var confirmResult = MessageBox.Show($"День {currentDateForCell:dd.MM.yyyy} имеет отсутствие. Удалить отсутствия на этот день?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
                                    if (confirmResult == MessageBoxResult.Yes)
                                        Odb.db.Database.ExecuteSqlCommand("DELETE FROM Zarplats.dbo.Schedule_Absence WHERE DateBegin <= @dta and DateEnd >= @dta", new SqlParameter("dta", currentDateForCell.Date));
                                }

                                var dateAndScheduleToDelete = selectedModel.DatesAndSchedules.FirstOrDefault(ds => ds.ID_Schedule == idScheduleToDelete);
                                if (dateAndScheduleToDelete != null)
                                {
                                    dateAndScheduleToDelete.ID_Schedule = 0;
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

                    tempList.Clear();
                    await UpdateDGVAsync();
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

                    await UpdateDGVAsync();
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
        }

        private async void StaffDGV_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
                await DeleteRow();
        }

        private void StaffTBX_GotFocus(object sender, RoutedEventArgs e)
        {
            StaffLV.Visibility = Visibility.Visible;
        }

        private void StaffTBX_LostFocus(object sender, RoutedEventArgs e)
        {
            StaffLV.Visibility = Visibility.Collapsed;
        }

        private void StaffAbsenceTBX_GotFocus(object sender, RoutedEventArgs e)
        {
            StaffAbsenceLV.Visibility = Visibility.Visible;
        }

        private void StaffAbsenceTBX_LostFocus(object sender, RoutedEventArgs e)
        {
            StaffAbsenceLV.Visibility = Visibility.Collapsed;
        }

        private void AbsenceMTBX_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MaskedTextBox maskedTextBox = sender as MaskedTextBox;
            maskedTextBox.Clear();
        }

        private void StaffLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedStaff = (StaffModel)StaffLV.SelectedItem;
            if (SelectedStaff != null)
            {
                StaffTBX.Text = $"{SelectedStaff.SHORT_FIO} ({SelectedStaff.TABEL_ID.Trim()})";
            }
        }

        private void StaffAbsenceLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedStaffForAbsence = (StaffModel)StaffAbsenceLV.SelectedItem;
            if (SelectedStaffForAbsence != null)
            {
                StaffAbsenceTBX.Text = $"{SelectedStaffForAbsence.SHORT_FIO} ({SelectedStaffForAbsence.TABEL_ID.Trim()})";
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

        private void ClearStaffAbsenceBtn_Click(object sender, RoutedEventArgs e)
        {
            StaffAbsenceTBX.Clear();
            StaffAbsenceLV.SelectedItem = null;
            SelectedStaffForAbsence = null;
        }

        private void ManageScheduleBtn_Click(object sender, RoutedEventArgs e)
        {
            ScheduleManageWindow scheduleManageWindow = new ScheduleManageWindow();
            scheduleManageWindow.TemplateCreated += ScheduleManageWindow_TemplateCreated;
            scheduleManageWindow.TemplateDeleted += ScheduleManageWindow_TemplateDeleted;
            scheduleManageWindow.ShowDialog();
        }

        private void ManageCauseBtn_Click(object sender, RoutedEventArgs e)
        {
            ManageCauseWindow manageCauseWindow = new ManageCauseWindow();
            manageCauseWindow.CauseAdded += ManageCauseWindow_CauseAdded;
            manageCauseWindow.CauseRemoved += ManageCauseWindow_CauseRemoved;
            manageCauseWindow.ShowDialog();
        }

        private void ManageCauseWindow_CauseAdded(object sender, EventArgs e)
        {
            CauseCB.ItemsSource = null;
            UpdateCauseList();
        }

        private void ManageCauseWindow_CauseRemoved(object sender, EventArgs e)
        {
            CauseCB.ItemsSource = null;
            UpdateCauseList();
        }

        private void UpdateCauseList()
        {
            CauseList = Odb.db.Database.SqlQuery<AbsenceModel>("select distinct * from Zarplats.dbo.AbsenceRef").ToList();
            CauseCB.ItemsSource = CauseList;
        }

        private void ScheduleManageWindow_TemplateCreated(object sender, EventArgs e)
        {
            UpdateTemplatesList();
        }

        private void ScheduleManageWindow_TemplateDeleted(object sender, EventArgs e)
        {
            UpdateTemplatesList();
        }

        private async void StaffRemoveBtn_Click(object sender, RoutedEventArgs e) => await DeleteRow();

        private async void StaffRefreshBtn_Click(object sender, RoutedEventArgs e) => await UpdateDGVAsync();

        public void AssignCMB()
        {
            filterCMB.ItemsSource = new filterCMB[]
            {
                new filterCMB { id = 0, filterName = "ФИО" },
                new filterCMB { id = 1, filterName = "табельному номеру" },
            };
            filterCMB.SelectedIndex = 0;

            MonthCMB.Items.Clear();
            DateTime currentDate = DateTime.Now;

            for (int i = 0; i < 2; i++)
            {
                for (int month = 1; month <= 12; month++)
                {
                    DateTime date = new DateTime(currentDate.Year + i, month, 1);
                    MonthCMB.Items.Add(date.ToString("MMMM yyyy"));
                }
            }

            MonthCMB.SelectedItem = currentDate.ToString("MMMM yyyy");

            MonthCMB.SelectionChanged += async (sender, e) =>
            {
                await UpdateDGVAsync();
            };
        }

       

        private void CauseCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedCause = (AbsenceModel)CauseCB.SelectedItem;
        }

        private void ClearAbsenceBtn_Click(object sender, RoutedEventArgs e)
        {
            CauseCB.SelectedItem = null;
            AbsenceStartDP.SelectedDate = DateTime.Now;
            AbsenceFinishDP.SelectedDate = DateTime.Now;
            AbsenceTimeBeginMTBX.Clear();
            AbsenceTimeEndMTBX.Clear();
        }

        private async void AddScheduleBtn_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() => App.DisableAllWindows());
            if (StaffLV.SelectedItem != null && TemplateCB.SelectedItem != null && ScheduleStartDP.SelectedDate != null && ScheduleEndDP.SelectedDate != null)
            {
                if (SelectedTemplate.isFlexible)
                    await FillFlexibleSchedule();
                else if (!SelectedTemplate.isFlexible)
                    await FillStaticSchedule();
            }
            else
            {
                MessageBox.Show("Не все поля графика заполнены.");
            }
            Dispatcher.Invoke(() => App.EnableAllWindows());
        }

        private async void AddAbsenceBtn_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() => App.DisableAllWindows());
            if (StaffAbsenceLV.SelectedItem != null || CauseCB.SelectedItem != null && IsTimeCorrect())
                await AddAbsence();
            else
                MessageBox.Show("Не все поля отсутствия заполнены!");
            Dispatcher.Invoke(() => App.EnableAllWindows());
        }

        private bool IsTimeCorrect()
        {
            string maskedTextBegin = AbsenceTimeBeginMTBX.Text;
            string maskedTextEnd = AbsenceTimeEndMTBX.Text;

            if (!maskedTextBegin.Any(char.IsDigit) || !maskedTextEnd.Any(char.IsDigit))
                return true;

            string[] timeComponentsBegin = maskedTextBegin.Split(':');
            string[] timeComponentsEnd = maskedTextEnd.Split(':');

            if (timeComponentsBegin.Length == 2 && timeComponentsEnd.Length == 2)
            {
                if (int.TryParse(timeComponentsBegin[0], out int hoursBegin) &&
                    int.TryParse(timeComponentsBegin[1], out int minutesBegin) &&
                    int.TryParse(timeComponentsEnd[0], out int hoursEnd) &&
                    int.TryParse(timeComponentsEnd[1], out int minutesEnd))
                {
                    if (hoursBegin >= 24 || hoursEnd >= 24 || minutesBegin > 59 || minutesEnd > 59)
                    {
                        MessageBox.Show("Некорректное время.");
                        return false;
                    }
                    else
                        return true;
                }
            }

            MessageBox.Show("Некорректное время.");
            return false;
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

        private bool HasAbsence(int staffId, DateTime date)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("select * from Zarplats.dbo.Schedule_Absence where id_Staff = @idstaff and DateBegin <= @date and DateEnd >= @date", connection))
                    {
                        command.Parameters.AddWithValue("@idstaff", staffId);
                        command.Parameters.AddWithValue("@date", date);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            return reader.Read();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при проверке наличия отсутствия: " + ex.Message);
                return false;
            }
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

        #region Search Staff
        private void StaffTBX_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SearchStaff("schedule");
        }

        private void StaffAbsenceTBX_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SearchStaff("absence");
        }

        private void SearchStaff(string type)
        {
            List<StaffModel> staff = new List<StaffModel>();
            string txt = StaffTBX.Text;
            if (txt.Length == 0)
                staff = StaffList.ToList();
            staff = StaffListInPosition.Where(u => u.StaffForSearch.ToLower().Contains(txt.ToLower())).ToList();
            if (type == "schedule")
                StaffLV.ItemsSource = staff;
            else
                StaffAbsenceLV.ItemsSource = staff;
        }
        #endregion
        #endregion

        #region Заполнение графиков
        private async Task FillFlexibleSchedule()
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

                var absencePeriods = await CalculateAbsencePeriodsForEachDay(SelectedStaff.STAFF_ID, current);
                double intersectionTime = CalculateIntersectionTime(lunchTimeBegin, lunchTimeEnd, absencePeriods);
                //if (intersectionTime > 0)
                //{
                //    DateTime intersectionEnd = lunchTimeEnd > absenceEnd ? absenceEnd : lunchTimeEnd;
                //    intersectionTime = (intersectionEnd - lunchTimeBegin).TotalHours;
                //}
                double workingHours = CalculateWorkingHours(workBegin, workEnd, lunchTimeBegin, lunchTimeEnd, current) - (intersectionTime > 0 ? intersectionTime : totalAbsenceTime);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // проверка существования записи в графике
                    using (SqlCommand checkCommand = new SqlCommand("SELECT COUNT(*) FROM Zarplats.dbo.Staff_Schedule WHERE STAFF_ID = @staffId AND DTA = @date", connection))
                    {
                        checkCommand.Parameters.AddWithValue("@staffId", SelectedStaff.STAFF_ID);
                        checkCommand.Parameters.AddWithValue("@date", current.Date);

                        int existingCount = (int)await checkCommand.ExecuteScalarAsync();

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

                                await updateCommand.ExecuteNonQueryAsync();
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

                                await insertCommand.ExecuteNonQueryAsync();
                            }
                        }
                    }
                }

                flexibleDaysIndex++;
                current = current.AddDays(1);
            }
            MessageBox.Show($"График заполнен!");
            await UpdateDGVAsync();
        }

        private async Task FillStaticSchedule()
        {
            DateTime selectedStartDate = ScheduleStartDP.SelectedDate ?? DateTime.Now;
            DateTime selectedFinishDate = ScheduleEndDP.SelectedDate ?? DateTime.Now;
            DateTime current = selectedStartDate;

            List<ScheduleTemplateModel> Days = GetDaysInfo(SelectedTemplate.ID_Template);

            DayOfWeek startDayOfWeek = selectedStartDate.DayOfWeek;
            var currentDay = Days.FirstOrDefault(d => d.Day == startDayOfWeek.ToString());

            while (current <= selectedFinishDate)
            {
                if (currentDay != null && !currentDay.isRestingDay && SelectedStaff != null)
                {
                    DateTime workBegin = ConvertToDateTime(current, currentDay.WorkBegin);
                    DateTime workEnd = ConvertToDateTime(current, currentDay.WorkEnd);
                    DateTime lunchTimeBegin = ConvertToDateTime(current, currentDay.LunchTimeBegin);
                    DateTime lunchTimeEnd = ConvertToDateTime(current, currentDay.LunchTimeEnd);

                    var absenceInfo = await CalculateAbsenceHoursForEachDay(SelectedStaff.STAFF_ID, current);
                    double totalAbsenceTime = absenceInfo.Item1;
                    DateTime absenceStart = absenceInfo.Item2;
                    DateTime absenceEnd = absenceInfo.Item3;

                    var absencePeriods = await CalculateAbsencePeriodsForEachDay(SelectedStaff.STAFF_ID, current);
                    double intersectionTime = CalculateIntersectionTime(lunchTimeBegin, lunchTimeEnd, absencePeriods);
                    //if (intersectionTime > 0)
                    //{
                    //    DateTime intersectionEnd = lunchTimeEnd > absenceEnd ? absenceEnd : lunchTimeEnd;
                    //    intersectionTime = (intersectionEnd - lunchTimeBegin).TotalHours;
                    //}
                    double workingHours = CalculateWorkingHours(workBegin, workEnd, lunchTimeBegin, lunchTimeEnd, current) - (intersectionTime > 0 ? intersectionTime : totalAbsenceTime);

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();

                        // проверка существования записи в графике
                        using (SqlCommand checkCommand = new SqlCommand("SELECT COUNT(*) FROM Zarplats.dbo.Staff_Schedule WHERE STAFF_ID = @staffId AND DTA = @date", connection))
                        {
                            checkCommand.Parameters.AddWithValue("@staffId", SelectedStaff.STAFF_ID);
                            checkCommand.Parameters.AddWithValue("@date", current.Date);

                            int existingCount = (int)await checkCommand.ExecuteScalarAsync();

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

                                    await updateCommand.ExecuteNonQueryAsync();
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

                                    await insertCommand.ExecuteNonQueryAsync();
                                }
                            }
                        }
                    }
                }

                current = current.AddDays(1);
                currentDay = Days.FirstOrDefault(d => d.Day == current.DayOfWeek.ToString());   // переход к следующему дню недели в записях из базы данных
            }
            MessageBox.Show($"График заполнен!");
            await UpdateDGVAsync();
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
            string timeBeginValue = AbsenceTimeBeginMTBX.Text;
            string timeEndValue = AbsenceTimeEndMTBX.Text;

            int checkExistingAbsence = Odb.db.Database.SqlQuery<int>(
                "IF EXISTS (SELECT * FROM Zarplats.dbo.Schedule_Absence WHERE DateBegin <= @DateEnd AND DateEnd >= @DateBegin AND id_Staff = @staffid AND " +
                "((TimeBegin IS NULL AND TimeEnd IS NULL) OR (TimeBegin <= @TimeEnd AND TimeEnd >= @TimeBegin)) AND " +
                "(@TimeBegin IS NULL OR @TimeEnd IS NULL OR (TimeBegin <= @TimeBegin AND TimeEnd >= @TimeEnd))) " +
                "SELECT 1 ELSE SELECT 0",
                new SqlParameter("DateBegin", AbsenceStartDP.SelectedDate),
                new SqlParameter("DateEnd", AbsenceFinishDP.SelectedDate),
                new SqlParameter("staffid", SelectedStaffForAbsence.STAFF_ID),
                new SqlParameter("TimeBegin", timeBeginValue.Any(char.IsDigit) ? timeBeginValue : (object)DBNull.Value),
                new SqlParameter("TimeEnd", timeEndValue.Any(char.IsDigit) ? timeEndValue : (object)DBNull.Value)
            ).SingleOrDefault();
            if (!Convert.ToBoolean(checkExistingAbsence))
            {
                Odb.db.Database.ExecuteSqlCommand("INSERT INTO Zarplats.dbo.Schedule_Absence (AbsenceRef_ID, id_Staff, DateBegin, DateEnd, TimeBegin, TimeEnd) VALUES (@AbsenceRef_ID, @staffid, @DateBegin, @DateEnd, @TimeBegin, @TimeEnd)",
                    new SqlParameter("AbsenceRef_ID", SelectedCause.ID_AbsenceRef),
                    new SqlParameter("staffid", SelectedStaffForAbsence.STAFF_ID),
                    new SqlParameter("DateBegin", AbsenceStartDP.SelectedDate),
                    new SqlParameter("DateEnd", AbsenceFinishDP.SelectedDate),
                    new SqlParameter("TimeBegin", timeBeginValue.Any(char.IsDigit) ? timeBeginValue : (object)DBNull.Value),
                    new SqlParameter("TimeEnd", timeEndValue.Any(char.IsDigit) ? timeEndValue : (object)DBNull.Value));

                DateTime absenceStart = AbsenceStartDP.SelectedDate ?? DateTime.Now;
                DateTime absenceEnd = AbsenceFinishDP.SelectedDate ?? DateTime.Now;

                // обновление рабочих часов
                await UpdateWorkingHours(new List<StaffModel> { SelectedStaffForAbsence }, absenceStart, absenceEnd);
                int rowIndex = StaffList.ToList().FindIndex(staff => staff.STAFF_ID == SelectedStaffForAbsence.STAFF_ID);
                await UpdateAffectedCellsAsync(absenceStart, absenceEnd, rowIndex);
                MessageBox.Show("Отсутствие добавлено!");
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
            await Task.Run(() => { Odb.db.Database.ExecuteSqlCommand("DELETE FROM Zarplats.dbo.Schedule_Absence WHERE DateBegin <= @dta and DateEnd >= @dta", new SqlParameter("dta", selectedRow.DTA)); });

            // обновление рабочих часов для всех дней, затронутых удаленным отсутствием
            Console.WriteLine("Начали обновлять рабочие часы для периода");
            await UpdateWorkingHours(StaffDGV.SelectedRows.Cast<StaffModel>().ToList(), absenceStart, absenceEnd);
            Console.WriteLine("Закончили обновлять рабочие часы для периода");
        }


        #endregion

        #region Вычисления рабочих часов
        private async Task UpdateWorkingHours(List<StaffModel> selectedStaffList, DateTime startDate, DateTime endDate)
        {
            List<StaffModel> affectedRows = await GetAffectedRowsAsync(startDate, endDate);

            foreach (StaffModel selectedStaff in selectedStaffList)
            {
                foreach (StaffModel affectedRow in affectedRows)
                {
                    DateTime workBegin = ConvertToDateTime(affectedRow.DTA, affectedRow.WorkBegin);
                    DateTime workEnd = ConvertToDateTime(affectedRow.DTA, affectedRow.WorkEnd);
                    DateTime lunchTimeBegin = ConvertToDateTime(affectedRow.DTA, affectedRow.LunchTimeBegin);
                    DateTime lunchTimeEnd = ConvertToDateTime(affectedRow.DTA, affectedRow.LunchTimeEnd);

                    var absenceInfo = await CalculateAbsenceHoursForEachDay(affectedRow.STAFF_ID, affectedRow.DTA);
                    double totalAbsenceTime = absenceInfo.Item1;
                    DateTime absenceStart = absenceInfo.Item2;
                    DateTime absenceEnd = absenceInfo.Item3;

                    var absencePeriods = await CalculateAbsencePeriodsForEachDay(affectedRow.STAFF_ID, affectedRow.DTA);
                    double intersectionTime = CalculateIntersectionTime(lunchTimeBegin, lunchTimeEnd, absencePeriods);
                    double workingHours = CalculateWorkingHours(workBegin, workEnd, lunchTimeBegin, lunchTimeEnd, affectedRow.DTA) - (intersectionTime > 0 ? intersectionTime : totalAbsenceTime);

                    await Odb.db.Database.ExecuteSqlCommandAsync("update Zarplats.dbo.Staff_Schedule set WorkingHours = @workingHours where ID_Schedule = @id", new SqlParameter("workingHours", workingHours), new SqlParameter("id", affectedRow.ID_Schedule));
                }
            }
        }

        public async Task UpdateWorkingHoursForAffectedRows(List<StaffModel> selectedStaffList, List<StaffModel> affectedRows)
        {
            foreach (StaffModel selectedStaff in selectedStaffList)
            {
                foreach (StaffModel affectedRow in affectedRows)
                {
                    DateTime workBegin = ConvertToDateTime(affectedRow.DTA, affectedRow.WorkBegin);
                    DateTime workEnd = ConvertToDateTime(affectedRow.DTA, affectedRow.WorkEnd);
                    DateTime lunchTimeBegin = ConvertToDateTime(affectedRow.DTA, affectedRow.LunchTimeBegin);
                    DateTime lunchTimeEnd = ConvertToDateTime(affectedRow.DTA, affectedRow.LunchTimeEnd);

                    var absenceInfo = await CalculateAbsenceHoursForEachDay(affectedRow.STAFF_ID, affectedRow.DTA);
                    double totalAbsenceTime = absenceInfo.Item1;
                    DateTime absenceStart = absenceInfo.Item2;
                    DateTime absenceEnd = absenceInfo.Item3;

                    var absencePeriods = await CalculateAbsencePeriodsForEachDay(affectedRow.STAFF_ID, affectedRow.DTA);
                    double intersectionTime = CalculateIntersectionTime(lunchTimeBegin, lunchTimeEnd, absencePeriods);
                    if (intersectionTime > 0)
                    {
                        DateTime intersectionEnd = lunchTimeEnd > absenceEnd ? absenceEnd : lunchTimeEnd;
                        intersectionTime = (intersectionEnd - lunchTimeBegin).TotalHours;
                    }
                    double workingHours = CalculateWorkingHours(workBegin, workEnd, lunchTimeBegin, lunchTimeEnd, affectedRow.DTA) - (intersectionTime > 0 ? intersectionTime : totalAbsenceTime);

                    await Odb.db.Database.ExecuteSqlCommandAsync("update Zarplats.dbo.Staff_Schedule set WorkingHours = @workingHours where ID_Schedule = @id", new SqlParameter("workingHours", workingHours), new SqlParameter("id", affectedRow.ID_Schedule));
                }
            }
        }

        public async Task<List<StaffModel>> GetAffectedRowsAsync(DateTime absenceBegin, DateTime absenceEnd)
        {
            Console.WriteLine("Получаем список затронутых строк");
            Dispatcher.Invoke(() => StatusTB.Text = "Получаем список затронутых дней...");
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                var query = $"SELECT DISTINCT a.ID_Schedule, a.STAFF_ID, LTRIM(e.TABEL_ID) as TABEL_ID, e.SHORT_FIO, a.WorkBegin, a.WorkEnd, a.DTA, a.LunchTimeBegin, a.LunchTimeEnd, a.WorkingHours, b.ID_Absence, c.Cause as CauseAbsence, b.DateBegin, b.DateEnd, b.TimeBegin, b.TimeEnd FROM [Zarplats].[dbo].[Staff_Schedule] as a left join Zarplats.dbo.Schedule_Absence as b on a.STAFF_ID = b.id_Staff and a.DTA between b.DateBegin and b.DateEnd left join Zarplats.dbo.AbsenceRef as c on b.AbsenceRef_ID = c.ID_AbsenceRef left join perco...staff as e on a.STAFF_ID = e.ID_STAFF left join Zarplats.dbo.StaffView as f on a.STAFF_ID = f.STAFF_ID where f.Position = '{SelectedDep.Position}' and a.DTA between '{absenceBegin}' and '{absenceEnd}' order by a.DTA";

                using (var command = new SqlCommand(query, connection))
                {
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

                        Console.WriteLine("Закончили получать список затронутых строк");
                        Dispatcher.Invoke(() => StatusTB.Text = "Список затронутых дней получен.");
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

        private double CalculateIntersectionTime(DateTime workLunchStart, DateTime workLunchEnd, List<Tuple<DateTime, DateTime>> absencePeriods)
        {
            double totalIntersectionTime = 0;

            foreach (var absencePeriod in absencePeriods)
            {
                DateTime absenceStart = absencePeriod.Item1;
                DateTime absenceEnd = absencePeriod.Item2;

                // Находим максимальное начальное время и минимальное конечное время, чтобы определить пересечение
                DateTime intersectionStart = workLunchStart > absenceStart ? workLunchStart : absenceStart;
                DateTime intersectionEnd = workLunchEnd < absenceEnd ? workLunchEnd : absenceEnd;

                // Проверяем, есть ли пересечение
                if (intersectionStart < intersectionEnd)
                {
                    // Рассчитываем продолжительность пересечения
                    double intersectionHours = (intersectionEnd - intersectionStart).TotalHours;

                    // Добавляем продолжительность пересечения к общему времени пересечения
                    totalIntersectionTime += intersectionHours;
                }
            }

            return totalIntersectionTime;
        }

        private async Task<List<Tuple<DateTime, DateTime>>> CalculateAbsencePeriodsForEachDay(int staffId, DateTime currentDate)
        {
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
                        var absencePeriods = new List<Tuple<DateTime, DateTime>>();

                        while (await reader.ReadAsync())
                        {
                            DateTime currentAbsenceTimeBegin = ConvertToDateTime(currentDate, reader["TimeBegin"].ToString());
                            DateTime currentAbsenceTimeEnd = ConvertToDateTime(currentDate, reader["TimeEnd"].ToString());

                            absencePeriods.Add(new Tuple<DateTime, DateTime>(currentAbsenceTimeBegin, currentAbsenceTimeEnd));
                        }

                        return absencePeriods;
                    }
                }
            }
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

        private void AddCauseWindow_CauseAdded(object sender, EventArgs e)
        {
            CauseCB.ItemsSource = null;
            CauseCB.ItemsSource = CauseList;
        }
    }
}
