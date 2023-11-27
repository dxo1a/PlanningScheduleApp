using Prism.Commands;
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
    public partial class DialogueCustomWindow : Window
    {
        public DialogueCustomWindowViewModel ViewModel { get; set; }

        public DialogueCustomWindow(string yesBtnContent, string cancelBtnContent, string action, string message)
        {
            try
            {
                InitializeComponent();
                ViewModel = new DialogueCustomWindowViewModel(this, action, message);
                DataContext = ViewModel;

                YesBtn.Content = yesBtnContent;
                CancelBtn.Content = cancelBtnContent;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception when initializing DialogueCustomWindow: {ex.Message}");
            }
        }
    }

    public class DialogueCustomWindowViewModel
    {
        private readonly DialogueCustomWindow _window;

        public DialogueCustomWindowViewModel(DialogueCustomWindow window, string action, string message)
        {
            _window = window;
            Message = message;
            Action = action;
            YesCommand = new DelegateCommand(Yes);
            NoCommand = new DelegateCommand(No);
        }

        public string Message { get; }
        public string Action { get; }

        public ICommand YesCommand { get; }

        public ICommand NoCommand { get; }

        private void Yes()
        {
            // Обработка нажатия кнопки "Yes"
            _window.DialogResult = true;
            _window.Close();
        }

        private void No()
        {
            // Обработка нажатия кнопки "No"
            _window.DialogResult = false;
            _window.Close();
        }
    }

}
