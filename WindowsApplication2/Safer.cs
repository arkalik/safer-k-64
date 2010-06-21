using System;
using System.Collections.Generic;
using System.Text;

namespace WindowsApplication1
{
    class Safer
    {
        private string text;
        private string key;
        private int RoundValue;

        public int Round
        {
            get { return RoundValue; }
            set { RoundValue = value; }
        }

        public string Text
        {
            get{ return text; }
            set{ text = value; }
        }

        public string Key
        {
            get{
                if (key.Length != 8)
                {
                    throw new Exception("Encryption key must have 8 bytes");
                }
                return key; 
            }
            set{
                if (value.Length != 8)
                {
                    throw new Exception("Encryption key must have 8 bytes");
                }
                key = value; 
            }
        }

        // left cycling byte shift. For example, 111001 left cycling shift on 2 bytes = 100111
        private uint LeftCicle(uint value, int count)
        {
            return (value >> count) + (((value << (32 - count)) >> (32 - count)) << count);
        }

        // get advanced encryption key
        private byte[] GetKey(int round, byte[] key)
        {
            char[,] b = new char[round + 1, 8];
            for (int i = 0; i < round + 1; i++)
            {
                // const for advancing
                b[i, 0] = '1';
                b[i, 1] = '6';
                b[i, 2] = '7';
                b[i, 3] = '3';
                b[i, 4] = '3';
                b[i, 5] = 'B';
                b[i, 6] = '1';
                b[i, 7] = 'E';
            }
            for (int i = 2; i <= round; i++)
            {
                for (int j = 0; j < key.Length; j++)
                {
                    uint uByte = LeftCicle(Convert.ToUInt32(key[j]), 3);
                    uint specB = Convert.ToUInt32(Convert.ToByte(b[i, j]));
                    // addition modulo 256
                    key[j] = Convert.ToByte((uByte + specB) % 256);
                }
            }
            return key;
        }

        // get 8 byte ration unencrypted data
        private byte[] GetTextBlock8Byte(string source, int blockNumber)
        {
            int eight = 8;
            byte[] b = Encoding.Default.GetBytes(source);
            byte[] resultBlock = new byte[eight];
            int firstByte = blockNumber * eight;
            int j = 0;
            for (int i = firstByte; i < firstByte + eight; i++)
            {
                if (i >= b.Length)
                    resultBlock[j] = 0;
                else
                    resultBlock[j] = b[i];
                j++;
            }
            return resultBlock;
        }

        private int GetTextBlockLength(string source)
        {
            byte[] b = Encoding.Default.GetBytes(source);
            if (b.Length % 8 == 0)
                return b.Length / 8;
            else
                return b.Length / 8 + 1;
        }

        // XOR
        private byte XOR(byte text, byte key)
        {
            return Convert.ToByte(Convert.ToInt32(text) ^ Convert.ToInt32(key));
        }

        // addition modulo 256
        private byte Mod256(byte text, byte key)
        {
            return Convert.ToByte((Convert.ToInt32(text) + Convert.ToInt32(key)) % 256);
        }

        // reverse for addition modulo 256
        private byte DiffMod256(byte text, byte key)
        {
            int y = Convert.ToInt32(text);
            int k = Convert.ToInt32(key);
            if (y < k)
                y += 256;
            return Convert.ToByte(y - k % 256);
        }

        // operation E. using matrix map value.
        private byte E(byte text)
        {
            int[] replacement = new int[256] { 1, 45, 226, 147, 190, 69, 21, 174, 120, 3, 135, 164, 184, 56, 207, 63, 8, 103, 9, 148, 235, 38, 168, 107, 189, 24, 52, 27, 187, 191, 114, 247, 64, 53, 72, 156, 81, 47, 59, 85, 227, 192, 159, 216, 211, 243, 141, 177, 255, 167, 62, 220, 134, 119, 215, 166, 17, 251, 244, 186, 146, 145, 100, 131, 241, 51, 239, 218, 44, 181, 178, 43, 136, 209, 153, 203, 140, 132, 29, 20, 129, 151, 113, 202, 95, 163, 139, 87, 60, 130, 196, 82, 92, 28, 232, 160, 4, 180, 133, 74, 246, 19, 84, 182, 223, 12, 26, 142, 222, 224, 57, 252, 32, 155, 36, 78, 169, 152, 158, 171, 242, 96, 208, 108, 234, 250, 199, 217, 0, 212, 31, 110, 67, 188, 236, 83, 137, 254, 122, 93, 73, 201, 50, 194, 249, 154, 248, 109, 22, 219, 89, 150, 68, 233, 205, 230, 70, 66, 143, 10, 193, 204, 185, 101, 176, 210, 198, 172, 30, 65, 98, 41, 46, 14, 116, 80, 2, 90, 195, 37, 123, 138, 42, 91, 240, 6, 13, 71, 111, 112, 157, 126, 16, 206, 18, 39, 213, 76, 79, 214, 121, 48, 104, 54, 117, 125, 228, 237, 128, 106, 144, 55, 162, 94, 118, 170, 197, 127, 61, 175, 165, 229, 25, 97, 253, 77, 124, 183, 11, 238, 173, 75, 34, 245, 231, 115, 35, 33, 200, 5, 225, 102, 221, 179, 88, 105, 99, 86, 15, 161, 49, 149, 23, 7, 58, 40 };
            int t = Convert.ToInt32(text);
            return Convert.ToByte(replacement[t]);
        }

