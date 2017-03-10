using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApplication1
{
    public class MyLine
    {
        private Point _start;
        private Point _end;
        private int _direction;

        public MyLine() { }

        public MyLine(Point s, Point e, int d)
        {
            _start = s;
            _end = e;
            _direction = d;
        }

        public Point Start
        {
            get { return _start; }
            set { _start = value; }
        }

        public Point End
        {
            get { return _end; }
            set { _end = value; }
        }

        public int Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }
    }
}
