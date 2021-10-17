using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class File
    {
        public byte[] _blob { get; set; }
        public byte[] FileHash { get; set; }
        public User Owner { get; set; }
        public string FileName { get; set; }

        public File(User Owner, byte[] Blob, byte[] Hash, string FileName)
        {
            this._blob = Blob;
            this.Owner = Owner;
            this.FileHash = Hash;
            this.FileName = FileName;
        }

        

        public byte[] Encrypt() 
        {
            return _blob;
        }

        public void Decrypt(Stream EncryptedBlob) 
        {
            //setBlob(EncryptedBlob);
        }
    }
}
