using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace RotationPrototype
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Point center;

        Point size;

        bool inRotate;

        //bool canRotate;

        public MainWindow()
        {
            InitializeComponent();

            //inRotate = false;

            //size.X = this.ActualWidth;
            //size.Y = this.ActualHeight;

            //center.X = (this.ActualWidth / 2);
            //center.Y = (this.ActualHeight / 2);
        }

        private void myRect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //if (menu_rttControl.IsChecked == false)
            //    return;
            //Mouse.Capture(myRect);
            if (!inRotate)
                inRotate = true;
        }

        private void myRect_MouseMove(object sender, MouseEventArgs e)
        {
            string header = Menu_zoom.Header as string;
            myRect.ToolTip = (header == "Zoom in") ? "Double click to zoom in" : "Double click to zoom out";
            //if(Mouse.Captured == myRect)
            if (inRotate)
            {
                Point pt = Mouse.GetPosition(this);
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate {
                    
                    // Calculate an angle
                    //if(pt.X != center.X)
                    //double rads = Math.Atan((pt.Y - myRectRotate.CenterY) / (pt.X - myRectRotate.CenterX));
                    double rads = Math.Atan((pt.Y - center.Y) / (pt.X - center.X));
                    myRectRotate.Angle = rads * 180 / Math.PI;
                    
                    //// Apply a 180 degree shift when X is negative so that we can rotate
                    //// all of the way around
                    //if (pt.X - myRectRotate.CenterX < 0)
                    if (pt.X - center.X < 0)
                    {
                        myRectRotate.Angle += 180;
                    }
                }));
            }
        }

        private void myRect_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Mouse.Capture(null);
            if (inRotate)
                inRotate = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            inRotate = false;

            size.X = this.ActualWidth;
            size.Y = this.ActualHeight;
            
            center.X = (this.ActualWidth / 2);
            center.Y = (this.ActualHeight / 2);
        }

        private void myRect_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //myRectRotate.Angle = 0;
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void menuRtt_Clicked(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            if(item.Name == "Menu_origin")
                myRectRotate.Angle = 0;
            else if(item.Name == "Menu_down")
                myRectRotate.Angle = 180;
            else if (item.Name == "Menu_ninety")
                myRectRotate.Angle = 90;
            else if (item.Name == "Menu_ninetydown")
                myRectRotate.Angle = 270;
        }

        private void menuZoom_Clicked(object sender, RoutedEventArgs e)
        {
            string header = Menu_zoom.Header as string;
            Menu_zoom.Header = (header == "Zoom in") ? "Zoom out" : "Zoom in";
        }

        private void menuTsc_Clicked(object sender, RoutedEventArgs e)
        {
            string subPath = System.Environment.CurrentDirectory + "\\Screenshot";

            if (Directory.Exists(subPath) == false)
                Directory.CreateDirectory(subPath);

            DateTime dateTime = DateTime.Now;

            string date = GetDateStr(dateTime);
            string time = GetTimeStr(dateTime);

            subPath += "\\" + date;
            if (Directory.Exists(subPath) == false)
                Directory.CreateDirectory(subPath);


            string path = subPath + "\\" + date + "_" + time;
            using (FileStream fs1 = new FileStream(path, FileMode.OpenOrCreate))
            {
                try
                {
                    //RenderTargetBitmap bitmap = new RenderTargetBitmap((int)myRect.ActualWidth, (int)myRect.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                    //RenderTargetBitmap bitmap = new RenderTargetBitmap((int)myRect.ActualWidth, (int)myRect.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                    //bitmap.Render(myRect);
                    
                    RenderTargetBitmap bitmap = new RenderTargetBitmap(1920, 1080, 96, 96, PixelFormats.Pbgra32);
                    //RenderTargetBitmap bitmap = new RenderTargetBitmap(1920, 1080, 1 / 300, 1 / 300, PixelFormats.Pbgra32);
                    DrawingVisual visual = new DrawingVisual();
                    using (DrawingContext context = visual.RenderOpen())
                    {
                        VisualBrush brush = new VisualBrush(myRect);
                        context.DrawRectangle(brush,
                                              null,
                                              new Rect(new Point(), new Size(myRect.ActualWidth, myRect.ActualHeight)));
                    }

                    visual.Transform = new ScaleTransform(1920 / myRect.ActualWidth, 1080 / myRect.ActualHeight);

                    bitmap.Render(visual);

                    BitmapFrame frame = BitmapFrame.Create(bitmap);

                    JpegBitmapEncoder enc = new JpegBitmapEncoder();
                    enc.Frames.Add(frame);
                    enc.Save(fs1);
                    MessageBox.Show(path);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    //throw;
                }

            }
        }

        private String GetDateStr(DateTime dateTime)
        {
            //DateTime dateTime = DateTime.Now;
            string day = (dateTime.Day < 10) ? String.Format("0{0}", dateTime.Day) : dateTime.Day.ToString();
            string month = (dateTime.Month < 10) ? String.Format("0{0}", dateTime.Month) : dateTime.Month.ToString();
            string year = dateTime.Year.ToString();
            return String.Format("{0}{1}{2}", year, month, day);
        }

        private String GetTimeStr(DateTime dateTime)
        {
            //DateTime dateTime = DateTime.Now;
            string hour = (dateTime.Hour < 10) ? String.Format("0{0}", dateTime.Hour) : dateTime.Hour.ToString();
            string minute = (dateTime.Minute < 10) ? String.Format("0{0}", dateTime.Minute) : dateTime.Minute.ToString();
            string second = (dateTime.Second < 10) ? String.Format("0{0}", dateTime.Second) : dateTime.Second.ToString();
            return String.Format("{0}{1}{2}.jpg", hour, minute, second);
        }

        private void menuTsc2_Clicked(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "스크린샷"; //default file name
            dlg.DefaultExt = ".jpg"; //default file extension
            dlg.Filter = "JPG 이미지|*.jpg|PNG 이미지|*.png|Bitmap 이미지|*.bmp"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string path = dlg.FileName;

                using (FileStream fs1 = new FileStream(path, FileMode.OpenOrCreate))
                {
                    try
                    {
                        //RenderTargetBitmap bitmap = new RenderTargetBitmap((int)myRect.ActualWidth, (int)myRect.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                        //RenderTargetBitmap bitmap = new RenderTargetBitmap((int)myRect.ActualWidth, (int)myRect.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                        //bitmap.Render(myRect);

                        RenderTargetBitmap bitmap = new RenderTargetBitmap(1920, 1080, 96, 96, PixelFormats.Pbgra32);
                        //RenderTargetBitmap bitmap = new RenderTargetBitmap(1920, 1080, 1 / 300, 1 / 300, PixelFormats.Pbgra32);
                        DrawingVisual visual = new DrawingVisual();
                        using (DrawingContext context = visual.RenderOpen())
                        {
                            VisualBrush brush = new VisualBrush(myRect);
                            context.DrawRectangle(brush,
                                                  null,
                                                  new Rect(new Point(), new Size(myRect.ActualWidth, myRect.ActualHeight)));
                        }

                        visual.Transform = new ScaleTransform(1920 / myRect.ActualWidth, 1080 / myRect.ActualHeight);

                        bitmap.Render(visual);

                        BitmapFrame frame = BitmapFrame.Create(bitmap);

                        JpegBitmapEncoder enc = new JpegBitmapEncoder();
                        enc.Frames.Add(frame);
                        enc.Save(fs1);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        //throw;
                    }

                }
            }

        }
    }
}
