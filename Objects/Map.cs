using System;
using System.Collections.Generic;
using System.Text;

namespace EscapeFromTheWoods
{
    public struct Map
    {
        public Map(short xmin, short xmax, short ymin, short ymax)
        {
            this.xmin = xmin;
            this.xmax = xmax;
            this.ymin = ymin;
            this.ymax = ymax;
        }

        public short xmin { get; set; }
        public short xmax { get; set; }
        public short ymin { get; set; }
        public short ymax { get; set; }
    }
}
