using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users
{
    public class TypeObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string ImageSource { get; set; }
        public TypeObject(int Id, string Name, string Color, string ImageSource)
        {
            this.Id = Id;
            this.Name = Name;
            this.Color = Color;
            this.ImageSource = Environment.CurrentDirectory + @"\IconObjects\" + ImageSource;
        }
    }
}
