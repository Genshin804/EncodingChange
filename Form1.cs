using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EnCodingChange
{
    public partial class Form1 : Form
    {        
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            txtPath.Text = folderBrowserDialog1.SelectedPath;
        }

        private void btn_Change_Click(object sender, EventArgs e)
        {
            List<string> ResultList = new List<string>();
            string strPath = txtPath.Text;

            if (string.IsNullOrEmpty(strPath))
            {
                MessageBox.Show("請填寫路徑");
                return;
            }
            GetDirectory(strPath);
        }

        /// <summary>
        /// 檢查檔案編碼
        /// </summary>
        /// <param name="path">路徑</param>
        /// <returns></returns>
        public string CheckEncoding(string path)
        {
            string vstrFormFile = path;
            using (FileStream fs = File.Open(vstrFormFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.GetEncoding(950), true))
                {
                    sr.Read();
                    sr.Close();

                    return sr.CurrentEncoding.BodyName.ToString();
                }
                fs.Close();                
            }           
        }

        private void GetDirectory(string path)
        {
            List<string> ResultList = new List<string>();
            DirectoryInfo d = new DirectoryInfo(path);

            string strExtension = this.txtExtension.Text;
            string strKeyword = this.txtKeyword.Text;

            var files = d.GetFiles().Where(x => x.Extension.Contains(strExtension) && x.Name.Contains(strKeyword)).ToList();
            //MessageBox.Show(files.Count().ToString());
            if(files.Count() != 0)
            {
                this.txtMsg.Text += "當前發現需轉換檔案：" + files.Count().ToString() + "個\r\n";

                foreach(var file in files)
                {
                    var type = CheckEncoding(file.FullName);

                    this.txtMsg.Text += "檔案：" + file.FullName.ToString() + "\r\n編碼:" + type;

                    if (type.ToUpper().Contains("BIG5"))
                    {
                        ChangeEncoding(file.FullName, Encoding.GetEncoding(950));

                        this.txtMsg.Text += "\r\n已轉換 UTF8";
                    }
                    else
                    {
                        this.txtMsg.Text += "\r\n未轉換";
                    }
                }                
            }
            else
            {
                this.txtMsg.Text += "無需要轉換的檔案\r\n";
            }
        }

        private void ChangeEncoding(string filename, Encoding encoding)
        {
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            byte[] flieByte = new byte[fs.Length];
            fs.Read(flieByte, 0, flieByte.Length);
            fs.Close();

            StreamWriter docWriter;
            Encoding ec = Encoding.GetEncoding("UTF-8");
            docWriter = new StreamWriter(filename, false, ec);
            docWriter.Write(encoding.GetString(flieByte));
            docWriter.Close();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //var a = CodePagesEncodingProvider.Instance.GetEncoding(950);
            //var b = Encoding.GetEncoding(950);
            //this.txtMsg.Text += $"Name={b.EncodingName}, CodePage={b.CodePage}";

            //var encodings = Encoding.GetEncodings();

            //foreach(var item in encodings)
            //{
            //    Console.WriteLine($"DisplayNameite={item.DisplayName}, CodePage={item.CodePage}");
            //    txtMsg.Text += $"DisplayNameitem={item.DisplayName}, CodePage={item.CodePage} \r\n";
            //}
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var encodings = Encoding.GetEncodings();

            foreach (var item in encodings)
            {
                txtMsg.Text += $"DisplayNameitem={item.DisplayName}, CodePage={item.CodePage} \r\n";
            }

            txtMsg.Text += $"Count={encodings.Count()}";
        }
    }
}
