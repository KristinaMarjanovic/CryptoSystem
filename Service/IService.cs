using Core.DataTypes;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Microsoft.Samples.SoapAndHttpEndpoints
{
    [ServiceContract]
    public interface IService
    {

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/UploadFile", RequestFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare)]
        string UploadFile(FileTransfer fileTransfer);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/DownloadFile")]
        FileTransfer DownloadFile();

        [OperationContract]
        [WebGet]
        FileList GetFiles();


    }
}
