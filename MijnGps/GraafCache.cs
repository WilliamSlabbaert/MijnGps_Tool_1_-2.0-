using System;
using System.Collections.Generic;
using System.Text;

namespace MijnGps
{
    class GraafCache
    {
        public int ID { get; }
        public Segment Segment { get; }

        public GraafCache(int id, Segment segment)
        {
            this.ID = id;
            this.Segment = segment;
        }
    }
}
