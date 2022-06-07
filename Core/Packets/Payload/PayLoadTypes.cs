using System;

namespace Core.Packets.Payload
{
    public class PayLoadTypes
    {
        static PayLoadTypes()
        {
            
            PayLoadTypes.GetService = new PayLoadType(0, MessageType.GetService);
            PayLoadTypes.StateService = new PayLoadType(5, MessageType.StateService);

            PayLoadTypes.SetPower = new PayLoadType(2, MessageType.SetPower);

            PayLoadTypes.GetLabel = new PayLoadType(0, MessageType.GetLabel);
            PayLoadTypes.StateLabel = new PayLoadType(32, MessageType.StateLabel);




            PayLoadTypes.GetVersion = new PayLoadType(0, MessageType.GetVersion);
            PayLoadTypes.StateVersion = new PayLoadType(12, MessageType.StateVersion);

            PayLoadTypes.GetColor = new PayLoadType(0, MessageType.GetColor);
            PayLoadTypes.SetColor = new PayLoadType(13, MessageType.SetColor);

            PayLoadTypes.LightState = new PayLoadType(52, MessageType.LightState);
        }
        
        public static PayLoadType GetService { get;private set; }
        public static PayLoadType StateService { get;private set; }
        
        
        public static PayLoadType SetPower { get; private set; }


        public static PayLoadType GetLabel { get; private set; }
        public static PayLoadType StateLabel { get; private set; }

        public static PayLoadType GetVersion { get; private set; }
        public static PayLoadType StateVersion { get; private set; }


        public static PayLoadType GetColor { get; private set; }
        public static PayLoadType SetColor { get; private set; }


        public static PayLoadType LightState { get; private set; }


    }

    public class PayLoadType
    {
        public PayLoadType(Int16 PayloadSize, MessageType messsageType)
        {
            
            this.Size = PayloadSize;
            
            this.MesssageType = messsageType;

            this.Identifier = (Int16)messsageType;
        }

        /// <summary>
        /// Number of bytes this packet takes up
        /// </summary>
        public Int16 Size { get; private set; }
        /// <summary>
        /// The number assoshiated to this payload. e.g. if SetColor its Identifier would be 102
        /// </summary>
        public Int16 Identifier { get; private set; }
        /// <summary>
        /// The type of message this payload is, each payload type has its own unique number
        /// </summary>
        public MessageType MesssageType { get; private set; }
    }

    /// <summary>
    /// All the types of paypload messages we could send or recieve
    /// </summary>
    public enum MessageType
    {
        GetService = 2,
        StateService = 3,

        SetPower = 21,

        GetLabel = 23,
        StateLabel = 25,

        GetVersion = 32,
        StateVersion = 33,

        Acknowledgement = 45,

        GetColor = 101,
        SetColor = 102,

        LightState = 107
    }
}
