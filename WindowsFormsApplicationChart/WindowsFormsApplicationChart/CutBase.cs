using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApplicationChart
{
    class CutBase : Series
    {
        public CutBase()
        {
            ChartType = SeriesChartType.Line;
            Color = Color.DarkRed;
            BorderWidth = 2;
        }
    }
}
