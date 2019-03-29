using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace USBBurner
{
    class elementy
    {
        public Button info { get; set; }
        public Button retry { get; set; }
        public Label Letter { get; set; }
        public ProgressBar progress { get; set; }
        //public Label status { get; set; }
        public Label status { get; set; }
        public Panel retrypanel;
        public Panel retrypanel2;
        public int Top
        {
            set
            {
                Top = value;
                info.Top = Top + 10;
            }
            get { return Top; }
        }
public int Left
        {
            set
            {
                Left = value;
                info.Left = Left + 10;
            }
            get { return Left; }
        }

        public elementy()
        {
            info = new Button();
            info.Text = "info";
            info.Size = new System.Drawing.Size(70, 30);
            retry = new Button();
            retry.Text = "Retry";
            retry.Size = new System.Drawing.Size(70, 30);
            retry.Visible = false;
            Letter = new Label();
            Letter.Text = "";

            //status = new Label();
            status = new Label();
            status.AutoSize = true;
            status.MaximumSize = new System.Drawing.Size(340, 1000);
            progress = new ProgressBar();
            progress.Minimum = 0;
            progress.Maximum = 100;
            progress.Width = 220;
            
            //progress.Top = status.Top + status.Height + 10;
            //retry.Top = progress.Top +progress.Height +10;
            status.SizeChanged += Status_SizeChanged;
            //status.Multiline = true;
            status.Size = new System.Drawing.Size(360, 50);
            status.Text = "Устройство обнаружено. ";
            retrypanel = new Panel();
            retrypanel2 = new Panel();
            retrypanel.AutoSize = true;
            retrypanel2.AutoSize = true;
            retrypanel.MaximumSize = new System.Drawing.Size(350, 1050);
            retrypanel.Controls.Add(status);
            retrypanel2.Controls.Add(retry);
            retrypanel.Controls.Add(progress);
            //retry.Left = 0;// progress.Left + progress.Width + 10;
            //retry.Top = -50;// progress.Top - 10;
            //status.TextChanged += Status_TextChanged;

            /*progress.Refresh();
            progress.CreateGraphics().DrawString("asfdsdgsdg",
                new Font("Arial", (float)8.25, FontStyle.Regular),
                Brushes.Black, new PointF(progress.Width / 2 - 10,
                    progress.Height / 2 - 7));*/

            //progress.CreateGraphics().DrawString("sfdg", new Font("Arial", 10), Brushes.Black, new Rectangle(0, 0, progress.Size.Width, progress.Size.Height));
            //status.BackColor = System.Drawing.Color.Transparent;
            //status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //status.Dock = DockStyle.Fill;
            //status.Parent = progress;
            //progress.Controls.Add(status);

            //progress.CreateGraphics().DrawString("статус", new Font("Arial", (float)7.15, FontStyle.Regular), Brushes.Black, new PointF(0, 0));
            //retrypanel.Controls.Add(status);
            //retrypanel.Controls.Add(retry);
            //retry.Location = new System.Drawing.Point(0, 60);
            //status.Location = new System.Drawing.Point(0, 0);
            //status.Show();
        }

        /*private void Status_TextChanged(object sender, EventArgs e)
        {
            if (progress.InvokeRequired) progress.Invoke(new Action(() =>
            {
                progress.Refresh();
                progress.CreateGraphics().DrawString(status.Text,
                    new Font("Arial", (float)8.25, FontStyle.Regular),
                    Brushes.Black, new PointF(progress.Width / 2 - status.Width / 2,
                        progress.Height / 2 - status.Height / 2));
            }));
            else
            {
                progress.Refresh();
                progress.CreateGraphics().DrawString(status.Text,
                    new Font("Arial", (float)8.25, FontStyle.Regular),
                    Brushes.Black, new PointF(progress.Width / 2 - status.Width / 2,
                        progress.Height / 2 - status.Height / 2));
            }
        }*/

        private void Status_SizeChanged(object sender, EventArgs e)
        {
            progress.Top = status.Top + status.Height + 10;
            retry.Top = progress.Top + progress.Height + 10;
        }
    }
   
}
