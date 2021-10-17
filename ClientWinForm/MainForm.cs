using Core;
using Core.DataTypes;
using Crypto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace ClientWinForm
{
    public partial class MainForm : Form
    {
        private static byte[] rc6MainKey = new byte[] { 00, 01, 02, 03, 04, 05, 06, 07, 08, 09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f };
        private static int rc6KeyLong = 128;

        private static RC6 rc6 = new RC6(rc6KeyLong, rc6MainKey);
        private static byte[] rc6FileKey = new byte[] { 00, 01, 02, 03, 04, 05, 06, 07, 08, 09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f };


        private static byte[] a5MainKey = new byte[] { 0x12, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF };
        private static A5Enc a5 = new A5Enc();

        private static byte[] cbcMainKey = new byte[] { 0x10, 0x24, 0x45, 0x87, 0x89, 0x1B, 0xCD, 0xCF };
        private static CBC cbc = new CBC();

        private static string selectedAlgorithm;

        public HttpService service;
        public User owner;
        public UserFiles userFiles;
        public MainForm()
        {
            Uri baseAddress = new Uri("http://localhost/Service.svc/Http/");
            service = new HttpService(baseAddress);
            owner = new User("milos", "12345");
            if (System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(),owner.Username)))
            {
                var fileRc6 = new RC6(rc6KeyLong, rc6FileKey); 
                byte[] encoded = System.IO.File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), owner.Username));
                byte[] decoded = fileRc6.DecodeRc6(encoded);
                
                DataContractSerializer dcs = new DataContractSerializer(typeof(UserFiles));
                using (XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(decoded, new XmlDictionaryReaderQuotas()))
                {
                    userFiles = (UserFiles)dcs.ReadObject(reader);
                }
            }
            else
                userFiles = null;
            InitializeComponent();
        }

        private void btnLoadUserFiles_Click(object sender, EventArgs e)
        {
            dgwFilesList.AutoGenerateColumns = true;
            dgwFilesList.DataSource = null;
            dgwFilesList.DataSource = service.GetFilesList(owner).Files;
            dgwFilesList.Refresh();
        }

        private void btnDownloadAndOpen_Click(object sender, EventArgs e)
        {
            var anyRowSelected = dgwFilesList.SelectedRows;
            if (anyRowSelected == null || anyRowSelected.Count == 0)
            {
                MessageBox.Show("No file selected, please select a file by clicking on theS triangle icon at the start of the row!");
                return;
            }
            var row = dgwFilesList.SelectedRows[0];
            var fileName = row.Cells[0].Value.ToString();
            var fileHash = userFiles?.fileHashes.Find(x=> x.Username == owner.Username && x.Filename == fileName);

            var file = new Core.File(owner, null, fileHash.Hash, fileName);
            var fileTransfer = service.DownloadFile(file, owner);
            if (!fileTransfer.FileHash.SequenceEqual( fileHash.Hash))
            {
                DialogResult dialogResult = MessageBox.Show("Hashes do not match! Downloading and opening this file might be harmful! Are you sure you wish to continue?", "Are you sure you want to continue?", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.No)
                    return;
            }
            var decoded = Decode(fileHash.Algorithm, fileHash.Key, fileTransfer.EncodedBlob);
            var pathParts = fileName.Split('\\');
            var lastPathPart = pathParts[pathParts.Length - 1];
            var newPath = Path.Combine(Directory.GetCurrentDirectory(), lastPathPart);
            System.IO.File.WriteAllBytes(newPath, decoded);
            OpenWithDefaultProgram(newPath);
        }

        private void btnUploadNewFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"C:\",
                Title = "Browse Text Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "txt",
                Filter = "txt files (*.txt)|*.txt",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog1.FileName;
                using (StreamReader sr = System.IO.File.OpenText(fileName))
                {
                    string fileContent = sr.ReadToEnd();
                    byte[] encoded = Encode(Encoding.UTF8.GetBytes(fileContent));
               
                    CRC crc = new CRC();
                    var hashBytes = crc.ComputeHash(Encoding.UTF8.GetBytes(fileContent));
                    if (userFiles == null) userFiles = new UserFiles();
                    if (userFiles.fileHashes == null) userFiles.fileHashes = new List<EncryptionData>();

                    userFiles.SetEncryptionData(owner.Username, fileName, selectedAlgorithm, SelectedEncKey(), hashBytes);

                    Core.File file = new Core.File(owner, encoded, hashBytes, fileName);
                    service.UploadFile(file, owner);
                    dgwFilesList.DataSource = service.GetFilesList(owner).Files;
                    Task.Run(() => UpdateUserFiles());

                }
            }

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
             Task.Run(() => UpdateUserFiles());

        }

        private void UpdateUserFiles()
        {

            //delete old file 
            if (owner != null && System.IO.File.Exists(owner.Username))
            {
                System.IO.File.Delete(owner.Username);
                System.Threading.Thread.Sleep(200);
            }

            //write to a new file
            if (userFiles != null)
            {
                DataContractSerializer dcs = new DataContractSerializer(typeof(UserFiles));
                MemoryStream stream = new MemoryStream();
                dcs.WriteObject(stream, userFiles);
                var unencoded = stream.ToArray();
                var fileRc6 = new RC6(rc6KeyLong, rc6FileKey);
                byte[] encoded = fileRc6.EncodeRc6(unencoded);
                System.IO.File.WriteAllBytes(Path.Combine(Directory.GetCurrentDirectory(), owner.Username), encoded);
            }
        }

        private void cmbSelectEncryption_SelectedIndexChanged(object sender, EventArgs e)
        {
            var newSelection = cmbSelectEncryption.SelectedItem.ToString();
            if (newSelection != selectedAlgorithm) 
            {
                selectedAlgorithm = newSelection;
            }

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            selectedAlgorithm = "RC6";
            cmbSelectEncryption.DataSource = new[] { "RC6", "A5/1", "CBC" };
            cmbSelectEncryption.SelectedIndex = 0;
        }

        private static void OpenWithDefaultProgram(string path)
        {
            Process fileopener = new Process();
            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = "\"" + path + "\"";
            fileopener.Start();
        }

        private static byte[] Encode(byte[] blob) 
        {
            byte[] encoded = new byte[] { };
            switch (selectedAlgorithm) 
            {
                case "RC6":
                    rc6 = new RC6(rc6KeyLong, rc6MainKey);
                    encoded = rc6.EncodeRc6(blob);
                    break;
                case "A5/1":
                    encoded = a5.A5Encyptor(blob, a5MainKey);
                    break;
                case "CBC":
                    encoded = cbc.encode(blob, cbcMainKey);
                    break;
                default:
                    break;
            };
            return encoded;
        }

        private static byte[] Decode(string selectedAlgorithm, byte[] key, byte[] blob)
        {
            byte[] decoded = new byte[] { };
            switch (selectedAlgorithm)
            {
                case "RC6":
                    rc6 = new RC6(rc6KeyLong, key);
                    decoded = rc6.DecodeRc6(blob);
                    break;
                case "A5/1":
                    decoded = a5.A5Encyptor(blob, key);
                    break;
                case "CBC":
                    decoded = cbc.decode(blob, key);
                    break;
                default:
                    break;
            }
            return decoded;

        }

        private static byte[] SelectedEncKey()
        {
            switch (selectedAlgorithm)
            {
                case "RC6":
                    return rc6MainKey;
                case "A5/1":
                    return a5MainKey;
                case "CBC":
                    return cbcMainKey;
                default:
                    return new byte[] { };
            }
        }
    }
}