        // operation L. using matrix map value.
        private byte L(byte text)
        {
            int[] replacement = new int[256] { 128, 0, 176, 9, 96, 239, 185, 253, 16, 18, 159, 228, 105, 186, 173, 248, 192, 56, 194, 101, 79, 6, 148, 252, 25, 222, 106, 27, 93, 78, 168, 130, 112, 237, 232, 236, 114, 179, 21, 195, 255, 171, 182, 71, 68, 1, 172, 37, 201, 250, 142, 65, 26, 33, 203, 211, 13, 110, 254, 38, 88, 218, 50, 15, 32, 169, 157, 132, 152, 5, 156, 187, 34, 140, 99, 231, 197, 225, 115, 198, 175, 36, 91, 135, 102, 39, 247, 87, 244, 150, 177, 183, 92, 139, 213, 84, 121, 223, 170, 246, 62, 163, 241, 17, 202, 245, 209, 23, 123, 147, 131, 188, 189, 82, 30, 235, 174, 204, 214, 53, 8, 200, 138, 180, 226, 205, 191, 217, 208, 80, 89, 63, 77, 98, 52, 10, 72, 136, 181, 86, 76, 46, 107, 158, 210, 61, 60, 3, 19, 251, 151, 81, 117, 74, 145, 113, 35, 190, 118, 42, 95, 249, 212, 85, 11, 220, 55, 49, 22, 116, 215, 119, 167, 230, 7, 219, 164, 47, 70, 243, 97, 69, 103, 227, 12, 162, 59, 28, 133, 24, 4, 29, 41, 160, 143, 178, 90, 216, 166, 126, 238, 141, 83, 75, 161, 154, 193, 14, 122, 73, 165, 44, 129, 196, 199, 54, 43, 127, 67, 149, 51, 242, 108, 104, 109, 240, 2, 40, 206, 221, 155, 234, 94, 153, 124, 20, 134, 207, 229, 66, 184, 64, 120, 45, 58, 233, 100, 31, 146, 144, 125, 57, 111, 224, 137, 48 };
            int t = Convert.ToInt32(text);
            return Convert.ToByte(replacement[t]);
        }

        // reverse for operation PHT (Pseudo Hadamard Transform)
        private byte[] IPHT(byte x1, byte x2)
        {
            int intX1 = Convert.ToInt32(x1);
            int intX2 = Convert.ToInt32(x2);
            int diff1 = -intX1 + 2 * intX2;
            if (diff1 < 0)
                diff1 += 256;
            int diff2 = intX1 - intX2;
            if (diff2 < 0)
                diff2 += 256;
            int intY1 = diff1 % 256;
            int intY2 = diff2 % 256;
            byte[] b = new byte[2];
            b[1] = Convert.ToByte(intY1);
            b[0] = Convert.ToByte(intY2);
            return b;
        }

        // operation PHT (Pseudo Hadamard Transform)
        private byte[] PHT(byte x1, byte x2)
        {
            int intX1 = Convert.ToInt32(x1);
            int intX2 = Convert.ToInt32(x2);
            int intY1 = (2 * intX1 + intX2) % 256;
            int intY2 = (intX1 + intX2) % 256;
            byte[] b = new byte[2];
            b[0] = Convert.ToByte(intY1);
            b[1] = Convert.ToByte(intY2);
            return b;
        }

        public byte[] execPHTStage(byte[] x, int round)
        {
            byte[] b = new byte[8];
            byte[] temp = new byte[2];
            temp = this.PHT(x[0], x[1]);
            if (round != 0)
            {
                b[0] = temp[0];
                b[4] = temp[1];
            }
            else
            {
                Array.Copy(temp, 0, b, 0, 2);
            }
            temp = this.PHT(x[2], x[3]);
            if (round != 0)
            {
                b[1] = temp[0];
                b[5] = temp[1];
            }
            else
            {
                Array.Copy(temp, 0, b, 2, 2);
            }
            temp = this.PHT(x[4], x[5]);
            if (round != 0)
            {
                b[2] = temp[0];
                b[6] = temp[1];
            }
            else
            {
                Array.Copy(temp, 0, b, 4, 2);
            }
            temp = this.PHT(x[6], x[7]);
            if (round != 0)
            {
                b[3] = temp[0];
                b[7] = temp[1];
            }
            else
            {
                Array.Copy(temp, 0, b, 6, 2);
            }
            return b;
        }

