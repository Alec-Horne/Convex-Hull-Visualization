using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WpfApplication1
{
    class AnimatedPen
    {
        private Canvas canvas;

        public AnimatedPen(Canvas c)
        {
            canvas = c;
        }

        public async Task DrawDashedAnimatedLine(Point p1, Point p2, Brush b, int lt, int pt, int s)
        {
            Line line = new Line();
            line.Name = "DashedLine";
            canvas.Children.Add(line);
            line.Stroke = b;
            line.StrokeThickness = lt;
            line.StrokeDashArray = new DoubleCollection() { 2 };
            line.X1 = p1.X;
            line.Y1 = p1.Y;
            line.X2 = p2.X;
            line.Y2 = p2.Y;
            Storyboard sb = new Storyboard();
            DoubleAnimation da = new DoubleAnimation(line.Y1, line.Y2, new Duration(new TimeSpan(0, 0, 0, 0, s)));
            DoubleAnimation da1 = new DoubleAnimation(line.X1, line.X2, new Duration(new TimeSpan(0, 0, 0, 0, s)));
            Storyboard.SetTargetProperty(da, new PropertyPath("(Line.Y2)"));
            Storyboard.SetTargetProperty(da1, new PropertyPath("(Line.X2)"));
            sb.Children.Add(da);
            sb.Children.Add(da1);
            await sb.BeginAsync(line);
        }

        public async Task DrawSolidAnimatedLine(Point p1, Point p2, Brush b, int lt, int pt, int s)
        {
            Line line = new Line();
            line.Name = "SolidLine";
            canvas.Children.Add(line);
            line.Stroke = b;
            line.StrokeThickness = lt;
            line.X1 = p1.X;
            line.Y1 = p1.Y;
            line.X2 = p2.X;
            line.Y2 = p2.Y;
            Storyboard sb = new Storyboard();
            DoubleAnimation da = new DoubleAnimation(line.Y1, line.Y2, new Duration(new TimeSpan(0, 0, 0, 0, s)));
            DoubleAnimation da1 = new DoubleAnimation(line.X1, line.X2, new Duration(new TimeSpan(0, 0, 0, 0, s)));
            Storyboard.SetTargetProperty(da, new PropertyPath("(Line.Y2)"));
            Storyboard.SetTargetProperty(da1, new PropertyPath("(Line.X2)"));
            sb.Children.Add(da);
            sb.Children.Add(da1);
            await sb.BeginAsync(line);
        }
    }
}
