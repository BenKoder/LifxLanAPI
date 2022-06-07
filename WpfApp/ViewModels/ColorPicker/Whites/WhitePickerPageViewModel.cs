using Core.LifxProducts;
using Core.LifxProducts.Colors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WpfApp.ViewModels.Base;

namespace WpfApp.ViewModels.ColorPicker.Whites
{
    public class WhitePickerPageViewModel : BaseViewModel
    {
        /// <summary>
        /// Differnt shades of white
        /// </summary>
        private ColorData[] ButtonColorData = new ColorData[16];
        private LifxBulb _LifxBlub;



        public ICommand TurnLightOnOrOffCommand;
        public ICommand WhiteColorSelectedCommand;

        public WhitePickerPageViewModel()
        {
            this.TurnLightOnOrOffCommand = new RelayCommand(() => 
            {
                // if the light is on, try and turn it off.
                // if the lgiht is off, try and turn it on.
                this.IsLightOn = !this.IsLightOn;
            });

            this.WhiteColorSelectedCommand = new RelayParameterizedCommand((lifxColor) => 
            {

                this._LifxBlub.SetColor((LifxColor)lifxColor);
            });

            this._LifxBlub = ApplicationViewModel.InstanceAppViewModelInstance.SelectedProduct as LifxBulb;

            this.IsLightOn = this._LifxBlub.IsPowerdOn;



            // Create all the colors for the white color wheel
            this.CreateColorData();
        }


        private bool _IsLightOn = false;
        /// <summary>
        /// True if light is on, else false
        /// </summary>
        public bool IsLightOn
        {
            get => this._IsLightOn;
            set
            {
                // no need to update the value if the value has not changed.
                if (this._IsLightOn == value)
                    return;
                
                this._IsLightOn = value;
                // send a request to turn the light on or off
                this._LifxBlub.TurnLightOnOrOff(this._IsLightOn);
                // fire event to say we have changed the light on or off
                this.OnPropertyChanged(nameof(this.IsLightOn));

                // Check to see if the light was turned on or off.
                // Although we requested the light to be turned on/off, we don't acuatly
                // know if it was turned on/off. So we will wait 2 seconds and then
                // recheck the lifxbulb and see if the request sucseeded.
                new DispatcherTimer(new TimeSpan(0, 0, 0, 2), 
                                    DispatcherPriority.Background,
                                    (object sender, EventArgs e) => 
                                    {
                                        ((DispatcherTimer)sender).Stop();
                                        if(this._IsLightOn != this._LifxBlub.IsPowerdOn)
                                        {
                                            this._IsLightOn = this._LifxBlub.IsPowerdOn;
                                            // fire event to say we have changed the light on or off
                                            this.OnPropertyChanged(nameof(this.IsLightOn));
                                        }
                                        
                                    }, 
                                    Dispatcher.CurrentDispatcher).Start();
            }
        }

        private ColorData _CurrentColor;
        /// <summary>
        /// Currentselected Color on the color wheel
        /// </summary>
        public ColorData CurrentColor
        {
            get => this._CurrentColor;
            set
            {
                this._CurrentColor = value;
            }
        }


        #region private methods

        
        /// <summary>
        /// Create all color data needed for the color wheel
        /// </summary>
        private void CreateColorData()
        {

            ButtonColorData[0] = new ColorData(0, "#ff9e6a", 1500,"Candlelight");
            ButtonColorData[1] = new ColorData(22.5, "#ffba97", 2000,"Sunset");
            ButtonColorData[2] = new ColorData(45, "#ffd1b8", 2500,"Ultra Warm");
            ButtonColorData[3] = new ColorData(67.5, "#ffd8c3", 2700, "Incandescent");
            ButtonColorData[4] = new ColorData(90, "#ffe3d1", 3000,"Warm");
            ButtonColorData[5] = new ColorData(112.5, "#fff2e6", 3500,"Neutral");
            ButtonColorData[6] = new ColorData(135, "#ffffff", 4000,"cool");
            ButtonColorData[7] = new ColorData(157.5, "#fffdff", 4500,"Cool Daylight");
            ButtonColorData[8] = new ColorData(180, "#fffcff", 5000,"Soft Daylight");
            ButtonColorData[9] = new ColorData(202.5, "#faf5ff", 5600,"Daylight");
            ButtonColorData[10] = new ColorData(247.5, "#f5f2ff", 6000, "Noon Daylight");
            ButtonColorData[11] = new ColorData(247.5, "#efeeff", 6500, "Bright Daylight");
            ButtonColorData[12] = new ColorData(270, "#ebecff", 7000, "Cloudy Daylight");
            ButtonColorData[13] = new ColorData(292.5, "#e7eaff", 7500, "Blue Daylight");
            ButtonColorData[14] = new ColorData(315, "#e4e8ff", 8000, "Blue Overcast");
            ButtonColorData[15] = new ColorData(337.5, "#dfe5ff", 9000, "Blue Ice");

            this.CurrentColor = ButtonColorData[0];
        }

        #endregion
    }



    /// <summary>
    /// Data for an individual color on the white color wheel.
    /// </summary>
    public class ColorData : BaseViewModel
    {
        public ColorData(double angel, string buttonColorInHex, int ColorInKelvins, string friendlyName)
        {
            BrushConverter converter = new BrushConverter();

            this.Angel = angel;
            this.ButtonColor = ((SolidColorBrush)converter.ConvertFromString(buttonColorInHex)).Color;
            this.ColorInKelvins = ColorInKelvins;
            this.Name = friendlyName;
        }
        private double _Angel = 0;

        /// <summary>
        /// The angel this color exists on the color wheel
        /// </summary>
        public double Angel
        {
            get => this._Angel;
            set
            {
                this._Angel = value;
                this.OnPropertyChanged(nameof(this.Angel));
            }
        }

        private Color _ButtonColor;

        /// <summary>
        /// The color this color will show to the user on the color wheel
        /// </summary>
        public Color ButtonColor 
        { 
            get => this._ButtonColor; 
            set
            {
                this._ButtonColor = value;
                this.OnPropertyChanged(nameof(this.ButtonColor));
            }

        }

        private int _ColorInKelvins;
        /// <summary>
        /// The Kelvins value the color is (used to pass to lifx bulb)
        /// </summary>
        public int ColorInKelvins 
        { 
            get => _ColorInKelvins;
            set
            {
                this._ColorInKelvins = value;
                this.OnPropertyChanged(nameof(this.ColorInKelvins));
            }
        }


        private string _Name;
        /// <summary>
        /// Friendly name given to the color
        /// </summary>
        public string Name 
        {
            get => this.Name;
            set
            {
                this._Name = value;
                this.OnPropertyChanged(nameof(this.Name));
            }

        }


    }
}
