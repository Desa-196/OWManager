﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users
{
    public class MapObject: INotifyPropertyChanged
    {
        public TypeObject TypeObject { get; set; }

        public int? Id = null;

        public string _Name;
        public string Name {
            get { return _Name; }
            set 
            {
                _Name = value;
                OnPropertyChanged("Name");
            }
        }
        public double _XCoordinate;
        public double XCoordinate {
            get { return _XCoordinate; }
            set 
            {
                _XCoordinate = value;
                OnPropertyChanged("XCoordinate");
            }
        }
        public double _YCoordinate;
        public double YCoordinate
        {
            get { return _YCoordinate; }
            set
            {
                _YCoordinate = value;
                OnPropertyChanged("YCoordinate");
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
