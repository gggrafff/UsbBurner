using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections;
using System.IO;
using System.Management;
using System.Management.Instrumentation;
using System.Security.Cryptography;

namespace USBBurner
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            startvar = false;
            //tableLayoutPanel1.VerticalScroll.Visible = true;
            //tableLayoutPanel1.scroll

        }
        //VScrollBar scr = new VScrollBar();
        private void open_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) folder.Text = folderBrowserDialog1.SelectedPath;
        }

        private const int WM_DEVICECHANGE = 0x0219;
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        private const int DBT_DEVICEARRIVAL = 0x8000;
        private const uint DBT_DEVTYP_VOLUME = 0x00000002;

        bool startvar;
        Button but = new Button();
        List<device> listdevice=new List<device>();
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_DEVICECHANGE && startvar)
            {
                switch (m.WParam.ToInt32())
                {
                    case (DBT_DEVICEARRIVAL):
                        {

                            // Код поиска имеющихся носителей
                            listdevice.Add(new device(m,files1));
                            
                            listdevice.Last().id = listdevice.Count - 1;
                            listdevice.Last().fail += Form1_fail;
                            listdevice.Last().labelt = name.Text;
                            listdevice.Last().FromDir = (new DirectoryInfo(folder.Text)).FullName;
                            BitArray bArray = new BitArray(new byte[]
                            {
                                (byte)(listdevice.Last().dbcv.dbcv_unitmask & 0x00FF),
                                (byte)((listdevice.Last().dbcv.dbcv_unitmask & 0xFF00) >> 8),
                                (byte)((listdevice.Last().dbcv.dbcv_unitmask & 0xFF0000) >> 16),
                                (byte)((listdevice.Last().dbcv.dbcv_unitmask & 0xFF000000) >> 24)
                            });
                            int driveLetter = Char.ConvertToUtf32("A", 0);
                            for (int i = 0; i < bArray.Length; i++)
                            {
                                if (bArray.Get(i))
                                {
                                    listdevice.Last().setLetter(Char.ConvertFromUtf32(driveLetter));
                                }
                                driveLetter += 1;
                            }


                            tableLayoutPanel1.Controls.Add(listdevice.Last().nabor.Letter);
                            tableLayoutPanel1.Controls.Add(listdevice.Last().nabor.info);
                            //tableLayoutPanel1.Controls.Add(listdevice.Last().nabor.progress);
                            tableLayoutPanel1.Controls.Add(listdevice.Last().nabor.retrypanel);
                            tableLayoutPanel1.Controls.Add(listdevice.Last().nabor.retrypanel2);
                            //tableLayoutPanel1.SetRowSpan(scr, tableLayoutPanel1.RowCount);
                            //tableLayoutPanel1.RowCount++;
                            //listdevice.Last().Progress += Form1_Progress;
                            //((ProgressBar)tableLayoutPanel1.Controls[2]).Value = 75;
                            listdevice.Last().process(ALL_FILES_LENGTH,checkBox1.Checked);
                            //MessageBox.Show("000");
                        }
                        break;
                    case (DBT_DEVICEREMOVECOMPLETE):
                        {
                            // Код поиска имеющихся носителей
                            device.DEV_BROADCAST_VOLUME dbcv = (device.DEV_BROADCAST_VOLUME)Marshal.PtrToStructure(m.LParam, typeof(device.DEV_BROADCAST_VOLUME));
                            
                            BitArray bArray = new BitArray(new byte[]
                            {
                                (byte)(dbcv.dbcv_unitmask & 0x00FF),
                                (byte)((dbcv.dbcv_unitmask & 0xFF00) >> 8),
                                (byte)((dbcv.dbcv_unitmask & 0xFF0000) >> 16),
                                (byte)((dbcv.dbcv_unitmask & 0xFF000000) >> 24)
                            });
                            int driveLetter = Char.ConvertToUtf32("A", 0);
                            for (int i = 0; i < bArray.Length; i++)
                            {
                                if (bArray.Get(i))
                                {
                                    foreach(device dev in listdevice)
                                    {
                                        if (dev.Letter == Char.ConvertFromUtf32(driveLetter)) {
                                            /*for(int j= listdevice.IndexOf(dev); j< listdevice.Count-1; j++)
                                            {
                                                //tableLayoutPanel1.Controls[0].
                                                //tableLayoutPanel1.Controls.Add(tableLayoutPanel1.Controls[j*3+3],0,j);
                                                //tableLayoutPanel1.Controls.Add(tableLayoutPanel1.Controls[j * 3 + 4], 1, j);
                                                //tableLayoutPanel1.Controls.Add(tableLayoutPanel1.Controls[j * 3 + 5], 2, j);
                                            }
                                            tableLayoutPanel1.Controls.RemoveAt(tableLayoutPanel1.Controls.Count);
                                            tableLayoutPanel1.Controls.RemoveAt(tableLayoutPanel1.Controls.Count);
                                            tableLayoutPanel1.Controls.RemoveAt(tableLayoutPanel1.Controls.Count);*/
                                            tableLayoutPanel1.Controls.RemoveAt(dev.id*4);
                                            tableLayoutPanel1.Controls.RemoveAt(dev.id * 4);
                                            tableLayoutPanel1.Controls.RemoveAt(dev.id * 4);
                                            tableLayoutPanel1.Controls.RemoveAt(dev.id * 4);
                                            foreach (device devi in listdevice) if (devi.id > dev.id) devi.id--;
                                            listdevice.Remove(dev);
                                            break; }
                                    }
                                } 
                                driveLetter += 1;
                            }


                            //MessageBox.Show("111");
                        }
                        break;
                }
            }
            base.WndProc(ref m);
        }

        private void Form1_fail(object sender)
        {
            if (tableLayoutPanel1.InvokeRequired) tableLayoutPanel1.Invoke(new Action<int>((s) => { //tableLayoutPanel1.Controls.RemoveAt(s*3+2);
                //tableLayoutPanel1.Controls.Add(((device)sender).nabor.retrypanel, 2, s);
                //((Panel)tableLayoutPanel1.Controls[s * 3 + 2]).Controls.Add(((device)sender).nabor.status);
                //((Panel)tableLayoutPanel1.Controls[s * 3 + 2]).Controls.Add(((device)sender).nabor.retry);
                try { ((Button)((Panel)tableLayoutPanel1.Controls[s * 4 + 3]).Controls[1]).Visible = true;
                    //((Button)tableLayoutPanel1.Controls[s * 4 + 3]).Visible = true;
                }
                catch { return; }//.Location = new System.Drawing.Point(0, 60);
                //((Label)((Panel)tableLayoutPanel1.Controls[s * 3 + 2]).Controls[0]).Location = new System.Drawing.Point(0, 0);
            }), ((device)sender).id);
            else
            {
                //tableLayoutPanel1.Controls.RemoveAt(((device)sender).id * 3+2);
                //tableLayoutPanel1.Controls.Add(((device)sender).nabor.retrypanel, 2, ((device)sender).id);
                //((Panel)tableLayoutPanel1.Controls[((device)sender).id * 3 + 2]).Controls.Add(((device)sender).nabor.status);
                //((Panel)tableLayoutPanel1.Controls[((device)sender).id * 3 + 2]).Controls.Add(((device)sender).nabor.retry);
                //((Button)((Panel)tableLayoutPanel1.Controls[((device)sender).id * 3 + 2]).Controls[1]).Location = new System.Drawing.Point(0, 60);
                //((Label)((Panel)tableLayoutPanel1.Controls[((device)sender).id * 3 + 2]).Controls[0]).Location = new System.Drawing.Point(0, 0);
                try{ ((Button)((Panel)tableLayoutPanel1.Controls[((device)sender).id * 4 + 3]).Controls[1]).Visible = true;
                    //((Button)tableLayoutPanel1.Controls[((device)sender).id * 4 + 3]).Visible = true;
                }
                catch { return; }
            }
           
        }

        //delegate void changeprogress(int pr); 
        /* private void Form1_Progress(object sender, progressargs e)
         {
             //((ProgressBar)tableLayoutPanel1.Controls[e.progressnum]).Value = 50;// e.current;textBox1.Invoke(new Del((s) => textBox1.Text = s), "newText");
             //((ProgressBar)tableLayoutPanel1.Controls[e.progressnum]).Invoke(new changeprogress((pr) => ((ProgressBar)tableLayoutPanel1.Controls[e.progressnum])).Value = pr), 50);
             if (((ProgressBar)tableLayoutPanel1.Controls[e.progressnum]).InvokeRequired) ((ProgressBar)tableLayoutPanel1.Controls[e.progressnum]).Invoke(new Action<int>((s) => ((ProgressBar)tableLayoutPanel1.Controls[e.progressnum]).Value = s), 50);
             else ((ProgressBar)tableLayoutPanel1.Controls[e.progressnum]).Value = 50;

         }*/


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
            foreach (ManagementObject queryObj in searcher.Get())
            {
                ids.Add("ProcessorId", queryObj["ProcessorId"].ToString()); break;
            }

            //мать
            searcher = new ManagementObjectSearcher("root\\CIMV2",
                   "SELECT * FROM CIM_Card");
            foreach (ManagementObject queryObj in searcher.Get())
            {
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
            foreach (ManagementObject queryObj in searcher.Get())
            {
                ids.Add("OSSerialNumber", queryObj["SerialNumber"].ToString()); break;
            }

            //мышь
            searcher = new ManagementObjectSearcher("root\\CIMV2",
                       "SELECT * FROM Win32_PointingDevice");
            foreach (ManagementObject queryObj in searcher.Get())
            {
                ids.Add("MouseID", queryObj["DeviceID"].ToString()); break;
            }

            //звук
            searcher = new ManagementObjectSearcher("root\\CIMV2",
                   "SELECT * FROM Win32_SoundDevice");
            foreach (ManagementObject queryObj in searcher.Get())
            {
                ids.Add("SoundCardID", queryObj["DeviceID"].ToString()); break;
            }

            //CD-ROM
            searcher = new ManagementObjectSearcher("root\\CIMV2",
                   "SELECT * FROM Win32_CDROMDrive");
            foreach (ManagementObject queryObj in searcher.Get())
            {
                ids.Add("CDROMID", queryObj["DeviceID"].ToString()); break;
            }

            //UUID
            searcher = new ManagementObjectSearcher("root\\CIMV2",
                   "SELECT UUID FROM Win32_ComputerSystemProduct");
            foreach (ManagementObject queryObj in searcher.Get())
            {
                ids.Add("UUID", queryObj["UUID"].ToString()); break;
            }

            return ids;
        }
        ulong ALL_FILES_LENGTH = 0;
        List<string> files1 = new List<string>();
        List<string> files3 = new List<string>();
        private void start_Click(object sender, EventArgs e)
        {
            //startvar = !startvar;
            //MessageBox.Show("knopka nazhata");
            Microsoft.Win32.RegistryKey key;
            Dictionary<string, string> ids = IDS();
            key = Microsoft.Win32.Registry.LocalMachine.CreateSubKey("software\\usbmf");
            if (key.GetValue("key") == null)
            {
                key.Close();
                MessageBox.Show("Ваша копия программы не зарегистрирована.");
                this.Close();
                return;
            } else 
            if (key.GetValue("key").ToString()!=GetMd5Hash(GetMd5Hash(ids["KeyBoardID"] + ids["UUID"] + ids["OSSerialNumber"]))) { 
            key.Close();
            MessageBox.Show("Ваша копия программы не зарегистрирована.");
            this.Close();
                return;
            }
            key.Close();
            //MessageBox.Show("licenziya proverena");
            switch (!startvar)
            {
                case true:
            start.Text = "Стоп";
                    folder.Enabled = false;
                    name.Enabled = false;
                    checkBox1.Enabled = false;
                    //if (checkBox1.Checked)
                    //{
                    scan.Text = "Сканирование директории. ";
                        scan.Visible = true;
                    files1 = new List<string>();
                    files3 = new List<string>();
                    (new System.Threading.Thread(delegate ()
                        {
                            //total = device.GetDirectorySizeSmall(new DirectoryInfo(folder.Text));
                            ALL_FILES_LENGTH=device.SborSpiskaFailov(folder.Text, files1, files3);
                            if (files3.Count > 0)
                            {
                                string mess = "Следующие файлы не будут откопированы из-за недопустимого размера:\n";
                                foreach (string fi in files3) mess += fi + "\n";
                                mess += "Всё равно хотите продолжить запись?";
                                DialogResult dr=MessageBox.Show(mess,"Файлы недопустимого размера",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
                                if (dr==DialogResult.No)
                                {
                                    start.Invoke(new Action(() => { start.Text = "Старт";
                                    folder.Enabled = true;
                                    name.Enabled = true;
                                    checkBox1.Enabled = true;}));
                                    
                                    return;
                                }
                            }
                            if (scan.InvokeRequired) scan.Invoke(new Action<bool>((s) => scan.Visible = s), false);
                            else scan.Visible =false;
                            startvar = !startvar;
                        })).Start();
                    //}
                    //else total = 0;
            break;
            case false:
                    startvar = !startvar;
                    start.Text = "Старт";
                    folder.Enabled = true;
                    name.Enabled = true;
                    checkBox1.Enabled = true;
                    break;
            }
            //MessageBox.Show("poehali");
        }
        //ulong total;

        private void label1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.mosflash.ru");
        }

        private void tableLayoutPanel1_ControlAdded(object sender, ControlEventArgs e)
        {

            if (scan.InvokeRequired) scan.Invoke(new Action(() => { tableLayoutPanel1.Height = tableLayoutPanel1.PreferredSize.Height; }));
            else tableLayoutPanel1.Height = tableLayoutPanel1.PreferredSize.Height;
            if (scan.InvokeRequired) scan.Invoke(new Action(() =>
            {
                //tableLayoutPanel1.Height = tableLayoutPanel1.PreferredSize.Height;
                scan.Text = "Всего " + tableLayoutPanel1.Controls.Count / 4 + " процессов.";
                if (startvar) scan.Visible = true;
            }));
            else
            {
                // tableLayoutPanel1.Height = tableLayoutPanel1.PreferredSize.Height;
                scan.Text = "Всего " + tableLayoutPanel1.Controls.Count / 4 + " процессов.";
                if (startvar) scan.Visible = true;
            }
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
            if (tableLayoutPanel1.InvokeRequired) tableLayoutPanel1.Invoke(new Action(() =>
            {
                tableLayoutPanel1.Height = tableLayoutPanel1.PreferredSize.Height;
                //scan.Text = "Всего " + tableLayoutPanel1.RowCount.ToString() + " процессов.";
                //if (startvar) scan.Visible = true;
            }));
            else { tableLayoutPanel1.Height = tableLayoutPanel1.PreferredSize.Height;
                //scan.Text = "Всего " + tableLayoutPanel1.RowCount.ToString() + " процессов.";
                //if (startvar) scan.Visible = true;
            }

        }

        private void tableLayoutPanel1_ControlRemoved(object sender, ControlEventArgs e)
        {
            if (scan.InvokeRequired) scan.Invoke(new Action(() =>
            {
                //tableLayoutPanel1.Height = tableLayoutPanel1.PreferredSize.Height;
                scan.Text = "Всего " + tableLayoutPanel1.Controls.Count/3 + " процессов.";
                if (startvar) scan.Visible = true;
            }));
            else
            {
               // tableLayoutPanel1.Height = tableLayoutPanel1.PreferredSize.Height;
                scan.Text = "Всего " + tableLayoutPanel1.Controls.Count / 3 + " процессов.";
                if (startvar) scan.Visible = true;
            }
        }

        /*private void tableLayoutPanel1_PaddingChanged(object sender, EventArgs e)
        {
            tableLayoutPanel1.Height = tableLayoutPanel1.PreferredSize.Height;
        }*/

        /*private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            tableLayoutPanel1.VerticalScroll.Value = vScrollBar1.Value;
        }*/

        /* private void tableLayoutPanel1_ClientSizeChanged(object sender, EventArgs e)
         {
             vScrollBar1.Maximum = tableLayoutPanel1.VerticalScroll.Maximum;
         }*/

        /*private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            tableLayoutPanel1.VerticalScroll.Value = vScrollBar1.Value;
            
        }*/

        /*private void tableLayoutPanel1_ControlAdded(object sender, ControlEventArgs e)
        {
            vScrollBar1.Maximum = tableLayoutPanel1.VerticalScroll.Maximum;
        }*/
    }
}
