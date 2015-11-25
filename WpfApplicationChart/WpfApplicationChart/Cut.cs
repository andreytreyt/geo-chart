using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplicationChart
{
    class Cut : CutBase
    {
        #region Fields
        private double _x1;
        private double _x2;
        private double _y1;
        private double _y2;
        #endregion

        #region Constructors
        public Cut(double x1, double y1, double x2, double y2)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }

        public Cut(PointCollection points)
        {
            X1 = (points)[0].Y;
            Y1 = (points)[0].X;
            X2 = (points)[1].Y;
            Y2 = (points)[1].X;
        }
        #endregion

        #region Properties
        public double X1
        {
            get { return _x1; }
            set
            {
                _x1 = value;
                ChangeItems();
            }
        }

        public double X2
        {
            get { return _x2; }
            set
            {
                _x2 = value;
                ChangeItems();
            }
        }

        public double Y1
        {
            get { return _y1; }
            set
            {
                _y1 = value;
                ChangeItems();
            }
        }

        public double Y2
        {
            get { return _y2; }
            set
            {
                _y2 = value;
                ChangeItems();
            }
        }

        public double Lenght { get; private set; }

        public double Area { get; private set; }

        public double Slope { get; private set; }

        public Chart ParentChart { get; private set; }

        public FrameworkElement DetailsPanel { get; set; }
        #endregion

        private void ChangeItems()
        {
            var points = new PointCollection();
            points.Add(new Point(Y1, X1));
            points.Add(new Point(Y2, X2));
            ItemsSource = points;

            this.Lenght = MathLenght();
            this.Area = MathArea();
            this.Slope = MathSlope();
        }

        #region Math
        private double MathLenght()
        {
            return Math.Abs(X2 - X1);
        }

        private double MathArea()
        {
            return 0;
        }

        private double MathSlope()
        {
            return 0;
        }
        #endregion

        #region Draw & Erase & Stick
        public void Draw(Chart chart)
        {
            this.ParentChart = chart;
            Erase();
            chart.Series.Add(this);
        }

        public void Erase()
        {
            if (this.ParentChart.Series.IndexOf(this) > -1) this.ParentChart.Series.Remove(this);
        }

        /* прилипание
         * line - основная линия графика
         * point - точка позиции курсора мыши
        */
        public static Point CalcStick(Line line, Point point)
        {
            return point;
        }

        public static void EraseAll(Chart chart)
        {
            chart.Series.Clear();
        }
        #endregion

        #region Add & Remove
        public void AddToPanel(ItemsControl panel)
        {
            StackPanel cut = new StackPanel
            {
                Width = 165, 
                Orientation = Orientation.Horizontal
            };
            
            TextBlock cutTextBlock =  new TextBlock
            {
                Width = 155
            };
            cutTextBlock.MouseDown += new MouseButtonEventHandler(CutTextBlock_OnMouseDown);
            cut.Children.Add(cutTextBlock);

            TextBlock removeTextBlock = new TextBlock
            {
                Text = "x", 
                Foreground = new SolidColorBrush(Colors.DarkRed),
                FontWeight = FontWeights.Bold
            };
            removeTextBlock.MouseDown += new MouseButtonEventHandler(RemoveTextBlock_OnMouseDown);
            cut.Children.Add(removeTextBlock);

            panel.Items.Add(cut);
            ChangePanel(panel);
        }

        private void RemoveFromPanel(ItemsControl panel, StackPanel cut)
        {
            if (this.Parent == null) return;

            this.Erase();
            this.DetailsPanel.DataContext = null;
            panel.Items.Remove(cut);
            ChangePanel(panel);
        }

        public void ChangePanel(ItemsControl panel)
        {
            int i = 0;
            foreach (StackPanel item in panel.Items)
            {
                (item.Children[0] as TextBlock).Text = string.Concat("отрезок ", (++i).ToString());
            }
        }

        private void CutTextBlock_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender == null) return;

            Style style;
            foreach (var series in this.ParentChart.Series)
            {
                if (series is Cut)
                {
                    style = new Style(typeof(Polyline));
                    style.Setters.Add(new Setter(Polyline.StrokeThicknessProperty, 2.0));
                    (series as Cut).PolylineStyle = style;
                }
            }
            style = new Style(typeof(Polyline));
            style.Setters.Add(new Setter(Polyline.StrokeThicknessProperty, 4.0));
            this.PolylineStyle = style;

            this.DetailsPanel.DataContext = null;
            this.DetailsPanel.DataContext = this;
        }

        private void RemoveTextBlock_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender == null) return;

            StackPanel cut = (sender as TextBlock).Parent as StackPanel;
            RemoveFromPanel(cut.Parent as ItemsControl, cut);
        }
        #endregion
    }
}
