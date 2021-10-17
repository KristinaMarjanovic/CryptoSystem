using Core.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        private string _password { get; set; }
        public FileList FileList { get; }

        public User(string Username, string Password)
        {
            this.Username = Username;
            this._password = Password;
            this.FileList = new FileList();
            this.FileList.Files = new List<DataTypes.File>();
        }

        public User(string Username, int Id)
        {
            this.Username = Username;
            this.Id = Id;

        }

        public string AuthorizationToken()
        {
            var result = string.Format("Basic {0}", Convert.ToBase64String(Encoding.UTF8.GetBytes(
                string.Format("{0}:{1}", Username, _password))));

            return result;
        }

        public void AddFile(DataTypes.File file)
        {
            FileList.Files.Add(file);
        }

        public void AddFiles(List<DataTypes.File> files)
        {
            FileList.Files.AddRange(files);
        }
    }
}
