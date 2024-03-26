using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Users
{
    public class MapObject: INotifyPropertyChanged
    {
        public TypeObject TypeObject { get; set; }

        private bool isCut = false;
        public bool IsCut
        {
            get { return isCut; }
            set
            {
                isCut = value;
                OnPropertyChanged("IsCut");
            }
        } 
        
        private int location;
        public int Location
        {
            get { return location; }
            set
            {
                location = value;
                OnPropertyChanged("Location");
            }
        } 
        
        private int? _Id = null;
        public int? Id
        {
            get { return _Id; }
            set
            {
                _Id = value;
                OnPropertyChanged("Id");
            }
        }

        private string _Name;
        public string Name {
            get { return _Name; }
            set 
            {
                _Name = value;
                OnPropertyChanged("Name");
            }
        }
        private double _XCoordinate;
        public double XCoordinate {
            get { return _XCoordinate; }
            set 
            {
                _XCoordinate = value;
                OnPropertyChanged("XCoordinate");
            }
        }
        private double _YCoordinate;
        public double YCoordinate
        {
            get { return _YCoordinate; }
            set
            {
                _YCoordinate = value;
                OnPropertyChanged("YCoordinate");
            }
        }
        private string _Description;
        public string Description
        {
            get { return _Description; }
            set
            {
                _Description = value;
                OnPropertyChanged("Description");
            }
        }

        public MyCommand ChangeObject
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

        public MapObject( double XCoordinate, double YCoordinate, TypeObject TypeObject)
        {
            this.XCoordinate = XCoordinate;
            this.YCoordinate = YCoordinate;
            this.TypeObject = TypeObject;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


}
