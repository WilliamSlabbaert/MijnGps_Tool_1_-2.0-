using System;
using System.Collections.Generic;
using System.Text;

namespace MijnGps
{
    class Straat
    {
        public int ID { get; }
        public Graaf graaf { get; set; }
        public string Name { get; }

        public Straat(int id, string name)
        {
            this.ID = id;
            this.Name = name;
            this.graaf = null;
        }

        public Straat(int id, string name, Graaf graaf)
        {
            this.ID = id;
            this.Name = name;
            this.graaf = graaf;
        }

        public void showStraat()
        {
            Console.WriteLine("[" + this.ID + "] " + this.Name + " : " + this.graaf.ToString());
        }

        public List<Knoop> getKnopen()
        {
            return this.graaf.getKnopen();
        }

        public override string ToString()
        {
            
            return $"id =>{ID}<= name =>{Name}<";
        }
    }
}
