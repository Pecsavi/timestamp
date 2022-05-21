using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace timestamp
{


    public partial class Timestamp : Form
    {

        int counter = 0;
        string inputFeld, loginfo;
        Font font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
        Label lbl;
        DateTime startTime, endTime;
        TimeSpan deltaEtap, deltaTread, halfMinute= TimeSpan.FromSeconds(30);
        Screen scr;
       
        bool newActivity = false;
        string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\timestamp.txt";
        StreamWriter sw;

        public Timestamp()
        {
            InitializeComponent();
            this.TopMost = true;
            this.StartPosition = FormStartPosition.Manual;
            scr = Screen.FromPoint(this.Location);
            this.Location = new Point(scr.WorkingArea.Right - this.Width - 20, scr.WorkingArea.Top);
            label1.Text = ("New work:" + DateTime.Now.ToString("d"));
            label1.Font = font;
            button2.Enabled = false;
        }

        private void form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            endTime = DateTime.Now;

            if (button1.Text== "ongoing")
            {
                deltaEtap = endTime.Subtract(startTime) + halfMinute;
                deltaTread = deltaTread.Add(deltaEtap);
            }
            
            sw = new StreamWriter(path, true);
            
            sw.WriteLine("esc:" + endTime.ToString("HH:mm") + " The user has logged out" );
            sw.WriteLine("");
            sw.WriteLine("Work finished:" + "  -  Sum Time:" + deltaTread.ToString(@"hh\:mm"));
            sw.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            newActivity = true;
            Export(path);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "ongoing")
            {
                button1.BackColor = Color.LightPink;
                button1.Text = "stopped";
            }
            else
            {
                button1.BackColor = Color.LightGreen;
                button1.Text = "ongoing";
            }
            Export(path);
        }
        
        public void Export(string path )
        {
            inputFeld = "";
            
            loginfo = "";

            sw = new StreamWriter(path , true);
            if (counter==0)
            { 
                sw.WriteLine("\n"+label1.Text);
            }
            if (newActivity)
            {
                endTime = DateTime.Now;
                InputBox(this, "new activity", "Description:", ref inputFeld);
                deltaEtap = endTime.Subtract(startTime) + halfMinute;
                deltaTread = deltaTread.Add(deltaEtap);
                
                loginfo = "      -" + endTime.ToString("HH:mm") + "   " + inputFeld + "   " +
                    "\u0394:" + deltaTread.ToString(@"hh\:mm");
                
                startTime = DateTime.Now;
                newActivity = false;
                counter--;
            }
           
            else if (counter % 2 == 0)
            {
                startTime = DateTime.Now;
                InputBox(this,"new activity", "Description:", ref inputFeld);
                loginfo = "in :" + startTime.ToString("HH:mm")+"  " + inputFeld;
                button2.Enabled = true;
            }
            else if (counter % 2 != 0)
            {
                endTime = DateTime.Now;
                deltaEtap = endTime.Subtract(startTime) + halfMinute;
                deltaTread = deltaTread.Add(deltaEtap);
                loginfo = "out:" + endTime.ToString("HH:mm") + "   " +
                    "\u0394:" + deltaEtap.ToString(@"hh\:mm") + " sum \u0394: " + deltaTread.ToString(@"hh\:mm");
                button2.Enabled = false;
                
            }
                       
           
            sw.WriteLine(loginfo);
            sw.Close();
            Felrak(loginfo);
            counter++;


        }
        public void Felrak(string loginfo)
        {

            foreach (Control nagy in Controls)
            {
                foreach (Control elem in nagy.Controls)
                {
                    if (elem is Label)
                    {
                        elem.Location = new Point(elem.Location.X, elem.Location.Y + 25);
                    }
                }
            }

            lbl = new Label();
            lbl.AutoSize = true;
            lbl.Font = font;
            lbl.Location = new Point(5, 11);
            lbl.Text = loginfo;
            panel1.Controls.Add(lbl);
            panel1.Height= panel1.Height + 25;


        }

        public static DialogResult InputBox(object sender, string title, string promptText, ref string value)
        {
           
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();
            
            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            
            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.Manual;
            Form form2 = sender as Form;
            form.Location = new Point (form2.Bounds.X-form.Width, form2.Bounds.Y);
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            
            value = textBox.Text;


            return dialogResult;
        }

        
    }
}
