using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Management;
using System.Management.Instrumentation;
using System.Collections;
using Microsoft.Win32;
using System.Threading;
using Metasharp;
using System.Drawing;

namespace USBBurner
{
    public class progressargs
    {
        public int current;
        public int all;
        public int progressnum;
        public progressargs(int current, int all, int progressnum)
        {
            this.current = current;
            this.all = all;
            this.progressnum = progressnum;
        }
    }
    class device
    {
        public struct DEV_BROADCAST_VOLUME
        {
            public uint dbcv_size;
            public uint dbcv_devicetype;
            public uint dbcv_reserved;
            public uint dbcv_unitmask;
            public ushort dbcv_flags;
        }
        public DEV_BROADCAST_VOLUME dbcv {
            get; set; }
        public string Letter;

        public void setLetter(string Letter) { this.Letter = Letter;
                DriveInfo[] drives = DriveInfo.GetDrives();
                foreach (DriveInfo dr in drives) if (dr.Name == Letter+":\\") drive = dr;
                nabor.Letter.Text = Letter + @":\";
            }
        public string getLetter() { return Letter; }
        
        public DriveInfo drive { set; get; }
        public device(Message m,List<string> files1)
        {
            this.dbcv = (DEV_BROADCAST_VOLUME)Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_VOLUME));
            nabor = new elementy();
            nabor.info.Click += Info_Click;
            this.total = 0;
            nabor.retry.Click += Retry_Click;
            this.files1 = files1;
        }

        private void Retry_Click(object sender, EventArgs e)
        {
            process(total,proverka);
        }

        public elementy nabor { get; set; }
        
        private void Info_Click(object sender, EventArgs e)
        {
            string DriveDescr = String.Format("Название: {0}", drive.Name) + Environment.NewLine + String.Format("Тип диска: {0}", drive.DriveType) + Environment.NewLine;

            if (drive.IsReady)
            {
                DriveDescr += String.Format("Объем диска: {0:f2}", (float)drive.TotalSize / 1024 / 1024 / 1024) + " Гб" + Environment.NewLine;
                DriveDescr += String.Format("Общее свободное пространство: {0:f2}", (float)drive.TotalFreeSpace / 1024 / 1024 / 1024) + " Гб" + Environment.NewLine;
                DriveDescr += String.Format("Доступное свободное пространство: {0:f2}", (float)drive.AvailableFreeSpace / 1024 / 1024 / 1024) + " Гб" + Environment.NewLine;
                DriveDescr += String.Format("Метка: {0}", drive.VolumeLabel) + Environment.NewLine;
                DriveDescr += String.Format("Файловая система: {0}", drive.DriveFormat) + Environment.NewLine;
            }
            MessageBox.Show(DriveDescr,"Информация об устройстве",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }
        public ManagementOperationObserver watcherformat = new ManagementOperationObserver();
        /*public void process2(ulong total)
        {
            try { 
            if (total!=0) this.total = total;
            ManagementObjectSearcher searcher=new ManagementObjectSearcher();
            
            //watcherformat.Progress += Watcher_Progress;
            watcherformat.Completed += Watcherformat_Completed;

            new Thread(() => {
                if (FormatDrive(searcher, watcherformat, Letter + ":", "FAT32", true, 4096, labelt, false))
                { 
                    if (nabor.status.InvokeRequired) nabor.status.Invoke(new Action<string>((s) => nabor.status.Text = s), "Идёт форматирование. ");
                    else nabor.status.Text = "Идёт форматирование. ";
                }
                else { 
                    if (nabor.status.InvokeRequired) nabor.status.Invoke(new Action<string>((s) => nabor.status.Text = s), "Ошибка при попытке форматировать. ");
                    else nabor.status.Text = "Ошибка при попытке форматировать. ";
                }
            }).Start();            // Выполнить в новом потоке

                //if (FormatDrive(searcher,watcherformat, Letter+":", "NTFS", true, 4096, "MOSFLASH", false)) MessageBox.Show("Succesfull"); else MessageBox.Show("Fail");
            }
            catch (Exception e)
            {
                if (nabor.status.InvokeRequired) nabor.status.Invoke(new Action<string>((s) => nabor.status.Text += s), e.Message);
                else nabor.status.Text += e.Message;
                fail(this);
                return;
            }
        }*/

        /*      public void process(ulong total)
          {
              const byte Create = 2;
              const byte Open = 3;
              bool k;
              char[] bufw = new char[512];
              bufw[0] = 'a';
              bufw[1] = '7';
              bufw[2] = '3';
              bufw[3] = 'a';
              bufw[4] = '7';
              bufw[5] = '3';
              LDrive ld = new LDrive("\\\\.\\"+drive.Name[0]+":", Open);

              if (nabor.status.InvokeRequired) nabor.status.Invoke(new Action<string>((s) => nabor.status.Text += s), ld.Error().ToString());
              else nabor.status.Text += ld.Error().ToString();
              k = ld.Write(bufw, 512);
              k = ld.Seek(0, 0);
              if (nabor.status.InvokeRequired) nabor.status.Invoke(new Action<string>((s) => nabor.status.Text += s), ld.Error().ToString());
              else nabor.status.Text += ld.Error().ToString();
              k = ld.Seek(0, 0);
              if (nabor.status.InvokeRequired) nabor.status.Invoke(new Action<string>((s) => nabor.status.Text += s), ld.Error().ToString());
              else nabor.status.Text += ld.Error().ToString();
              char[] bufr = new char[10];
              k = ld.Read(bufr, 10);
              if (nabor.status.InvokeRequired) nabor.status.Invoke(new Action<string>((s) => nabor.status.Text += s), ld.Error().ToString());
              else nabor.status.Text += ld.Error().ToString();
              for (int n = 0; n < bufr.Length; n++)
                  Console.WriteLine(bufr[n]);
              Console.WriteLine(ld.N);
              Console.Read();
              ld.close();
              Console.Read();
          }*/
        bool proverka;
        public void process(ulong total,bool proverka)
        {
            /*nabor.progress.Refresh();
            nabor.progress.CreateGraphics().DrawString("asfdsdgsdg",
                new Font("Arial", (float)8.25, FontStyle.Regular),
                Brushes.Black, new PointF(nabor.progress.Width / 2 - 10,
                    nabor.progress.Height / 2 - 7));*/
            this.proverka=proverka;
                this.total = total;
            bool proveril = false;
                try
                {
                    if ((ulong)drive.TotalSize < total)
                    {
                        if (nabor.status.InvokeRequired) nabor.status.Invoke(new Action<string>((s) => nabor.status.Text = s), "Недостаточно места на флешке. ");
                        else nabor.status.Text = "Недостаточно места на флешке. ";
                    proveril = true;
                        fail(this);
                        return;
                    }
                }
                catch
                {
                    if (nabor.status.InvokeRequired) nabor.status.Invoke(new Action<string>((s) => nabor.status.Text = s), "Не удалось определить объём флешки. ");
                    else nabor.status.Text = "Не удалось определить объём флешки. ";
                }
            try
            {
                Process prc = new Process();
                //prc.Exited += Prc_Exited; ;
                new Thread(() => {
                    try
                    {
                        
                        if (Directory.GetFiles(drive.RootDirectory.FullName).Length==0 & Directory.GetDirectories(drive.RootDirectory.FullName).Length == 0)
                        {
                            if (nabor.status.InvokeRequired) nabor.status.Invoke(new Action<string>((s) => nabor.status.Text = s), "Переименование. ");
                            else nabor.status.Text = "Переименование. ";
                            if (!DriveManager.RenameDrive(prc, drive.Name[0], labelt))
                            {
                                if (nabor.status.InvokeRequired) nabor.status.Invoke(new Action<string>((s) => nabor.status.Text = s), "Ошибка при попытке форматировать. ");
                                else nabor.status.Text = "Ошибка при попытке форматировать. ";
                                throw new Exception();
                            }
                        }
                        else
                        {
                            if (nabor.status.InvokeRequired) nabor.status.Invoke(new Action<string>((s) => nabor.status.Text = s), "Форматирование. ");
                            else nabor.status.Text = "Форматирование. ";
                            if (!DriveManager.FormatDrive(prc, drive.Name[0], labelt, "FAT32"))
                            {
                                if (nabor.status.InvokeRequired) nabor.status.Invoke(new Action<string>((s) => nabor.status.Text = s), "Ошибка при попытке форматировать. ");
                                else nabor.status.Text = "Ошибка при попытке форматировать. ";
                                throw new Exception();
                            }
                        }

                    /*}
                    catch (Exception e)
                    {
                        if (nabor.status.InvokeRequired) nabor.status.Invoke(new Action<string>((s) => nabor.status.Text = "Ошибка при попытке форматировать. " + s), e.Message);
                        else nabor.status.Text = "Ошибка при попытке форматировать. " + e.Message;
                        fail(this);
                        Thread.CurrentThread.Abort();
                        return;
                    }
                    try { */
                    if ((ulong)drive.AvailableFreeSpace < total && proveril == false)
                        {
                            if (nabor.status.InvokeRequired) nabor.status.Invoke(new Action<string>((s) => nabor.status.Text = s), "Недостаточно места на флешке. ");
                            else nabor.status.Text = "Недостаточно места на флешке. ";
                            proveril = true;
                            fail(this);
                            Thread.CurrentThread.Abort();
                            return;
                        }
                        
                }
                    catch (Exception e)
            {
                if (nabor.status.InvokeRequired) nabor.status.Invoke(new Action<string>((s) => nabor.status.Text = "Не читается. " + s), e.Message);
                else nabor.status.Text = "Не читается. " + e.Message;
                fail(this);
                        Thread.CurrentThread.Abort();
                        return;
            }
                    //fail(this);
                    try { Prc_Exited(); }
                    catch (Exception e) {
                        if (nabor.status.InvokeRequired) nabor.status.Invoke(new Action<string>((s) => nabor.status.Text = "Ошибка при попытке копирования. " + s), e.Message);
                        else nabor.status.Text = "Ошибка при попытке копирования. " + e.Message;
                        fail(this);
                        Thread.CurrentThread.Abort();
                        return;
                    }
                }).Start();            // Выполнить в новом потоке
            }
            catch (Exception e)
            {
                if (nabor.status.InvokeRequired) nabor.status.Invoke(new Action<string>((s) => nabor.status.Text += s), e.Message);
                else nabor.status.Text += e.Message;
                fail(this);
                Thread.CurrentThread.Abort();
                return;
            }

        }
        List<string> files1;
            List<string> files2 = new List<string>();
        private void Prc_Exited()//object sender, EventArgs e)
        {
            if (nabor.status.InvokeRequired) nabor.status.Invoke(new Action<string>((s) => nabor.status.Text = s), "Копирование файлов. ");
            else nabor.status.Text = "Копирование файлов. ";
            //MessageBox.Show("format done");
            
            //MessageBox.Show("sbor spiska failov nachalo");
            //ALL_FILES_LENGTH = 0;
            CreateDirectoryes(FromDir, drive.RootDirectory.FullName);
            CreateFinishList(FromDir, drive.RootDirectory.FullName, files1, files2);
            

            //MessageBox.Show("sbor zavershen"+files1.Count+"      "+files2.Count);
            Copyring cop = new Copyring();
            //MessageBox.Show("nachalo kopirovaniya");
            cop.CopyFiles(files1.ToArray(), files2.ToArray(),nabor.progress,total);
            //MessageBox.Show("kopirovanie zaversheno");
            if (nabor.status.InvokeRequired) nabor.status.Invoke(new Action<string>((s) => nabor.status.Text = s), "Копирование завершено. ");
            else nabor.status.Text = "Копирование завершено. ";
       //     fail(this);
            if (proverka)
            {
                if (nabor.status.InvokeRequired) nabor.status.Invoke(new Action<string>((s) => nabor.status.Text = s), "Проверка.");
                else nabor.status.Text = "Проверка.";
                ulong totalresult = GetDirectorySizeSmall(drive.RootDirectory);
                if (totalresult == total)
                {
                    if (nabor.status.InvokeRequired) nabor.status.Invoke(new Action<string>((s) => nabor.status.Text = s), "Успешно записано. ");
                    else nabor.status.Text = "Успешно записано.";
                }
                else
                {
                    if (nabor.status.InvokeRequired) nabor.status.Invoke(new Action<string>((s) => nabor.status.Text = s), "Записано с ошибками. ");
                    else nabor.status.Text = "Записано с ошибками.";
                    fail(this);
                }
            }
        }

        ulong total;
        /*private void Watcherformat_Completed(object sender, CompletedEventArgs e)
        {
            
            if (nabor.status.InvokeRequired) nabor.status.Invoke(new Action<string>((s) => nabor.status.Text = s), "Форматирование завершено. Копирование файлов. ");
            else nabor.status.Text = "Форматирование завершено. Копирование файлов. ";
            CopyDir(FromDir, drive.RootDirectory.FullName);
            if (nabor.status.InvokeRequired) nabor.status.Invoke(new Action<string>((s) => nabor.status.Text = s), "Копирование завершено. ");
            else  nabor.status.Text = "Копирование завершено. "; 
            //fail(this);
            if (total!=0)
            {
                if (nabor.status.InvokeRequired) nabor.status.Invoke(new Action<string>((s) => nabor.status.Text = s), "Копирование завершено. Идёт проверка.");
                else nabor.status.Text = "Копирование завершено. Идёт проверка.";
                ulong totalresult=GetDirectorySize(drive.RootDirectory);
                if (totalresult == total) {
                    if (nabor.status.InvokeRequired) nabor.status.Invoke(new Action<string>((s) => nabor.status.Text = s), "Успешно записано. Извлеките флешку.");
                    else nabor.status.Text = "Успешно записано. Извлеките флешку.";} else
                {
                    if (nabor.status.InvokeRequired) nabor.status.Invoke(new Action<string>((s) => nabor.status.Text = s), "Записано с ошибками. Повторите попытку.");
                    else nabor.status.Text = "Записано с ошибками. Повторите попытку.";
                    fail(this);
                }
            }
        }*/
        public int id { get; set; }
        public delegate void failhandler(object sender);
        public event failhandler fail;
        internal static ulong SborSpiskaFailov(string FromDir, List<string> files1, List<string> files3)
        {
            try
            {
                ulong result=0;
                foreach (string s1 in Directory.GetFiles(FromDir))
                {
                    
                    FileInfo fi = new FileInfo(s1);
                    if (Convert.ToUInt64(fi.Length) > 4294967000)
                    {
                        files3.Add(s1);
                    }
                    else
                    {
                        result += Convert.ToUInt64(fi.Length);
                        files1.Add(s1);
                    }
                    
                }
                foreach (string s in Directory.GetDirectories(FromDir))
                {
                    result+=SborSpiskaFailov(s, files1, files3);
                }
                return result;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return 0;
            }
        }



        internal static void CreateDirectoryes(string FromDir, string ToDir)
        {
            try
            {
                Directory.CreateDirectory(ToDir);
                foreach (string s in Directory.GetDirectories(FromDir))
                {
                    CreateDirectoryes(s, ToDir + "\\" + Path.GetFileName(s));
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }
        }

        internal static void CreateFinishList(string FromDir, string ToDir, List<string> files1, List<string> files2)
        {
            try
            {
                
                foreach (string s in files1)
                {
                    files2.Add(ToDir  + Path.GetFullPath(s).Replace(FromDir+"\\",""));
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }
        }

        internal static ulong GetDirectorySizeSmall(DirectoryInfo di)
        {
            try { 
            ulong size = 0;
            foreach (FileInfo fi in di.GetFiles())
                size += (ulong)fi.Length;
            foreach (DirectoryInfo d in di.GetDirectories())
        size += GetDirectorySizeSmall(d);
            return size;}
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                return 0;
            }
        }
        public string FromDir;
        /*void CopyDir(string FromDir, string ToDir)
        {
            try {
                Directory.CreateDirectory(ToDir);
                foreach (string s1 in Directory.GetFiles(FromDir))
                {
                    string s2 = ToDir + "\\" + Path.GetFileName(s1);
                    File.Copy(s1, s2);
                }
                foreach (string s in Directory.GetDirectories(FromDir))
                {
                    CopyDir(s, ToDir + "\\" + Path.GetFileName(s));
                }
            }
            catch (Exception e)
            {
                if (nabor.status.InvokeRequired) nabor.status.Invoke(new Action<string>((s) => nabor.status.Text += s), e.Message);
                else nabor.status.Text += e.Message;
                fail(this);
                return;
            }
        }*/


        
        /*void SborSpiskaFailov2(string FromDir, string ToDir, List<string> files1, List<string> files2, List<string> files3)
        {
            try
            {
                Directory.CreateDirectory(ToDir);
                foreach (string s1 in Directory.GetFiles(FromDir))
                {
                    files1.Add(s1);
                    FileInfo fi = new FileInfo(s1);
                    if (Convert.ToUInt64(fi.Length) > 4294967000)
                    {
                        files3.Add(s1);
                    }
                    else {
                    ALL_FILES_LENGTH += Convert.ToUInt64(fi.Length);
                    files2.Add(ToDir + "\\" + Path.GetFileName(s1)); }
                    
                    
                }
                foreach (string s in Directory.GetDirectories(FromDir))
                {
                    SborSpiskaFailov2(s, ToDir + "\\" + Path.GetFileName(s),files1,files2, files3);
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show("oshibka pri sbore");
                if (nabor.status.InvokeRequired) nabor.status.Invoke(new Action<string>((s) => nabor.status.Text += s), e.Message);
                else nabor.status.Text += e.Message;
                fail(this);
                return;
            }
        }*/
        /*private void Watcher_Progress(object sender, ProgressEventArgs e)
        {
            // (int)(e.Current / e.UpperBound*100);
            //nabor.Letter.Text = "50";
            //Progress(this, new progressargs(e.Current, e.UpperBound, numprogress));
            //if (nabor.progress.InvokeRequired) nabor.progress.Invoke(new Action<int>((s) => nabor.progress.Value = s), e.Current);
            //else nabor.progress.Value = e.Current;

        }*/
        /* public delegate void progresshandler(object sender, progressargs e);
         public event progresshandler Progress;*/
        public string labelt;


        public bool FormatDrive(ManagementObjectSearcher searcher,ManagementOperationObserver watcher,string driveLetter,
    string fileSystem = "FAT32", bool quickFormat = true,
    int clusterSize = 8192, string label = "", bool enableCompression = false)
        {try
            {
                if (driveLetter.Length != 2 || driveLetter[1] != ':' || !char.IsLetter(driveLetter[0]))
                    return false;

                //query and format given drive         
                searcher = new ManagementObjectSearcher
                 (@"select * from Win32_Volume WHERE DriveLetter = '" + driveLetter + "'");


                ManagementObjectCollection moc = searcher.Get();

                foreach (ManagementObject vi in moc)
                {
                    vi.InvokeMethod(watcher, "Format", new object[]
                  { fileSystem, quickFormat,clusterSize, label, enableCompression });
                }

                return true;
            }
            catch (Exception e)
            {
                if (nabor.status.InvokeRequired) nabor.status.Invoke(new Action<string>((s) => nabor.status.Text += s), e.Message);
                else nabor.status.Text += e.Message;
                fail(this);
                return false;
            }
        }

    }
}
