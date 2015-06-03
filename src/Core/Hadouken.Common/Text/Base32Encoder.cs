using System;
using System.Linq;

namespace Hadouken.Common.Text {
    public class Base32Encoder {
        public static byte[] ToBytes(string input) {
            if (string.IsNullOrEmpty(input)) {
                throw new ArgumentNullException("input");
            }

            input = input.TrimEnd('='); //remove padding characters
            var byteCount = input.Length*5/8; //this must be TRUNCATED
            var returnArray = new byte[byteCount];

            byte curByte = 0, bitsRemaining = 8;
            var arrayIndex = 0;

            foreach (var cValue in input.Select(CharToValue)) {
                int mask;
                if (bitsRemaining > 5) {
                    mask = cValue << (bitsRemaining - 5);
                    curByte = (byte) (curByte | mask);
                    bitsRemaining -= 5;
                }
                else {
                    mask = cValue >> (5 - bitsRemaining);
                    curByte = (byte) (curByte | mask);
                    returnArray[arrayIndex++] = curByte;
                    curByte = (byte) (cValue << (3 + bitsRemaining));
                    bitsRemaining += 3;
                }
            }

            //if we didn't end with a full byte
            if (arrayIndex != byteCount) {
                returnArray[arrayIndex] = curByte;
            }

            return returnArray;
        }

        public static string ToString(byte[] input) {
            if (input == null || input.Length == 0) {
                throw new ArgumentNullException("input");
            }

            var charCount = (int) Math.Ceiling(input.Length/5d)*8;
            var returnArray = new char[charCount];

            byte nextChar = 0, bitsRemaining = 5;
            var arrayIndex = 0;

            foreach (var b in input) {
                nextChar = (byte) (nextChar | (b >> (8 - bitsRemaining)));
                returnArray[arrayIndex++] = ValueToChar(nextChar);

                if (bitsRemaining < 4) {
                    nextChar = (byte) ((b >> (3 - bitsRemaining)) & 31);
                    returnArray[arrayIndex++] = ValueToChar(nextChar);
                    bitsRemaining += 5;
                }

                bitsRemaining -= 3;
                nextChar = (byte) ((b << bitsRemaining) & 31);
            }

            //if we didn't end with a full char
            if (arrayIndex == charCount) {
                return new string(returnArray);
            }
            returnArray[arrayIndex++] = ValueToChar(nextChar);
            while (arrayIndex != charCount) {
                returnArray[arrayIndex++] = '='; //padding
            }

            return new string(returnArray);
        }

        private static int CharToValue(char c) {
            int value = c;

            //65-90 == uppercase letters
            if (value < 91 && value > 64) {
                return value - 65;
            }
            //50-55 == numbers 2-7
            if (value < 56 && value > 49) {
                return value - 24;
            }
            //97-122 == lowercase letters
            if (value < 123 && value > 96) {
                return value - 97;
            }

            throw new ArgumentException("Character is not a Base32 character.", "c");
        }

        private static char ValueToChar(byte b) {
            if (b < 26) {
                return (char) (b + 65);
            }

            if (b < 32) {
                return (char) (b + 24);
            }

            throw new ArgumentException("Byte is not a value Base32 value.", "b");
        }
    }
}