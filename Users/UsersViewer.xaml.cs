using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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

    public class SerchHistoryItem
    {
        public DateTime SearchDateTime { get; set; }
      
        public string DateTimeStamp
        {
            private set { }
            get
            {
                return SearchDateTime.ToString("dd.MM.yyyy HH:mm");
            }
        }
        public string UserName { get; set; }
        public string TelephoneNumber { get; set; }
        public string ComputerName { get; set; }
        public SerchHistoryItem(DateTime SearchDateTime, string UserName, string TelephoneNumber, string ComputerName)
        {
            this.SearchDateTime = SearchDateTime;
            this.UserName = UserName;
            this.TelephoneNumber = TelephoneNumber;
            this.ComputerName = ComputerName;
        }
        public override string ToString()
        {
            return SearchDateTime.ToString("dd.MM.yyyy HH:mm") + "\n\t Имя пользователя: " + UserName + "\n\t Номер телефона: " + TelephoneNumber + " \n\tИмя компьютера: " + ComputerName;
        }
    }
    public partial class UsersViewer : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<SerchHistoryItem> HistoryList = new ObservableCollection<SerchHistoryItem>();
        public ObservableCollection<User> SearchList = new ObservableCollection<User>();

        public string MenuImage { get; set; } = "Ресурсы/menu/MenuIcoSearchUser.png";
        public string MenuImageSelected { get; set; } = "Ресурсы/menu/MenuIcoSearchUserSelected.png";

        public bool _NothingFound = false;
        public bool NothingFound
        {
            get { return _NothingFound; }
            set
            {
                _NothingFound = value;
                OnPropertyChanged("NothingFound");
            }
        }
        
        public string _SearchUserName;
        public string SearchUserName
        {
            get { return _SearchUserName; }
            set
            {
                _SearchUserName = value;
                OnPropertyChanged("SearchUserName");
            }
        }
        public string _SearchTelephoneNumber;
        public string SearchTelephoneNumber
        {
            get { return _SearchTelephoneNumber; }
            set
            {
                _SearchTelephoneNumber = value;
                OnPropertyChanged("SearchTelephoneNumber");
            }
        }
        public string _SearchComputerName;
        public string SearchComputerName
        {
            get { return _SearchComputerName; }
            set
            {
                _SearchComputerName = value;
                OnPropertyChanged("SearchComputerName");
            }
        }
        public string _UserName;
        public string UserName
        {
            get { return _UserName; }
            set
            {
                _UserName = value;
                OnPropertyChanged("UserName");
            }
        }

        public static readonly DependencyProperty BalanceContentProperty = DependencyProperty.Register(
        "EditObject", typeof(User), typeof(UsersViewer));

        //Содержит объект который сейчас редактируется
        public User EditObject
        {
            get
            { return (User)GetValue(BalanceContentProperty); }
            set
            { 
                SetValue(BalanceContentProperty, value);
            }
        }

        public string PingTest(string Address, bool WithDelay = false)
        {
            using (Ping pinger = new Ping())
            {
                try
                {
                    PingReply reply = pinger.Send(Address, 1000);

                    if (reply.Status == IPStatus.Success)
                    {
                        if(WithDelay)Thread.Sleep(1000);
                        return reply.Address.ToString();
                    }
                    else 
                    {
                        return null;
                    }
                }
                catch{
                    if(WithDelay) Thread.Sleep(1000);
                    return null; 
                }
            }
        }
        public string TestPort(string Address, int Port)
        {
            string pingResult = PingTest(Address);
            if (pingResult != null)
            {
                using (TcpClient tcpClient = new TcpClient())
                {
                    var result = tcpClient.BeginConnect(Address, Port, null, null);

                    if (result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5)))
                    {
                        return pingResult;
                    }
                    else { return null; }
                  

                }
            }
            else
            {
                return null;
            }
        }


        public UsersViewer()
        {

            InitializeComponent();
 
            this.DataContext = this;
        }


        public MyCommand ZoomPhoto
        {
            get
            {
                return new MyCommand((obj) =>
                {
                    (Application.Current.MainWindow as MainWindow).EnableBlur = true;

                    EditObject = ((obj as Grid).DataContext as User);

                    (Application.Current.MainWindow as MainWindow).TypeMessage = new ControlViewerZoomPhoto(EditObject);
                    

                },
                (obj) =>
                {
                    return true;
                });
            }
        }
        
        public MyCommand PasswordChange
        {
            get
            {
                return new MyCommand((obj) =>
                {
                    (Application.Current.MainWindow as MainWindow).EnableBlur = true;
                    var MessageChangePassword = new ControlChangePassword();
                    (Application.Current.MainWindow as MainWindow).TypeMessage = MessageChangePassword;
                    
                    EditObject = ((obj as Grid).DataContext as User);
                    MessageChangePassword.ChangePasswordUser = EditObject;

                },
                (obj) =>
                {
                    return true;
                });
            }
        }
        public MyCommand ConnectRadmin
        {
            get
            {
                return new MyCommand((obj) =>
                {
                    if (System.IO.File.Exists(Properties.Settings.Default.RadminPath))
                    {
                        EditObject = ((obj as Grid).DataContext as User);
                        System.Diagnostics.Process.Start(Properties.Settings.Default.RadminPath, "/connect:" + EditObject.ComputerName);
                    }
                    else
                    {
                        (Application.Current.MainWindow as MainWindow).EnableBlur = true;
                        (Application.Current.MainWindow as MainWindow).TypeMessage = new ControlViewError("RadminViewer не установлен, или установлен по другому пути. \nСтандарный путь: " + Properties.Settings.Default.RadminPath);
                    }
                },
                (obj) =>
                {
                    return true;
                });
            }
        }
        public MyCommand OpenFolder
        {
            get
            {
                return new MyCommand((obj) =>
                {
                    RunOpenFolder((obj as Grid).DataContext as User);
                    
                },
                (obj) =>
                {
                    return true;
                });
            }
        }
        public async void RunOpenFolder(User EditObject)
        {
            (Application.Current.MainWindow as MainWindow).EnableBlur = true;
            (Application.Current.MainWindow as MainWindow).TypeMessage = new ControlAnimationLoad();


            string result = null;

            await Task.Run(() => { result = TestPort(EditObject.ComputerName, 135); });

            if (result != null)
            {
                (Application.Current.MainWindow as MainWindow).EnableBlur = false;
                EditObject.ComputerIP = result;
                System.Diagnostics.Process.Start("explorer.exe", @"\\" + EditObject.ComputerName + @"\C$\Users\" + EditObject.UserName);
            }
            else
            {
                (Application.Current.MainWindow as MainWindow).TypeMessage = new ControlViewError("Не возможно открыть папку пользователя, компьютер выключен или недоступен.");
                EditObject.ComputerIP = null;
            }
        }

        public MyCommand ComputerControl
        {
            get
            {
                return new MyCommand((obj) =>
                {

                    EditObject = ((obj as Grid).DataContext as User);
                    RunComputerControl(EditObject);

                },
                (obj) =>
                {
                    return true;
                });
            }
        }

        public async void RunComputerControl(User EditObject)
        {
            (Application.Current.MainWindow as MainWindow).EnableBlur = true;
            (Application.Current.MainWindow as MainWindow).TypeMessage = new ControlAnimationLoad();
            

            string result = null;

            await Task.Run(()=> { result = TestPort(EditObject.ComputerName, 135); });

            if (result != null)
            {
                (Application.Current.MainWindow as MainWindow).EnableBlur = false;
                System.Diagnostics.Process.Start(@"compmgmt.msc", "/computer=" + EditObject.ComputerName);
                EditObject.ComputerIP = result;
            }
            else
            {
                (Application.Current.MainWindow as MainWindow).TypeMessage = new ControlViewError("Не возможно запустить удаленное управление, компьютер выключен или порт удаленного управления недоступен.");
                EditObject.ComputerIP = null;
            }
        }
        public MyCommand Search
        {
            get
            {
                return new MyCommand(async (obj) =>
                {

                    (Application.Current.MainWindow as MainWindow).OldFocusElement = (Application.Current.MainWindow as MainWindow).FocusElement;

                    NetworkTester.StopScan();

                    (Application.Current.MainWindow as MainWindow).EnableBlur = true;
                    (Application.Current.MainWindow as MainWindow).TypeMessage = new ControlAnimationLoad();

                    SearchList.Clear();

                    ListUsers.ItemsSource = SearchList;
                    try
                    {
                        var ListUserFromOu = await DomainSearch.AsyncSearchUsers(SearchUserName, SearchTelephoneNumber, SearchComputerName) ;

                        //Если передали параметр NoHistory, значит не пишем запрос в историю.
                        if ((string)obj != "NoHistory")
                        {
                            //Записываем параметры поиска в список для истории
                            HistoryList.Add(new SerchHistoryItem(DateTime.Now, SearchUserName, SearchTelephoneNumber, SearchComputerName));
                        }
                        await Task.Run(() => 
                        {
                            var t1 = new Thread(() => { 
                                foreach (User user in ListUserFromOu)
                                {
                                    Dispatcher.Invoke(()=> SearchList.Add(user));
                                    Thread.Sleep(10);

                                }
                            });
                            t1.Start();
                            t1.Join();
                        });
                        
                        (Application.Current.MainWindow as MainWindow).EnableBlur = false;

                        Keyboard.Focus((Application.Current.MainWindow as MainWindow).OldFocusElement);
                        
                        NetworkTester.StartScan(SearchList);
                    }
                    catch(Exception e)
                    {
                        (Application.Current.MainWindow as MainWindow).EnableBlur = true;
                        (Application.Current.MainWindow as MainWindow).TypeMessage = new ControlViewError(e.Message);
                    }
                

                },
                (obj) =>
                {
                    return (!(SearchUserName == "" || SearchUserName == null) || !(SearchTelephoneNumber == "" || SearchTelephoneNumber == null) || !(SearchComputerName == "" || SearchComputerName == null)) ;


                });
            }
        }
        
        public MyCommand RenewPing
        {
            get
            {
                return new MyCommand((obj) =>
                {
                    EditObject = ((obj as Grid).DataContext as User);
                    EditObject.PingState = null;
                    AsyncRenewPing(EditObject);

                },
                (obj) =>
                {
                    return (!(SearchUserName == "" || SearchUserName == null) || !(SearchTelephoneNumber == "" || SearchTelephoneNumber == null) || !(SearchComputerName == "" || SearchComputerName == null)) ;


                });
            }
        }

        public async void AsyncRenewPing(User EditObject)
        {
            string result = null;

            await Task.Run(() => { result = PingTest(EditObject.ComputerName, true); });

            if (result != null)
            {
                EditObject.PingState = true;
                EditObject.ComputerIP = result;
            }
            else
            {
                EditObject.PingState = false;
                EditObject.ComputerIP = null;
            }

        }
        public MyCommand CopyToBuffer
        {
            get
            {
                return new MyCommand((obj) =>
                {
                    Clipboard.SetText((string)obj);

                },
                (obj) =>
                {
                    return true;
                });
            }
        }

        //Открывает карту с локацией в которой найден компьютер с именем переданным в параметре и выделяет его
        public MyCommand ViewObjectOnMap
        {
            get
            {
                return new MyCommand((obj) =>
                {
                    
                    //делаем запрос к базе, вычисляем id локации на которой расположен компьютер
                    int? IdLocationsByName = SQLiteBase.GetLocationsByName((string)obj);
                    //Если запрос вернул не null, то загружаем найденную локацию на карту
                    if (IdLocationsByName != null)
                    {
                        //Выбираем меню карта
                        (Application.Current.MainWindow as MainWindow).SelectedMenuId = 1;

                        //Загружаем локацию
                        ((Application.Current.MainWindow as MainWindow).SelectedObj as ControlMaps).SetLocation.Execute(IdLocationsByName);

                        //Перебираем все что есть на этой локации 
                        foreach (MapObject Computer in ((Application.Current.MainWindow as MainWindow).SelectedObj as ControlMaps).ArrayMapObjects)
                        {
                            //И если объект с типом 1-компьютер и имя совпадает с запрашиваемым, то выделяем его
                            if (Computer.Name == (string)obj && Computer.TypeObject.Id == 1)
                            {
                                ((Application.Current.MainWindow as MainWindow).SelectedObj as ControlMaps).ObjectSelectedItem = Computer;
                                break;
                            }
                        }
                    }

                },
                (obj) =>
                {
                    return true;
                });
            }
        }
       
        public MyCommand History
        {
            get
            {
                return new MyCommand((obj) =>
                {
                    (Application.Current.MainWindow as MainWindow).EnableBlur = true;
                    (Application.Current.MainWindow as MainWindow).TypeMessage = new ControlHistoryViewer((string Name, string Telephone, string ComputerName)=> {
                        SearchUserName = Name;
                        SearchTelephoneNumber = Telephone;
                        SearchComputerName = ComputerName;
                        Search.Execute("NoHistory");
                    } );
                    ((Application.Current.MainWindow as MainWindow).TypeMessage as ControlHistoryViewer).HistoryList = HistoryList;
                },
                (obj) =>
                {
                    return true;
                    
                });
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }


    public class InvertBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culturev)
        {
            return ((Visibility)value == Visibility.Collapsed);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    //Вычитаем из ширины окна, ширину скролла
    public class YesNoToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culturev)
        {
            return (double)value - 20;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    
}

