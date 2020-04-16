using System;
using System.Collections.Generic;
using System.Text;

namespace MijnGps
{
    class Punt
    {
        public Double x { get; set; }
        public Double y { get; set; }

        public Punt(Double x, Double y)
        {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(object obj)
        {
            return obj is Punt punt &&
                this.x == punt.x &&
                this.y == punt.y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.x, this.y);
        }

        public override string ToString()
        {
            return "Punt:(" + this.x + " x " + this.y + ")";
        }
    }
}
