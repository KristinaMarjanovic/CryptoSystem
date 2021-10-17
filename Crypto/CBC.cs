using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Crypto
{
    public class CBC
    {

        public string inputText_; //tekst koji ce biti enkriptovan ili dekriptovan
        public string key_; //kljuc, string u binarnom obliku (niz 0 i 1)
        public string iv_ = bitArrayToString(new BitArray(new byte[] { 0x12, 0x23, 0x45, 0x67, 0x81, 0xAB, 0xCF, 0xED })); //vektor inicijalizacije, string u binarnom obliku (niz 0 i 1)
        public byte[] outputBytes_; //rezultat enkripcije ili dekripcije u bajtovima

        
        //U vasoj aplikaciji obezbedite da se na dugme enkript i dekript prosledi odgovarajucoj metodi putanja fajla fajla kojeg zelite da obradite
        //Prilikom enkripcije i dekripcije sam koristio xor sa kljucem jer je najprostije (x xor k xor k = x)

        public CBC()
        {
            

        }

        //iako su iste metode za enkripciju i dekripciju bloka, ostavio sam oba ako neko drugi zeli da pravi drukcije
        public String encryptionBlock(String plainText)    //enkripcija jednog bloka
        {
            String output = "";

            for (int i = 0; i != key_.Length; i++)
                output += xorBit(plainText[i], key_[i]); //vrsim xorovanje Plaintext bloka sa kljucem, moze se smisliti drugi nacin enkripcije

            return output;
        }
        public String decryptionBlock(String cipherText)   //dekripcija jednog bloka
        {
            String output = "";

            for (int i = 0; i != key_.Length; i++)
                output += xorBit(cipherText[i], key_[i]); //vrsim xorovanje Ciphertext bloka sa kljucem, moze se smisliti drugi nacin dekripcije

            return output;
        }

        public byte[] binaryStringToBytes(string binary) //konvertuje string bitova u niz bajtova
        {
            int i = 0;
            int j = 0;
            BitArray bits = new BitArray(8);
            byte[] bytes = new byte[binary.Length / 8];

            while (j != binary.Length)
            {
                for (i = 0; i != 8; i++)
                    bits.Set(i, (binary[j + i] == '1' ? true : false));
                bits.CopyTo(bytes, j / 8);
                j += 8;
            }

            return bytes;
        }

        public char xorBit(char _bit1, char _bit2) //vrsi XOR nad _bit1 i _bit2 koji su napisani kao karakteri
        {
            return ((_bit1 == _bit2) ? '0' : '1');
        }

        public byte[] encode(byte[] blob, byte[] key) //enkripcija
        {
            key_ = bitArrayToString(new BitArray(key));
            if (key_.Length != iv_.Length)  //provera da li su kljuc i iv istih duzina
            {
                throw new Exception("Key and Iv must be same size");
            }
            this.inputText_ = bitArrayToString(new BitArray(blob));
            //dodatno sam zauzeo memoriju da bih se lakse snasao sa imenima
            string inputBit = this.inputText_;
            int inputBitLength = inputBit.Length;
            int blockLength = this.key_.Length;

            string blockCipher = ""; //Ciphertext blok
            string blockTemp = "";  //u njega smestamo ono sto xorujemo da bi njegovom enkripcijom dobili blockCipher
            string output = ""; //ceo Ciphertext
            int i = 0;
            int j = 0;

            while (inputBit.Length % blockLength != 0)     //dodajemo nule kako bi tekst koji enkriptujemo bio celobrojni umnozak velicine kljuca
                inputBit += '0';

            for (i = 0; i != blockLength; i++)
            {
                blockTemp += xorBit(inputBit[j], iv_[i]);     //xorujemo prvi blok sa iv
                j++;
            }

            blockCipher = encryptionBlock(blockTemp);   //enkriptujemo rezultat xorovanja
            output += blockCipher;  //dodajemo u Ciphertext
            blockTemp = ""; //prelazimo na sledeci blok

            while (j != inputBit.Length)
            {
                for (i = 0; i != blockLength; i++)                //xorujemo drugi blok i sve preostale sa Cyphertext blokom iz prethodne runde
                {
                    blockTemp += xorBit(blockCipher[i], inputBit[j]);
                    j++;
                }
                blockCipher = encryptionBlock(blockTemp);
                output += blockCipher;
                blockTemp = "";
            }

            return binaryStringToBytes(output.Substring(0, inputBitLength)); //pripremamo dobijeni string u bajtove za upis

        }

        public byte[] decode(byte[] blob, byte[] key) //dekripcija
        {
            key_ = bitArrayToString(new BitArray(key));
            if (key_.Length != iv_.Length)  //provera da li su kljuc i iv istih duzina
            {
                throw new Exception("Key and Iv must be same size");
            }
            inputText_ = bitArrayToString(new BitArray(blob)); //ucitavamo Ciphertext ceo

            //dodatno sam zauzeo memoriju da bih se lakse snasao sa imenima
            string inputBit = this.inputText_;
            int inputBitLength = inputBit.Length;
            int blockLength = this.key_.Length;

            string blockCipher = "";    //rezultat dekripcije Cyphertext bloka
            string blockPlain = ""; //Plaintext blok
            string blockTemp = "";  //pamtimo nedekriptovan Cyphertext blok prethodne runde
            string output = ""; //ceo Plaintext
            string tempText = "";
            int i = 0;
            int j = 0;

            while (inputBit.Length % blockLength != 0) //dodajemo nule kako bi tekst koji enkriptujemo bio celobrojni umnozak velicine kljuca
                inputBit += '0';

            blockCipher = decryptionBlock(inputBit.Substring(0, blockLength));   //dekriptujemo prvi Ciphertext blok

            for (i = 0; i != blockLength; i++)
            {
                blockPlain += xorBit(blockCipher[i], iv_[i]);    //xorujemo dekriptovan prvi Ciphertext blok sa iv
                j += 1;
            }

            output += blockPlain;   //dodajemo rezultat u Plaintext
            blockTemp = inputBit.Substring(0, blockLength);
            blockPlain = "";
            blockCipher = "";
            tempText = "";

            while (j != inputBit.Length)
            {
                for (i = 0; i != blockLength; i++)
                {
                    blockCipher += inputBit[j];
                    j += 1;
                }
                tempText = blockCipher;
                blockCipher = decryptionBlock(blockCipher); //dekriptujemo drugi i preostale Ciphertext blokove

                for (i = 0; i != blockLength; i++)
                    blockPlain += xorBit(blockCipher[i], blockTemp[i]);  //xorujemo dekriptovan drugi i svaki preostale Ciphertext blokove sa nedekriptovanim Ciphertext blokom prethodne runde 

                output += blockPlain; //dodajemo rezultat u Plaintext
                blockTemp = tempText;
                blockPlain = "";
                blockCipher = "";
            }

            return binaryStringToBytes(output.Substring(0, inputBitLength)); //pripremamo dobijeni string u bajtove za upis

        }

        private static string bitArrayToString(BitArray array)
        {
            string s = "";
            foreach (bool bit in array)
            {
                s += bit ? "1" : "0";
            }
            return s;
        }


    }
}