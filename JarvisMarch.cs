using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApplication1
{
    public class JarvisMarch
    {
        //algorithm path for animation
        private List<MyLine> path;
        private Point prev;

        //constructor
        public JarvisMarch()
        {
            path = new List<MyLine>();
            prev = new Point();
        }

        //compute the convex hull - return points on hull
        public List<Point> ComputeConvexHull(List<Point> points)
        {
            //get a new list with the leftmost point at the 0 index
            List<Point> pts = new List<Point>();
            foreach (Point p in points)
            {
                if (pts.Count == 0)
                    pts.Add(p);
                else
                {
                    if (pts[0].X > p.X)
                        pts[0] = p;
                    else if (pts[0].X == p.X)
                        if (pts[0].Y > p.Y)
                            pts[0] = p;
                }
            }
            Point p1;
            int counter = 0;
            //check each point to find the next point on the convex hull
            while (counter < pts.Count)
            {
                p1 = nextPoint(points, pts[counter]);
                if (p1 != pts[0])
                {
                    //next point on convex hull
                    path.Add(new MyLine(pts[pts.Count - 1], p1, -1));
                    pts.Add(p1);
                }
                counter++;
            }
            //return points on convex hull
            return pts;
        }

        //counter-clockwise - check points for left/right turn
        private int ccw(Point p1, Point p2, Point p3)
        {
            if (p2.X != p3.X && p2.Y != p3.Y)
            {
                if (p3.X != prev.X && p3.Y != prev.Y)
                {
                    //demo lines
                    path.Add(new MyLine(p1, p2, 1));
                    path.Add(new MyLine(p1, p3, 1));
                }
            }
            prev = p3;
            return ((p2.X - p1.X) * (p3.Y - p1.Y) - (p3.X - p1.X) * (p2.Y - p1.Y)).CompareTo(0);
        }

        //get the distance from one point to another
        private int distanceTo(Point p1, Point p2)
        {
            int xDist = (int)(p2.X - p1.X);
            int yDist = (int)(p2.Y - p1.Y);
            return xDist * xDist + yDist * yDist;
        }

        //get the next point on the convex hull
        private Point nextPoint(List<Point> points, Point p1)
        {
            Point p2 = p1;
            foreach (Point p in points)
            {
                if (ccw(p1, p2, p) == -1 || ccw(p1, p2, p) == 0 && distanceTo(p1, p) > distanceTo(p1, p2))
                    p2 = p;
            }
            return p2;
        }

        //get the angle between two points
        private double getAngle(Point p1, Point p2)
        {
            float xDiff = (float)(p2.X - p1.X);
            float yDiff = (float)(p2.Y - p1.Y);
            return Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI;
        }

        //return the path for demo
        public List<MyLine> getAlgorithmPath()
        {
            return path;
        }

    }
}