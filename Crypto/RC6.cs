using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Crypto
{
    public class RC6 // 32/20/16 - 128/192/256
    {
        /* Spisak promenljivih*/
        private const int R = 20; // broj rundi
        private static uint[] RoundKey = new uint[2 * R + 4];  // kljuc runde
        private const int W = 32; // duzina slova u bitovima
        private static byte[] MainKey; // kljuc
        private const uint P32 = 0xB7E15163; // konstante
        private const uint Q32 = 0x9E3779B9;
        /*Generisanje kljuca*/
        //Konstruktor s generisanjem random kljuca
        public RC6(int keyLong)
        {
            GenerateKey(keyLong, null);
        }
        //Konstruktor koji koristi zakucani kljuc
        public RC6(int keyLong, byte[] key)
        {
            GenerateKey(keyLong, key);
        }
        // Shift right without loss
        private static uint RightShift(uint value, int shift)
        {
            return (value >> shift) | (value << (W - shift));
        }
        // Shift left without loss
        private static uint LeftShift(uint value, int shift)
        {
            return (value << shift) | (value >> (W - shift));
        }
        // Generate main key and round keys
        private static void GenerateKey(int Long, byte[] keyCheck)
        {
            // If the main key is not set in advance, use the random key generator
            if (keyCheck == null)
            {
                AesCryptoServiceProvider aesCrypto = new AesCryptoServiceProvider // keys to generate
                {
                    // Set the key size specified in the class constructor
                    KeySize = Long
                };
                aesCrypto.GenerateKey();
                MainKey = aesCrypto.Key;
            }
            else MainKey = keyCheck;
            int c = 0;
            int i, j;
            // Depending on the size of the key, choose how many blocks to break main key
            switch (Long)
            {
                case 128:// key length
                    c = 4; // number of words in the key
                    break;
                case 192:// key length
                    c = 6; // number of words in the key
                    break;
                case 256: // key lengthа
                    c = 8; // number of words in the key
                    break;
            }
            uint[] L = new uint[c];
            for (i = 0; i < c; i++)
            {
                L[i] = BitConverter.ToUInt32(MainKey, i * 4); // break the key into words
            }
            // The generation of round keys in accordance with the documentation
            RoundKey[0] = P32;
            for (i = 1; i < 2 * R + 4; i++)
                RoundKey[i] = RoundKey[i - 1] + Q32; // add a constant to the round key
            uint A, B; // регистры
            A = B = 0;
            i = j = 0;
            int V = 3 * Math.Max(c, 2 * R + 4);  // maximum of rounds or the number of words in a key
            for (int s = 1; s <= V; s++)
            {
                A = RoundKey[i] = LeftShift((RoundKey[i] + A + B), 3); // shift left by 3
                B = L[j] = LeftShift((L[j] + A + B), (int)(A + B)); // shift left by a + b
                i = (i + 1) % (2 * R + 4);
                j = (j + 1) % c;
            }
        }
        // splits into an array of bytes
        private static byte[] ToArrayBytes(uint[] uints, int Long)
        {
            byte[] arrayBytes = new byte[Long * 4];
            for (int i = 0; i < Long; i++)
            {
                byte[] temp = BitConverter.GetBytes(uints[i]);
                temp.CopyTo(arrayBytes, i * 4);
            }
            return arrayBytes;
        }
        public byte[] EncodeRc6(byte[] byteText)
        {
            uint A, B, C, D;
            // Convert the received text to an array of bytes
            int i = byteText.Length;
            while (i % 16 != 0)
                i++;
            // Create a new array, the multiplicity of which is a multiple of 16, since the algorithm describes working with four blocks of 4 bytes each.
            byte[] text = new byte[i];
            // Write plaintext there
            byteText.CopyTo(text, 0);
            byte[] cipherText = new byte[i];
            // Loop for each block of 16 bytes
            for (i = 0; i < text.Length; i = i + 16)
            {
                // The resulting block of 16 bytes is divided into 4 machine words (32 bits each)
                A = BitConverter.ToUInt32(text, i);
                B = BitConverter.ToUInt32(text, i + 4);
                C = BitConverter.ToUInt32(text, i + 8);
                D = BitConverter.ToUInt32(text, i + 12);
                // The encryption algorithm itself in accordance with the documentation
                B = B + RoundKey[0];
                D = D + RoundKey[1];
                for (int j = 1; j <= R; j++)
                {
                    uint t = LeftShift((B * (2 * B + 1)), (int)(Math.Log(W, 2)));
                    uint u = LeftShift((D * (2 * D + 1)), (int)(Math.Log(W, 2)));
                    A = (LeftShift((A ^ t), (int)u)) + RoundKey[j * 2];
                    C = (LeftShift((C ^ u), (int)t)) + RoundKey[j * 2 + 1];
                    uint temp = A;
                    A = B;
                    B = C;
                    C = D;
                    D = temp;
                }
                A = A + RoundKey[2 * R + 2];
                C = C + RoundKey[2 * R + 3];
                // Convert machine words to byte array
                uint[] tempWords = new uint[4] { A, B, C, D };
                byte[] block = ToArrayBytes(tempWords, 4);
                // Write converted 16 bytes to a byte array of ciphertext
                block.CopyTo(cipherText, i);
            }
            return cipherText;
        }
        public byte[] DecodeRc6(byte[] cipherText)
        {
            uint A, B, C, D;
            int i;
            byte[] plainText = new byte[cipherText.Length];
            // Break the ciphertext into blocks of 16 bytes
            for (i = 0; i < cipherText.Length; i = i + 16)
            {
                // Blocking into 4 machine words of 32 bits
                A = BitConverter.ToUInt32(cipherText, i);
                B = BitConverter.ToUInt32(cipherText, i + 4);
                C = BitConverter.ToUInt32(cipherText, i + 8);
                D = BitConverter.ToUInt32(cipherText, i + 12);
                // The decryption process itself in accordance with the documentation
                C = C - RoundKey[2 * R + 3];
                A = A - RoundKey[2 * R + 2];
                for (int j = R; j >= 1; j--)
                {
                    uint temp = D;
                    D = C;
                    C = B;
                    B = A;
                    A = temp;
                    uint u = LeftShift((D * (2 * D + 1)), (int)Math.Log(W, 2));
                    uint t = LeftShift((B * (2 * B + 1)), (int)Math.Log(W, 2));
                    C = RightShift((C - RoundKey[2 * j + 1]), (int)t) ^ u;
                    A = RightShift((A - RoundKey[2 * j]), (int)u) ^ t;
                }
                D = D - RoundKey[1];
                B = B - RoundKey[0];
                // Convert machine words to byte array
                uint[] tempWords = new uint[4] { A, B, C, D };
                byte[] block = ToArrayBytes(tempWords, 4);
                // Write the decrypted bytes to the byte array of the decrypted text
                block.CopyTo(plainText, i);
            }
            int len = plainText.Length - 1;
            while (plainText[len] == 0)
            {
                len--;
            }
            return plainText.Take(len + 1).ToArray();
        }
    }
}
