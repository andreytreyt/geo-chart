using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApplicationChart
{
    class Line : Series
    {
        #region Constructors
        public Line()
        { }

        public Line(List<DataPoint> points)
        {
            foreach (DataPoint point in points)
            {
                Points.Add(new DataPoint(point.XValue, point.YValues[0]));
            }

            ChartType = SeriesChartType.Line;
            Color = Color.Green;
            BorderWidth = 2;
        }
        #endregion

        public void Draw(Chart chart)
        {
            if (chart.Series.Count > 0) chart.Series.RemoveAt(0);
            chart.Series.Insert(0, this);
        }
    }
}
