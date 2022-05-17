using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace timestamp
{


    public partial class Timestamp : Form
    {

        int counter = 0;
        string title="Log";
        string text, in_out, sum;
        Font font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
        Label lbl;
        DateTime time1, time2;
        TimeSpan delta1, delta2;
        Screen scr;
        //Form form1;
        

        public Timestamp()
        {
           
                InitializeComponent();
                this.TopMost = true;
                this.StartPosition = FormStartPosition.Manual;
                scr = Screen.FromPoint(this.Location);
                this.Location = new Point(scr.WorkingArea.Right - this.Width - 20, scr.WorkingArea.Top);
                label1.Text = ("New work tread:" + DateTime.Now.ToString("d"));
                label1.Font = font;

            

        }

        private void form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            string Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\timestamp.txt";
            StreamWriter sw = new StreamWriter(Path, true);
            sw.WriteLine("Work tread finished:" + DateTime.Now.ToString("HH:mm:ss") + "  -  Sum Time:" + delta2.ToString(@"dd\:hh\:mm"));
            sw.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Export();
        }
        
        public void Export()
        {
            text = "";
            button1.Text = "activity is in progress";
            string Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)+ @"\timestamp.txt";
           
            StreamWriter sw = new StreamWriter(Path , true);
            if (counter==0)
            { 
                sw.WriteLine("\n"+label1.Text);
            }
            
            in_out = "in: ";
            sum = "";
            if (counter % 2 == 0)
            {
                button1.BackColor = Color.LightGreen;
                sw.WriteLine("");
                time1 = DateTime.Now;
                InputBox(this,"check in", "Description of starting activity:", ref text);
            }
            else
            {
                button1.BackColor = Color.LightPink;
                button1.Text = "activity is stopped";
                time2 = DateTime.Now;
                delta1 = time2.Subtract(time1);
                delta2 = delta2.Add(delta1);
                in_out = "out:";
                sum = "S:" + delta1.ToString(@"hh\:mm")+ " SS:" + delta2.ToString(@"hh\:mm");
                
            }
            
            
            string loginfo = sum=="" ?  in_out + time1.ToString("HH:mm:ss") +  "   " + text : in_out + time2.ToString("HH:mm:ss") + "   " + sum ;
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
            lbl.Location = new Point(5, 10);
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
