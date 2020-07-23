using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Users
{
    public class User:INotifyPropertyChanged
    {

        public bool IsAddToMap { get; set; } = false;
        public string AxaptaId { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Description { get; set; }
        public bool EnableAccount { get; set; } = true;
        public string ComputerName { get; set; }
        public string _ComputerIP;
        public string ComputerIP 
        {
            get
            {
                return _ComputerIP;
            }
            set 
            {
                _ComputerIP = value;
                OnPropertyChanged("ComputerIP");
            } 
        }
        public DateTime TimeLoginInComputer { get; set; }
        public string DomainOU { get; set; }
        public string TelephoneNumber { get; set; }

        public bool? _PingState = null;
        public bool? PingState
        {
            get { return _PingState; }
            set
            {
                _PingState = value;
                OnPropertyChanged("PingState");
            }
        }
        public string AddressPhoto
        {
            get 
            {
                if (AxaptaId == "" || AxaptaId == null || !System.IO.File.Exists(Properties.Settings.Default.PhotoPath + "//" + AxaptaId + @".jpg"))
                {
                    return "Ресурсы/no_photo.jpg";
                }
                else 
                {
                
                    return Properties.Settings.Default.PhotoPath + "//" + AxaptaId + @".jpg";
                   
                }
            }
            set { AddressPhoto = value; }
        }

        
        public User(string Name) 
        {
            this.Name = Name;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
