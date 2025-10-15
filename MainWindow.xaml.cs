using Anime_Stiker.CustomControl;
using Anime_Stiker.Utility;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

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

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        SetBothTextHoldersPosition();
        InitCanvasTextHolderCurved();
        InitCanvasTextHolderStraight();
    }

    private void SetBothTextHoldersPosition()
    {
        // Some varible
        var xPercentage = SliderX.Value / 100;
        var yPercentage = SliderY.Value / 100;

        // Set CanvasTextHolderStaight correspond to X,Y Slider
        Canvas.SetLeft(
            CanvasTextHolderStaight,
            CanvasTextHolderStaight.ActualWidth * xPercentage - CanvasTextHolderStaight.ActualWidth / 2);
        Canvas.SetTop(
            CanvasTextHolderStaight,
            CanvasTextHolderStaight.ActualHeight * (1 - yPercentage) -
            CanvasTextHolderStaight.ActualHeight / 2);

        // Set CanvasTextHolderCurve correspond to X,Y Slider
        Canvas.SetLeft(
            CanvasTextHolderCurved,
            CanvasTextHolderCurved.ActualWidth * xPercentage - CanvasTextHolderCurved.ActualWidth / 2);
        Canvas.SetTop(
            CanvasTextHolderCurved,
            CanvasTextHolderCurved.ActualHeight * (1 - yPercentage) -
            CanvasTextHolderCurved.ActualHeight / 2);
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
        CanvasTextHolderStaight.Children.Clear();

        var textList = TBoxText.Text.Split('\n');
        for (int i = 0; i < textList.Length; i++) 
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
            Canvas.SetLeft(textBlock, CanvasTextHolderStaight.ActualWidth / 2 - textBlock.ActualWidth / 2);
            Canvas.SetTop(textBlock, CanvasTextHolderStaight.ActualHeight / 2 - (textBlock.ActualHeight / 2) + (space * i));
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
        CanvasTextHolderCurved.Children.Clear();

        var textList = TBoxText.Text.Split('\n');
        var parentCenter = new Point()
        {
            X = CanvasTextHolderCurved.ActualWidth / 2,
            Y = CanvasTextHolderCurved.ActualHeight
        };
        var parentRadius = CanvasTextHolderCurved.ActualHeight / 2;
        var topMostPoint = new Point(parentCenter.X, parentCenter.Y - parentRadius);
        var letterSpace = SliderLetterSpace.Value / 100;
        var curveness = SliderCurveness.Value / 100;
        var space = SliderSpace.Value;

        for (int i = 0; i < textList.Length; i++)
        {
            var text = textList[i];
            var n = text.Length;

            // For the circle that gonna render text on
            var renderCircleCenterPoint = new Point()
            {
                X = parentCenter.X,
                Y = parentCenter.Y + (parentCenter.Y * letterSpace) + (space * i),
            };
            var renderCircleRadius = Math.Abs(renderCircleCenterPoint.Y - topMostPoint.Y) - (SliderSpace.Value * i);
            var padding = Math.PI / (2 + (n + letterSpace) * curveness);
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

    private void SliderX_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        var newValue = CanvasSticker.ActualWidth * SliderX.Value / 100 - CanvasSticker.ActualWidth / 2 ;

        Canvas.SetLeft(CanvasTextHolderStaight, newValue) ;
        Canvas.SetLeft(CanvasTextHolderCurved, newValue);
    }

    private void SliderY_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        var newValue = CanvasSticker.ActualHeight * (1 - SliderY.Value / 100) - CanvasSticker.ActualHeight / 2;

        Canvas.SetTop(CanvasTextHolderStaight, newValue);
        Canvas.SetTop(CanvasTextHolderCurved, newValue);
    }

    private void ChkCurve_Checked(object sender, RoutedEventArgs e)
    {
        // Change visible canvas
        CanvasTextHolderStaight.Visibility = Visibility.Hidden;
        CanvasTextHolderCurved.Visibility = Visibility.Visible;

        // Enable curve control
        SliderCurveness.IsEnabled = true;
        SliderLetterSpace.IsEnabled = true;
    }

    private void ChkCurve_Unchecked(object sender, RoutedEventArgs e)
    {
        // Change visible canvas
        CanvasTextHolderStaight.Visibility = Visibility.Visible;
        CanvasTextHolderCurved.Visibility = Visibility.Hidden;

        // Disable curve control
        SliderCurveness.IsEnabled = false;
        SliderLetterSpace.IsEnabled = false;
    }

    private void SliderRotation_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        // Rotate CanvasTextHolder
        RotateTransform rotateTransformCanvasTextHolder = new()
        {
            CenterX = CanvasSticker.ActualWidth / 2,
            CenterY = CanvasSticker.ActualHeight / 2,
            Angle = SliderRotation.Value,
        };

        CanvasTextHolderStaight.RenderTransform = rotateTransformCanvasTextHolder;
        CanvasTextHolderCurved.RenderTransform = rotateTransformCanvasTextHolder;
    }

    private void UpdateCanvasTextHolders(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        Dispatcher.BeginInvoke(
            new Action(() =>
            {
                this.InitCanvasTextHolderCurved();
                this.InitCanvasTextHolderStraight();
            }), DispatcherPriority.Background);
    }

    private DispatcherOperation? _pendingTextUpdate;
    private void TBoxText_TextChanged(object sender, TextChangedEventArgs e)
    {
        // This ensures only the *latest* update gets processed.
        if (_pendingTextUpdate != null && _pendingTextUpdate.Status == DispatcherOperationStatus.Pending)
        {
            _pendingTextUpdate.Abort();
        }

        // Use Background priority to ensure stable layout dimensions.
        _pendingTextUpdate = Dispatcher.BeginInvoke(
            new Action(() =>
            {
                this.InitCanvasTextHolderCurved();
                this.InitCanvasTextHolderStraight();

                // Optional: Clear the reference when done
                _pendingTextUpdate = null;
            }), DispatcherPriority.Background);
    }

    private void BtnCopy_Click(object sender, RoutedEventArgs e)
    {
        double dpi = 96.0;
        RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
            (int)Math.Round(CanvasSticker.ActualWidth * (dpi / 96.0)),
            (int)Math.Round(CanvasSticker.ActualHeight * (dpi / 96.0)),
            dpi,
            dpi,
            PixelFormats.Pbgra32);
        renderBitmap.Render(CanvasSticker);

        // Create the PNG MemoryStream
        MemoryStream pngStream = new();
        PngBitmapEncoder encoder = new();
        encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
        encoder.Save(pngStream);

        // Create the DataObject
        DataObject data = new();

        // The RenderTargetBitmap object is what Clipboard.SetImage() would use (standard formats)
        data.SetData(typeof(BitmapSource), renderBitmap);

        // Add the PNG stream with a custom format name "PNG"
        // Applications like MS Office, Paint.NET, etc., will look for and use this.
        data.SetData("PNG", pngStream);

        // Set the DataObject to the Clipboard
        Clipboard.Clear();
        Clipboard.SetDataObject(data, true);
    }

    private void BtnChooseChar_Click(object sender, RoutedEventArgs e)
    {
        var characterWindow = new CharactersWindow();

        characterWindow.ShowDialog();
    }
}