        public byte[] execIPHTStage(byte[] x, int round)
        {
            byte[] b = new byte[8];
            byte[] temp = new byte[2];
            temp = this.IPHT(x[0], x[1]);
            if (round != 0)
            {
                b[0] = temp[0];
                b[2] = temp[1];
            }
            else
                Array.Copy(temp, 0, b, 0, 2);
            temp = this.IPHT(x[2], x[3]);
            if (round != 0)
            {
                b[4] = temp[0];
                b[6] = temp[1];
            }
            else
                Array.Copy(temp, 0, b, 2, 2);
            temp = this.IPHT(x[4], x[5]);
            if (round != 0)
            {
                b[1] = temp[0];
                b[3] = temp[1];
            }
            else
                Array.Copy(temp, 0, b, 4, 2);
            temp = this.IPHT(x[6], x[7]);
            if (round != 0)
            {
                b[5] = temp[0];
                b[7] = temp[1];
            }
            else
                Array.Copy(temp, 0, b, 6, 2);
            return b;
        }

        // SAFER K-64 encryption
        public byte[] encrypt()
        {
            int sourceLength = this.GetTextBlockLength(this.Text);
            byte[] key = Encoding.Default.GetBytes( this.Key );
            byte[] bigKey;
            byte[] bigKey2;
            byte[] sourceBlock = new byte[8];
            byte[] result = new byte[sourceLength * 8];
            for (int round = 0; round < this.Round; round++)
            {
                int r = 0;
                for (int i = 0; i < sourceLength; i++)
                {
                    sourceBlock = this.GetTextBlock8Byte(this.Text, i);
                    // Encryption Key advancing
                    bigKey = this.GetKey(2 * (i + 1) - 1, key);
                    bigKey2 = this.GetKey(2 * (i + 1), key);
                    for (int j = 0; j < sourceBlock.Length; j++)
                    {
                        switch (j + 1)
                        {
                            case 1:
                            case 4:
                            case 5:
                            case 8:
                                result[r] = this.XOR(sourceBlock[j], bigKey[j]);
                                result[r] = this.E(result[r]);
                                result[r] = this.Mod256(result[r], bigKey2[j]);
                                break;
                            default:
                                result[r] = this.Mod256(sourceBlock[j], bigKey[j]);
                                result[r] = this.L(result[r]);
                                result[r] = this.XOR(result[r], bigKey2[j]);
                                break;
                        }
                        r++;
                    }
                    byte[] temp = new byte[8];
                    Array.Copy(result, r - 8, temp, 0, 8);
                    temp = this.execPHTStage(temp, 1);
                    temp = this.execPHTStage(temp, 2);
                    temp = this.execPHTStage(temp, 0);
                    Array.Copy(temp, 0, result, r - 8, 8);
                }
            }
            return result;
        }

        // SAFER K-64 decryption
        public byte[] decrypt()
        {
            int sourceLength = this.GetTextBlockLength(this.Text);
            byte[] key = Encoding.Default.GetBytes(this.Key);
            byte[] bigKey;
            byte[] bigKey2;
            byte[] sourceBlock = new byte[8];
            byte[] result = new byte[sourceLength * 8];
            for (int round = 0; round < this.Round; round++)
            {
                int r = 0;
                for (int i = 0; i < sourceLength; i++)
                {

                    sourceBlock = this.GetTextBlock8Byte(this.Text, i);
                    byte[] temp = new byte[8];
                    Array.Copy(sourceBlock, 0, temp, 0, 8);
                    temp = this.execIPHTStage(sourceBlock, 1);
                    temp = this.execIPHTStage(temp, 2);
                    temp = this.execIPHTStage(temp, 0);
                    Array.Copy(temp, 0, result, r, 8);

                    bigKey2 = this.GetKey(2 * (i + 1) - 1, key);
                    bigKey = this.GetKey(2 * (i + 1), key);
                    for (int j = 0; j < sourceBlock.Length; j++)
                    {
                        switch (j + 1)
                        {
                            case 1:
                            case 4:
                            case 5:
                            case 8:
                                result[r] = this.DiffMod256(result[r], bigKey2[j]);
                                result[r] = this.L(result[r]);
                                result[r] = this.XOR(result[r], bigKey[j]);
                                break;
                            default:
                                result[r] = this.XOR(result[r], bigKey2[j]);
                                result[r] = this.E(result[r]);
                                result[r] = this.DiffMod256(result[r], bigKey[j]);
                                break;
                        }
                        r++;
                    }
                }
            }
            return result;
        }
    }
}
