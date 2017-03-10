using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        private List<Point> points, result;
        private List<MyLine> path;
        private AnimatedPen ap;
        private int height;
        private int width;
        private int pointSize;
        private int lineThickness;
        private int animationSpeed;
        private Boolean isPaused, isClear, isFile;
        private MyLine bestGuessLine;

        public MainWindow()
        {
            InitializeComponent();
            points = new List<Point>();
            ap = new AnimatedPen(myCanvas);
            height = 410;
            width = 655;
            pointSize = 5;
            pointSize = (int)slider.Value;
            lineThickness = (int)slider1.Value;
            animationSpeed = (int)slider2.Value;
            isPaused = false;
            isClear = false;
            isFile = false;
            bestGuessLine = new MyLine();
        }

        protected async void button1_Click(object sender, RoutedEventArgs e)
        {
            if (isClear)
                isClear = false;
            JarvisMarch jm = new JarvisMarch();
            Stopwatch sm = new Stopwatch();
            sm.Start();
            result = jm.ComputeConvexHull(points);
            sm.Stop();
            Console.WriteLine("Jarvis March took " + sm.ElapsedMilliseconds + " milliseconds to compute.");
            path = jm.getAlgorithmPath();
            printResultToFile(result);

            int count = 0;
            for (int x = 0; x < path.Count; x++)
            {
                if (path[x].Direction == 1)
                {
                    if (count % 2 == 0)
                    {
                        if (bestGuessLine.End.X != path[x].End.X && bestGuessLine.End.Y != path[x].End.Y)
                        {
                            List<Line> lines = myCanvas.Children.OfType<Line>().ToList();
                            Line line = new Line();
                            foreach (Line l in lines)
                            {
                                if (l.Name == "SolidLine")
                                    line = l;
                            }
                            myCanvas.Children.Remove(line);
                            await ap.DrawSolidAnimatedLine(path[x].Start, path[x].End, Brushes.Black, lineThickness, pointSize, animationSpeed);
                            bestGuessLine = path[x];
                        }
                    }
                    else {
                        await ap.DrawDashedAnimatedLine(path[x].Start, path[x].End, Brushes.Black, lineThickness, pointSize, animationSpeed);
                        myCanvas.Children.RemoveAt(myCanvas.Children.Count - 1);
                    }
                    count++;
                    do
                    {
                        await System.Threading.Tasks.Task.Delay(5);
                    }
                    while (isPaused == true);

                    if(x == path.Count - 1)
                        await ap.DrawSolidAnimatedLine(result[result.Count - 1], result[0], Brushes.Black, lineThickness, pointSize, animationSpeed);
                    if (isClear)
                        break;
                }
                else
                    await ap.DrawSolidAnimatedLine(path[x].Start, path[x].End, Brushes.Black, lineThickness, pointSize, animationSpeed);
            }

            for (int x = 0; x < result.Count; x++)
            {
                if (isClear)
                    break;
                if (x == result.Count - 1) {
                    drawPoints(result, pointSize);
                    await ap.DrawSolidAnimatedLine(result[x], result[0], Brushes.Blue, lineThickness+1, pointSize, animationSpeed);
                }
                else
                {
                    drawPoints(result, pointSize);
                    await ap.DrawSolidAnimatedLine(result[x], result[x + 1], Brushes.Blue, lineThickness+1, pointSize, animationSpeed);
                }
            }
        }

        private void draw_Click(object sender, RoutedEventArgs e)
        {
            Random rand = new Random();
            int amount = Convert.ToInt32(textBox.Text);
            if (amount > 3)
            {
                for (int z = 0; z < amount; z++)
                {
                    int x = rand.Next(5, width);
                    int y = rand.Next(5, height);
                    Ellipse myEllipse = new Ellipse();
                    myEllipse.Fill = Brushes.Black;
                    myEllipse.StrokeThickness = 2;
                    myEllipse.Stroke = Brushes.Black;
                    myEllipse.Width = pointSize;
                    myEllipse.Height = pointSize;
                    myCanvas.Children.Add(myEllipse);
                    Canvas.SetLeft(myEllipse, x - (pointSize/2));
                    Canvas.SetTop(myEllipse, y - (pointSize/2));
                    points.Add(new Point(x, y));
                }
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            //remove all points from canvas
            List<Ellipse> pts = myCanvas.Children.OfType<Ellipse>().ToList();
            foreach (Ellipse p in pts)
            {
                myCanvas.Children.Remove(p);
            }
            //remove all lines from canvas
            List<Line> lines = myCanvas.Children.OfType<Line>().ToList();
            foreach (Line l in lines)
            {
                myCanvas.Children.Remove(l);
            }
            //remove points from point list
            points.Clear();

            //set clear boolean to true to signal algorithm to stop
            isClear = true;

            //set resume/pause button back to resume
            isPaused = false;
            pauseButton.Content = "Pause";
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            pointSize = (int)slider.Value;
            slider.ToolTip = pointSize;
            pointSizeGB.Header = "Point Size - " + pointSize;
        }

        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lineThickness = (int)slider1.Value;
            slider1.ToolTip = lineThickness;
            lineThicknessGB.Header = "Line Thickness - " + lineThickness;
        }

        private void slider2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            animationSpeed = (int)slider2.Value;
            slider2.ToolTip = animationSpeed;
            animationSpeedGB.Header = "Animation Speed - " + animationSpeed;
        }

        protected async void on_Click(object sender, RoutedEventArgs e)
        {
            if (isClear)
                isClear = false;
            GrahamScan gs = new GrahamScan(points, new Point(width, height));
            Stopwatch sm = new Stopwatch();
            sm.Start();
            result = gs.ComputeConvexHull();
            sm.Stop();
            Console.WriteLine("Graham Scan took " + sm.ElapsedMilliseconds + " milliseconds to compute.");
            path = gs.getAlgorithmPath();
            printResultToFile(result);

            for (int x = 0; x <= path.Count - 1; x++)
            {
                do
                {
                    await System.Threading.Tasks.Task.Delay(5);
                }
                while (isPaused == true);
                if (isClear)
                    break;
                if (path[x].Direction == 1)
                    await ap.DrawDashedAnimatedLine(path[x].Start, path[x].End, Brushes.Black, lineThickness, pointSize, animationSpeed);
                else
                    await ap.DrawDashedAnimatedLine(path[x].Start, path[x].End, Brushes.Blue, lineThickness, pointSize, animationSpeed);
                if(x == path.Count-1)
                    await ap.DrawDashedAnimatedLine(result[result.Count - 1], result[0], Brushes.Black, lineThickness, pointSize, animationSpeed);
            }


            for (int x = 0; x < result.Count; x++)
            {
                if (isClear)
                    break;
                if (x == result.Count - 1)
                {
                    drawPoints(result, pointSize);
                    await ap.DrawSolidAnimatedLine(result[x], result[0], Brushes.Blue, lineThickness + 1, pointSize, animationSpeed);
                }
                else
                {
                    drawPoints(result, pointSize);
                    await ap.DrawSolidAnimatedLine(result[x], result[x + 1], Brushes.Blue, lineThickness+1, pointSize, animationSpeed);
                }
            }
        }

        private async void button3_Click(object sender, RoutedEventArgs e)
        {
            if (isClear)
                isClear = false;
            while (points.Count >= 3)
            {
                GrahamScan gs = new GrahamScan(points, new Point(width, height));
                result = gs.ComputeConvexHull();
                points = points.Except(result).ToList();

                for (int x = 0; x < result.Count; x++)
                {
                    do
                    {
                        await System.Threading.Tasks.Task.Delay(5);
                    }
                    while (isPaused == true);

                    if (isClear)
                    {
                        break;
                    }
                    if (x == result.Count - 1)
                    { 
                        drawPoints(result, pointSize);
                        await ap.DrawSolidAnimatedLine(result[x], result[0], Brushes.Blue, lineThickness + 1, pointSize, animationSpeed);
                    }
                    else
                    {
                        drawPoints(result, pointSize);
                        await ap.DrawSolidAnimatedLine(result[x], result[x + 1], Brushes.Blue, lineThickness + 1, pointSize, animationSpeed);
                    }
                }
                if (isClear)
                {
                    isClear = false;
                    break;
                }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            ofd.InitialDirectory = @"c:\";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == true)
            {
                isFile = true;
                System.IO.StreamReader sr = new System.IO.StreamReader(ofd.FileName);
                string s = sr.ReadToEnd();
                string[] input = Regex.Split(s, @"\D+");
                for(int x = 0; x < input.Length; x += 2)
                    points.Add(new Point(Convert.ToInt32(input[x]), Convert.ToInt32(input[x + 1])));
                drawPoints(points, pointSize);
                sr.Close();
            }
        }

        private void printResultToFile(List<Point> res)
        {

        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (pauseButton.Content.ToString() == "Resume")
            {
                pauseButton.Content = "Pause";
                isPaused = false;
            }
            else
            {
                pauseButton.Content = "Resume";
                isPaused = true;
            }
        }

        private void drawPoints(List<Point> pts, int size)
        {
            for (int z = 0; z < pts.Count; z++)
            {
                Ellipse myEllipse = new Ellipse();
                myEllipse.Fill = Brushes.Black;
                myEllipse.StrokeThickness = 2;
                myEllipse.Stroke = Brushes.Black;
                myEllipse.Width = pointSize;
                myEllipse.Height = pointSize;
                myCanvas.Children.Add(myEllipse);
                Canvas.SetLeft(myEllipse, pts[z].X - (pointSize/2));
                Canvas.SetTop(myEllipse, pts[z].Y - (pointSize/2));
            }
        }
    }
}