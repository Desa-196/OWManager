﻿using System;
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

namespace Users.MapObjects
{
    /// <summary>
    /// Логика взаимодействия для PrinterObjectControl.xaml
    /// </summary>
    public partial class PrinterObjectControl : UserControl
    {
        public string ImageSource { get; set; } = Environment.CurrentDirectory + @"\IconObjects\Printer.png";

        public PrinterObjectControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}
