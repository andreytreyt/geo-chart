using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.DataVisualization.Charting;

namespace WpfApplicationChart
{
    public static class MouseCursor
    {
        public static Point GetPosition(Chart chart, Point position, int digits)
        {
            var XAxis = (LinearAxis)chart.Axes[0];
            var YAxis = (LinearAxis)chart.Axes[1];

            var lineWidth = chart.Width - 63;
            var lineHeight = chart.Height - 93;

            var ratioX = lineWidth / (XAxis.Maximum - XAxis.Minimum);
            var ratioY = lineHeight / (YAxis.Maximum - YAxis.Minimum);

            var resultX = Math.Round((double)(position.X / ratioX + XAxis.Minimum), digits);
            var resultY = Math.Round((double)((-1 * (position.Y - chart.Height + 93)) / ratioY + YAxis.Minimum), digits);

            return new Point(resultX, resultY);
        }
    }
}
