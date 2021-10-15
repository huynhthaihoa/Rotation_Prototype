using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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

        public MainWindow()
        {
            InitializeComponent();

        }

        private void myRect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Mouse.Capture(myRect);
            if (!inRotate)
                inRotate = true;
        }

        private void myRect_MouseMove(object sender, MouseEventArgs e)
        {

            //if(Mouse.Captured == myRect)
            if(inRotate)
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
    }
}
