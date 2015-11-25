using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls.DataVisualization.Charting;
using System.Xml.Serialization;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace WpfApplicationChart
{
    class Data
    {
        private static string OpenFile()
        {
            var dialog = new OpenFileDialog { DefaultExt = ".csv", Filter = "(.csv)|*.csv" };
            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        private static string SaveFile()
        {
            var dialog = new SaveFileDialog() { DefaultExt = ".csv", Filter = "(.csv)|*.csv" };
            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        public static ObservableCollection<PointCollection> Load()
        {
            var pointsObservable = new ObservableCollection<PointCollection>();

            bool cuts = false;
            int i = 0;

            var points = new PointCollection();
            var filename = OpenFile();

            if (string.IsNullOrEmpty(filename)) return null;

            var reader = new StreamReader(File.OpenRead(filename));
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();

                if (System.String.CompareOrdinal(line, "cuts") == 0)
                {
                    pointsObservable.Add(points);
                    points = new PointCollection();
                    cuts = true;
                    continue;
                }

                var values = line.Split(',');

                double x, y;
                if (double.TryParse(values[0], out x) && double.TryParse(values[1], out y))
                {
                    points.Add(new Point(y, x)); //x и y поменяны, чтобы график строился верно
                }
                else
                {
                    MessageBox.Show("Файл не валиден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }

                if (cuts)
                {
                    if (++i == 2)
                    {
                        pointsObservable.Add(points);
                        points = new PointCollection();
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

            foreach (Point point in (chart.Series[0] as LineSeries).Points)
            {
                writer.WriteLine(MouseCursor.GetPosition(chart, point, 2));
            }

            if (chart.Series.Count > 1)
            {
                writer.WriteLine("cuts");

                foreach (LineSeries series in chart.Series)
                {
                    if (series is Cut)
                    {
                        foreach (Point point in series.Points)
                        {
                            writer.WriteLine(MouseCursor.GetPosition(chart, point, 2));
                        }
                    }
                }
            }
            writer.Close();

            MessageBox.Show("Файл сохранен.", string.Empty, MessageBoxButton.OK);
        }
    }
}
