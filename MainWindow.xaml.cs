using Anime_Stiker.Utilty;
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
        var textList = TBoxText.Text.Split('\n');
        var center = new Point(CanvasTextHolderStaight.ActualWidth / 2, CanvasTextHolderStaight.ActualHeight / 2);
        var x = CanvasTextHolderStaight.ActualWidth * SliderX.Value / 100;
        var y = CanvasTextHolderStaight.ActualHeight - (CanvasTextHolderStaight.ActualHeight * SliderY.Value / 100);

        for(int i = 0; i < textList.Length; i++) 
        {
            var text = textList[i];

            // Create and add textBlock into CanvasTextHolder
            var textBlock = CreateTextBlock(text);
            CanvasTextHolderStaight.Children.Add(textBlock);
            CanvasTextHolderStaight.UpdateLayout();

            // Set coridanate
            var space = SliderSpace.Value;
            Canvas.SetLeft(textBlock, x - textBlock.ActualWidth / 2);
            Canvas.SetTop(textBlock, y - (textBlock.ActualHeight / 2) + (space * i));
        }

        // Rotate CanvasTextHolder
        RotateTransform rotateTransform = new()
        {
            CenterX = CanvasTextHolderStaight.ActualWidth / 2,
            CenterY = CanvasTextHolderStaight.ActualHeight / 2,
            Angle = SliderRotation.Value,
        };
        CanvasTextHolderStaight.RenderTransform = rotateTransform;
    }

    private OutlinedTextBlock CreateTextBlock(String text)
    {
        var textBlock = new OutlinedTextBlock()
        {

            Text = text,
            Stroke = Brushes.White,
            StrokeThickness = 3,
            FontSize = SliderFontSize.Value,
            FontWeight = FontWeights.UltraBold,
            Fill = Brushes.DeepPink,
            FontFamily = new FontFamily("Arial"),
        };

        return textBlock;
    }

    private double CalculatePartTaken(int n, double curveIntens)
    {
        return 2 + curveIntens;
    }

    private void InitCanvasTextHolderCurved()
    {
        var text = TBoxText.Text;
        var n = text.Length;
        var parentCenter = new Point(CanvasTextHolderCurved.ActualWidth / 2, CanvasTextHolderCurved.ActualHeight / 2);
        var parentRadius = (CanvasTextHolderCurved.ActualHeight / 2) * (SliderY.Value / 100);
        var topMostPoint = new Point(parentCenter.X, parentCenter.Y - parentRadius);

        // For the circle that gonna render text on
        var offset = SliderCurveIntensity.Value;
        var renderCircleCenterPoint = new Point()
        {
            X = parentCenter.X,
            Y = parentCenter.Y + parentCenter.Y * offset,
        };
        var renderCircleRadius =  Math.Abs(renderCircleCenterPoint.Y - topMostPoint.Y);
        var partTaken = CalculatePartTaken(n, offset);
        var padding = Math.PI / partTaken;
        var startAngle = Math.PI - padding;
        var endAngle = 0 + padding;
        var totalAngle = Math.Abs(startAngle - endAngle);
        var angleStep = totalAngle / (n - 1);

        for (int i = 0; i < n; i++)
        {
            var ch = text[i];

            // Create a canvas for each character
            var charCanvas = new Canvas();
            var textBlock = CreateTextBlock(ch.ToString());
            charCanvas.Children.Add(textBlock);
            CanvasTextHolderCurved.Children.Add(charCanvas);
            CanvasTextHolderCurved.UpdateLayout();

            // Calculate character position on circle
            double angle = startAngle - (i * angleStep);
            var characterPoint = new Point()
            {
                X = renderCircleCenterPoint.X + renderCircleRadius * Math.Cos(angle),
                Y = renderCircleCenterPoint.Y - renderCircleRadius * Math.Sin(angle) 
            };

            // Set character canvas position
            Canvas.SetLeft(charCanvas, characterPoint.X - textBlock.ActualWidth / 2);
            Canvas.SetTop(charCanvas, characterPoint.Y - textBlock.ActualHeight / 2);

            // Rotate character canvas into parent canvas center
            RotateTransform rotateTransformCharacterCanvas = new()
            {
                CenterX = textBlock.ActualWidth / 2,
                CenterY = textBlock.ActualHeight / 2,
                Angle = GeometryHelper.CalculateAngle(characterPoint, renderCircleCenterPoint, topMostPoint)
            };
            charCanvas.RenderTransform = rotateTransformCharacterCanvas;
        }

        // Rotate CanvasTextHolder
        RotateTransform rotateTransformCanvasTextHolder = new()
        {
            CenterX = CanvasTextHolderCurved.ActualWidth / 2,
            CenterY = CanvasTextHolderCurved.ActualHeight / 2,
            Angle = SliderRotation.Value,
        };
        CanvasTextHolderStaight.RenderTransform = rotateTransformCanvasTextHolder;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        InitCanvasTextHolderCurved();
        InitCanvasTextHolderStraight();
    }
}