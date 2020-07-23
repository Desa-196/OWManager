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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Users
{
    /// <summary>
    /// Логика взаимодействия для ControlViewError.xaml
    /// </summary>
    public partial class ControlViewError : UserControl
    {
        public string ErrorText{get; set;}
        public ControlViewError(string Text)
        {
            InitializeComponent();
            ErrorText = Text;
        }

        public MyCommand Close
        {
            get
            {
                return new MyCommand((obj) =>
                {
                    (Application.Current.MainWindow as MainWindow).EnableBlur = false;
                    Keyboard.Focus((Application.Current.MainWindow as MainWindow).OldFocusElement);

                },
                (obj) =>
                {
                    return true;
                });
            }
        }
    }
}
