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
using Users.Message;

namespace Users
{
    public class MenuItemLocations : MenuItem
    {
        public int Id{get; set;}
        public int IdParent{get; set;}
    }
    public class WidthCanvasMapConvertor : IMultiValueConverter
    {

        public object Convert(object[] value, Type targetType, object parameter, System.Globalization.CultureInfo culturev)
        {
            System.Drawing.SizeF ResolutionMap = (System.Drawing.SizeF)value[3];
            //пробуем установить ширину карты равной ширине канваса, 
            //расчитываем коэффициент изменения размера карты для определения пропорциональной  высоты карты
            double Coef = (double)value[0] / ResolutionMap.Width;

            if (ResolutionMap.Height * Coef > (double)value[1])
            {
                Coef = (double)value[1] / ResolutionMap.Height;
                return (double)ResolutionMap.Width*Coef;
            }
            else 
            {
                return (double)value[0];
            }
            
            
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
    public class HeightCanvasMapConvertor : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, System.Globalization.CultureInfo culturev)
        {
            System.Drawing.SizeF ResolutionMap = (System.Drawing.SizeF)value[3];
            //пробуем установить высоту карты равной ширине канваса, 
            //расчитываем коэффициент изменения размера карты для определения пропорциональной  ширины карты
            double Coef = (double)value[1] / ResolutionMap.Height;

