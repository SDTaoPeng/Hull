using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ConvexHull_1
{
    public class Node
    {
        public int id;
        public double x;
        public double y;
        public Node(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        public Node(double x, double y, int id)
        {
            this.x = x;
            this.y = y;
            this.id = id;
        }
        public Node(PointF pointf, int id)
        {
            this.x = pointf.X;
            this.y = pointf.Y;
            this.id = id;
        }
    }
}
