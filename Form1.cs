using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ConvexHull_1
{ 
    public partial class frmConvexHull : Form
    {
        private Graphics graphics;
        List<PointF> pointsList = new List<PointF>();
        List<PointDouble> pointsListDouble = new List<PointDouble>();
        List<PointF> pointManulList = new List<PointF>();
        List<Segment> Segments = new List<Segment>();
        int pID = 0;
        List<List<PointF>> renderingPoints = new List<List<PointF>>();
        List<List<Segment>> renderingEdges = new List<List<Segment>>();
        List<PointF[]> renderingAreas = new List<PointF[]>();
        Graphics g;

        //数据读取，显示全部点集
        private void DisFixPoints_Click(object sender, EventArgs e) 
        {
           pointsList= Read_Data(1);   //读取数据集
        }
        #region 读数据过程中调用的函数
        private List<PointF> Read_Data(int num)  //num为第几个txt
        {
            int i = 0;
            int j = 0;
            graphics = this.CreateGraphics();
            StreamReader myStreamReader;
            string readString;            
            List<PointF> Lstpoint = new List<PointF>();
            string text = num + "" + ".txt";
            DirectoryInfo dir = new DirectoryInfo(Application.StartupPath).Parent.Parent;
            string strFilePath = dir.FullName + "\\DataPoint\\"+text;
            StreamReader sr = new StreamReader(strFilePath);
            while (i >= 0)
            {
                if (sr.ReadLine() != null)
                {
                    i++;
                    j = i;//j为.txt的行数
                }
                else break;
            }
            myStreamReader = File.OpenText(strFilePath);
            for (int count = 0; count < j; count++)    //number为txt的行数
            {
                readString = myStreamReader.ReadLine();  //读取txt中每一行的内容
                string[] a = readString.Split(' ');      
                PointF b = new PointF();                 
                b.X = (float)Convert.ToDouble(a[0]);     
                b.Y = (float)Convert.ToDouble(a[1]);
                Lstpoint.Add(b);                         
                renderPoint(b.X, b.Y, Color.White);
            }
            return Lstpoint;
        }
        #endregion

        private void ConvexClick_Click(object sender, EventArgs e)
        {
            List<PointF> ListH = new List<PointF>();
            if (pointsList.Count != 0)        //如果pointsList中点集不为空
            {
                InitOrderdPoints(pointsList);
                ListH = ConvexCompute(pointsList);
                prepareNewConvexHull();
            }
            else
            {
                if (pointManulList.Count != 0)        //如果pointsList中点集不为空
                {
                    InitOrderdPoints(pointManulList);
                    ListH = ConvexCompute(pointManulList);
                }
            }
        }

        //存Hull实现完后，最终获取的数据点
        private void btnWritePoint_Click(object sender, EventArgs e)
        {
            int i = 0;
            int j = 0;
            int num =2;
            string text = num + "" + ".txt";
            DirectoryInfo dir = new DirectoryInfo(Application.StartupPath).Parent.Parent;
            string strFilePath = dir.FullName + "\\DataPoint\\"+text;
            WriteOnedata(pointsList, strFilePath);
            StreamReader sr = new StreamReader(strFilePath);
            while (i >= 0)   //i用来计数
            {
                if (sr.ReadLine() != null)
                {
                    i++;
                    j = i;//j为.txt的行数   
                }
                else break;
            }
        }

        public static bool WriteOnedata(List<PointF> listpoint, string strFilePath)
        {
            int i;
            FileStream fs = File.Open(strFilePath, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);

            for (i = 0; i < listpoint.Count; i++)
            {
                sw.WriteLine(listpoint[i].X + " " + listpoint[i].Y);
            }
            sw.Close();
            fs.Close();

            return true;
        }   

        private void btnReadPoint_Click(object sender, EventArgs e)
        {
            pointManulList=Read_Data(1);
        }

        private void renderPoint(float x, float y, Color c)
        {
            //渲染每个点集，将每个点画为圆形
            Pen p = new Pen(c, 1.7f);
            graphics = this.CreateGraphics();
            graphics.DrawEllipse(p, x - 5, y - 5, 10, 10);
            graphics.DrawLine(p, x, y - 5, x, y - 5 - 2.5f);
            graphics.DrawLine(p, x - 5, y, x - 5 - 2.5f, y);
            graphics.DrawLine(p, x, y + 5, x, y + 5 + 2.5f);
            graphics.DrawLine(p, x + 5, y, x + 5 + 2.5f, y);
            graphics.FillEllipse(Brushes.White, x - 1.25f, y - 1.25f, 2.5f, 2.5f);
        }

        private void renderEdges(List<Segment> edges)
        {
            List<PointF> list = new List<PointF>();
            for (int i = 0; i < edges.Count; i++)
            {
                if (i == 0)
                list.Add(edges[i].p);
                foreach (Segment s in edges)
                {
                    if (s.p == list[list.Count - 1])
                    {
                        list.Add(s.q);
                        break;
                    }
                }
            }
            Pen p = new Pen(Color.Yellow);  
            foreach (Segment opEdge in edges)  
            {
                graphics.DrawLine(p, opEdge.p, opEdge.q);
            }
        }

        private void render()
        {
            if (renderingEdges.Count > 0 || renderingPoints.Count > 0 || renderingAreas.Count > 0)
            {
                foreach (PointF[] pA in renderingAreas)
                {
                    graphics.FillPolygon(Brushes.MidnightBlue, pA);
                }
                foreach (List<PointF> splist in renderingPoints)
                {
                    foreach (PointF sp in splist)
                    {
                        renderPoint(sp.X, sp.Y, Color.Blue);

                    }
                }
                foreach (List<Segment> seglist in renderingEdges)
                {
                    foreach (Segment s in seglist)
                    {

                        graphics.DrawLine(new Pen(Color.LightSkyBlue), s.p, s.q);
                    }
                }
            }
            foreach (PointF sp2 in pointsList)
            {
                renderPoint(sp2.X, sp2.Y, Color.White);
            }
        }

        private void prepareNewConvexHull()
        {
            pointsList.Clear();
            Segments.Clear();
        }

        private List<PointF> ConvexCompute(List<PointF> pointsList)
        {
            List<PointF> ProcessingPoints = new List<PointF>();   //由点坐标(x,y)和点个数组成
            int i = 0;
            int j = 0;
            for (i = 0; i < Segments.Count; )  
            {
                ProcessingPoints = new List<PointF>(pointsList);
                for (j = 0; j < ProcessingPoints.Count; )
                {
                    if (Segments[i].contains(ProcessingPoints[j]))  
                    {
                        ProcessingPoints.Remove(ProcessingPoints[j]);  
                        j = 0;                                         ////重新遍历查询，j清零
                        continue;
                    }
                    j++;
                }
                //ProcessingPoints为点集，Segments为边缘
                if (!isEdge(ProcessingPoints, Segments[i]))  //如果不是边，则移除
                {
                    Segments.Remove(Segments[i]);            //移除第i条边

                    i = 0;                                   //重新遍历查询，i清零
                    continue;
                }
                else
                { i++; }
            }
            renderEdges(Segments);
            List<Segment> segmentsList = new List<Segment>(Segments);
            //renderingPoints.Add(superPointsList);
            //renderingEdges.Add(segmentsList);
            List<PointF> ListH = new List<PointF>();
            int fp;
            for(fp = 0; fp < segmentsList.Count; fp++)
            {
                PointF poinF= new PointF();
                poinF.X = segmentsList[fp].p.X;
                poinF.Y = segmentsList[fp].p.Y;
                ListH.Add(poinF);
            }
            return ListH;
        }

        private bool isEdge(List<PointF> processingPoints, Segment edge)
        {
            for (int k = 0; k < processingPoints.Count; k++)
            {
                if (isCounterclockwise(edge, processingPoints[k]))
                {
                    return false;
                }
            }
            return true;
        }
		
        private bool isCounterclockwise (Segment segment, PointF r) 
        {
            float D = 0;
            float px, py, qx, qy, rx, ry = 0;
            px = segment.p.X;
            py = segment.p.Y;
            qx = segment.q.X;
            qy = segment.q.Y;
            rx = r.X;
            ry = r.Y;

            D = ((qx * ry) - (qy * rx)) - (px * (ry - qy)) + (py * (rx - qx));

            if (D <= 0)   
                return false;    

            return true;
        }

        private void InitOrderdPoints(List<PointF> pointsList)
        {
            for (int i = 0; i < pointsList.Count; i++)
            {
                for (int j = 0; j < pointsList.Count; j++)
                {
                    if (i != j)    //线段的两个端点不重合
                    {
                        Segment op = new Segment();
                        PointF p1 = pointsList[i];  
                        PointF p2 = pointsList[j];
                        op.p = p1;
                        op.q = p2;
                        Segments.Add(op);  //存线段的2个端点的坐标
                    }
                }
            }
        }
        //--------------------------------------Concave Algorithm------------------------------------------------
        List<Node> dot_list = new List<Node>();
        public string seed;
        public int threshold=2;
        public int number_of_dots;
        public double concavity;
        public bool isSquareGrid;
        public List<Line> hull_concave_edges = new List<Line>();

        private void Execute_Click(object sender, EventArgs e)
        {
            if (pointsList.Count != 0)        //如果pointsList中点集不为空
            {
                for (int x = 0; x < pointsList.Count; x++)
                {
                    dot_list.Add(new Node(pointsList[x].X, pointsList[x].Y, x));
                }
            }
            else
            {
                pointsList = Read_Data(1);   //读取数据集
                for (int x1 = 0; x1 < pointsList.Count; x1++)
                {
                    dot_list.Add(new Node(pointsList[x1].X, pointsList[x1].Y, x1));
                }
            }

            //删除dot_list中的重复点集
            for (int pivot_position = 0; pivot_position < dot_list.Count; pivot_position++)
            {
                for (int position = 0; position < dot_list.Count; position++)
                    if (dot_list[pivot_position].x == dot_list[position].x 
                        && dot_list[pivot_position].y == dot_list[position].y
                        && dot_list[pivot_position].id != dot_list[position].id)
                    {
                        dot_list.RemoveAt(position);
                        position--;
                    }
            }
            generateHull();
            DrawPlot(g);
        }
        private void generateHull()
        {
            //先生成凸包，再生成凹包
            Hull.setConvHull(dot_list);
            hull_concave_edges=Hull.setConcaveHull(Math.Round(Convert.ToDecimal(concavity), 2), 
                                                   threshold, isSquareGrid);
        }
       //------------------------------------------------------------------------------------------------------
    }
}