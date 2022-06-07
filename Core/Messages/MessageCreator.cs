using Core.LifxProducts.Colors;
using Core.Packets;
using Core.Packets.Payload.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Messages
{
    /// <summary>
    /// Creates get and set messages to send accross the network
    /// </summary>
    public class MessageCreator
    {
        /// <summary>
        /// Creates a GetSercice message which should be broadcasted on the network to find lifx products.
        /// Lifx products shoudl respond with a StateService message letting us know it exists
        /// </summary>
        /// <returns>Data to send accross the network</returns>
        public LifxPacket CreateMessage_GetService()
        {
            // create the header packet 
            PacketHeader packetHeader = new PacketHeader();
            packetHeader.EncodeGetPacket(true, Packets.Payload.PayLoadTypes.GetService, String.Empty,0);

            // add the header packet to the lifxpacket (no payload with this message, just the header to send)
            LifxPacket packet = new Packets.LifxPacket();
            packet.HeaderPacket = packetHeader;

            // create the data to send across the network
            //byte[] ByteData = packet.CombinePacketsIntoOne();

            // return the data we will send
            //return ByteData;
            return packet;
        }

        public LifxPacket CreateMessage_GetVersion(string LightID,int MessageCount)
        {
            // create the header packet 
            PacketHeader packetHeader = new PacketHeader();
            packetHeader.EncodeGetPacket(false, Packets.Payload.PayLoadTypes.GetVersion, LightID, MessageCount);

            // add the header packet to the lifxpacket (no payload with this message, just the header to send)
            LifxPacket packet = new Packets.LifxPacket();
            packet.HeaderPacket = packetHeader;

            // create the data to send across the network
            byte[] ByteData = packet.CombinePacketsIntoOne();

            // return the data we will send
            return packet;
        }

        /// <summary>
        /// Creates a GetColor message to send to a lifx product on the network
        /// </summary>
        /// <param name="LightID">MAC address of the light</param>
        /// <param name="MessageCount">Message count for the light</param>
        /// <returns>>Packet ready to be converted to byte data to be sent accross the network</returns>
        public LifxPacket CreateMessage_GetColor(string LightID, int MessageCount)
        {
            // create the header packet 
            PacketHeader packetHeader = new PacketHeader();
            packetHeader.EncodeGetPacket(false, Packets.Payload.PayLoadTypes.GetColor, LightID, MessageCount);

            // add the header packet to the lifxpacket (no payload with this message, just the header to send)
            LifxPacket packet = new Packets.LifxPacket();
            packet.HeaderPacket = packetHeader;

            return packet;
        }
        /// <summary>
        /// Creates a SetColor message to send accross the network
        /// </summary>
        /// <param name="LightID">MAC address of the light</param>
        /// <param name="MessageCount">Message count for the light</param>
        /// <param name="lifxColor">The color we want the light set to</param>
        /// <returns>Packet ready to be converted to byte data to be sent accross the network</returns>
        public LifxPacket CreateMessage_SetColor(string LightID, int MessageCount, LifxColor lifxColor)
        {
            // create the header packet 
            PacketHeader packetHeader = new PacketHeader();
            packetHeader.EncodeSetPacket(false, Packets.Payload.PayLoadTypes.SetColor, LightID, MessageCount);

            SetColorPayload setColorPayload = new SetColorPayload();
            //setColorPayload.Saturation = lifxColor.Saturation;
            //setColorPayload.Hue = lifxColor.Hue;
            //setColorPayload.Brightness = lifxColor.Brightness;
            //setColorPayload.Kelvin = lifxColor.Kelvin;
            //setColorPayload.Duration = 500;

            setColorPayload.EncodePacket(lifxColor.Hue, lifxColor.Saturation, lifxColor.Brightness, lifxColor.Kelvin);

            // add the header packet to the lifxpacket (no payload with this message, just the header to send)
            LifxPacket packet = new Packets.LifxPacket();
            packet.HeaderPacket = packetHeader;
            packet.PayloadPacket = setColorPayload;

            return packet;
        }

        /// <summary>
        /// Creates a SetPower message to send accross the network to the specified product
        /// </summary>
        /// <param name="LightID">MAC address of the light</param>
        /// <param name="MessageCount">Message count for the light</param>
        /// <param name="PowerLevel">If you specify 0 the light will turn off and if you specify 65535 the device will turn on.</param>
        /// <returns></returns>
        public LifxPacket CreateMessage_SetPower(string LightID, int MessageCount, UInt16 PowerLevel)
        {
            // create the header packet 
            PacketHeader packetHeader = new PacketHeader();
            packetHeader.EncodeSetPacket(false,true,true, Packets.Payload.PayLoadTypes.SetPower, LightID, MessageCount);

            SetPowerPayload setPowerPayload = new SetPowerPayload();
            setPowerPayload.EncodePacket(PowerLevel);

            // add the header packet to the lifxpacket (no payload with this message, just the header to send)
            LifxPacket packet = new Packets.LifxPacket();
            packet.HeaderPacket = packetHeader;
            packet.PayloadPacket = setPowerPayload;

            return packet;
        }


        public LifxPacket CreateMessage_GetLabel(string LightID, int MessageCount)
        {
            // create the header packet 
            PacketHeader packetHeader = new PacketHeader();
            packetHeader.EncodeGetPacket(false, Packets.Payload.PayLoadTypes.GetLabel, LightID, MessageCount);

            // add the header packet to the lifxpacket (no payload with this message, just the header to send)
            LifxPacket packet = new Packets.LifxPacket();
            packet.HeaderPacket = packetHeader;

            return packet;
        }
    }
}
