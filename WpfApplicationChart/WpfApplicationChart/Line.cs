using System.Windows;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApplicationChart
{
    class Line : LineSeries
    {
        #region Fields
        private PointCollection _points;
        #endregion

        #region Constructors
        public Line()
        { }

        public Line(PointCollection points)
        {
            Points = points;

            DependentValuePath = "X";
            IndependentValuePath = "Y";
            Background = Brushes.Transparent;

            var style = new Style(typeof(LineDataPoint));
            style.Setters.Add(new Setter(TemplateProperty, null));
            style.Setters.Add(new Setter(BackgroundProperty, Brushes.DarkGreen));
            this.DataPointStyle = style;
        }
        #endregion

        #region Properties
        public PointCollection Points
        {
            get { return _points; }
            set
            {
                _points = value;
                ItemsSource = _points;
            }
        }
        #endregion

        public void Draw(Chart chart)
        {
            if (chart.Series.Count > 0) chart.Series.RemoveAt(0);
            chart.Series.Insert(0, this);
        }
    }
}
