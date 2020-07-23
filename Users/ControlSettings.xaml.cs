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
    /// Логика взаимодействия для ControlSettings.xaml
    /// </summary>
    public partial class ControlSettings : UserControl
    {
        public string MenuImage { get; set; } = "Ресурсы/menu/MenuIcoSetting.png";
        public string MenuImageSelected { get; set; } = "Ресурсы/menu/MenuIcoSettingSelected.png";
        public ControlSettings()
        {
            InitializeComponent();
        }
    }
}
