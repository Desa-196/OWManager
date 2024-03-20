using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices.AccountManagement;
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
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class ControlChangePassword : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty CurrentNumberProperty =
            DependencyProperty.Register("ChangePasswordUser", typeof(User),
            typeof(ControlChangePassword));
        public User ChangePasswordUser
        {
            get { return (User)GetValue(CurrentNumberProperty); }
            set
            {
                SetValue(CurrentNumberProperty, value);
            }
        }

        public bool _isCheckedDefaultPassword = true;
        public bool isCheckedDefaultPassword
        {
            get { return _isCheckedDefaultPassword; }
            set
            {
                _isCheckedDefaultPassword = value;
                OnPropertyChanged("isCheckedDefaultPassword");
            }
        }
        public bool _isCheckedUnlockUser = false;
        public bool isCheckedUnlockUser
        {
            get { return _isCheckedUnlockUser; }
            set
            {
                _isCheckedUnlockUser = value;
                OnPropertyChanged("isCheckedUnlockUser");
            }
        }
        public bool _isChekedRequireChangePassword = false;
        public bool isChekedRequireChangePassword
        {
            get { return _isChekedRequireChangePassword; }
            set
            {
                _isChekedRequireChangePassword = value;
                OnPropertyChanged("isChekedRequireChangePassword");
            }
        }
        public string _Password;
        public string Password
        {
            get { return _Password; }
            set
            {
                _Password = value;
                OnPropertyChanged("Password");
            }
        }

        public MyCommand ChangePassword
        {
            get
            {
                return new MyCommand((obj) =>
                {
                    using (var context = new PrincipalContext(ContextType.Domain))
                    {
                        using (var user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, ChangePasswordUser.UserName))
                        {
                            try
                            {
                                user.SetPassword((isCheckedDefaultPassword ? "Nhfrnjhbcn20" : Password));
                                //Если галка стоит, то делаем пароль истекшим для того чтобы система попросила пользователя сменить пароль при входе
                                if (isChekedRequireChangePassword) user.ExpirePasswordNow();
                                //Разблокирует пользователя
                                if (isCheckedUnlockUser) user.UnlockAccount();
                                //Если галка на стандартном пароле стоит, то устанавливаем пароль 111111, если нет, то тот что вбит пользователем.
                                user.SetPassword((isCheckedDefaultPassword ? "Nhfrnjhbcn20" : Password));


                                user.Save();
                                (Application.Current.MainWindow as MainWindow).EnableBlur = false;
                            }
                            catch(System.DirectoryServices.AccountManagement.PasswordException e) 
                            {
                                System.Windows.MessageBox.Show(e.Message);
                            }
                        }
                    }


                },
                (obj) =>
                {
                    if (Password != null)
                    {
                        if (Password.Length == 0 && isCheckedDefaultPassword == false) { return false; }
                        else { return true; }
                    }
                    else if (isCheckedDefaultPassword) { return true; }
                    else { return false; }


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

        public ControlChangePassword()
        {
            InitializeComponent();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

