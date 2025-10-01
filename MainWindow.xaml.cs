using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Anime_Stiker;


public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.SetWindowPosition();
    }

    private void SetWindowPosition()
    {
        this.WindowStartupLocation = WindowStartupLocation.Manual;

        double screenWidth = SystemParameters.PrimaryScreenWidth;
        //double screenHeight = SystemParameters.PrimaryScreenHeight;

        this.Left = screenWidth - this.Width;
        this.Top = 0;
    }

    private void InitCanvasTextHolderStraight()
    {
        var center = new Point(CanvasTextHolderStaight.ActualWidth / 2, CanvasTextHolderStaight.ActualHeight / 2);
        var x = CanvasTextHolderStaight.ActualWidth * SliderX.Value / 100;
        var y = CanvasTextHolderStaight.ActualHeight - (CanvasTextHolderStaight.ActualHeight * SliderY.Value / 100);

        var textBlock = new OutlinedTextBlock()
        {

            Text = TBoxText.Text,
            Stroke = Brushes.White,
            StrokeThickness = 3,
            FontSize = SliderFontSize.Value,
            FontWeight = FontWeights.UltraBold,
            Fill = Brushes.DeepPink,
            FontFamily = new FontFamily("Arial"),
        };
        CanvasTextHolderStaight.Children.Add(textBlock);
        CanvasTextHolderStaight.UpdateLayout();

        Canvas.SetLeft(textBlock, x - textBlock.ActualWidth / 2);
        Canvas.SetTop(textBlock, y - textBlock.ActualHeight / 2);
    }

    private void InitCanvasTextHolderCurved()
    {
        var text = TBoxText.Text;
        var parentCenter = new Point(CanvasTextHolderCurved.ActualWidth / 2, CanvasTextHolderCurved.ActualHeight / 2);
        var radius = (CanvasTextHolderCurved.ActualHeight / 2) * (SliderY.Value / 100);
        var n = text.Length;
        var offset = Math.PI * 1/4;
        var startAngle = Math.PI - offset;
        var endAngle = 0 + offset;
        var totalAngle = Math.Abs(startAngle - endAngle);
        var angleStep = totalAngle / (n - 1);

        for (int i = 0; i < n; i++)
        {
            // Create a canvas for each character
            var charCanvas = new Canvas();
            var textBlock = new OutlinedTextBlock()
            {
                Text = text[i].ToString(),
                Stroke = Brushes.White,
                StrokeThickness = 3,
                FontSize = SliderFontSize.Value,
                //FontSize = 30,
                FontWeight = FontWeights.UltraBold,
                Fill = Brushes.DeepPink,
                FontFamily = new FontFamily("Arial"),
            };
            charCanvas.Children.Add(textBlock);
            CanvasTextHolderCurved.Children.Add(charCanvas);
            charCanvas.UpdateLayout();

            // Calculate character position on circle
            double angle = startAngle - (i * angleStep);
            double x = parentCenter.X + radius * Math.Cos(angle) - textBlock.ActualWidth / 2;
            double y = parentCenter.Y - radius * Math.Sin(angle) - textBlock.ActualHeight / 2;

            // Set character canvas position
            Canvas.SetLeft(charCanvas, x);
            Canvas.SetTop(charCanvas, y);
        }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        InitCanvasTextHolderCurved();
        InitCanvasTextHolderStraight();
    }
}