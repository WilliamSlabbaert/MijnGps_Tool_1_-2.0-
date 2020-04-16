using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MijnGps
{
    class Gemeente
    {
        public int ID { get; set; }
        public String Name { get; set; }

        public List<Straat> straten = new List<Straat>();

        public Gemeente(int id, String name)
        {
            this.ID = id;
            this.Name = name;
        }

        public void addStreet(Straat straat)
        {
            if(!this.straten.Contains(straat))
            {
                this.straten.Add(straat);
            }
        }
        public override string ToString()
        {
           
          return $"({Name}) + ({straten.Count})";
        }
    }
}
