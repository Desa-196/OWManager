using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Users
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// (MainWindow)Application.Current.MainWindow
    
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public List<Control> PageList { get; set; } = new List<Control>();

        public MapObject CutBufferObject;

        public Control OldFocusElement;


        public int _SelectedMenuId;

        public int SelectedMenuId
        {
            get { return _SelectedMenuId; }
            set
            {
                _SelectedMenuId = value;
                NotifyPropertyChanged("SelectedMenuId");
           
            }
        }
       public Control _SelectedObj;

        public Control SelectedObj
        {
            get { return _SelectedObj; }
            set
            {
                if (value == null) _SelectedObj = PageList[0];
                else { _SelectedObj = value; }
                NotifyPropertyChanged("SelectedObj");
           
            }
        }
       
        public Control _Page;
        public Control Page
        {
            get { return _Page; }
            set
            {
                _Page = value;
                NotifyPropertyChanged("Page");
            }
        }
        public Control _FocusElement;
        public Control FocusElement
        {
            get { return _FocusElement; }
            set
            {
                _FocusElement = value;
                NotifyPropertyChanged("FocusElement");
            }
        }

        public UserControl _TypeMessage;
        public UserControl TypeMessage
        {
            get { return _TypeMessage; }
            set 
            {
                _TypeMessage = value;
                NotifyPropertyChanged("TypeMessage");
            }
        }
        public string _ChangePasswordUserName="";
        public string ChangePasswordUserName
        {
            get { return _ChangePasswordUserName; }
            set 
            {
                _ChangePasswordUserName = value;
                NotifyPropertyChanged("ChangePasswordUserName");
            }
        }

        public bool _EnableBlur = false;
        public bool EnableBlur
        {
            get { return _EnableBlur; }
            set 
            {
                _EnableBlur = value;
                NotifyPropertyChanged("EnableBlur");
            }
        }
        
        public MainWindow()
        {
            InitializeComponent();

            PageList.Add(new UsersViewer());
            PageList.Add(new ControlMaps());
            PageList.Add(new ControlSettings());

            Timeline.DesiredFrameRateProperty.OverrideMetadata(
       typeof(Timeline),
       new FrameworkPropertyMetadata { DefaultValue = 15 }
      );

            SelectedObj = PageList[0];

            this.DataContext = this;

        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //Вычитаем из ширины окна, ширину скролла
        


    }


}
