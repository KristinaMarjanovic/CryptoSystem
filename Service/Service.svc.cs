using System.ServiceModel;
using System.ServiceModel.Web;
using System.Net;
using System;
using System.Text;
using Core.DataTypes;
using System.Collections.Generic;
using Db;
using System.Data.SqlClient;
using System.Data;
using Core;

namespace Microsoft.Samples.SoapAndHttpEndpoints
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class Service : IService
    {
        private string _conn = "Server = milos-HP; Database = MyCloudStore; Trusted_Connection=True";


        public string UploadFile(FileTransfer fileTransfer)
        {
            IncomingWebRequestContext request = WebOperationContext.Current.IncomingRequest;
            WebHeaderCollection headers = request.Headers;
           
            var user = Authorize(headers);

            Core.File file = new Core.File(user, fileTransfer.EncodedBlob, fileTransfer.FileHash, fileTransfer.Name);
            InsertFile(file);

            return "Ok";
        }

        public FileTransfer DownloadFile()
        {
            IncomingWebRequestContext request = WebOperationContext.Current.IncomingRequest;
            WebHeaderCollection headers = request.Headers;
            string name = headers.Get("FileName");
            var user = Authorize(headers);

            Core.File file = new Core.File(user, null, null, name);
            var fileDownload= FileSelectByName(file);

            return fileDownload;
        }

        private FileTransfer FileSelectByName(Core.File file)
        {
            var db = new Database(_conn);
            var _params = new List<SqlParameter>();
            _params.Add(new SqlParameter("@UserID", file.Owner.Id));
            _params.Add(new SqlParameter("@FileName", file.FileName));
            var ds =db.ExecStoredProc("[dbo].[File_SelectByName]", _params.ToArray());
            var fileDownload = new FileTransfer();
            fileDownload.EncodedBlob = ds.Tables[0].Rows[0].Field<byte[]>("EncryptedBlob");
            fileDownload.FileHash = ds.Tables[0].Rows[0].Field<byte[]>("FileHash");
            fileDownload.Name = file.FileName;
            return fileDownload;
        }

        public FileList GetFiles()
        {
            IncomingWebRequestContext request = WebOperationContext.Current.IncomingRequest;
            WebHeaderCollection headers = request.Headers;
            var user = Authorize(headers);
            FileList fileList = new FileList();
            fileList.Files = new List<Core.DataTypes.File>();
            var db = new Database(_conn);
            var _params = new List<SqlParameter>();
            _params.Add(new SqlParameter("@UserID", user.Id));
            var ds = db.ExecStoredProc("[dbo].[File_SelectAllUserFiles]", _params.ToArray());
            var tb = ds.Tables[0];
            foreach (DataRow row in tb.Rows) 
            {
                var file = new Core.DataTypes.File();
                file.Name = row.Field<string>("FileName");
                file.UploadDate = row.Field<DateTime>("UploadDate");
                fileList.Files.Add(file);
            }
            return fileList;
        }

        private User Authorize(WebHeaderCollection headers)
        {
            string authToken = headers.Get("Authorization");
            var userAndPass = Encoding.UTF8.GetString(Convert.FromBase64String(authToken.Remove(0, 6))).Split(':');
            string userName = userAndPass[0];

            

            var db = new Database(_conn);
            var _params = new List<SqlParameter>();
            _params.Add(new SqlParameter("@UserName", userName));
            _params.Add(new SqlParameter("@HashPassword", authToken));

            var ds = db.ExecStoredProc("dbo.User_Login", _params.ToArray());
            int id = ds.Tables[0].Rows[0].Field<int>("UserId");
            string username = ds.Tables[0].Rows[0].Field<string>("UserName");
            int logstatus = ds.Tables[0].Rows[0].Field<int>("LogStatus");

            if (logstatus != 1) throw new WebFaultException<string>("Wrong username or password", HttpStatusCode.Unauthorized);

            var user = new User(username, id);
            return user;
        }

        private void InsertFile(Core.File file) 
        {
            var db = new Database(_conn);
            var _params = new List<SqlParameter>();
            _params.Add(new SqlParameter("@UserID", file.Owner.Id));
            _params.Add(new SqlParameter("@FileHash", file.FileHash));
            _params.Add(new SqlParameter("@FileName", file.FileName));
            _params.Add(new SqlParameter("@EncryptedBlob", file._blob));
            db.ExecStoredProc("dbo.File_Insert", _params.ToArray());
        }

    }
}
