using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Media;

namespace WpfApplicationChart
{
    class Axes
    {
        private static LinearAxis XAxis;
        private static LinearAxis YAxis;
        private static readonly TextBox XMin = new TextBox();
        private static readonly TextBox XMax = new TextBox();
        private static readonly TextBox YMin = new TextBox();
        private static readonly TextBox YMax = new TextBox();

        #region Public Methods
        public static void Set(Chart chart, PointCollection points)
        {
            chart.Axes.Clear();

            XAxis = new LinearAxis
            {
                Orientation = AxisOrientation.X,
                ShowGridLines = true,
                VerticalAlignment = VerticalAlignment.Bottom,
                Height = 70,
                Margin = new Thickness(0, 0, 0, -50)
            };

            YAxis = new LinearAxis
            {
                Orientation = AxisOrientation.Y,
                ShowGridLines = true,
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = 100,
                Margin = new Thickness(-60, 0, 0, 0)
            };

            chart.Axes.Add(XAxis);
            chart.Axes.Add(YAxis);

            var firstPoint = points[0];
            SetAxesMinMax(firstPoint.Y, firstPoint.Y, firstPoint.X, firstPoint.X);

            foreach (var point in points)
            {
                SetAxesMinMax(point.Y, point.X);
            }
        }

        public static void Change(Chart chart, double posX, double posY, int delta, double step)
        {
            if (delta > 0)
            {
                if (XAxis.Maximum - XAxis.Minimum < step || YAxis.Maximum - YAxis.Minimum < step) return;

                #region ZoomRelativePosition
                if (posX < chart.Width / 2 && posY < chart.Height / 2)
                {
                    SetAxesMinMax(
                        XAxis.Minimum,
                        XAxis.Maximum -= step,
                        YAxis.Minimum += step,
                        YAxis.Maximum);
                    return;
                }

                if (posX > chart.Width / 2 && posY > chart.Height / 2)
                {
                    SetAxesMinMax(
                        XAxis.Minimum += step,
                        XAxis.Maximum,
                        YAxis.Minimum,
                        YAxis.Maximum -= step);
                    return;
                }

                if (posX < chart.Width / 2 && posY > chart.Height / 2)
                {
                    SetAxesMinMax(
                        XAxis.Minimum,
                        XAxis.Maximum -= step,
                        YAxis.Minimum,
                        YAxis.Maximum -= step);
                    return;
                }

                if (posX > chart.Width / 2 && posY < chart.Height / 2)
                {
                    SetAxesMinMax(
                        XAxis.Minimum += step,
                        XAxis.Maximum,
                        YAxis.Minimum += step,
                        YAxis.Maximum);
                    return;
                }
                #endregion

                SetAxesMinMax(
                        XAxis.Minimum += step,
                        XAxis.Maximum -= step,
                        YAxis.Minimum += step,
                        YAxis.Maximum -= step);
            }
            else
            {
                #region UnzoomRelativePosition
                if (posX < chart.Width / 2 && posY < chart.Height / 2)
                {
                    SetAxesMinMax(
                        XAxis.Minimum,
                        XAxis.Maximum += step,
                        YAxis.Minimum -= step,
                        YAxis.Maximum);
                    return;
                }

                if (posX > chart.Width / 2 && posY > chart.Height / 2)
                {
                    SetAxesMinMax(
                        XAxis.Minimum -= step,
                        XAxis.Maximum,
                        YAxis.Minimum,
                        YAxis.Maximum += step);
                    return;
                }

                if (posX < chart.Width / 2 && posY > chart.Height / 2)
                {
                    SetAxesMinMax(
                        XAxis.Minimum,
                        XAxis.Maximum += step,
                        YAxis.Minimum,
                        YAxis.Maximum += step);
                    return;
                }

                if (posX > chart.Width / 2 && posY < chart.Height / 2)
                {
                    SetAxesMinMax(
                        XAxis.Minimum -= step,
                        XAxis.Maximum,
                        YAxis.Minimum -= step,
                        YAxis.Maximum);
                    return;
                }
                #endregion

                SetAxesMinMax(
                    XAxis.Minimum -= step,
                    XAxis.Maximum += step,
                    YAxis.Minimum -= step,
                    YAxis.Maximum += step);
            }
        }
        #endregion

        #region Private Methods
        private static void SetAxesMinMax(double? x, double? y)
        {
            double?
                xmin = XAxis.Minimum != null ? (double)XAxis.Minimum : 0,
                xmax = XAxis.Maximum != null ? (double)XAxis.Maximum : 0,
                ymin = YAxis.Minimum != null ? (double)YAxis.Minimum : 0,
                ymax = YAxis.Maximum != null ? (double)YAxis.Maximum : 0;

            if (x <= xmin) xmin = x;
            if (x > xmax) xmax = x;
            if (y <= ymin) ymin = y;
            if (y > ymax) ymax = y;

            SetAxesMinMax(xmin, xmax, ymin, ymax);
        }

        private static void SetAxesMinMax(double? xmin, double? xmax, double? ymin, double? ymax)
        {
            if (xmin <= xmax)
            {
                XAxis.Maximum += Math.Abs((double)xmin);
                XAxis.Minimum = xmin;
                XAxis.Maximum = xmax;

                XMin.Text = xmin.ToString();
                XMax.Text = xmax.ToString();
            }
            if (ymin <= ymax)
            {
                YAxis.Maximum += Math.Abs((double)ymin);
                YAxis.Minimum = ymin;
                YAxis.Maximum = ymax;

                YMin.Text = ymin.ToString();
                YMax.Text = ymax.ToString();
            }
        }
        #endregion

        #region MinMaxPanel
        public static StackPanel MinMaxPanel()
        {
            StackPanel minmaxPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            minmaxPanel.Children.Add(new TextBlock { Text = "X min", VerticalAlignment = VerticalAlignment.Center });
            minmaxPanel.Children.Add(XMin);
            minmaxPanel.Children.Add(new TextBlock { Text = "X max", VerticalAlignment = VerticalAlignment.Center });
            minmaxPanel.Children.Add(XMax);
            minmaxPanel.Children.Add(new TextBlock { Text = "Y min", VerticalAlignment = VerticalAlignment.Center });
            minmaxPanel.Children.Add(YMin);
            minmaxPanel.Children.Add(new TextBlock { Text = "Y max", VerticalAlignment = VerticalAlignment.Center });
            minmaxPanel.Children.Add(YMax);

            foreach (var child in minmaxPanel.Children)
            {
                if (child is TextBox)
                {
                    TextBox textBox = (TextBox)child;
                    textBox.Width = 50;
                    textBox.Margin = new Thickness(5, 0, 15, 0);
                    textBox.TextChanged += new TextChangedEventHandler(MinMax_OnTextChanged);
                }
            }

            return minmaxPanel;
        }

        private static void MinMax_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender == null) return;

            if ((sender as TextBox).Text.Length > 7) return;

            double xmin, xmax, ymin, ymax;
            if (double.TryParse(XMin.Text, out xmin) && double.TryParse(XMax.Text, out xmax) &&
                double.TryParse(YMin.Text, out ymin) && double.TryParse(YMax.Text, out ymax))
            {
                SetAxesMinMax(xmin, xmax, ymin, ymax);
            }
        }
        #endregion
    }
}
