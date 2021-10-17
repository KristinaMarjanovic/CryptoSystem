using Core.DataTypes;
using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Core
{
    public class HttpService
    {
        public Uri BaseAddress { get; }

        public HttpService(Uri BaseAddress)
        {
            this.BaseAddress = BaseAddress;
        }

        public FileList GetFilesList(User Owner)
        {
            using (WebClient httpClient = new WebClient())
            {
                httpClient.BaseAddress = BaseAddress.AbsoluteUri;
                httpClient.Headers[HttpRequestHeader.ContentType] = "application/xml";
                httpClient.Headers.Add("Authorization", Owner.AuthorizationToken());
                string xmlResponse = httpClient.DownloadString("GetFiles");
                DataContractSerializer dcs = new DataContractSerializer(typeof(FileList));
                using (XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(Encoding.UTF8.GetBytes(xmlResponse), new XmlDictionaryReaderQuotas()))
                {
                    FileList fileList = (FileList)dcs.ReadObject(reader);
                    return fileList;
                }
            }
        }

        public void UploadFile(File file, User Owner)
        {
            using (WebClient httpClient = new WebClient())
            {
                httpClient.BaseAddress = BaseAddress.AbsoluteUri;
                httpClient.Headers[HttpRequestHeader.ContentType] = "application/xml";
                httpClient.Headers.Add("Authorization", Owner.AuthorizationToken());
                httpClient.Headers.Add("FileName", file.FileName);

                FileTransfer ft = new FileTransfer();
                ft.Name = file.FileName;
                ft.FileHash = file.FileHash;
                ft.EncodedBlob = file._blob;
                DataContractSerializer dcs = new DataContractSerializer(typeof(FileTransfer));
                MemoryStream stream1 = new MemoryStream();
                dcs.WriteObject(stream1, ft);
                

                httpClient.UploadData("UploadFile", stream1.ToArray());

            }
        }

        public FileTransfer DownloadFile(File file, User Owner)
        {
            using (WebClient httpClient = new WebClient())
            {
                httpClient.BaseAddress = BaseAddress.AbsoluteUri;
                httpClient.Headers[HttpRequestHeader.ContentType] = "application/xml";
                httpClient.Headers.Add("Authorization", Owner.AuthorizationToken());
                httpClient.Headers.Add("FileName", file.FileName);
                string byteResponse = httpClient.UploadString("DownloadFile", "");
                DataContractSerializer dcs = new DataContractSerializer(typeof(FileTransfer));
                XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(Encoding.UTF8.GetBytes(byteResponse), new XmlDictionaryReaderQuotas());
                FileTransfer fileDownload = (FileTransfer)dcs.ReadObject(reader);
                return fileDownload;
            }
        }
    }
}
