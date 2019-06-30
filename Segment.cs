using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace ConvexHull_1
{
    class Segment
    {
        public PointF p;
        public PointF q;

        public bool contains(PointF point)
        {
            if (p.Equals(point) || q.Equals(point))

                return true;

            return false;
        }
    }
}
