﻿using Core.Messages;
using Core.Packets;
using Core.Packets.Payload.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.LifxProducts
{
    public class ProductBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        private string _Label = string.Empty;
        /// <summary>
        /// The name the user has assgined to this product
        /// </summary>
        public string Label 
        { 
            get => this._Label;
            set
            {
                this._Label = value;
                this.OnPropertyChanged(this,nameof(this.Label));
            }
        }
        /// <summary>
        /// True when <see cref="PowerLevel"/> is greater than zero, else zero
        /// </summary>
        public bool IsPowerdOn { get; private set; }

        private UInt16 _PowerLevel = 0;
        /// <summary>
        /// The current power level of the device. 0 means off and any other value means on. 
        /// Note that 65535 is full power and during a power transition (i.e. via SetLightPower (117)) 
        /// the value may be any value between 0 and 65535
        /// </summary>
        public UInt16 PowerLevel 
        {
            get => _PowerLevel;
            set
            {
                _PowerLevel = value;
                if (_PowerLevel > 0)
                    IsPowerdOn = true;
                else
                    IsPowerdOn = false;
            }

        }
        /// <summary>
        /// The MAC address of the device
        /// </summary>
        public string MacAddress { get; set; }
        /// <summary>
        /// Not currently used. Would be the port the product is using but at present all use the same port so this has
        /// been hard coded.
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// The IP address of the product on the local networok
        /// </summary>
        public string IpAddresss { get; set; }

        /// <summary>
        /// The exact product we are looking at (see https://lan.developer.lifx.com/docs/product-registry)
        /// </summary>
        public ProductType ProductType { get; set; }

        private int _MessageCount;
        /// <summary>
        /// Should be a number between 1 and 255.
        /// If number passed in is greator than 255, <see cref="MessageCount"/>
        /// will be set back to 1
        /// </summary>
        public int MessageCount
        {
            get => this._MessageCount;
            set
            {
                // dont' allow the number to be less than 0 or greater than 255.
                // set back to 1 if it is
                if (value < 0 || value > 255)
                    this._MessageCount = 1;
                else
                    this._MessageCount = value;
            }
        }

        /// <summary>
        /// Everytime a message is sent to this bulb, the date and time should be set
        /// </summary>
        public DateTime LastMessageSentAt { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Default is 50 milliseconds. No message should be sent if the previouse message  (<see cref="LastMessageSentAt"/>)
        /// was sent less than <see cref="TimeToAllowBetweenSendingMessagesInMilliSeconds"/>
        /// </summary>
        public int TimeToAllowBetweenSendingMessagesInMilliSeconds { get; set; } = 50;


        /// <summary>
        /// The last message that was sent across the network from this application to the device
        /// </summary>
        public LifxPacket LastLifxPacketMessageThatWasSent { get; set; } = null;

        public ProductBase(string MacAddress, int Port, string IpAddress, ProductType ProductType)
        {
            this.MacAddress = MacAddress;
            this.Port = Port;
            this.IpAddresss = IpAddress;
            this.ProductType = ProductType;

        }

        /// <summary>
        /// Child class will overwride this method and see what the message is and act apropriatly
        /// </summary>
        /// <param name="MessagePayload">The payload we have been sent from a lifx product</param>
        public virtual void ProcessMessage(PayloadBase MessagePayload)
        {

        }

        /// <summary>
        /// Gets called when we recieve an acknowledgent message from a lifx product.
        /// This is normaly because we just sent a set message of some kind
        /// The child class that inherits this class should overload this method
        /// </summary>
        public virtual void AcknowledgementReceived(Packets.LifxPacket lifxPacket)
        {

        }


        /// <summary>
        /// Passing true will turn the light on, passing false will turn the light off.
        /// </summary>
        /// <param name="OnOrOff">true turns light on, false turns light off</param>
        public void TurnLightOnOrOff(bool OnOrOff)
        {
            // if we want the bulb off, set to zero, if we want the bulb on, set to 65535
            UInt16 thePowerLevel = OnOrOff == true ? (UInt16)65535 : (UInt16)0;
            
            // create the packet we will be sending accross the networok
            MessageCreator messageCreator = new MessageCreator();
            LifxPacket packet = messageCreator.CreateMessage_SetPower(this.MacAddress, ++this.MessageCount, thePowerLevel);

            // raise an event to get the packet sent
            this.CallRaiseMessageEvent(this, packet);
        }


        public delegate void RaiseMessageDelegate(ProductBase product, LifxPacket lifxPacket);
        /// <summary>
        ///  Raise this event when you want to send a message to this product
        /// </summary>
        public event RaiseMessageDelegate RaiseMessage = delegate { };
        


        /// <summary>
        /// Allows child classes that inherit this class to call the <see cref="RaiseMessage"/> event
        /// Will only allow event to be raised if the previouse message was in the past bt more than <see cref="TimeToAllowBetweenSendingMessagesInMilliSeconds"/> milliseconds
        /// </summary>
        /// <param name="product"></param>
        /// <param name="lifxPacket"></param>
        /// <returns>true if allows event to be called, else false</returns>
        protected bool CallRaiseMessageEvent(ProductBase product, LifxPacket lifxPacket)
        {
            // check we are not trying to send a message too soon.
            if (DateTime.Now.Subtract(LastMessageSentAt).TotalMilliseconds < this.TimeToAllowBetweenSendingMessagesInMilliSeconds)
                return false;

            this.LastLifxPacketMessageThatWasSent = lifxPacket;
            this.RaiseMessage(product, lifxPacket);
            return true;
        }


        protected void OnPropertyChanged(object caller, string PropertyName)
        {
            PropertyChanged(caller, new PropertyChangedEventArgs(PropertyName));
        }
    }

    public enum ProductType
    {
        LIFX_Original_1000_1 = 1,
        LIFX_Color_650_3 = 3,
        LIFX_White_800_Low_Voltage_10 = 10,
        LIFX_White_800_High_Voltage_11 = 11,
        LIFX_Color_1000_15 = 15,
        LIFX_White_900_BR30_Low_Voltage_18 = 18,
        LIFX_White_900_BR30_High_Voltage_19 = 19,
        LIFX_Color_1000_BR30_20 = 20,
        LIFX_Color_1000_22 = 22,
        LIFX_A19_27 = 27,
        LIFX_BR30_28 = 28,
        LIFX_A19_Night_Vision_29 = 29,
        LIFX_BR30_Night_Vision_30 = 30,
        LIFX_Z_31 = 31,
        LIFX_Z_32 = 32,
        LIFX_Downlight_36 = 36,
        LIFX_Downlight_37 = 37,
        LIFX_Beam_38 = 38,
        LIFX_Downlight_White_to_Warm_39 = 39,
        LIFX_Downlight_40 = 40,
        LIFX_A19_43 = 43,
        LIFX_BR30_44 = 44,
        LIFX_A19_Night_Vision_45 = 45,
        LIFX_BR30_Night_Vision_46 = 46,
        LIFX_Mini_Color_49 = 49,
        LIFX_Mini_White_to_Warm_50 = 50,
        LIFX_Mini_White_51 = 51,
        LIFX_GU10_52 = 52,
        LIFX_GU10_53 = 53,
        LIFX_Tile_55 = 55,
        LIFX_Candle_57 = 57,
        LIFX_Mini_Color_59 = 59,
        LIFX_Mini_White_to_Warm_60 = 60,
        LIFX_Mini_White_61 = 61,
        LIFX_A19_62 = 62,
        LIFX_BR30_63 = 63,
        LIFX_A19_Night_Vision_64 = 64,
        LIFX_BR30_Night_Vision_65 = 65,
        LIFX_Mini_White_66 = 66,
        LIFX_Candle_68 = 68,
        LIFX_Candle_White_to_Warm_81 = 81,
        LIFX_Filament_Clear_82 = 82,
        LIFX_Filament_Amber_85 = 85,
        LIFX_Mini_White_87 = 87,
        LIFX_Mini_White_88 = 88,
        LIFX_Switch_89 = 89,
        LIFX_Clean_90 = 90,
        LIFX_Color_91 = 91,
        LIFX_Color_92 = 92,
        LIFX_A19_US_93 = 93,
        LIFX_BR30_94 = 94,
        LIFX_Candle_White_to_Warm_96 = 96,
        LIFX_A19_97 = 97,
        LIFX_BR30_98 = 98,
        LIFX_Clean_99 = 99,
        LIFX_Filament_Clear_100 = 100,
        LIFX_Filament_Amber_101 = 101,
        LIFX_A19_Night_Vision_109 = 109,
        LIFX_BR30_Night_Vision_110 = 110,
        LIFX_A19_Night_Vision_111 = 111,
        LIFX_BR30_Night_Vision_112 = 112,
        LIFX_Mini_WW_113 = 113,
        LIFX_Mini_WW_114 = 114,
        LIFX_Z_117 = 117,
        LIFX_Z_118 = 118,
        LIFX_Beam_119 = 119,
        LIFX_Beam_120 = 120,
        LIFX_Downlight_121 = 121,
        LIFX_Downlight_122 = 122,
        LIFX_Color_123 = 123,
        LIFX_Color_124 = 124,
        LIFX_White_to_Warm_125 = 125,
        LIFX_White_to_Warm_126 = 126,
        LIFX_White_127 = 127,
        LIFX_White_128 = 128,
        LIFX_Color_129 = 129,
        LIFX_Color_130 = 130,
        LIFX_White_to_Warm_131 = 131,
        LIFX_White_to_Warm_132 = 132,
        LIFX_White_133 = 133,
        LIFX_White_134 = 134,
        LIFX_GU10_Color_135 = 135,
        LIFX_GU10_Color_136 = 136,
        LIFX_Candle_Color_137 = 137,
        LIFX_Candle_Color_138 = 138,
        LIFX_Neon_141 = 141,
        LIFX_Neon_142 = 142,
        LIFX_String_143 = 143,
        LIFX_String_144 = 144,

        Unknown = 9999
    }
}
