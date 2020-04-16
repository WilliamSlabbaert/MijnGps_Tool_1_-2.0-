using System;
using System.Collections.Generic;
using System.Text;

namespace MijnGps
{
    class Provincie
    {
        public int ID { get; set; }
        public String Name { get; set; }

        public List<Gemeente> gemeentes = new List<Gemeente>();
        public int stratenCount;

        public Provincie(int id, String name)
        {
            this.ID = id;
            this.Name = name;
        }

        public void addGemeente(Gemeente gemeente)
        {
            if (!gemeentes.Contains(gemeente))
                gemeentes.Add(gemeente);
        }

        public override string ToString()
        {
            stratenCount = 0;
            foreach (Gemeente gem in gemeentes)
            {
                stratenCount += gem.straten.Count;
            }
            return ID + " " + Name + " Gemeentes=>(" + gemeentes.Count +") Straten=> (" +stratenCount+") ";

        }
    }
}
