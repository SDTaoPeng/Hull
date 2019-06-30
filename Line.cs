using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvexHull_1
{
    public class Line
    {
        public bool isChecked = false;
        public Node[] nodes = new Node[2];
        public Line(Node n1, Node n2)
        {
            nodes[0] = n1;
            nodes[1] = n2;
        }
        public static double getLength(Node node1, Node node2)
        
            double length;
            length = Math.Pow(node1.y - node2.y, 2) + Math.Pow(node1.x - node2.x, 2);
            return length;
        }
    }
}
