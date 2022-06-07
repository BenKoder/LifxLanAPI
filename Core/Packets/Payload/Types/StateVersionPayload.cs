using Core.LifxProducts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Packets.Payload.Types
{
    public class StateVersionPayload : PayloadBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="header"></param>
        /// <param name="data"></param>
        protected override bool DecodePacket(PacketHeader header, byte[] data)
        {
            this.PayloadType = MessageType.StateVersion;

            // a StateVersion should contain three bits of information
            // Vendor (4 byte)
            // Product (4 bytes)
            // ReservedOne (4 bytes)
            int PayLoadSize = PayLoadTypes.StateVersion.Size;

            // check to see if we have the correct number of bytes
            if (data.Length != PacketHeader.PacketHeaderSize + PayLoadSize)
                return false;

            this.Vendor = BitConverter.ToInt32(data, PacketHeader.PacketHeaderSize);
            this.Product = BitConverter.ToInt32(data, PacketHeader.PacketHeaderSize + 4);
            this.ReservedOne = BitConverter.ToInt32(data, PacketHeader.PacketHeaderSize + 8);

            // get a friendly name of the product we have found.
            // set to ProductType.Unknown if we don't know what it is
            if (Enum.IsDefined(typeof(ProductType), this.Product) == true)
                this.ProductEnumType = (ProductType)this.Product;
            else
                this.ProductEnumType = ProductType.Unknown;

            return true;
        }

        /// <summary>
        /// For LIFX products this value is 1
        /// </summary>
        public Int32 Vendor { get; private set; }

        /// <summary>
        /// The product id of the device
        /// </summary>
        public Int32 Product { get; private set;}
        /// <summary>
        /// Same as <see cref="Product"/> but displayed as a friendly readable Enum.
        /// All product types can be found at https://lan.developer.lifx.com/docs/product-registry
        /// </summary>
        public ProductType ProductEnumType { get; private set; }

        public Int32 ReservedOne { get; private set; }
    }
}
