using System;
using System.Collections.Generic;
using System.Text;

namespace MijnGps
{
    class Segment
    {
        public int id { get; }
        public Knoop start { get; set; }
        public Knoop end { get; set; }

        public List<Punt> vertices = new List<Punt>();

        public Segment(int id, Knoop start, Knoop end, List<Punt> vertices)
        {
            this.id = id;
            this.start = start;
            this.end = end;
            this.vertices = vertices;
        }

        public override bool Equals(object obj)
        {
            return obj is Segment segment &&
                this.start == segment.start &&
                this.end == segment.end;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.id, this.start.GetHashCode(), this.end.GetHashCode());
        }

        public override string ToString()
        {
            return "Segment:" + this.id + "," + this.start.ToString() + "," + this.end.ToString();
        }

        public double getLength()
        {
            double l = 0.0;
            for (int i = 1; i < vertices.Count; i++)
            {
                l += Math.Sqrt(Math.Pow(vertices[i].x - vertices[i - 1].x, 2) + Math.Pow(vertices[i].y - vertices[i - 1].y, 2));
            }
            return l;
        }
    }
}
