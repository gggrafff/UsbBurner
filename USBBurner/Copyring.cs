using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;

namespace USBBurner
{
    class Copyring
    {
        private const int SOSTOYANIE_CONTINUE = 0;
        private const int SOSTOYANIE_CANCEL = 1;
        private int SOSTOYANIE = SOSTOYANIE_CONTINUE;

        private const int COPY_FILE_RESTARTABLE = 0x00000002;

        private const int COPY_FILE_NO_BUFFERING = 0x00001000;


        private delegate int ProgressCallBack(
            Int64 TotalFileSize,             // Размер файла в байтах.
            Int64 TotalBytesTransferred,     // Кол-во перемещенных байт, с начала копирования
            Int64 StreamSize,                // Размер текущего потока, в байтах.
            Int64 StreamBytesTransferred,    // Кол-во перемещенных байт потока, с начала копирования
            uint dwStreamNumber,             // Номер потока
            uint dwCallbackReason,           // см. MSDN
            IntPtr hSourceFile,             // Хендл исходного файла
            IntPtr hDestinationFile,        // Хендл копируемого файла
            int lpData);

        private string[] StartNames;
        private string[] FinishNames;
        private UInt64 ALL_FILES_LENGTH = 0; //Общий размер файлов
        private int ALL_PROGRESS = 0; //Общий прогресс копирования
        //private int PROGRESS = 0; //Прогресс копирования файла
        private double PRE_BYTES = 0; //предыдущее значение скопированных битов
        private double ALL_BYTES_SEND = 0;

        [DllImport("Kernel32")]
        private static extern int CopyFileEx(string lpExistingFileName, string lpNewFileName,
            ProgressCallBack lpProgressRoutine, int lpData, bool pbCancel, int dwCopyFlags);

        /*private int PCallBack(Int64 TotalFileSize, Int64 TotalBytesTransferred, Int64 StreamSize,
            Int64 StreamBytesTransferred, uint dwStreamNumber, uint dwCallbackReason, IntPtr hSourceFile,
            IntPtr hDestinationFile, int lpData)
        {
            double rastPercent = 0;
            Int32 Percent = 0;
            double AllrastPercent = 0;
            Int32 AllPercent = 0;
            try
            {
                if (TotalFileSize > 0)
                {
                    rastPercent = TotalBytesTransferred / (TotalFileSize / 100);
                    Percent = Convert.ToInt32(Math.Round(rastPercent));
                    if (Percent > 100) Percent = 100;
                }
                if (ALL_FILES_LENGTH > 0)
                {
                    ALL_BYTES_SEND = ALL_BYTES_SEND + TotalBytesTransferred - PRE_BYTES;
                    AllrastPercent = (ALL_BYTES_SEND) / (ALL_FILES_LENGTH / 100);
                    AllPercent = Convert.ToInt32(Math.Round(AllrastPercent));
                    if (AllPercent > 100) AllPercent = 100;
                }
            }
            finally
            {
                ALL_PROGRESS = AllPercent;
                PROGRESS = Percent;
            }
            PRE_BYTES = Convert.ToInt64(TotalBytesTransferred);
            return SOSTOYANIE;
        }*/

        private int PCallBack2(Int64 TotalFileSize, Int64 TotalBytesTransferred, Int64 StreamSize,
            Int64 StreamBytesTransferred, uint dwStreamNumber, uint dwCallbackReason, IntPtr hSourceFile,
            IntPtr hDestinationFile, int lpData)
        {
            //double rastPercent = 0;
            //Int32 Percent = 0;
            double AllrastPercent = 0;
            Int32 AllPercent = 0;
            try
            {
                /*if (TotalFileSize > 0)
                {
                    rastPercent = TotalBytesTransferred / (TotalFileSize / 100);
                    Percent = Convert.ToInt32(Math.Round(rastPercent));
                    if (Percent > 100) Percent = 100;
                }*/
                if (ALL_FILES_LENGTH > 0)
                {
                    ALL_BYTES_SEND = ALL_BYTES_SEND + TotalBytesTransferred - PRE_BYTES;
                    AllrastPercent = (ALL_BYTES_SEND) / (ALL_FILES_LENGTH / 100);
                    AllPercent = Convert.ToInt32(Math.Round(AllrastPercent));
                    if (AllPercent > 100) AllPercent = 100;
                }
            }
            finally
            {
                ALL_PROGRESS = AllPercent;
                //PROGRESS = Percent;
            }
            PRE_BYTES = Convert.ToInt64(TotalBytesTransferred);
            /*if (fileprbar.InvokeRequired) fileprbar.Invoke(new Action(() => {
                fileprbar.Value = PROGRESS;
            }));
            else
            {
                fileprbar.Value = PROGRESS;
            }*/
            if (allprbar.InvokeRequired) allprbar.Invoke(new Action(() => {
                allprbar.Value = ALL_PROGRESS;
                //status.Text += " ";
            }));
            else
            {
                allprbar.Value = ALL_PROGRESS;
                //status.Text += " ";
            }
            return SOSTOYANIE;
        }
        //ProgressBar fileprbar = new ProgressBar();
        ProgressBar allprbar = new ProgressBar();
        private void TheadCopirovaniya()
        {
            //bool pbCancel = false;
            ProgressCallBack pcb = new ProgressCallBack(PCallBack2);
            //MessageBox.Show("Neposredstvenno kopiruem"+ StartNames.Length+"       "+FinishNames.Length);
            int k = 0;
            for (k = 0; k < StartNames.Length; k++)
                //if (SOSTOYANIE == SOSTOYANIE_CONTINUE)
                {
                    CopyFileEx(StartNames[k], FinishNames[k], pcb, 0, false, COPY_FILE_RESTARTABLE);
                    PRE_BYTES = 0;
                }
           // int b = 0;
        }

