using System.Text;

namespace Core.Helpers
{
    internal class ByteModifier
    {
        /// <summary>
        /// Find out if an individual bit is set to 0 or 1. (true or false)
        /// </summary>
        /// <param name="byteData">The 8 bit Byte to inspect</param>
        /// <param name="bitPosition">The bit position to inspect (1 to 8).</param>
        /// <returns>True: Bit set to 1; False: Bit set to 0</returns>
        public static bool IsBitSet(byte byteData, int bitPosition)
        {
            // byte myByte = 0101 1011
            //
            // Find out if the 1st bit (starting from the right hand side) is set to 1 or 0
            // GetBitValueInByte(myByte,1) : returns true
            //
            // Find out if the 3rd bit (Starting from the right hand side) is set to 1 or 0
            // GetBitValueInByte(myByte,3) : returns false



            bool returnValue = false;

            // each switch poistion corrisponds to bit position in byte
            // 1 = 1
            // 2 = 2
            // 3 = 4
            // 4 = 8
            // 5 = 16
            // 6 = 32
            // 7 = 64
            // 8 = 128
            switch (bitPosition)
            {
                case 1:

                    // in binary the first bit is equal to 1 if set
                    if ((byteData & 1) == 1)
                        returnValue = true;
                    break;

                case 2:

                    // in binary the second bit is equal to 2 if set
                    if ((byteData & 2) == 2)
                        returnValue = true;
                    break;

                case 3:

                    // in binary the third bit is equal to 4 if set
                    if ((byteData & 4) == 4)
                        returnValue = true;
                    break;

                case 4:

                    // in binary the fourth bit is equal to 8 if set
                    if ((byteData & 8) == 8)
                        returnValue = true;
                    break;

                case 5:

                    // in binary the fith bit is equal to 16 if set
                    if ((byteData & 16) == 16)
                        returnValue = true;
                    break;

                case 6:

                    // in binary the sixth bit is equal to 32 if set
                    if ((byteData & 32) == 32)
                        returnValue = true;
                    break;

                case 7:

                    // in binary the seventh bit is equal to 64 if set
                    if ((byteData & 64) == 64)
                        returnValue = true;
                    break;

                case 8:

                    // in binary the eighth bit is equal to 128 if set
                    if ((byteData & 128) == 128)
                        returnValue = true;
                    break;


            }

            return returnValue;
        }

        public static void SetBit(ref byte byteData, int bitPosition, bool BitValue)
        {
            // check to see if it is allready set, if so, we don't have to do anything
            if (IsBitSet(byteData, bitPosition) == BitValue)
                return;
            else
            {
                switch (bitPosition)
                {
                    case 1:

                        // if we want to set the bit to a 1
                        if (BitValue)
                            byteData += 1;
                        // if we want to set the bit to a 0
                        else
                            byteData -= 1;

                        break;

                    case 2:

                        // if we want to set the bit to a 1
                        if (BitValue)
                            byteData += 2;
                        // if we want to set the bit to a 0
                        else
                            byteData -= 2;

                        break;

                    case 3:

                        // if we want to set the bit to a 1
                        if (BitValue)
                            byteData += 4;
                        // if we want to set the bit to a 0
                        else
                            byteData -= 4;

                        break;

                    case 4:

                        if (BitValue)
                            byteData += 8;
                        else
                            byteData -= 8;

                        break;

                    case 5:

                        if (BitValue)
                            byteData += 16;
                        else
                            byteData -= 16;

                        break;

                    case 6:

                        if (BitValue)
                            byteData += 32;
                        else
                            byteData -= 32;

                        break;

                    case 7:

                        if (BitValue)
                            byteData += 64;
                        else
                            byteData -= 64;

                        break;


                    case 8:

                        if (BitValue)
                            byteData += 128;
                        else
                            byteData -= 128;

                        break;

                }
            }
        }


        /// <summary>
        /// Convert the 8 bit byte into a string (e.g. "01001101")
        /// </summary>
        /// <param name="byteData">The data to convert to a string</param>
        /// <returns>a string of 8 bits make up of 1's or 0's</returns>
        public static string ToString(byte byteData)
        {
            StringBuilder sb = new StringBuilder(8);

            // there are 8 bits in a byte
            for (int i = 8; i > 0; i--)
            {
                string bitValue = IsBitSet(byteData, i) ? "1" : "0";
                sb.Append(bitValue);
            }

            return sb.ToString();
        }
    }
}
