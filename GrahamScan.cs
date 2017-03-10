using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WpfApplication1
{
    class GrahamScan
    {
        private List<Point> points;
        private Point start;
        private List<MyLine> path;

        //Constructor
        //Get the start point and a list of all other points to sort
        public GrahamScan(List<Point> pts, Point s)
        {
            points = new List<Point>();
            path = new List<MyLine>();
            start = findStart(pts);

            //Get all points besides start for sorting
            foreach (Point p in pts)
            {
                if (p.X != start.X && p.Y != start.Y)
                    points.Add(p);
            }
        }

        //compute the convex hull - return points on hull
        public List<Point> ComputeConvexHull()
        {
            List<Point> result = new List<Point>();
            points = mergeSort(start, points);
            result.Add(start);
            result.Add(points[0]);
            result.Add(points[1]);
            points.RemoveAt(0);
            points.RemoveAt(0);
            path.Add(new MyLine(result[0], result[1], 1));
            path.Add(new MyLine(result[1], result[2], 1));

            for (int x = 0; x < points.Count; x++)
                turn(result, points[x]);

            path.Add(new MyLine(result[result.Count - 1], start, 1));

            return result;
        }

        //Find the starting point for the convex hull - The "lowest"
        private Point findStart(List<Point> pts)
        {
            Point p = pts.ElementAt(0);
            for (int x = 1; x < pts.Count; x++)
            {
                if (pts.ElementAt(x).Y > p.Y)
                    p = pts.ElementAt(x);
                else if (pts.ElementAt(x).Y == p.Y)
                    if (pts.ElementAt(x).X < p.X)
                        p = pts.ElementAt(x);
            }
            return p;
        }

        //use merge sort to sort the points based on polar angle to the starting point(lowest)
        private List<Point> mergeSort(Point start, List<Point> pts)
        {
            //base case
            if (pts.Count == 1)
            {
                return pts;
            }
            List<Point> result = new List<Point>();
            //get the middle point
            int m = (int)pts.Count / 2;
            //divide the list into two arrays for recursion
            List<Point> leftHalf = pts.GetRange(0, m);
            List<Point> rightHalf = pts.GetRange(m, pts.Count - m);

            //recursive case
            leftHalf = mergeSort(start, leftHalf);
            rightHalf = mergeSort(start, rightHalf);
            int leftP = 0;
            int rightP = 0;
            for (int i = 0; i < leftHalf.Count + rightHalf.Count; i++)
            {
                if (leftP == leftHalf.Count)
                {
                    result.Add(rightHalf[rightP]);
                    rightP++;
                }
                else if (rightP == rightHalf.Count)
                {
                    result.Add(leftHalf[leftP]);
                    leftP++;
                }
                else if (getAngle(start, leftHalf[leftP]) > getAngle(start, rightHalf[rightP]))
                {
                    result.Add(leftHalf[leftP]);
                    leftP++;
                }
                else {
                    result.Add(rightHalf[rightP]);
                    rightP++;
                }
            }
            //return the sorted list
            return result;
        }

        //get the angle between two points
        private double getAngle(Point p1, Point p2)
        {
            float xDifference = (float)(p2.X - p1.X);
            float yDifference = (float)(p2.Y - p1.Y);
            return Math.Atan2(yDifference, xDifference) * 180.0 / Math.PI;
        }

        //check turn for each point and add the points that are left turns
        //remove points that are right turns
        public void turn(List<Point> result, Point p)
        {
            while (result.Count > 1 && ccw(result[result.Count - 2], result[result.Count - 1], p) != -1)
                result.RemoveAt(result.Count - 1);

            if (result.Count == 0 || result[result.Count - 1] != p)
                result.Add(p);
        }

        //check wether three points make a left or right turn
        public int ccw(Point p1, Point p2, Point p3)
        {
            path.Add(new MyLine(p2, p3, 1));
            if (((p2.X - p1.X) * (p3.Y - p1.Y) - (p3.X - p1.X) * (p2.Y - p1.Y)) >= 0)
            {
                // path.Add(new MyLine(p3, p2, -1));
                // path.Add(new MyLine(p2, p1, -1));
            }
            return ((p2.X - p1.X) * (p3.Y - p1.Y) - (p3.X - p1.X) * (p2.Y - p1.Y)).CompareTo(0);
        }

        //get the algorithm path for animation demo
        public List<MyLine> getAlgorithmPath()
        {
            return path;
        }

    }
}