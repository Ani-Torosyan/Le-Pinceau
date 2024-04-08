using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Le_Pinceau
{
    /// <summary>
    /// Main window class for the Le Pinceau application.
    /// </summary>
    public partial class MainWindow : Window
    {
        // Default drawing attributes for the pen tool
        private readonly DrawingAttributes penAttributes = new()
        {
            Color = Colors.Black,
            Height = 2,
            Width = 2
        };

        // Default drawing attributes for the highlighter tool
        private readonly DrawingAttributes highlighterAttributes = new()
        {
            Color = Colors.Yellow,
            Height = 10,
            Width = 2,
            IsHighlighter = true,
        };

        /// <summary>
        /// Constructor for the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            // Set the default editing mode to none
            Canvas.EditingMode = InkCanvasEditingMode.None;
            // Set the default drawing attributes to the pen attributes
            Canvas.DefaultDrawingAttributes = penAttributes;

            // Attach event handlers for mouse interactions
            Canvas.MouseLeftButtonDown += Canvas_MouseDown;
            Canvas.MouseMove += Canvas_MouseMove;
            Canvas.MouseLeftButtonUp += Canvas_MouseUp;
        }

        #region Editing Mode

        /// <summary>
        /// Event handlers for toolbar buttons to set editing mode
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event data that contains information about the changed event.</param>
        private void SelectBtn_Click(object sender, RoutedEventArgs e)
        {
            SetEditingMode(EditingMode.Select);
        }

        private void PenBtn_Click(object sender, RoutedEventArgs e)
        {
            SetEditingMode(EditingMode.Pen);
        }

        private void HighlighterBtn_Click(object sender, RoutedEventArgs e)
        {
            SetEditingMode(EditingMode.Highlighter);
        }
        
        private void EraserBtn_Click(object sender, RoutedEventArgs e)
        {
            SetEditingMode(EditingMode.Eraser);
        }

        private void OvalBtn_Click(object sender, RoutedEventArgs e)
        {
            SetEditingMode(EditingMode.Oval);
        }

        private void RectangleBtn_Click(object sender, RoutedEventArgs e)
        {
            SetEditingMode(EditingMode.Rectangle);
        }

        private void LineBtn_Click(object sender, RoutedEventArgs e)
        {
            SetEditingMode(EditingMode.Line);
        }

        private void BucketBtn_Click(object sender, RoutedEventArgs e)
        {
            SetEditingMode(EditingMode.Bucket);
        }

        private void PickColorBtn_Click(object sender, RoutedEventArgs e)
        {
            SetEditingMode(EditingMode.Pick);
        }

        private void TextBtn_Click(object sender, RoutedEventArgs e)
        {
            SetEditingMode(EditingMode.Text);
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            SetEditingMode(EditingMode.Save);
        }

        private void LoadBtn_Click(object sender, RoutedEventArgs e)
        {
            SetEditingMode(EditingMode.Load);
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            SetEditingMode(EditingMode.Clear);
        }

        #region Save

        /// <summary>
        /// Saves the content of the canvas as a PNG image file.
        /// </summary>
        private void Save()
        {
            // Open a dialog for the user to specify the file location for saving
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Files (*.png)|*.png";

            // If the user selects a file and confirms the action
            if (saveFileDialog.ShowDialog() == true)
            {
                // Create a bitmap of the canvas content
                RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)Canvas.ActualWidth, (int)Canvas.ActualHeight, 96, 96, System.Windows.Media.PixelFormats.Default);
                renderTargetBitmap.Render(Canvas);

                // Encode the bitmap as PNG
                PngBitmapEncoder pngEncoder = new PngBitmapEncoder();
                pngEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

                // Write the PNG data to the selected file
                using (FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    pngEncoder.Save(fs);
                }
            }
        }

        #endregion
        #region Clear

        /// <summary>
        /// Clears the content of the canvas by removing all children elements and strokes.
        /// </summary>
        private void Clear()
        {
            Canvas.Children.Clear();
            Canvas.Strokes.Clear();
        }

        #endregion
        #region Load
        /// <summary>
        /// Loads an image from a file and displays it centered on the canvas.
        /// </summary>
        /// <param name="filePath">The path of the image file to load.</param>
        private void LoadImageIntoInkCanvas(string filePath)
        {
            // Create a bitmap image from the specified file
            BitmapImage bitmapImage = new BitmapImage(new Uri(filePath));

            // Calculate the dimensions of the image to fit within the canvas while preserving aspect ratio
            double inkCanvasWidth = Canvas.ActualWidth;
            double inkCanvasHeight = Canvas.ActualHeight;
            double aspectRatio = bitmapImage.Width / bitmapImage.Height;
            double newWidth = inkCanvasWidth;
            double newHeight = newWidth / aspectRatio;

            if (newHeight > inkCanvasHeight)
            {
                newHeight = inkCanvasHeight;
                newWidth = newHeight * aspectRatio;
            }

            // Create an image element and set its properties
            Image image = new Image
            {
                Source = bitmapImage
                /*Stretch = Stretch.Uniform,
                Width = newWidth,
                Height = newHeight*/
            };

            // Center the image within the canvas
            InkCanvas.SetLeft(image, (inkCanvasWidth - newWidth) / 2);
            InkCanvas.SetTop(image, (inkCanvasHeight - newHeight) / 2);

            // Add the image to the canvas
            Canvas.Children.Add(image);
        }
        #endregion

        /// <summary>
        /// Sets the editing mode of the canvas based on the specified mode.
        /// </summary>
        /// <param name="mode">The editing mode to set.</param>
        private void SetEditingMode(EditingMode mode)
        {
            // Uncheck all editing mode buttons
            SelectBtn.IsChecked = false;
            PenBtn.IsChecked = false;
            HighlighterBtn.IsChecked = false;
            EraserBtn.IsChecked = false;
            OvalBtn.IsChecked = false;
            RectangleBtn.IsChecked = false;
            LineBtn.IsChecked = false;
            BucketBtn.IsChecked = false;
            PickColorBtn.IsChecked = false;
            TextBtn.IsChecked = false;
            SaveBtn.IsChecked = false;
            LoadBtn.IsChecked = false;
            ClearBtn.IsChecked = false;

            // Set canvas editing mode to none initially
            Canvas.EditingMode = InkCanvasEditingMode.None;

            // Set the appropriate editing mode based on the input mode
            switch (mode)
            {
                case EditingMode.Select:
                    SelectBtn.IsChecked = true;
                    Canvas.EditingMode = InkCanvasEditingMode.Select;
                    break;

                case EditingMode.Pen:
                    PenBtn.IsChecked = true;
                    Canvas.EditingMode = InkCanvasEditingMode.Ink;
                    Canvas.DefaultDrawingAttributes = penAttributes;
                    break;

                case EditingMode.Highlighter:
                    HighlighterBtn.IsChecked = true;
                    Canvas.EditingMode = InkCanvasEditingMode.Ink;
                    Canvas.DefaultDrawingAttributes = highlighterAttributes;
                    break;

                case EditingMode.Eraser:
                    EraserBtn.IsChecked = true;
                    if (PartialStrokeRadio.IsChecked == true)
                    {
                        Canvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
                    }
                    else
                    {
                        Canvas.EditingMode = InkCanvasEditingMode.EraseByStroke;
                    }
                    break;

                case EditingMode.Oval:
                    OvalBtn.IsChecked = true;
                    break;

                case EditingMode.Rectangle:
                    RectangleBtn.IsChecked = true;
                    break;

                case EditingMode.Line:
                    LineBtn.IsChecked = true;
                    break;

                case EditingMode.Bucket:
                    BucketBtn.IsChecked = true;
                    break;

                case EditingMode.Pick:
                    PickColorBtn.IsChecked = true;
                    break;

                case EditingMode.Text:
                    TextBtn.IsChecked = true;
                    break;

                case EditingMode.Save:
                    SaveBtn.IsChecked = true;
                    Save();
                    break;

                case EditingMode.Load:
                    LoadBtn.IsChecked = true;
                    // Open file dialog to load image into ink canvas
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                    if (openFileDialog.ShowDialog() == true)
                    {
                        LoadImageIntoInkCanvas(openFileDialog.FileName);
                    }
                    break;

                case EditingMode.Clear:
                    ClearBtn.IsChecked = true;
                    Clear();
                    break;

                default:
                    break;
            }
        }

        #endregion
        #region Pen

        /// <summary>
        /// Event handler for when the selected color in the pen color picker changes.
        /// </summary>
        private void PenColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            // Check if the control is fully loaded
            if (IsLoaded)
            {
                // Set the color of the pen to the selected color, defaulting to black if none is selected
                penAttributes.Color = PenColorPicker.SelectedColor ?? Colors.Black;
                // Set the fill color to the selected color, defaulting to black if none is selected
                fillColor = PenColorPicker.SelectedColor ?? Colors.Black;
            }
        }

        #endregion
        #region Highlighter

        /// <summary>
        /// Event handler for when the yellow radio button is clicked to select yellow as the highlighter color.
        /// </summary>
        private void YellowRadio_Click(object sender, RoutedEventArgs e)
        {
            highlighterAttributes.Color = Colors.Yellow;
        }

        /// <summary>
        /// Event handler for when the cyan radio button is clicked to select cyan as the highlighter color.
        /// </summary>
        private void CyanRadio_Click(object sender, RoutedEventArgs e)
        {
            highlighterAttributes.Color = Colors.Cyan;
        }

        /// <summary>
        /// Event handler for when the magenta radio button is clicked to select magenta as the highlighter color.
        /// </summary>
        private void MagentaRadio_Click(object sender, RoutedEventArgs e)
        {
            highlighterAttributes.Color = Colors.Magenta;
        }

        #endregion
        #region Eraser Type

        /// <summary>
        /// Event handler for when the partial stroke radio button is clicked to select partial stroke erasing mode.
        /// </summary>
        private void PartialStrokeRadio_Click(object sender, RoutedEventArgs e)
        {
            if (EraserBtn.IsChecked == true)
            {
                Canvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
            }
        }

        /// <summary>
        /// Event handler for when the full stroke radio button is clicked to select full stroke erasing mode.
        /// </summary>
        private void FullStrokeRadio_Click(object sender, RoutedEventArgs e)
        {
            if (EraserBtn.IsChecked == true)
            {
                Canvas.EditingMode = InkCanvasEditingMode.EraseByStroke;
            }
        }

        #endregion
        #region Slider

        /// <summary>
        /// Event handler for when the ThicknessSlider's value changes.
        /// </summary>
        private void ThicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsLoaded)
            {
                penAttributes.Width = ThicknessSlider.Value;
                penAttributes.Height = ThicknessSlider.Value;

                highlighterAttributes.Width = ThicknessSlider.Value;
                highlighterAttributes.Height = ThicknessSlider.Value * 5;
            }
        }

        #endregion
        #region Shape Drawing

        private Point? startPoint = new Point();
        private Point startPointLine = new Point();
        private Line? currentLine = null;

        private Color fillColor = Colors.Black;
        private int ovals = 0, rectangles = 0, lines = 0;

        /// <summary>
        /// Event handler for handling mouse down events on the canvas.
        /// </summary>
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (OvalBtn.IsChecked == true || RectangleBtn.IsChecked == true)
            {
                // Get the starting point for shape drawing
                startPoint = e.GetPosition(Canvas);
            }

            if (BucketBtn.IsChecked == true)
            {
                Point startPoint = e.GetPosition(Canvas);
                // Fill the area starting from the specified point with the selected color
                Fill(Canvas, startPoint, fillColor);
            }

            if (PickColorBtn.IsChecked == true)
            {
                Point startPoint = e.GetPosition(Canvas);
                // Pick the color from the specified point and set it as the current drawing color
                Color color = Fill(Canvas, startPoint, fillColor);
                penAttributes.Color = color;
                fillColor = color;
            }

            if (TextBtn.IsChecked == true)
            {
                Point startPoint = e.GetPosition(Canvas);
                // Create a new text box for user input
                TextBox textBox = new TextBox
                {
                    Width = Canvas.ActualWidth,
                    Background = Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    Foreground = new SolidColorBrush(fillColor),
                    FontSize = 18
                };

                double left = startPoint.X;
                double top = startPoint.Y;
                double maxWidth = Canvas.ActualWidth - startPoint.X;
                double maxHeight = Canvas.ActualHeight - startPoint.Y;
                textBox.MaxWidth = maxWidth;
                textBox.MaxHeight = maxHeight;

                InkCanvas.SetLeft(textBox, startPoint.X);
                InkCanvas.SetTop(textBox, startPoint.Y);

                Canvas.Children.Add(textBox);

                textBox.Focus();
                textBox.SelectAll();
            }

            if (LineBtn.IsChecked == true)
            {
                // Get the starting point for line drawing
                startPointLine = e.GetPosition(Canvas);

                // Create a new line object
                currentLine = new Line
                {
                    Stroke = new SolidColorBrush(penAttributes.Color),
                    StrokeThickness = penAttributes.Width
                };

                currentLine.X1 = startPointLine.X;
                currentLine.Y1 = startPointLine.Y;
                currentLine.X2 = startPointLine.X;
                currentLine.Y2 = startPointLine.Y;

                Canvas.Children.Add(currentLine);
            }
        }

        /// <summary>
        /// Event handler for handling mouse move events on the canvas.
        /// </summary>
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && startPoint.HasValue && OvalBtn.IsChecked == true)
            {
                Point currentPoint = e.GetPosition(Canvas);

                // Remove the ovals made in a process of drawing and positioning a new one
                if (Canvas.Children.Count > ovals + rectangles + lines && Canvas.Children[Canvas.Children.Count - 1] is Ellipse ellipse)
                {
                    Canvas.Children.Remove(ellipse);
                }

                // Create a new ellipse for drawing
                Ellipse oval = new Ellipse
                {
                    Stroke = new SolidColorBrush(penAttributes.Color),
                    StrokeThickness = penAttributes.Width,
                    Fill = Brushes.Transparent,
                    Width = Math.Abs(currentPoint.X - startPoint.Value.X),
                    Height = Math.Abs(currentPoint.Y - startPoint.Value.Y)
                };

                InkCanvas.SetLeft(oval, Math.Min(startPoint.Value.X, currentPoint.X));
                InkCanvas.SetTop(oval, Math.Min(startPoint.Value.Y, currentPoint.Y));

                Canvas.Children.Add(oval);
            }

            if (e.LeftButton == MouseButtonState.Pressed && startPoint.HasValue && RectangleBtn.IsChecked == true)
            {
                Point currentPoint = e.GetPosition(Canvas);

                // Remove the rectangles made in a process of drawing and positioning a new one
                if (Canvas.Children.Count > rectangles + ovals + lines && Canvas.Children[Canvas.Children.Count - 1] is Rectangle rectangle)
                {
                    Canvas.Children.Remove(rectangle);
                }

                // Create a new rectangle for drawing
                Rectangle rect = new Rectangle
                {
                    Stroke = new SolidColorBrush(penAttributes.Color),
                    StrokeThickness = penAttributes.Width,
                    Fill = Brushes.Transparent,
                    Width = Math.Abs(currentPoint.X - startPoint.Value.X),
                    Height = Math.Abs(currentPoint.Y - startPoint.Value.Y)
                };

                InkCanvas.SetLeft(rect, Math.Min(startPoint.Value.X, currentPoint.X));
                InkCanvas.SetTop(rect, Math.Min(startPoint.Value.Y, currentPoint.Y));

                Canvas.Children.Add(rect);
            }

            if (e.LeftButton == MouseButtonState.Pressed && currentLine != null && LineBtn.IsChecked == true)
            {
                Point endPoint = e.GetPosition(Canvas);

                currentLine.X2 = endPoint.X;
                currentLine.Y2 = endPoint.Y;
            }
        }


        /// <summary>
        /// Event handler for handling mouse up events on the canvas.
        /// </summary>
        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // Increment respective counter if Oval, Rectangle, or Line drawing mode is active
            if (OvalBtn.IsChecked == true)
            {
                ovals++;
            }
            if(RectangleBtn.IsChecked == true)
            {
                rectangles++;
            }
            if (LineBtn.IsChecked == true)
            {
                lines++;
            }
        }

        #endregion
        #region Bucket

        /// <summary>
        /// Fills the specified area in the given InkCanvas with the provided fill color using the flood fill algorithm.
        /// </summary>
        /// <param name="inkCanvas">The InkCanvas to be filled.</param>
        /// <param name="startPoint">The starting point for the fill operation.</param>
        /// <param name="fillColor">The color to fill the area with.</param>
        /// <returns>The color of the filled area.</returns>
        private Color Fill(InkCanvas inkCanvas, Point startPoint, Color fillColor)
        {
            // Rendering the current state of the InkCanvas onto a RenderTargetBitmap
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)inkCanvas.ActualWidth, (int)inkCanvas.ActualHeight, 96d, 96d, PixelFormats.Default);
            renderBitmap.Render(inkCanvas);

            // Converting the RenderTargetBitmap to a WriteableBitmap
            BitmapSource bitmapSource = new FormatConvertedBitmap(renderBitmap, PixelFormats.Bgra32, null, 0);
            WriteableBitmap writableBitmap = new WriteableBitmap(bitmapSource);

            // Applying the flood fill algorithm and getting the color of the filled area
            Color targetColor = FloodFill(writableBitmap, startPoint, fillColor);

            // Creating an Image control to display the filled area
            Image filledImage = new Image();
            filledImage.Source = writableBitmap;

            // Adding the filled Image control to the InkCanvas
            inkCanvas.Children.Add(filledImage);

            // Returning the color of the filled area
            return targetColor;
        }

        /// <summary>
        /// Performs the flood fill algorithm on the provided WriteableBitmap starting from the specified point.
        /// </summary>
        /// <param name="bitmap">The WriteableBitmap to be filled.</param>
        /// <param name="startPoint">The starting point for the fill operation.</param>
        /// <param name="fillColor">The color to fill the area with.</param>
        /// <returns>The color of the filled area.</returns>
        private Color FloodFill(WriteableBitmap bitmap, Point startPoint, Color fillColor)
        {
            // Getting width and height of the bitmap
            int width = bitmap.PixelWidth;
            int height = bitmap.PixelHeight;

            // Calculating the stride of the bitmap
            int stride = width * 4;

            // Copying the pixel data of the bitmap into an array
            byte[] pixels = new byte[height * stride];
            bitmap.CopyPixels(pixels, stride, 0);

            // Extracting integer coordinates from the start point
            int x = (int)startPoint.X;
            int y = (int)startPoint.Y;

            // Getting the color of the pixel at the start point
            Color targetColor = GetPixel(pixels, stride, x, y, width, height);

            if (PickColorBtn.IsChecked == true)
            {
                return targetColor;
            }

            // Initializing a stack for the flood fill algorithm
            Stack<Point> stack = new Stack<Point>();
            stack.Push(new Point(x, y));

            // Iteratively performing the flood fill algorithm
            while (stack.Count > 0)
            {
                Point point = stack.Pop();
                x = (int)point.X;
                y = (int)point.Y;

                if (x >= 0 && y >= 0 && x < width && y < height && GetPixel(pixels, stride, x, y, width, height) == targetColor)
                {
                    SetPixel(pixels, stride, x, y, width, fillColor);

                    stack.Push(new Point(x + 1, y));
                    stack.Push(new Point(x - 1, y));
                    stack.Push(new Point(x, y + 1));
                    stack.Push(new Point(x, y - 1));
                }
            }

            // Writing the modified pixel data back to the bitmap
            bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);

            // Returning the color of the filled area
            return targetColor;
        }

        /// <summary>
        /// Retrieves the color of the pixel at the specified coordinates from the provided pixel data.
        /// </summary>
        /// <param name="pixels">The pixel data array.</param>
        /// <param name="stride">The stride of the bitmap.</param>
        /// <param name="x">The x-coordinate of the pixel.</param>
        /// <param name="y">The y-coordinate of the pixel.</param>
        /// <param name="width">The width of the bitmap.</param>
        /// <param name="height">The height of the bitmap.</param>
        /// <returns>The color of the specified pixel.</returns>
        private Color GetPixel(byte[] pixels, int stride, int x, int y, int width, int height)
        {
            int index = y * stride + 4 * x;

            // Extracting color components (blue, green, red, alpha) from the pixel data
            byte blue = pixels[index];
            byte green = pixels[index + 1];
            byte red = pixels[index + 2];
            byte alpha = pixels[index + 3];

            // Creating and returning a Color object from the color components
            return Color.FromArgb(alpha, red, green, blue);
        }

        /// <summary>
        /// Sets the color of the pixel at the specified coordinates in the provided pixel data.
        /// </summary>
        /// <param name="pixels">The pixel data array.</param>
        /// <param name="stride">The stride of the bitmap.</param>
        /// <param name="x">The x-coordinate of the pixel.</param>
        /// <param name="y">The y-coordinate of the pixel.</param>
        /// <param name="width">The width of the bitmap.</param>
        /// <param name="color">The color to set the pixel to.</param>
        private void SetPixel(byte[] pixels, int stride, int x, int y, int width, Color color)
        {
            int index = y * stride + 4 * x;

            // Setting the color components (blue, green, red, alpha) of the pixel
            pixels[index] = color.B;
            pixels[index + 1] = color.G;
            pixels[index + 2] = color.R;
            pixels[index + 3] = color.A;
        }
    }
    #endregion

    /// <summary>
    /// Enum representing different editing modes.
    /// </summary>
    public enum EditingMode
    {
        Select, Pen, Highlighter, Eraser, Oval, Rectangle, Line, Bucket, Pick, Text, Save, Load, Clear
    }
}