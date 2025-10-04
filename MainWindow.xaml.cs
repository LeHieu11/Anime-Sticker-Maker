using Anime_Stiker.Utility;
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

    private void InitCanvasTextHolderStraight()
    {
        var textList = TBoxText.Text.Split('\n');
        var x = CanvasTextHolderStaight.ActualWidth * SliderX.Value / 100;
        var y = CanvasTextHolderStaight.ActualHeight - (CanvasTextHolderStaight.ActualHeight * SliderY.Value / 100);

        for(int i = 0; i < textList.Length; i++) 
        {
            var text = textList[i];

            // Create and add textBlock into CanvasTextHolder
            var textBlock = CreateTextBlock(text);
            CanvasTextHolderStaight.Children.Add(textBlock);

            // Update width and height of textblock
            textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            textBlock.Arrange(new Rect(textBlock.DesiredSize));

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

    private void InitCanvasTextHolderCurved()
    {
        var textList = TBoxText.Text.Split('\n');
        var parentCenter = new Point()
        {
            X = CanvasTextHolderCurved.ActualWidth / 2,
            Y = CanvasTextHolderCurved.ActualHeight / 2
        };
        var parentRadius = (CanvasTextHolderCurved.ActualHeight / 2) * (SliderY.Value / 100);
        var topMostPoint = new Point(parentCenter.X, parentCenter.Y - parentRadius);
        var offset = SliderCurveIntensity.Value;

        for (int i = 0; i < textList.Length; i++)
        {
            var text = textList[i];
            var n = text.Length;

            // For the circle that gonna render text on
            var renderCircleCenterPoint = new Point()
            {
                X = parentCenter.X,
                Y = parentCenter.Y + (parentCenter.Y * offset) + (SliderSpace.Value * i),
            };
            var renderCircleRadius = Math.Abs(renderCircleCenterPoint.Y - topMostPoint.Y) - (SliderSpace.Value * i);
            var partTaken = 2 + n * SliderPadding.Value;
            var padding = Math.PI / partTaken;
            var startAngle = Math.PI - padding;
            var endAngle = 0 + padding;
            var totalAngle = Math.Abs(startAngle - endAngle);
            var angleStep = totalAngle / (n - 1);

            for (int j = 0; j < n; j++)
            {
                var ch = text[j];

                // Create a canvas for each character
                var charCanvas = new Canvas();
                var textBlock = CreateTextBlock(ch.ToString());
                charCanvas.Children.Add(textBlock);
                CanvasTextHolderCurved.Children.Add(charCanvas);

                // Update width and height of textblock
                textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                textBlock.Arrange(new Rect(textBlock.DesiredSize));

                // Calculate character position on circle
                double angle = startAngle - (j * angleStep);
                var characterPoint = new Point()
                {
                    X = renderCircleCenterPoint.X + renderCircleRadius * Math.Cos(angle),
                    Y = renderCircleCenterPoint.Y - renderCircleRadius * Math.Sin(angle)
                };

                // Set character canvas position
                Canvas.SetLeft(charCanvas, characterPoint.X - textBlock.ActualWidth / 2);
                Canvas.SetTop(charCanvas, characterPoint.Y - textBlock.ActualHeight / 2);

                // Rotate character point to center
                RotateTransform rotateTransformCharacterCanvas = new()
                {
                    CenterX = textBlock.ActualWidth / 2,
                    CenterY = textBlock.ActualHeight / 2,
                    Angle = GeometryHelper.CalculateAngle(characterPoint, renderCircleCenterPoint, topMostPoint)
                };
                charCanvas.RenderTransform = rotateTransformCharacterCanvas;
            }
        }

        // Rotate CanvasTextHolder
        RotateTransform rotateTransformCanvasTextHolder = new()
        {
            CenterX = CanvasTextHolderCurved.ActualWidth / 2,
            CenterY = CanvasTextHolderCurved.ActualHeight / 2,
            Angle = SliderRotation.Value,
        };
        CanvasTextHolderCurved.RenderTransform = rotateTransformCanvasTextHolder;
    }



    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        InitCanvasTextHolderCurved();
        InitCanvasTextHolderStraight();
    }
}