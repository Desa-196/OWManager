using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Логика взаимодействия для ControlHistoryViewer.xaml
    /// </summary>
    public partial class ControlHistoryViewer : UserControl, INotifyPropertyChanged
    {
        public SerchHistoryItem _SelectIteam;
        public SerchHistoryItem SelectIteam
        {
            get { return _SelectIteam; }
            set
            {
                _SelectIteam = value;
                OnPropertyChanged("SelectIteam");
            }
        }

        Action<string, string, string> search;
        public ObservableCollection<SerchHistoryItem> _HistoryList;
        public ObservableCollection<SerchHistoryItem> HistoryList 
        {
            get { return _HistoryList; }
            set
            {
                _HistoryList = value;
                OnPropertyChanged("HistoryList");
            }
        }
        public ControlHistoryViewer( Action<string,string,string> search )
        {
            InitializeComponent();
            this.search = search;
           
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public MyCommand Search
        {
            get
            {
                return new MyCommand((obj) =>
                {
                    search.Invoke(SelectIteam.UserName, SelectIteam.TelephoneNumber, SelectIteam.ComputerName);
 
                },
                (obj) =>
                {
                    return (SelectIteam != null);

                });
            }
        }
        public MyCommand Close
        {
            get
            {
                return new MyCommand((obj) =>
                {
                    (Application.Current.MainWindow as MainWindow).EnableBlur = false;
                    (Application.Current.MainWindow as MainWindow).TypeMessage = null;
                },
                (obj) =>
                {
                    return true;

                });
            }
        }
        
        public MyCommand LoadSearchHistoryIteam
        {
            get
            {
                return new MyCommand((obj) =>
                {
                    System.Windows.MessageBox.Show("4534");
                },
                (obj) =>
                {
                    if (HistoryList != null)
                    {
                        if (HistoryList.Count == 0)
                        {
                            return false;
                        }
                        else { return true; }

                    }
                    else { return false; }
                    

                });
            }
        }
    }
}
