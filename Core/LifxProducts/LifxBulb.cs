using Core.LifxProducts.Colors;
using Core.LifxProducts.HelperClasses;
using Core.Messages;
using Core.Packets;
using Core.Packets.Payload.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.LifxProducts
{
    public class LifxBulb : ProductBase
    {
        public LifxBulb(string MacAddress, int Port, string IpAddress, ProductType productType,
                        bool HasFullColor, bool HasNightVision, KelvinRange kelvinRange) 
                    : base(MacAddress,Port,IpAddress, productType)
        {
            this.HasFullColor = HasFullColor;
            this.HasNightVision = HasNightVision;
            this.KelvinRangeSupported = kelvinRange;

            this.BulbStatus = new BulbStatus();
        }

        public ProductType ProductType { get; protected set; }
        public bool HasFullColor { get; protected set; }
        public bool HasNightVision { get; set; }
        public KelvinRange KelvinRangeSupported { get; set; }

        /// <summary>
        /// Current status of the bulb
        /// </summary>
        public BulbStatus BulbStatus { get; protected set; }


        public override void ProcessMessage(PayloadBase MessagePayload)
        {
            switch (MessagePayload.PayloadType)
            {
                // we have recieved a light state message (light color, brightness, etc)
                case Packets.Payload.MessageType.LightState:
                    this.UpdateBulbStatus((LightStatePayload)MessagePayload);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Gets called when we recieve an Acknowledgement message. Normaly this is
        /// because we sent a set message. See <see cref="LastPayloadMessageThatWasSent"/> to see what the message we sent was
        /// </summary>
        public override void AcknowledgementReceived(LifxPacket lifxPacket)
        {
            switch (this.LastLifxPacketMessageThatWasSent.HeaderPacket.PayloadType)
            {
                // our last message was a color change requst to the bulb.
                // the bulb has responded 
                case Packets.Payload.MessageType.SetColor:
                    // get the color payload packet
                    SetColorPayload colorPayload = (SetColorPayload)this.LastLifxPacketMessageThatWasSent.PayloadPacket;
                    // create a lifxColor from the color we sent in the last message
                    LifxColor color = new LifxColor(colorPayload.Hue, colorPayload.Saturation, colorPayload.Brightness, colorPayload.Kelvin);
                    // update the bulb information to the color we asked to be changed too
                    this.UpdateBulbColor(color);
                    break;

                // our last message was to tell the light to turn on or off
                case Packets.Payload.MessageType.SetPower:
                    // if the power level was zero. It means we wanted
                    // to turn the light on
                    if (this.PowerLevel < 1)
                        this.PowerLevel = UInt16.MaxValue;
                    // if the power level was greater than zero, it means we wanted to turn the power off
                    else
                        this.PowerLevel = 0;
                    break;

                default:
                    break;
            }
        }
        /// <summary>
        /// This is normaly because of a message we recieved from the bulb with new information about the bulbs state
        /// update this blubs refernce to color, brightness power level, label, based on the passed in data
        /// </summary>
        /// <param name="lightState">state to set the light too</param>
        private void UpdateBulbStatus(LightStatePayload lightState)
        {
            this.BulbStatus.CurrentColor.Kelvin = lightState.Kelvin;
            this.BulbStatus.CurrentColor.Hue = LifxColor.ConvertHueFromLifexEquiverlent(lightState.Hue);
            this.BulbStatus.CurrentColor.Saturation = LifxColor.ConvertSaturationOrBrightnessFromLifxEquiverlent(lightState.Saturation);
            this.BulbStatus.CurrentColor.Brightness = LifxColor.ConvertSaturationOrBrightnessFromLifxEquiverlent(lightState.Brightness);

            this.PowerLevel = lightState.Power;

            this.Label = lightState.Label;
        }

        private void UpdateBulbColor(LifxColor color)
        {
            this.BulbStatus.CurrentColor.Kelvin = color.Kelvin;
            this.BulbStatus.CurrentColor.Hue = color.Hue;
            this.BulbStatus.CurrentColor.Saturation = color.Saturation;
            this.BulbStatus.CurrentColor.Brightness = color.Brightness;
        }

        /// <summary>
        /// Sends a GetColorMessage to the bulb asking it what its current color is
        /// </summary>
        public void RequestBulbColor()
        {
            LifxPacket lifxPacket;
            MessageCreator messageCreator = new MessageCreator();
            
            lifxPacket = messageCreator.CreateMessage_GetColor(this.MacAddress, ++this.MessageCount);
            this.CallRaiseMessageEvent(this, lifxPacket);
        }

        public void SetColor(Colors.LifxColor lifxColor)
        {
            LifxPacket lifxPacket;
            MessageCreator messageCreator = new MessageCreator();
            

            // make sure the kelvin value is within the range of what the bulb supports
            if (lifxColor.Kelvin > this.KelvinRangeSupported.Too)
                lifxColor.Kelvin = this.KelvinRangeSupported.Too;
            else if(lifxColor.Kelvin < this.KelvinRangeSupported.From)
                lifxColor.Kelvin = this.KelvinRangeSupported.From;

            lifxPacket = messageCreator.CreateMessage_SetColor(this.MacAddress, ++this.MessageCount, lifxColor);
            this.CallRaiseMessageEvent(this,lifxPacket);
        }
        
    }
}
