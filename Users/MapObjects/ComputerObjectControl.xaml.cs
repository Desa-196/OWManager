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
    /// Логика взаимодействия для ComputerObjectControl.xaml
    /// </summary>
    public partial class ComputerObjectControl : UserControl
    {
        public static readonly DependencyProperty RelationXCoordinateDP = DependencyProperty.Register(
        "RelationXCoordinate", typeof(double), typeof(ComputerObjectControl));

        //Содержит объект который сейчас редактируется
        public double RelationXCoordinate
        {
            get
            { return (double)GetValue(RelationXCoordinateDP); }
            set
            {
                SetValue(RelationXCoordinateDP, value);
                Canvas.SetLeft(this, RelationXCoordinate * value);
            }
        }
        public static readonly DependencyProperty BalanceContentProperty = DependencyProperty.Register(
        "ParentActualWidth", typeof(double), typeof(ComputerObjectControl));

        //Содержит объект который сейчас редактируется
        public double ParentActualWidth
        {
            get
            { return (double)GetValue(BalanceContentProperty); }
            set
            {
                SetValue(BalanceContentProperty, value);
                Canvas.SetLeft(this, RelationXCoordinate * value);
            }
        }

        public string ImageSource { get; set; } = Environment.CurrentDirectory + @"\IconObjects\PC.png";
        public ComputerObjectControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}
