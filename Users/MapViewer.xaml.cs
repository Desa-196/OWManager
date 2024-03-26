using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Users
{

    public partial class MapViewer : UserControl, INotifyPropertyChanged
    {


        public MapObject CutBufferObject
        {
            get { return (Application.Current.MainWindow as MainWindow).CutBufferObject; }
            set
            {
                (Application.Current.MainWindow as MainWindow).CutBufferObject = value;
                OnPropertyChanged("CutBufferObject");
            }
        }

        Point coordinateClickContextMenu;

        bool IsMapMove = false;

        System.Drawing.Size SourceMapSize;

        public MapObject SelectedObject
        {
            get
            {
                return (MapObject)GetValue(SelectedObjectProperty);
            }
            set
            {
                SetValue(SelectedObjectProperty, value);
            }
        }


        public int ItemSelection
        {
            get
            {
                return (int)GetValue(ItemSelectionProperty);
            }
            set
            {
                if (SourceElements != null)
                {
                    SelectedObject = SourceElements.Where(x => x.Id == value).FirstOrDefault();

                }
                SetValue(ItemSelectionProperty, value);
            }
        }
        public ObservableCollection<MapObject> SourceElements
        {
            get
            { return (ObservableCollection<MapObject>)GetValue(SourceElementsProperty); }
            set
            {
                SetValue(SourceElementsProperty, value);
            }
        }

        public string MapImageSource
        {
            get
            {
                return (string)GetValue(MapImageSourceProperty);
            }
            set
            {
                SetValue(MapImageSourceProperty, value);
            }
        }

        public int MapId
        {
            get
            {
                return (int)GetValue(MapIdProperty);
            }
            set
            {
                System.Windows.MessageBox.Show(value.ToString());
                Normal.Execute(null);
                SetValue(MapIdProperty, value);
            }
        }

        public static readonly DependencyProperty MapIdProperty = DependencyProperty.Register(
        "MapId", typeof(int), typeof(MapViewer));

        public static readonly DependencyProperty SourceElementsProperty = DependencyProperty.Register(
        "SourceElements", typeof(ObservableCollection<MapObject>), typeof(MapViewer));

        public static readonly DependencyProperty ItemSelectionProperty = DependencyProperty.Register(
        "ItemSelection", typeof(int), typeof(MapViewer));

        public static readonly DependencyProperty SelectedObjectProperty = DependencyProperty.Register(
        "SelectedObject", typeof(MapObject), typeof(MapViewer));

        public static readonly DependencyProperty MapImageSourceProperty = DependencyProperty.Register(
        "MapImageSource", typeof(string), typeof(MapViewer), new FrameworkPropertyMetadata(null,
        FrameworkPropertyMetadataOptions.AffectsRender,
        new PropertyChangedCallback(OnUriChanged)));

        //Функция обратного вызова, выполняется когда обновляется картинка карты
        private static void OnUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            string path = d.GetValue(MapImageSourceProperty).ToString();

            //Получаем разрешение исходной картинки карты
            (d as MapViewer).SourceMapSize = new System.Drawing.Bitmap(path).Size;

            (d as MapViewer).UserControl_Loaded(null, null);
            (d as MapViewer).MultiplicityZoom = 1;
            (d as MapViewer).UserControl_SizeChanged(null, null);

        }

        //Коэфициент умножения при увеличении/уменьшении
        float CoefZoom = 1.1F;

        public Point CoordinateMousePress = new Point(0, 0);
        public Point CoordinateMapsPress = new Point(0, 0);

        ListBox ImagePress;

        double InitialWidth = 0;
        double InitialHeight = 0;

        double OldWidth = 0;
        double OldHeight = 0;

        public int MultiplicityZoom = 1;
        int MaxZoom = 40;

        double _WidthMap;
        public double WidthMap
        {
            get { return _WidthMap; }
            set
            {
                _WidthMap = value;
                OnPropertyChanged("WidthMap");
            }
        }
        double _HeightMap;
        public double HeightMap
        {
            get { return _HeightMap; }
            set
            {
                _HeightMap = value;
                OnPropertyChanged("HeightMap");
            }
        }
        Point _CoordinateMap = new Point(0, 0);
        public Point CoordinateMap
        {
            get { return _CoordinateMap; }
            set
            {
                _CoordinateMap = value;
                OnPropertyChanged("CoordinateMap");
            }
        }
        public MapViewer()
        {
            InitializeComponent();
            Application.Current.MainWindow.MouseMove += Image_MouseMove;
            Application.Current.MainWindow.MouseUp += Image_MouseLeftButtonUp;
            ItemSelection = -1;
            PastMenu.DataContext = this;
        }
        public void AddObject(string Name, string description, TypeObject Type, Point addObjectCoordinate)
        {
            double CreateObjectCoordinateX;
            double CreateObjectCoordinateY;

            if (addObjectCoordinate.X == 0 && addObjectCoordinate.Y == 0)
            {
                CreateObjectCoordinateX = ((this.ActualWidth / 2) - CoordinateMap.X) / WidthMap;
                CreateObjectCoordinateY = ((this.ActualHeight / 2) - CoordinateMap.Y) / HeightMap;
            }
            else
            {
                CreateObjectCoordinateX = addObjectCoordinate.X;
                CreateObjectCoordinateY = addObjectCoordinate.Y;
            }
            SourceElements.Add(new MapObject(CreateObjectCoordinateX, CreateObjectCoordinateY, Type));
            SourceElements[SourceElements.Count - 1].Name = Name;
            SourceElements[SourceElements.Count - 1].Description = description;
            ItemSelection = SourceElements.Count - 1;
            (Application.Current.MainWindow as MainWindow).EnableBlur = false;
            Save.Execute(null);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Map_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitialWidth = this.ActualWidth;
            InitialHeight = this.ActualHeight;
            OldWidth = InitialWidth;
            OldHeight = InitialHeight;
            UserControl_SizeChanged(sender, e);
        }
        private void UserControl_SizeChanged(object sender, RoutedEventArgs e)
        {

            if (MultiplicityZoom == 1)
            {

                //пробуем установить ширину карты равной ширине канваса, 
                //расчитываем коэффициент изменения размера карты для определения пропорциональной  высоты карты
                double Coef = this.ActualWidth / SourceMapSize.Width;

                if (SourceMapSize.Height * Coef > this.ActualHeight)
                {
                    Coef = this.ActualHeight / SourceMapSize.Height;
                    WidthMap = (double)SourceMapSize.Width * Coef;
                    CoordinateMap = new Point(((this.ActualWidth - WidthMap) / 2.0), 0);
                    HeightMap = this.ActualHeight;
                }
                else
                {
                    WidthMap = this.ActualWidth;
                    HeightMap = (double)SourceMapSize.Height * Coef;
                    CoordinateMap = new Point(0, ((this.ActualHeight - HeightMap) / 2.0));
                }
            }
            else
            {
                if (Math.Abs(this.ActualWidth - OldWidth) > 0)
                {
                    if (WidthMap - this.ActualWidth + CoordinateMap.X < 0 && this.ActualWidth < WidthMap)
                    {
                        CoordinateMap = new Point(CoordinateMap.X + Math.Abs(WidthMap - this.ActualWidth + CoordinateMap.X), CoordinateMap.Y);
                    }
                    else if (WidthMap - this.ActualWidth + CoordinateMap.X < 0 && this.ActualWidth > WidthMap)
                    {
                        double coef = WidthMap / HeightMap;
                        WidthMap += Math.Abs(WidthMap - this.ActualWidth + CoordinateMap.X);
                        HeightMap = WidthMap / coef;
                    }

                }
                if (Math.Abs(this.ActualHeight - OldHeight) > 0)
                {
                    if (HeightMap - this.ActualHeight + CoordinateMap.Y < 0 && this.ActualHeight < HeightMap)
                    {
                        CoordinateMap = new Point(CoordinateMap.X, CoordinateMap.Y + Math.Abs(HeightMap - this.ActualHeight + CoordinateMap.Y));
                    }
                    else if (HeightMap - this.ActualHeight + CoordinateMap.Y < 0 && this.ActualHeight > HeightMap)
                    {
                        double coef = HeightMap / WidthMap;
                        HeightMap += Math.Abs(HeightMap - this.ActualHeight + CoordinateMap.Y);
                        WidthMap = HeightMap / coef;
                    }

                }

                OldWidth = this.ActualWidth;
                OldHeight = this.ActualHeight;
            }
        }
        public MyCommand Zoom
        {
            get
            {
                return new MyCommand((obj) =>
                {
                    var CursorCoordinateRelativeMap = new Point((this.ActualWidth / 2) - CoordinateMap.X, (this.ActualHeight / 2) - CoordinateMap.Y);
                    ChangeZoom(CursorCoordinateRelativeMap, new Point(this.ActualWidth / 2, this.ActualHeight / 2), true);
                },
                (obj) =>
                {
                    return MultiplicityZoom == MaxZoom ? false : true;
                });
            }
        }
        public MyCommand UnZoom
        {
            get
            {
                return new MyCommand((obj) =>
                {
                    var CursorCoordinateRelativeMap = new Point((this.ActualWidth / 2) - CoordinateMap.X, (this.ActualHeight / 2) - CoordinateMap.Y);
                    ChangeZoom(CursorCoordinateRelativeMap, new Point(this.ActualWidth / 2, this.ActualHeight / 2), false);

                },
                (obj) =>
                {
                    return MultiplicityZoom == 1 ? false : true;
                });
            }
        }
        /// <summary>
        /// Комманда отключает увеличение карты
        /// </summary>
        public MyCommand Normal
        {
            get
            {
                return new MyCommand((obj) =>
                {
                    MultiplicityZoom = 1;
                    UserControl_SizeChanged(null, null);
                },
                (obj) =>
                {
                    return MultiplicityZoom == 1 ? false : true;
                });
            }
        }

        public MyCommand Save
        {
            get
            {
                return new MyCommand((obj) =>
                {
                    SQLiteBase.SaveElements(SourceElements, MapId);
                },
                (obj) =>
                {
                    return true;
                });
            }
        }

        public MyCommand AddObjectCommand
        {
            get
            {
                return new MyCommand((obj) =>
                {
                    (Application.Current.MainWindow as MainWindow).EnableBlur = true;
                    (Application.Current.MainWindow as MainWindow).TypeMessage = new Message.AddObject(this);

                },
                (obj) =>
                {
                    return true;
                });
            }
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender is ListBox)
            {
                IsMapMove = false;
                ImagePress = sender as ListBox;

                if (MultiplicityZoom != 1)
                {

                    CoordinateMousePress.X = e.GetPosition((sender as ListBox).Parent as Canvas).X;
                    CoordinateMousePress.Y = e.GetPosition((sender as ListBox).Parent as Canvas).Y;
                    CoordinateMapsPress = CoordinateMap;
                }

            }
            if (e.RightButton == MouseButtonState.Pressed && sender is ListBox)
            {
                coordinateClickContextMenu = new Point(e.GetPosition(sender as ListBox).X / (sender as ListBox).ActualWidth, e.GetPosition(sender as ListBox).Y / (sender as ListBox).ActualHeight);
            }
        }

        Point CoordinateMouseOnObject;
        Point CoordinateMouseOnMap;
        double InitializeCoordinateX;
        double InitializeCoordinateY;
        Border IsMoveObject;

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsMapMove == false && ImagePress is ListBox)
            {
                ItemSelection = -1;
            }
            ImagePress = null;
            IsMoveObject = null;
            MapsImage.IsEnabled = true;
            Save.Execute(null);
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (ImagePress is ListBox && Mouse.LeftButton == MouseButtonState.Pressed && MultiplicityZoom != 1)
            {
                IsMapMove = true;

                double CoordinateMapX = CoordinateMapsPress.X - (CoordinateMousePress.X - e.GetPosition(ImagePress.Parent as Canvas).X);
                double CoordinateMapY = CoordinateMapsPress.Y - (CoordinateMousePress.Y - e.GetPosition(ImagePress.Parent as Canvas).Y);

                if (CoordinateMapX > 0) CoordinateMapX = 0;
                if (CoordinateMapY > 0) CoordinateMapY = 0;

                if (CoordinateMapX + WidthMap < this.ActualWidth) CoordinateMapX = (WidthMap - this.ActualWidth) * -1;
                if (CoordinateMapY + HeightMap < this.ActualHeight) CoordinateMapY = (HeightMap - this.ActualHeight) * -1;

                CoordinateMap = new Point(CoordinateMapX, CoordinateMapY);
            }
            if (IsMoveObject != null && e.LeftButton == MouseButtonState.Pressed)
            {
                MapsImage.IsEnabled = false;
                Point DisplacementMouse = new Point(
                    e.GetPosition(this).X - CoordinateMouseOnMap.X,
                    e.GetPosition(this).Y - CoordinateMouseOnMap.Y
                    );


                double XCoordinate = InitializeCoordinateX * MapsImage.ActualWidth + DisplacementMouse.X;
                double YCoordinate = InitializeCoordinateY * MapsImage.ActualHeight + DisplacementMouse.Y;

                double WidthObject = (IsMoveObject.Parent as Grid).ActualWidth;
                double HeightObject = (IsMoveObject.Parent as Grid).ActualHeight;

                if (XCoordinate > MapsImage.ActualWidth - WidthObject / 2) XCoordinate = MapsImage.ActualWidth - WidthObject / 2;
                else if (XCoordinate < WidthObject / 2) XCoordinate = WidthObject / 2;

                if (YCoordinate > MapsImage.ActualHeight - HeightObject / 2) YCoordinate = MapsImage.ActualHeight - HeightObject / 2;
                else if (YCoordinate < HeightObject / 2) YCoordinate = HeightObject / 2;

                (IsMoveObject.DataContext as MapObject).XCoordinate = XCoordinate / MapsImage.ActualWidth;
                (IsMoveObject.DataContext as MapObject).YCoordinate = YCoordinate / MapsImage.ActualHeight;

            }
        }

        void ChangeZoom(Point CursorCoordinateRelativeMap, Point CursorCoordinateRelativeThis, bool Zoom)
        {
            if (Zoom && MultiplicityZoom < MaxZoom)
            {
                double RealCoefZoom = 0;

                //Если после увеличения карты её ширина оказывается меньше ширины окна, растягиваем до размера.
                if (WidthMap * CoefZoom < this.ActualWidth)
                {
                    RealCoefZoom = this.ActualWidth / WidthMap;
                    HeightMap *= RealCoefZoom;
                    WidthMap = this.ActualWidth;
                }
                //Если после увеличения карты её высота оказывается меньше высоты окна, растягиваем до размера.
                else if (HeightMap * CoefZoom < this.ActualHeight)
                {
                    RealCoefZoom = this.ActualHeight / HeightMap;
                    WidthMap *= RealCoefZoom;
                    HeightMap = this.ActualHeight;
                }
                //Если новый размер карты полностью покрывает размер окна, то увеличиваем.
                else
                {
                    RealCoefZoom = CoefZoom;
                    WidthMap *= CoefZoom;
                    HeightMap *= CoefZoom;
                }

                //Определяем координаты курсора мыши и пересчитываем координаты на увеличенной карте.
                CursorCoordinateRelativeMap = new Point
                (
                    CursorCoordinateRelativeMap.X * RealCoefZoom,
                    CursorCoordinateRelativeMap.Y * RealCoefZoom
                );

                MultiplicityZoom += 1;
                CommandManager.InvalidateRequerySuggested();

                double CoordinateMapX = (CursorCoordinateRelativeMap.X - (CursorCoordinateRelativeThis.X)) * -1;
                double CoordinateMapY = (CursorCoordinateRelativeMap.Y - (CursorCoordinateRelativeThis.Y)) * -1;

                if (CoordinateMapX > 0) CoordinateMapX = 0;
                if (CoordinateMapY > 0) CoordinateMapY = 0;

                if (CoordinateMapX + WidthMap < this.ActualWidth) CoordinateMapX = (WidthMap - this.ActualWidth) * -1;
                if (CoordinateMapY + HeightMap < this.ActualHeight) CoordinateMapY = (HeightMap - this.ActualHeight) * -1;

                CoordinateMap = new Point(CoordinateMapX, CoordinateMapY);

            }
            else if (!Zoom)
            {
                MultiplicityZoom -= 1;
                CommandManager.InvalidateRequerySuggested();
                //Если после уменьшения кары она всё ещё не меньше окна, то уменьшаем
                if (WidthMap / CoefZoom > this.ActualWidth && HeightMap / CoefZoom > this.ActualHeight)
                {
                    double RelativeClickCoordinateX = CursorCoordinateRelativeMap.X / CoefZoom;
                    double RelativeClickCoordinateY = CursorCoordinateRelativeMap.Y / CoefZoom;

                    HeightMap /= CoefZoom;
                    WidthMap /= CoefZoom;


                    double CoordinateMapX = (RelativeClickCoordinateX - (CursorCoordinateRelativeThis.X)) * -1;
                    double CoordinateMapY = (RelativeClickCoordinateY - (CursorCoordinateRelativeThis.Y)) * -1;

                    if (CoordinateMapX > 0) CoordinateMapX = 0;
                    if (CoordinateMapY > 0) CoordinateMapY = 0;

                    if (CoordinateMapX + WidthMap < this.ActualWidth) CoordinateMapX = (WidthMap - this.ActualWidth) * -1;
                    if (CoordinateMapY + HeightMap < this.ActualHeight) CoordinateMapY = (HeightMap - this.ActualHeight) * -1;

                    CoordinateMap = new Point(CoordinateMapX, CoordinateMapY);
                }
                else
                {
                    Normal.Execute(null);
                }
            }
        }

        private void MapsImage_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            //Определяем координаты курсора мыши и пересчитываем координаты на увеличенной карте.
            Point RelativeClickCoordinate = e.GetPosition((ListBox)sender);

            //Определяем координаты курсора мыши на самом контроле
            Point CanvasCoordinateClick = e.GetPosition(this);

            if (e.Delta > 0)
            {
                ChangeZoom(RelativeClickCoordinate, CanvasCoordinateClick, true);
            }
            else
            {
                ChangeZoom(RelativeClickCoordinate, CanvasCoordinateClick, false);
            }

        }


        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsMoveObject = (sender as Border);
            CoordinateMouseOnObject = e.GetPosition((sender as Border).Parent as Grid);
            CoordinateMouseOnMap = e.GetPosition(this);
            InitializeCoordinateX = (IsMoveObject.DataContext as MapObject).XCoordinate;
            InitializeCoordinateY = (IsMoveObject.DataContext as MapObject).YCoordinate;
        }

        private void MenuItem_Delete(object sender, RoutedEventArgs e)
        {
            var MapObject = ((sender as MenuItem).DataContext as MapObject);
            MessageBoxResult result = System.Windows.MessageBox.Show($"Вы уверены что хотите удалить '{MapObject.Name}'?", "Удаление", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                SQLiteBase.DeleteElement(MapObject.Id);
                SourceElements.Remove(MapObject);
            }
        }

        private void SearchUser(object sender, RoutedEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow).SelectedMenuId = 0;
            ((Application.Current.MainWindow as MainWindow).SelectedObj as UsersViewer).SearchComputerName = ((sender as MenuItem).DataContext as MapObject).Name;
            ((Application.Current.MainWindow as MainWindow).SelectedObj as UsersViewer).SearchTelephoneNumber = string.Empty;
            ((Application.Current.MainWindow as MainWindow).SelectedObj as UsersViewer).SearchUserName = string.Empty;
            ((Application.Current.MainWindow as MainWindow).SelectedObj as UsersViewer).Search.Execute(null);
        }

        private void AddContextMenuClick(object sender, RoutedEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow).EnableBlur = true;
            (Application.Current.MainWindow as MainWindow).TypeMessage = new Message.AddObject(this, coordinateClickContextMenu);
        }

        private void CutObject(object sender, RoutedEventArgs e)
        {
            if (CutBufferObject != null)
            {
                CutBufferObject.IsCut = false;
            }
            CutBufferObject = (sender as MenuItem).DataContext as MapObject;
            ((sender as MenuItem).DataContext as MapObject).IsCut = true;

        }

        private void PastObject(object sender, RoutedEventArgs e)
        {
            double CreateObjectCoordinateX;
            double CreateObjectCoordinateY;

            if (coordinateClickContextMenu.X == 0 && coordinateClickContextMenu.Y == 0)
            {
                CreateObjectCoordinateX = ((this.ActualWidth / 2) - CoordinateMap.X) / WidthMap;
                CreateObjectCoordinateY = ((this.ActualHeight / 2) - CoordinateMap.Y) / HeightMap;
            }
            else
            {
                CreateObjectCoordinateX = coordinateClickContextMenu.X;
                CreateObjectCoordinateY = coordinateClickContextMenu.Y;
            }

            //Если объект в буфере в той же локации куда хотим вставить
            if (CutBufferObject.Location == MapId)
            {
                CutBufferObject.XCoordinate = CreateObjectCoordinateX;
                CutBufferObject.YCoordinate = CreateObjectCoordinateY;
                SQLiteBase.ChangeLocationObject(CutBufferObject, MapId, CreateObjectCoordinateX, CreateObjectCoordinateY);
                CutBufferObject.IsCut = false;
                CutBufferObject = null;
               

            }
            //А если вставляем на другую локацию
            else
            {
                CutBufferObject.XCoordinate = CreateObjectCoordinateX;
                CutBufferObject.YCoordinate = CreateObjectCoordinateY;
                CutBufferObject.Location = MapId;
                SourceElements.Add(CutBufferObject);
                SQLiteBase.ChangeLocationObject(CutBufferObject, MapId, CreateObjectCoordinateX, CreateObjectCoordinateY);
                ItemSelection = SourceElements.Count - 1;
                CutBufferObject.IsCut = false;
                CutBufferObject = null;
            }

        }
    }
}
