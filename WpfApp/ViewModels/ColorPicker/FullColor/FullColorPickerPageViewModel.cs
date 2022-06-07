using Core.LifxProducts;
using Core.LifxProducts.Colors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace WpfApp.ViewModels.ColorPicker.FullColor
{
    public class FullColorPickerPageViewModel : BaseViewModel
    {
        private LifxBulb _LifxBlub;
        public FullColorPickerPageViewModel()
        {
            this._LifxBlub = ApplicationViewModel.InstanceAppViewModelInstance.SelectedProduct as LifxBulb;

            this.IsLightOn = this._LifxBlub.IsPowerdOn;
        }

        public void ChangeBulbColor(int hue, int saturation)
        {
            LifxColor color = new LifxColor((ushort)hue, (ushort)saturation, this._LifxBlub.BulbStatus.CurrentColor.Kelvin, this._LifxBlub.BulbStatus.CurrentColor.Brightness);
            this._LifxBlub.SetColor(color);
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
                                        if (this._IsLightOn != this._LifxBlub.IsPowerdOn)
                                        {
                                            this._IsLightOn = this._LifxBlub.IsPowerdOn;
                                            // fire event to say we have changed the light on or off
                                            this.OnPropertyChanged(nameof(this.IsLightOn));
                                        }

                                    },
                                    Dispatcher.CurrentDispatcher).Start();
            }
        }
    }
}
