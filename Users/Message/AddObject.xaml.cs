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

namespace Users.Message
{
    public partial class AddObject : UserControl, INotifyPropertyChanged
    {
        public List<TypeObject> ArrayTypeObject { get; set; }

        MapViewer Sender;

        public string _ObjectName;
        public string ObjectName
        {
            get { return _ObjectName; }
            set
            {
                _ObjectName = value;
                OnPropertyChanged("ObjectName");
            }
        }
        public TypeObject _SelectedItem;
        public TypeObject SelectedItem
        {
            get { return _SelectedItem; }
            set
            {
                _SelectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }
        public AddObject(MapViewer sender)
        {
            Sender = sender;
            InitializeComponent();

            this.DataContext = this;

            ArrayTypeObject = SQLiteBase.GetTypeObjectMap();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public MyCommand AddObjectOnMap
        {
            get
            {
                return new MyCommand((obj) =>
                {
                    Sender.AddObject(ObjectName, SelectedItem);
                    (Application.Current.MainWindow as MainWindow).EnableBlur = false;

                },
                (obj) =>
                {
                    if (SelectedItem != null && ObjectName != null && ObjectName.Length > 0)
                    {
                        return true;
                    }
                    else 
                    {
                        return false;
                    }
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

                },
                (obj) =>
                {
                    return true;
                });
            }
        }
    }
}