            if (ResolutionMap.Width * Coef > (double)value[0])
            {
                Coef = (double)value[0] / ResolutionMap.Width;
                return (double)ResolutionMap.Height*Coef;
            }
            else 
            {
                return (double)value[1];
            }
            
            
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    public partial class ControlMaps : UserControl, INotifyPropertyChanged
    {
        bool IsFiltered = false;
        public List<MenuItemLocations> Locations { get; set; }

        private List<TypeObject> typeObjects;
        public List<TypeObject> ArrayTypeObject { get; set; } = SQLiteBase.GetTypeObjectMap();
        public ObservableCollection<MapObject> ArrayMapObjects { get; set; } = new ObservableCollection<MapObject>();

        public Canvas ImagePress;

        public MapObject _ObjectSelectedItem;
        public MapObject ObjectSelectedItem
        {
            get { return _ObjectSelectedItem; }
            set 
            {
                _ObjectSelectedItem = value;
                OnPropertyChanged("ObjectSelectedItem");
            }
        }
        public string _SearchString;
        public string SearchString
        {
            get { return _SearchString; }
            set 
            {
                _SearchString = value;
                OnPropertyChanged("SearchString");
            }
        }

        public int _MapId;
        public int MapId
        {
            get { return _MapId; }
            set 
            {
                _MapId = value;
                OnPropertyChanged("MapId");
            }
        }

        public string _MapImage = "Warshavka.jpg";
        public string MapImage
        {
            get { return Environment.CurrentDirectory + @"\maps\" + _MapImage; }
            set 
            {
                _MapImage = value;
                OnPropertyChanged("MapImage");
            }
        }
      

        public string _NameSetLocations = "Варшавка 1-й этаж";
        public string NameSetLocations
        {
            get { return _NameSetLocations; }
            set 
            {
                _NameSetLocations = value;
                OnPropertyChanged("NameSetLocations");
            }
        }

        public double _MultiplicityZoom = 1;
        public double MultiplicityZoom
        {
            get { return _MultiplicityZoom; }
            set 
            {
                _MultiplicityZoom = value;
                OnPropertyChanged("MultiplicityZoom");
            }
        }
        public bool _IsWidthMoreThenHeight = true;
        public bool IsWidthMoreThenHeight
        {
            get { return _IsWidthMoreThenHeight; }
            set 
            {
                _IsWidthMoreThenHeight = value;
                OnPropertyChanged("IsWidthMoreThenHeight");
            }
        }
        public System.Drawing.SizeF _Resolution;
        public System.Drawing.SizeF Resolution
        {
            get { return _Resolution; }
            set 
            {
                _Resolution = value;
                OnPropertyChanged("Resolution");
            }
        }

        public Point CoordinateMousePress = new Point(0,0);
        public Point CoordinateMapsPress = new Point(0, 0);

        Point _CoordinateMap = new Point(0,0);
        public Point CoordinateMap
        {
            get { return _CoordinateMap; }
            set {
                _CoordinateMap = value;
                OnPropertyChanged("CoordinateMap");
            }
        }
        public void  AddObject(string Name, TypeObject Type) 
        {

            ArrayMapObjects.Add(new MapObject(0.1, 0.1, Type));
            ArrayMapObjects[ArrayMapObjects.Count - 1].Name = Name;
            (Application.Current.MainWindow as MainWindow).EnableBlur = false;
        }

        public MyCommand AddObjectCommand
        {
            get
            {
                return new MyCommand((obj) =>
                {
                    (Application.Current.MainWindow as MainWindow).EnableBlur = true;
                    (Application.Current.MainWindow as MainWindow).TypeMessage = new Message.AddObject(obj as MapViewer);

                },
                (obj) =>
                {
                    return true;
                });
            }
        }
        public MyCommand SetLocation
        {
            get
            {
                return new MyCommand((obj) =>
                {

                    Map.Normal.Execute(null);
                    MapImage = SQLiteBase.GetMapImageSource((int)obj);
                    ArrayMapObjects.Clear();

   
                    MapId = (int)obj;
                    
                    foreach (MapObject MapObject in SQLiteBase.GetObjectMap((int)obj))
                    {
                        ArrayMapObjects.Add(MapObject);
                    }

                    NameSetLocations = SQLiteBase.GetNameLocationsFromId((int)obj);

                },
                (obj) =>
                {
                    return true;
                });
            }
        }
        public MyCommand SetFilter
        {
            get
            {
                return new MyCommand((obj) =>
                {
                    IsFiltered = true;
                    ArrayMapObjects.Clear();
                    if ((obj as System.Collections.IList).Count > 0)
                    {
                        List<TypeObject> ArraySelectedType = new List<TypeObject>();
                        foreach (TypeObject type in (System.Collections.IList)obj)
                        {
                            ArraySelectedType.Add(type);
                        }
                        foreach (MapObject ObjectMap in SQLiteBase.GetObjectMap(MapId, ArraySelectedType, SearchString))
                        {
                            ArrayMapObjects.Add(ObjectMap);
                        }
                    }

                },
                (obj) =>
                {
                    return true;
                });
            }
        }
        bool _IsSelectedAll = false;
        public bool IsSelectedAll
        {
            get { return _IsSelectedAll; }
            set
            {
                _IsSelectedAll = value;
                OnPropertyChanged("IsSelectedAll");
            }
        }
        public MyCommand SelectAllTypeFilter
        {
            get
            {
                return new MyCommand((obj) =>
                {
                    if (!IsSelectedAll) (obj as ListBox).SelectAll();
                    else (obj as ListBox).UnselectAll();
                    IsSelectedAll = !IsSelectedAll;

                },
                (obj) =>
                {
                    return true;
                });
            }
        }
        public MyCommand CancelFilter
        {
            get
            {
                return new MyCommand((obj) =>
                {
                    ArrayMapObjects.Clear();
                    
                    foreach (MapObject ObjectMap in SQLiteBase.GetObjectMap(MapId))
                    {
                        ArrayMapObjects.Add(ObjectMap);
                    }
                    IsFiltered = false;



                },
                (obj) =>
                {
                    return IsFiltered;
                });
            }
        }


        public MyCommand ViewUsers
        {
            get
            {
                return new MyCommand((obj) =>
                {

                    (Application.Current.MainWindow as MainWindow).SelectedMenuId = 0;
                    ((Application.Current.MainWindow as MainWindow).SelectedObj as UsersViewer).SearchComputerName = (string)obj;
                    ((Application.Current.MainWindow as MainWindow).SelectedObj as UsersViewer).SearchTelephoneNumber = string.Empty;
                    ((Application.Current.MainWindow as MainWindow).SelectedObj as UsersViewer).SearchUserName = string.Empty;
                    ((Application.Current.MainWindow as MainWindow).SelectedObj as UsersViewer).Search.Execute(null);


                },
                (obj) =>
                {
                    return true;
                });
            }
        }

        public string MenuImage { get; set; } = "Ресурсы/menu/MenuIcoMaps.png";
        public string MenuImageSelected { get; set; } = "Ресурсы/menu/MenuIcoMapsSelected.png";
        public ControlMaps()
        {
            InitializeComponent();
            this.DataContext = this;
            Application.Current.MainWindow.MouseMove += Image_MouseMove;
            Application.Current.MainWindow.MouseUp += Image_MouseLeftButtonUp;

            ArrayMapObjects = SQLiteBase.GetObjectMap(2);
            MapId = 2;

            typeObjects = SQLiteBase.GetTypeObjectMap();

            //Делаем запрос на получение основных пунктов меню, -1 - значит поле parent_id отсутствует. 
            Locations = SQLiteBase.GetMenuItem(-1);

            foreach (MenuItemLocations MainItem in Locations)
            {
                //Список элементов который будет пополнятся элементами меню для поиска дочерних элементов
                List<MenuItemLocations> ochered = new List<MenuItemLocations>();

                //Добавляем в очередь главное меню
                ochered.Add(MainItem);

                for (int i = 0; ochered.Count > i; i++)
                {
                    //Получаем дочерние элементы из базы
                    List<MenuItemLocations> ChildrenElementsList = SQLiteBase.GetMenuItem(ochered[i].Id);
                    ochered[i].Command = SetLocation;
                    ochered[i].CommandParameter = ochered[i].Id;

                    //Если дочерние элементы существуют
                    if (ChildrenElementsList != null)
                    {
                        //Добавляем в меню по которому сейчас проходим найденные дочерние элементы 
                        ochered[i].ItemsSource = ChildrenElementsList;

                        //Проходимся по ним и добавляем в очередь на поиск дочерних элементов
                        foreach (MenuItemLocations ChildrenElement in ChildrenElementsList)
                        {
                            ochered.Add(ChildrenElement);
                        }
                    }
                    //Если дочерние элементы отсутствуют, значит это последний элемент в списке, навесим на него команду
                    else 
                    {
                        
                    }
                }
            }


        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ImagePress = sender as Canvas;
                CoordinateMousePress.X = e.GetPosition((sender as Canvas).Parent as Canvas).X;
                CoordinateMousePress.Y = e.GetPosition((sender as Canvas).Parent as Canvas).Y;
                CoordinateMapsPress = CoordinateMap;
     
            }
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ImagePress = null; 
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (ImagePress is Canvas && Mouse.LeftButton == MouseButtonState.Pressed)
            {
                CoordinateMap = new Point
                    (
                        CoordinateMapsPress.X - (CoordinateMousePress.X - e.GetPosition(ImagePress.Parent as Canvas).X),
                        CoordinateMapsPress.Y - (CoordinateMousePress.Y - e.GetPosition(ImagePress.Parent as Canvas).Y)
                    );
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           ( sender as ListBox).ScrollIntoView((sender as ListBox).SelectedItem);

        }
    }
}
