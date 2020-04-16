using System;
using System.Collections.Generic;
using System.Text;

namespace MijnGps
{
    class Graaf
    {
        public int id { get; set; }
        public Dictionary<Knoop, List<Segment>> map = new Dictionary<Knoop, List<Segment>>();

        public Graaf(int id)
        {
            this.id = id;
        }

        public List<Knoop> getKnopen()
        {
            return new List<Knoop>(this.map.Keys);
        }

        public static Graaf buildGraaf(int id, List<Segment> segments)
        {
            Graaf graaf = new Graaf(id);
            foreach (Segment segment in segments)
            {
                graaf.addSegment(segment);
            }
            return graaf;
        }

        public void addSegment(Segment segment)
        {
            if (!this.map.ContainsKey(segment.start))
                this.map.Add(segment.start, new List<Segment>());

            if (!this.map.ContainsKey(segment.end))
                this.map.Add(segment.end, new List<Segment>());

            this.map[segment.start].Add(segment);
            this.map[segment.end].Add(segment);
        }

        public override string ToString()
        {
            return "" +getLength();
        }
        public double getLength()
        {
            double length = 0.0;
            foreach (Knoop key in map.Keys)
            {
                foreach (Segment s in map[key])
                    length += s.getLength();
            }
            return length;
        }
    }
}
