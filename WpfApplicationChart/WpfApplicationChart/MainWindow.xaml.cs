using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfApplicationChart
{
    public partial class MainWindow : Window
    {
        private Line MainLine;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Window_Loaded);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            MenuPanel.Children.Add(Axes.MinMaxPanel());
        }

        private void Clear()
        {
            CutDetailsPanel.DataContext = null;
            CutCollectionPanel.Items.Clear();
            Cut.EraseAll(MainChart);

            MainChart.MouseMove -= new MouseEventHandler(MainChart_OnMouseMove);
            MainGrid.MouseWheel -= new MouseWheelEventHandler(MainGrid_MouseWheel);
            MainChart.MouseLeftButtonDown -= new MouseButtonEventHandler(Cut_MainChart_OnMouseLeftButtonDown);
        }

        private void LoadMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var data = Data.Load();
            if (data == null || data.Count <= 0) return;

            Clear();

            MainLine = new Line(data[0]);
            MainLine.Draw(MainChart);

            Axes.Set(MainChart, data[0]);
            
            Cut cut;
            for (int i = 1; i < data.Count; i++)
            {
                cut = new Cut(data[i]);
                cut.Draw(MainChart);
                cut.DetailsPanel = CutDetailsPanel;
                cut.AddToPanel(CutCollectionPanel);
            }

            MainChart.MouseMove += new MouseEventHandler(MainChart_OnMouseMove);
            MainGrid.MouseWheel += new MouseWheelEventHandler(MainGrid_MouseWheel);
            MainChart.MouseLeftButtonDown += new MouseButtonEventHandler(Cut_MainChart_OnMouseLeftButtonDown);
        }

        private void SaveMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Data.Save(MainChart);
        }

        private void MainGrid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Axes.Change(MainChart, e.GetPosition(MainChart).X, e.GetPosition(MainChart).Y, e.Delta, 5);
        }

        private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            MenuPanel.Width = e.NewSize.Width - CutsPanel.Width;
            MainChart.Width = MenuPanel.Width - 20;
            MainChart.Height = e.NewSize.Height - 50;

            CutsPanel.Height = e.NewSize.Height;
            CutCollectionPanel.Height = CutsPanel.Height - CutDetailsPanel.Height - 70;
        }

        private void MainChart_OnMouseMove(object sender, MouseEventArgs e)
        {
            var coordinates = MouseCursor.GetPosition(MainChart, e.GetPosition(MainLine), 2);
            Coordinates.Content = string.Concat("X = ", coordinates.X, "; Y = ", coordinates.Y);
        }

        #region Cuts
        private Cut cut;
        private void Cut_MainChart_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var point = MouseCursor.GetPosition(MainChart, e.GetPosition(MainLine), 2);
            var pointStick = Cut.CalcStick(MainLine, new Point(point.X, point.Y)); //прилипание первой точки

            cut = new Cut(pointStick.X, pointStick.Y, point.X, point.Y);
            cut.Draw(MainChart);

            MainChart.MouseMove += new MouseEventHandler(Cut_MainChart_OnMouseMove);
            MainChart.MouseLeave += new MouseEventHandler(Cut_MainChart_OnMouseLeave);
            MainChart.MouseLeftButtonUp += new MouseButtonEventHandler(Cut_MainChart_OnMouseLeftButtonUp);
        }

        private void Cut_MainChart_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (cut == null) return;

            var point = MouseCursor.GetPosition(MainChart, e.GetPosition(MainLine), 2);

            cut.X2 = point.X;
        }

        private void Cut_MainChart_OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (cut == null) return;

            cut.Erase();
            cut = null;
            MainChart.MouseMove -= new MouseEventHandler(Cut_MainChart_OnMouseMove);
            MainChart.MouseLeave -= new MouseEventHandler(Cut_MainChart_OnMouseLeave);
        }

        private void Cut_MainChart_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (cut == null) return;

            var point = MouseCursor.GetPosition(MainChart, e.GetPosition(MainLine), 2);
            var pointStick = Cut.CalcStick(MainLine, new Point(point.X, point.Y)); //прилипание второй точки

            cut.X2 = pointStick.X;
            cut.Y2 = pointStick.Y;

            cut.Erase();
            if (cut.X2 != cut.X1)
            {
                cut.Draw(MainChart);
                cut.DetailsPanel = CutDetailsPanel;
                cut.AddToPanel(CutCollectionPanel);
            }

            cut = null;

            MainChart.MouseMove -= new MouseEventHandler(Cut_MainChart_OnMouseMove);
            MainChart.MouseLeave -= new MouseEventHandler(Cut_MainChart_OnMouseLeave);
            MainChart.MouseLeftButtonUp -= new MouseButtonEventHandler(Cut_MainChart_OnMouseLeftButtonUp);
        }

        private void CutCoordinateTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender == null) return;

            double x1, x2;
            if (double.TryParse(X1TextBox.Text, out x1) && double.TryParse(X2TextBox.Text, out x2))
            {
                if (CutDetailsPanel.DataContext == null) return;

                var cut = CutDetailsPanel.DataContext as Cut;
                cut.X1 = x1;
                cut.X2 = x2;
                LenghtTextBlock.Text = cut.Lenght.ToString();
                AreaTextBlock.Text = cut.Area.ToString();
                SlopeTextBlock.Text = cut.Slope.ToString();
            }
            else
            {
                LenghtTextBlock.Text = string.Empty;
                AreaTextBlock.Text = string.Empty;
                SlopeTextBlock.Text = string.Empty;
            }
        }
        #endregion
    }
}
