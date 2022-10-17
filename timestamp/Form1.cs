using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;



namespace timestamp
{
    

    public partial class Timestamp : Form
    {
        private static System.Timers.Timer aTimer;
        static uint maxido = 0, ido=0; //a stopper idö és annak maximuma
        TimeSpan workOut, SworkOut= TimeSpan.FromSeconds(0); //a az állással eltöltött részidö és ezek összege
        static uint idoCheck = 100000, limit = 900000; //IdoCheck (ms) egy intervallum ami után frissülnek az adatok. limit elérése után az idöintervallum bekerül workOut-ba
        bool kiskepernyo = true; //a Form kissebre-nagyobbra állításához
       
        string inputFeld, loginfo, buttonIdeiglenes;
        
        Font font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
        Label lbl;

        DateTime startTime = DateTime.Now, endTime;//idöpont
        TimeSpan deltaEtap, deltaTread, halfMinute = TimeSpan.FromSeconds(25);//idötartam
       
        Screen scr;

        bool newActivity = false;
        string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\timestamp.txt";
        
        StreamWriter sw;

        private Point lastLocation;
        private bool mouseDown;

        public Timestamp()
        {
            InitializeComponent();
            this.TopMost = true;
            this.StartPosition = FormStartPosition.Manual;
            scr = Screen.FromPoint(this.Location);
            this.Location = new Point(scr.WorkingArea.Right/2, scr.WorkingArea.Bottom/2);
           

            label1.Text = ("New work:" + DateTime.Now.ToString("d"));
            label1.Font = font;
            sw = new StreamWriter(path, true);
            sw.WriteLine("\n\n" + label1.Text);
            sw.Close();

            szamlalo();
        }

        static void szamlalo()
        {

            // Create a timer with a two second interval.
            aTimer = new System.Timers.Timer(idoCheck);

            // Hook up the Elapsed event for the timer.
            aTimer.Elapsed += OnTimedEvent;

        }

        static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e) // ezt kell végrehajtania
        {
            
            ido = IdleTimeFinder.GetIdleTime();
            if (maxido < ido)
            {
                maxido = ido;
            }
        }

        private void form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            endTime = DateTime.Now;

            if (button1.Text == "ongoing")
            {
                if (maxido >= limit)
                {
                    workOut = TimeSpan.FromMilliseconds(maxido);
                    deltaEtap = endTime.Subtract(startTime) + halfMinute;
                    SworkOut = SworkOut + workOut;
                }
                else
                {
                    deltaEtap = endTime.Subtract(startTime) + halfMinute;
                }
          
                deltaTread = deltaTread.Add(deltaEtap);
            }

            sw = new StreamWriter(path, true);

            sw.WriteLine("esc:" + endTime.ToString("HH:mm") + " The user has logged out " + deltaEtap.ToString(@"hh\:mm"));
            sw.WriteLine("Work finished:" + endTime.ToString("HH:mm") + "  Brutto Time: " + deltaTread.ToString(@"hh\:mm")+ " Summ Pause(which>15min):" + SworkOut.ToString(@"hh\:mm"));
            
            sw.Close();

        }

        private void button1_Click(object sender, EventArgs e)

        {
            if (button1.Text=="Start")
            {
                
                aTimer.AutoReset = true;// Have the timer fire repeated events (true is the default)
                aTimer.Enabled = true; // Start the timer
                newActivity = true;
                button1.BackColor = Color.LightGreen;
                button1.Text = "ongoing";
            }
           

            else if (button1.Text == "ongoing")
            {
                if (maxido < limit)
                {
                    maxido = 0;
                }
                
                workOut = TimeSpan.FromMilliseconds(maxido);
                SworkOut = SworkOut + workOut;
                aTimer.Enabled = false;
                newActivity = false;
                button1.BackColor = Color.LightPink;
                button1.Text = "stopped";
                
            }
            else 
            {
                maxido = 0;
                workOut = TimeSpan.FromMilliseconds(maxido);
                aTimer.AutoReset = true;
                aTimer.Enabled = true;
                newActivity = true;
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
            
            if (newActivity)
            {
                startTime = DateTime.Now;
                InputBox(this, "new activity", "Description:", ref inputFeld);
                loginfo = "in :" + startTime.ToString("HH:mm") + "  " + inputFeld;
                newActivity = false;
            }
           
            else 
            {
                endTime = DateTime.Now;
                deltaEtap = endTime.Subtract(startTime) + halfMinute;
                deltaEtap = endTime.Subtract(startTime) + halfMinute;                         
                deltaTread = deltaTread.Add(deltaEtap);
                loginfo = "out:" + endTime.ToString("HH:mm") + "   " +
                    "\u0394:" + deltaEtap.ToString(@"hh\:mm") + " \u0394\u0394:" + deltaTread.ToString(@"hh\:mm") + "   Workout:" + workOut.ToString(@"hh\:mm");
            }

            sw.WriteLine(loginfo);
            sw.Close();
            Felrak(loginfo);

        }

        private void Timestamp_DoubleCklick(object sender, EventArgs e)
        {
            if (kiskepernyo)
            {
                this.Width = 580;
                this.Height = 150;
                kiskepernyo = false;
            }
            else
            {
                this.Width = 144;
                this.Height = 38;
                kiskepernyo = true;
            }
           
        }

        public void Felrak(string loginfo) //a panel1-re felrakja az új tevékenységet leiró labelt
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
            Screen scr = Screen.FromPoint(form.Location);
            form.Location = new Point(scr.WorkingArea.Right /3 , scr.WorkingArea.Bottom / 3);

            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            
            value = textBox.Text;


            return dialogResult;
        }

        private void label2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            buttonIdeiglenes = button1.Text;
            if (maxido < limit)
            {
                maxido = 0;
            }
            TimeSpan kiesett = SworkOut + TimeSpan.FromMilliseconds(maxido);
            TimeSpan elteltEtap = DateTime.Now.Subtract(startTime);
            TimeSpan eredmeny = elteltEtap + deltaTread - kiesett;
            button1.Text = (eredmeny).ToString();
            mouseDown = true;
            lastLocation = e.Location;
            this.Opacity = 1D;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new Point(
                    (this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);

                this.Update();
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            this.Opacity = 0.5D;
            mouseDown = false;
            button1.Text= buttonIdeiglenes;
        }

       
    }
}
