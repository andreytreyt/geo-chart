using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApplicationChart
{
    class Data
    {
        private static string OpenFile()
        {
            var dialog = new OpenFileDialog { DefaultExt = ".csv", Filter = "(.csv)|*.csv" };
            dialog.ShowDialog();
            return dialog.FileName;
        }

        private static string SaveFile()
        {
            var dialog = new SaveFileDialog() { DefaultExt = ".csv", Filter = "(.csv)|*.csv" };
            dialog.ShowDialog();
            return dialog.FileName;
        }

        public static ObservableCollection<List<DataPoint>> Load()
        {
            var pointsObservable = new ObservableCollection<List<DataPoint>>();

            bool cuts = false;
            int i = 0;

            var points = new List<DataPoint>();
            var filename = OpenFile();

            if (string.IsNullOrEmpty(filename)) return null;

            var reader = new StreamReader(File.OpenRead(filename));
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();

                if (System.String.CompareOrdinal(line, "cuts") == 0)
                {
                    pointsObservable.Add(points);
                    points = new List<DataPoint>();
                    cuts = true;
                    continue;
                }

                var values = line.Split(',');

                double x, y;
                if (double.TryParse(values[0], out x) && double.TryParse(values[1], out y))
                {
                    points.Add(new DataPoint(x, y));
                }
                else
                {
                    MessageBox.Show("Файл не валиден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }

                if (cuts)
                {
                    if (++i == 2)
                    {
                        pointsObservable.Add(points);
                        points = new List<DataPoint>();
                        i = 0;
                    }
                }
            }

            if (!cuts) pointsObservable.Add(points);

            reader.Close();
            return pointsObservable;
        }

        public static void Save(Chart chart)
        {
            string filename = SaveFile();

            if (string.IsNullOrEmpty(filename) || chart.Series.Count <= 0) return;

            File.Delete(filename);
            var writer = new StreamWriter(File.OpenWrite(filename));

            foreach (DataPoint point in chart.Series[0].Points)
            {
                writer.WriteLine(string.Concat(point.XValue, ",", point.YValues[0]));
            }

            if (chart.Series.Count > 1)
            {
                writer.WriteLine("cuts");

                foreach (Series series in chart.Series)
                {
                    if (series is Cut)
                    {
                        foreach (DataPoint point in series.Points)
                        {
                            writer.WriteLine(string.Concat(point.XValue, ",", point.YValues[0]));
                        }
                    }
                }
            }
            writer.Close();

            MessageBox.Show("Файл сохранен.", string.Empty, MessageBoxButtons.OK);
        }
    }
}