        private void GetFilesLength()
        {
            for (int i = 0; i < StartNames.Length; i++)
            {
                if (File.Exists(StartNames[i]))
                {
                    FileInfo fi = new FileInfo(StartNames[i]);
                    ALL_FILES_LENGTH += Convert.ToUInt64(fi.Length);
                }
            }
            //MessageBox.Show(ALL_FILES_LENGTH.ToString());
        }

        private void Obnulenie() //обнуление глобальных переменных
        {
            ALL_FILES_LENGTH = 0; //Общий размер файлов
            ALL_PROGRESS = 0; //Общий прогресс копирования
            //PROGRESS = 0; //Прогресс копирования файла
            PRE_BYTES = 0; //предыдущее значение скопированных битов
            ALL_BYTES_SEND = 0;
        }
        
        public bool CopyFiles(string[] SNames, string[] FNames, ProgressBar AllFilesProgressBar,ulong ALL_FILES_LENGTHin)
        {//, ProgressBar FileProgressBar
            Obnulenie();
            //MessageBox.Show("obnulenie done");
            /*if (FileProgressBar.InvokeRequired) FileProgressBar.Invoke(new Action(() => {
                FileProgressBar.Minimum = 0;
                FileProgressBar.Maximum = 100;
            }));
            else
            {
                FileProgressBar.Minimum = 0;
                FileProgressBar.Maximum = 100;
            }*/
            if (AllFilesProgressBar.InvokeRequired) AllFilesProgressBar.Invoke(new Action(() => {
                AllFilesProgressBar.Minimum = 0;
                AllFilesProgressBar.Maximum = 100;
            }));
            else
            {
                AllFilesProgressBar.Minimum = 0;
                AllFilesProgressBar.Maximum = 100;
            }
            allprbar = AllFilesProgressBar;
            
            ALL_FILES_LENGTH = ALL_FILES_LENGTHin;
            //fileprbar = FileProgressBar;
            //////////////////////////////////////
            StartNames = SNames;
            FinishNames = FNames;
            //GetFilesLength();
            //////////////////////////////////////
            SOSTOYANIE = SOSTOYANIE_CONTINUE;
            //Thread t = new Thread(TheadCopirovaniya);
            //MessageBox.Show("sozdan potok");
            //t.Start();
            TheadCopirovaniya();
            /*while (t.IsAlive == true)
            {
                Application.DoEvents();

                if (FileProgressBar.InvokeRequired) FileProgressBar.Invoke(new Action(() => {
                    FileProgressBar.Value = PROGRESS;
                }));
                else
                {
                    FileProgressBar.Value = PROGRESS;
                }
                if (AllFilesProgressBar.InvokeRequired) AllFilesProgressBar.Invoke(new Action(() => {
                    AllFilesProgressBar.Value = ALL_PROGRESS;
                }));
                else
                {
                    AllFilesProgressBar.Value = ALL_PROGRESS;
                }
                
            }*/
            if (SOSTOYANIE == SOSTOYANIE_CONTINUE) return true;
            else return false;
        }

        /*public void CancelCopyringFiles()
        {
            SOSTOYANIE = SOSTOYANIE_CANCEL;
        }*/
    }
}
