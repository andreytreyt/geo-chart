using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Media;

namespace WpfApplicationChart
{
    class CutBase : LineSeries
    {
        public CutBase()
        {
            DependentValuePath = "X";
            IndependentValuePath = "Y";
            Background = Brushes.Transparent;

            var style = new Style(typeof(LineDataPoint));
            style.Setters.Add(new Setter(TemplateProperty, null));
            style.Setters.Add(new Setter(BackgroundProperty, Brushes.DarkRed));
            this.DataPointStyle = style;
        }
    }
}
