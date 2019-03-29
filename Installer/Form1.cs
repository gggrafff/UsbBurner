using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Management.Instrumentation;
using System.Security.Cryptography;

namespace Installer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Dictionary<string, string> ids  = IDS();
            kod.Text= GetMd5Hash(ids["KeyBoardID"]+ ids["UUID"] + ids["OSSerialNumber"]);
            //fhsd.Text = GetMd5Hash(kod.Text);

        }
        /*public static string ProcID()
        {
            using (System.Management.ManagementClass theClass = new System.Management.ManagementClass("Win32_Processor"))
            {
                System.Management.ManagementObjectCollection theCollectionOfResults = theClass.GetInstances();

                foreach (System.Management.ManagementObject currentResult in theCollectionOfResults)
                {
                    if (currentResult["ProcessorID"] != null)

                        return currentResult["ProcessorID"].ToString();
                }
            }
            return string.Empty;
        }*/

        public static Dictionary<string, string> IDS()
        {
            Dictionary<string, string> ids =
            new Dictionary<string, string>();

            ManagementObjectSearcher searcher;

            //процессор
            searcher = new ManagementObjectSearcher("root\\CIMV2",
                   "SELECT * FROM Win32_Processor");
            foreach (ManagementObject queryObj in searcher.Get()) { 
                ids.Add("ProcessorId", queryObj["ProcessorId"].ToString()); break;
            }

            //мать
            searcher = new ManagementObjectSearcher("root\\CIMV2",
                   "SELECT * FROM CIM_Card");
            foreach (ManagementObject queryObj in searcher.Get()) { 
                ids.Add("CardID", queryObj["SerialNumber"].ToString()); break;
            }

            //клавиатура
            searcher = new ManagementObjectSearcher("root\\CIMV2",
                   "SELECT * FROM CIM_KeyBoard");
            foreach (ManagementObject queryObj in searcher.Get())
            {
                ids.Add("KeyBoardID", queryObj["DeviceId"].ToString()); break;
            }
            //ОС
            searcher = new ManagementObjectSearcher("root\\CIMV2",
                   "SELECT * FROM CIM_OperatingSystem");
            foreach (ManagementObject queryObj in searcher.Get()) { 
                ids.Add("OSSerialNumber", queryObj["SerialNumber"].ToString()); break;
        }

        //мышь
        searcher = new ManagementObjectSearcher("root\\CIMV2",
                   "SELECT * FROM Win32_PointingDevice");
            foreach (ManagementObject queryObj in searcher.Get()) { 
                ids.Add("MouseID", queryObj["DeviceID"].ToString()); break;
            }

            //звук
            searcher = new ManagementObjectSearcher("root\\CIMV2",
                   "SELECT * FROM Win32_SoundDevice");
            foreach (ManagementObject queryObj in searcher.Get()) { 
                ids.Add("SoundCardID", queryObj["DeviceID"].ToString()); break;
            }

            //CD-ROM
            searcher = new ManagementObjectSearcher("root\\CIMV2",
                   "SELECT * FROM Win32_CDROMDrive");
            foreach (ManagementObject queryObj in searcher.Get()) { 
                ids.Add("CDROMID", queryObj["DeviceID"].ToString()); break;
            }

            //UUID
            searcher = new ManagementObjectSearcher("root\\CIMV2",
                   "SELECT UUID FROM Win32_ComputerSystemProduct");
            foreach (ManagementObject queryObj in searcher.Get()) { 
                ids.Add("UUID", queryObj["UUID"].ToString()); break;
            }

            return ids;
        }
        static string GetMd5Hash(string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            
            //using (MD5 md5Hash = MD5.Create())
            //{
               string hash = GetMd5Hash(kod.Text);
            //}
            if (hash != fhsd.Text) {
                MessageBox.Show("Ключ неверный");

            } else
            {
                Directory.CreateDirectory(folder.Text);
                File.WriteAllBytes(folder.Text+"\\USBBurner.exe", Installer.Properties.Resources.USBBurner);
                ShortCut.Create(folder.Text + "\\USBBurner.exe", Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\USBburner.lnk", "", "USBburner");
                Microsoft.Win32.RegistryKey key;
                key = Microsoft.Win32.Registry.LocalMachine.CreateSubKey("software\\usbmf");
                key.SetValue("key", hash);
                key.Close();
                MessageBox.Show("Спасибо за установку.");
                this.Close();
            }
            //Installer.Properties.Resources.USBBurner.
        }

        private void label1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.mosflash.ru");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) folder.Text = folderBrowserDialog1.SelectedPath;
            if (folder.Text.Last() == '\\') folder.Text += "MosFlash"; else folder.Text += "\\MosFlash";
        }
    }
}
