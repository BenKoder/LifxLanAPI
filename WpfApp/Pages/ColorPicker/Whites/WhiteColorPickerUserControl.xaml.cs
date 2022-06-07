using Core.LifxProducts.Colors;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp.Pages.ColorPicker.Whites
{
    /// <summary>
    /// Interaction logic for WhiteColorPickerUserControl.xaml
    /// </summary>
    public partial class WhiteColorPickerUserControl : UserControl
    {
        private Button _OnOffButton;
        private List<Button> _ButtonColorsList;
        public WhiteColorPickerUserControl()
        {
            this._ButtonColorsList = new List<Button>();
            InitializeComponent();

            CreateColorData();
            CreateButtonCircle();
            CreateOnOffButton();

            this.Loaded += (object sender, RoutedEventArgs e) => 
            {
                ViewModels.ColorPicker.Whites.WhitePickerPageViewModel VM = ((ViewModels.ColorPicker.Whites.WhitePickerPageViewModel)this.DataContext);
                this._OnOffButton.Command = VM.TurnLightOnOrOffCommand;

                // go through each color button around the wheel and add a command to it
                // so we can act on it when its clicked.
                foreach(Button button in this._ButtonColorsList)
                {
                    ColorData cd = (ColorData)button.Tag;

                    button.Command = VM.WhiteColorSelectedCommand;
                    button.CommandParameter = cd.LifxColor;
                }
            };


        }

       

        private void CreateButtonCircle()
        {
            double degrees = 0;

            while(degrees < 360)
            {
                // create the button at the specified angel and add it to the page.
                // also add the button to a list so we can add a command bindings to that
                // button for when its pressed
                this._ButtonColorsList.Add(CreateButton(degrees, 500, new Point(500,500)));
                degrees += 22.5;
                //degrees += 360;
            }
            

            //this.DrawPosition(new Point(500, 350),Colors.Green);
        }

        private Button CreateButton(double angel, double RadiusFromCentre, Point CentreLocation)
        {
            Button cmd = new Button();
            double x, y;
            //x = CaculateXLocation(angel, RadiusFromCentre);
            //y = CaculateYLocation(angel, RadiusFromCentre);

            //RadiusFromCentre = RadiusFromCentre - CaculateButtonSize(CentreLocation).Height;

            //Size WidthAndHeightOfButton = this.CaculateWidthAndHeightOfImageAtCurrentAngel(angel, new Size(280.526, 223.867));

            Point location, newLocation;
            
            location = CaculatePoint(RadiusFromCentre,angel, CentreLocation);
            //newLocation = CaculatePointAtCentreOfImage(angel, WidthAndHeightOfButton, location);
            cmd.Margin = new Thickness(location.X, location.Y, 0, 0);

            cmd.Style = this.FindResource("button-light-color-style") as Style;
            cmd.Width = 280.526;
            cmd.Height = 223.867;
            //cmd.Width = CaculateButtonSize(CentreLocation).Width;
            //cmd.Height = CaculateButtonSize(CentreLocation).Height;

            cmd.RenderTransformOrigin = new Point(0.5, 0.5);
            cmd.Background = new SolidColorBrush(GetColorData(angel).ButtonColor);
            cmd.BorderBrush = Brushes.Transparent;
            cmd.Tag = GetColorData(angel);
            cmd.Click += Cmd_Click;

            cmd.HorizontalAlignment = HorizontalAlignment.Left;
            cmd.VerticalAlignment = VerticalAlignment.Top;

            cmd.RenderTransform = new RotateTransform(angel);
            //cmd.LayoutTransform = new RotateTransform(angel, WidthAndHeightOfButton.Width, WidthAndHeightOfButton.Height);

            cmd.Opacity = 0.5;



            this.GridContainer.Children.Add(cmd);
            if (angel == 0)
                this._SelectedButton = cmd;

            //this.DrawPosition(location,Colors.Pink);

            return cmd;


        }

        private Size CaculateButtonSize(Point CenterLocation)
        {
            double ButtonWidthAndHeightRatio = 1.253092238;

            double ButtonHeight = CenterLocation.Y / 4;
            double ButtonWidth = ButtonHeight * ButtonWidthAndHeightRatio;

            return new Size(ButtonWidth,ButtonHeight);
        }

        private Button _SelectedButton;
        private void Cmd_Click(object sender, RoutedEventArgs e)
        {
            
            Button ClickedButton = (Button)sender;
            /*
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.To = ((ColorData)ClickedButton.Tag).Angel;
            doubleAnimation.From = this._CurrentSelectedColor.Angel;
            doubleAnimation.Duration = new Duration(new TimeSpan(0, 0, 0, 1));

            
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(doubleAnimation);
            Storyboard.SetTarget(doubleAnimation, this.GridContainer);
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("LayoutTransform.Angle"));
            //Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(Grid.LayoutTransform).(RotateTransform.Angle)"));

            this._CurrentSelectedColor = (ColorData)ClickedButton.Tag;
            storyboard.Begin();
            */
            _SelectedButton.BorderBrush = Brushes.Transparent;
            ClickedButton.BorderBrush = new SolidColorBrush(Colors.DeepSkyBlue);
            _SelectedButton = ClickedButton;

            
        }

        private double CaculateXLocation(int angle, int RadiusFromCentre)
        {
            return RadiusFromCentre * Math.Cos(angle);
        }

        private double CaculateYLocation(int angle, int RadiusFromCentre)
        {
            return RadiusFromCentre * Math.Sin(angle);
        }

        public Point CaculatePoint(double radius, double angle, Point CentrePoint)
        {
            double x = (double)(radius * Math.Cos(angle * Math.PI / 180)) + CentrePoint.X;
            double y = (double)(radius * Math.Sin(angle * Math.PI / 180)) + CentrePoint.Y;

            return new Point(x, y);
        }


        private void DrawPosition(Point position, Color color)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Width = 680;
            ellipse.Height = 680;
            //ellipse.Margin = new Thickness(position.X, position.Y, 0, 0);
            ellipse.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#3c4654");
            ellipse.HorizontalAlignment = HorizontalAlignment.Center;
            ellipse.VerticalAlignment = VerticalAlignment.Center;

            GridContainer.Children.Add(ellipse);
        }

        private void CreateOnOffButton()
        {
            /*
            Ellipse ellipse = new Ellipse();
            ellipse.Width = 680;
            ellipse.Height = 680;
            //ellipse.Margin = new Thickness(position.X, position.Y, 0, 0);
            ellipse.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#3c4654");
            ellipse.HorizontalAlignment = HorizontalAlignment.Center;
            ellipse.VerticalAlignment = VerticalAlignment.Center;
            */

            Button ButtonOnOff = new Button();
            ButtonOnOff.Style = this.FindResource("button-on-off-style") as Style;
            ButtonOnOff.Width = 680;
            ButtonOnOff.Height = 680;
            ButtonOnOff.HorizontalAlignment = HorizontalAlignment.Center;
            ButtonOnOff.VerticalAlignment = VerticalAlignment.Center;

            ButtonOnOff.Opacity = 0.5;
            
            // keep a reference to the button so we can add a command binding to the button when the page has loaded.
            this._OnOffButton = ButtonOnOff;
            GridContainer.Children.Add(ButtonOnOff);
        }


        private Size CaculateWidthAndHeightOfImageAtCurrentAngel(double CurrentAngle, Size SizeAtZeroDegrees)
        {
            Size SizeAtAngle = new Size();
            
            if(CurrentAngle == 0)
            {
                SizeAtAngle = SizeAtZeroDegrees;
            }
            else if(CurrentAngle < 90)
            {
                SizeAtAngle.Width = Math.Abs((SizeAtZeroDegrees.Width * Math.Cos(CurrentAngle)) + (SizeAtZeroDegrees.Height * Math.Sin(CurrentAngle)));
                SizeAtAngle.Height= Math.Abs((SizeAtZeroDegrees.Width * Math.Sin(CurrentAngle)) + (SizeAtZeroDegrees.Height * Math.Cos(CurrentAngle)));
            }
            else if(CurrentAngle == 90)
            {
                SizeAtAngle.Width = SizeAtZeroDegrees.Height;
                SizeAtAngle.Height = SizeAtZeroDegrees.Width;
            }
            else if(CurrentAngle > 90)
            {
                SizeAtAngle.Width = Math.Abs((SizeAtZeroDegrees.Height * Math.Cos(CurrentAngle)) + (SizeAtZeroDegrees.Width * Math.Sin(CurrentAngle)));
                SizeAtAngle.Height = Math.Abs((SizeAtZeroDegrees.Height * Math.Sin(CurrentAngle)) + (SizeAtZeroDegrees.Width * Math.Cos(CurrentAngle)));
            }


            return SizeAtAngle;
        }

        private Point CaculatePointAtCentreOfImage(double angel, Size WidthAndHeightOfImage, Point currentPosition)
        {
            /*double HalfOfWidth, HalfOfHeight;
            HalfOfHeight = WidthAndHeightOfImage.Height / 2;
            HalfOfWidth = WidthAndHeightOfImage.Width / 2;

            Point aPoint = new Point();

            if ((angel >= 0 && angel <= 90) || (angel > 270))
            {
                aPoint.X = currentPosition.X - HalfOfWidth;
                aPoint.Y = currentPosition.Y - HalfOfHeight;
            }
            else
            {
                //aPoint.X = currentPosition.X;
                //aPoint.Y = currentPosition.Y;

                aPoint.X = currentPosition.X;
                aPoint.Y = currentPosition.Y;
            }


            return aPoint;*/

            return currentPosition;
        }










        private void DrawCircleButtons()
        {
            int NumOfButtons = 12;
            double AngleBetweenEachButton = 360 / NumOfButtons;
            Point CentrePoint = new Point(GridContainer.ActualWidth / 2, GridContainer.ActualHeight / 2);
            Size SizeOfButton = new Size();

            
        }



        ColorData[] ButtonColorData = new ColorData[16];
        private ColorData _CurrentSelectedColor;
        private void CreateColorData()
        {

            ButtonColorData[0] = new ColorData(0, "#ff9e6a", 1500,"Candlelight", ColorsList.CandleLight);
            ButtonColorData[1] = new ColorData(22.5, "#ffba97", 2000,"Sunset", ColorsList.Sunset);
            ButtonColorData[2] = new ColorData(45, "#ffd1b8", 2500,"Ultra Warm", ColorsList.UltraWarm);
            ButtonColorData[3] = new ColorData(67.5, "#ffd8c3", 2700, "Incandescent", ColorsList.Incandescent);
            ButtonColorData[4] = new ColorData(90, "#ffe3d1", 3000,"Warm", ColorsList.Warm);
            ButtonColorData[5] = new ColorData(112.5, "#fff2e6", 3500,"Neutral", ColorsList.Neutral);
            ButtonColorData[6] = new ColorData(135, "#ffffff", 4000,"cool", ColorsList.Cool);
            ButtonColorData[7] = new ColorData(157.5, "#fffdff", 4500,"Cool Daylight", ColorsList.CoolDaylight);
            ButtonColorData[8] = new ColorData(180, "#fffcff", 5000,"Soft Daylight", ColorsList.SoftDaylight);
            ButtonColorData[9] = new ColorData(202.5, "#faf5ff", 5600,"Daylight", ColorsList.Daylight);
            ButtonColorData[10] = new ColorData(247.5, "#f5f2ff", 6000, "Noon Daylight", ColorsList.NoonDaylight);
            ButtonColorData[11] = new ColorData(247.5, "#efeeff", 6500, "Bright Daylight", ColorsList.BrightDaylight);
            ButtonColorData[12] = new ColorData(270, "#ebecff", 7000, "Cloudy Daylight", ColorsList.CloudyDaylight);
            ButtonColorData[13] = new ColorData(292.5, "#e7eaff", 7500, "Blue Daylight", ColorsList.BlueDaylight);
            ButtonColorData[14] = new ColorData(315, "#e4e8ff", 8000, "Blue Overcast", ColorsList.BlueOvercast);
            ButtonColorData[15] = new ColorData(337.5, "#dfe5ff", 9000, "Blue Ice", ColorsList.BlueIce);

            this._CurrentSelectedColor = ButtonColorData[0];
        }

        
        private ColorData GetColorData(double angel)
        {
            ColorData colorData;
            switch(angel)
            {
                case 0:
                    colorData = this.ButtonColorData[0];
                    break;

                case 22.5:
                    colorData =  this.ButtonColorData[1];
                    break;

                case 45:
                    colorData = this.ButtonColorData[2];
                    break;

                case 67.5:
                    colorData = this.ButtonColorData[3];
                    break;

                case 90:
                    colorData = this.ButtonColorData[4];
                    break;

                case 112.5:
                    colorData = this.ButtonColorData[5];
                    break;

                case 135:
                    colorData = this.ButtonColorData[6];
                    break;

                case 157.5:
                    colorData = this.ButtonColorData[7];
                    break;

                case 180:
                    colorData = this.ButtonColorData[8];
                    break;

                case 202.5:
                    colorData = this.ButtonColorData[9];
                    break;

                case 225:
                    colorData = this.ButtonColorData[10];
                    break;

                case 247.5:
                    colorData = this.ButtonColorData[11];
                    break;

                case 270:
                    colorData = this.ButtonColorData[12];
                    break;

                case 292.5:
                    colorData = this.ButtonColorData[13];
                    break;

                case 315:
                    colorData = this.ButtonColorData[14];
                    break;

                case 337.5:
                    colorData = this.ButtonColorData[15];
                    break;


                default:
                    colorData = this.ButtonColorData[0];
                    break;
            }

            return colorData;
        }




    }


    public class ColorData
    {
        public ColorData(double angel, string buttonColorInHex, int ColorInKelvins, string friendlyName, LifxColor lifxColor)
        {
            BrushConverter converter = new BrushConverter();

            this.Angel = angel;
            this.ButtonColor = ((SolidColorBrush)converter.ConvertFromString(buttonColorInHex)).Color;
            this.ColorInKelvins = ColorInKelvins;
            this.Name = friendlyName;
            this.LifxColor = lifxColor;
        }
        public double Angel { get; set; }
        public Color ButtonColor { get; set; }
        public int ColorInKelvins { get; set; }
        public LifxColor LifxColor { get; set; }

        /// <summary>
        /// Friendly name given to the color
        /// </summary>
        public string Name { get; set; }
    }
}
