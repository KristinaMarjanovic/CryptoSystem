using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Core.DataTypes
{
    [DataContract]
    public class FileList
    {
        [DataMember]
        public List<File> Files { get; set; }
    }
    [DataContract]
    public class File
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public DateTime UploadDate { get; set; }
    }

    [DataContract]
    public class FileTransfer
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public byte[] FileHash { get; set; }
        [DataMember]
        public byte[] EncodedBlob { get; set; }
    }


    [DataContract]
    public class UserFiles {
        
        [DataMember]
        public List<EncryptionData> fileHashes { get; set; }

        public void SetEncryptionData(string Username, string Filename, string Algorithm, byte[] Key, byte[] Hash) 
        {
            var ed = fileHashes.Find(x => x.Username == Username && x.Filename == Filename);
            if (ed == null)
            {
                var encData = new EncryptionData();
                encData.Username = Username;
                encData.Filename = Filename;
                encData.Algorithm = Algorithm;
                encData.Hash = Hash;
                encData.Key = Key;
                fileHashes.Add(encData);
            }
            else 
            {
                ed.Algorithm = Algorithm;
                ed.Hash = Hash;
                ed.Key = Key;
            }
        
        }


    }
    [DataContract]
    public class EncryptionData {

        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public string Filename { get; set; }

        [DataMember]
        public string Algorithm { get; set; }

        [DataMember]
        public byte[] Hash { get; set; }

        [DataMember]
        public byte[] Key { get; set; }

    }

 



}
