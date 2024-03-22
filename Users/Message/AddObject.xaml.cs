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

        Point addObjectCoordinate;

        private const uint maxCharCount = 500;

        private uint _CountChar = 500;
        public uint CountChar
        {
            get { return _CountChar; }
            set
            {
                _CountChar = value;
                OnPropertyChanged("CountChar");
            }
        }

        private string _ObjectDescription;
        public string ObjectDescription
        {
            get { return _ObjectDescription; }
            set
            {
                if (value.Length <= maxCharCount)
                {
                    _ObjectDescription = value;
                    CountChar = maxCharCount - (uint)value.Length;
                    OnPropertyChanged("ObjectDescription");
                }
            }
        }

        private MapObject editObject;

        //Если окно для добавления новой записи - то true, а если для редактирования то - false
        private bool _CreateTypeWindow = true;
        public bool CreateTypeWindow
        {
            get { return _CreateTypeWindow; }
            set
            {
                _CreateTypeWindow = value;
                OnPropertyChanged("CreateTypeWindow");
            }
        }


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
        public AddObject(MapViewer sender, Point addObjectCoordinate)
        {
            this.addObjectCoordinate = addObjectCoordinate;
            Sender = sender;
            InitializeComponent();

            this.DataContext = this;

            ArrayTypeObject = SQLiteBase.GetTypeObjectMap();
        }

        public AddObject(MapObject editObject)
        {
            this.editObject = editObject;

            InitializeComponent();

            this.DataContext = this;

            ArrayTypeObject = SQLiteBase.GetTypeObjectMap();

            CreateTypeWindow = false;
            SelectedItem = ArrayTypeObject.Where(x => x.Id == editObject.TypeObject.Id).FirstOrDefault();
            ObjectName = editObject.Name;
            ObjectDescription = editObject.Description;

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
                    if (CreateTypeWindow == false)
                    {
                        editObject.Name = ObjectName;
                        editObject.Description = ObjectDescription;
                        editObject.TypeObject = SelectedItem;
                        SQLiteBase.EditObject(editObject);
                    }
                    else
                    {
                        Sender.AddObject(ObjectName, ObjectDescription, SelectedItem, addObjectCoordinate);
                        (Application.Current.MainWindow as MainWindow).EnableBlur = false;
                    }

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
