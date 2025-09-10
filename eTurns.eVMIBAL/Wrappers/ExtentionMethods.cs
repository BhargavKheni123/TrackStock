using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eTurns.eVMIBAL.Wrappers
{
    public static class ExtentionMethods
    {
        public static string ToHex(this byte[] comByte)
        {
            //create a new StringBuilder object
            StringBuilder builder = new StringBuilder(comByte.Length * 3);
            //loop through each byte in the array
            foreach (byte data in comByte)
            {
                //convert the byte to a string and add to the stringbuilder
                builder.Append(Convert.ToString(data, 16).PadLeft(2, '0').PadRight(3, ' '));
            }
            //return the converted value
            return builder.ToString().ToUpper();
        }

        public static byte[] ToByte(this string strHex)
        {
            //remove any spaces from the string
            strHex = strHex.Replace(" ", "");
            //create a byte array the length of the
            //divided by 2 (Hex is 2 characters in length)
            byte[] comBuffer = new byte[strHex.Length / 2];
            //loop through the length of the provided string
            for (int i = 0; i < strHex.Length; i += 2)
            {
                //convert each set of 2 characters to a byte
                //and add to the array
                comBuffer[i / 2] = (byte)Convert.ToByte(strHex.Substring(i, 2), 16);
            }
            //return the array
            return comBuffer;
        }

        public static int ToInt(this string strHex)
        {
            int num = Convert.ToInt32(strHex, 16);
            return num;
        }
        public static string ToHex(this string strValue)
        {
            string sResult = string.Empty;

            foreach (char sChar in strValue)
            {
                int value = Convert.ToInt32(sChar);
                // Convert the decimal value to a hexadecimal value in string form.
                sResult += String.Format("{0:X2}", value);
                // sResult += sChar.ToString("X2");
            }

            return sResult;
        }

        public static string ToASCII(this string strHex)
        {
            // initialize the ASCII code string as empty.
            String ascii = "";

            for (int i = 0; i < strHex.Length; i += 2)
            {

                // extract two characters from hex string
                String part = strHex.Substring(i, 2);

                // change it into base 16 and 
                // typecast as the character
                char ch = (char)Convert.ToInt32(part, 16);
                // add this char to final ASCII string
                ascii = ascii + ch;
            }
            return ascii;
        }

    }

}
