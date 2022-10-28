using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Threading;



namespace timestamp
{
    

    public partial class Timestamp : Form
    {
        private static System.Timers.Timer aTimer;
        uint maxpasszivIdo = 0, passzivIdo = 0, atmeneti = 0, summa=0;//a stopper idö és annak maximuma
        TimeSpan workOut, SworkOut= TimeSpan.FromSeconds(0); //a az állással eltöltött részidö és ezek összege
        uint limit = 600000; // limit elérése után az idöintervallum bekerül workOut-ba
        bool kiskepernyo = true; //a Form kissebre-nagyobbra állításához
        string inputFeld, loginfo, buttonIdeiglenes;
        readonly Font font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
        Label lbl;
        DateTime EtapStartTime = DateTime.Now, EtapEndTime;
        DateTime StartTime = DateTime.Now;
        TimeSpan deltaEtap, deltaTread;//idötartam
        readonly Screen scr;
        bool newActivity = false;
        readonly string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\timestamp.txt";
        StreamWriter sw;
        private Point lastLocation;
        private bool mouseDown;
        static bool kapcsolo=true;
        bool aktiv_kovetkezik_e = false;
        
        
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
         
            Szamlalo();
            Aktivitate.AllapotvaltozasEsemeny += EsemenyKezeles;
           
        }
        void EsemenyKezeles(object sender, Esemeny esemeny) //Etap közti aktiv inaktiv bekerül a log-ba
        {
            
            sw = new StreamWriter(path, true);
            sw.WriteLine(esemeny.esemenyleiras + esemeny.idopont.ToString("HH:mm:ss"));
            sw.Close();
 
        }

        
        void Szamlalo()
        {
            // Create a timer with a 1 min interval.
            aTimer = new System.Timers.Timer(60000);

            // Hook up the Elapsed event for the timer.
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;// az idözítö ismétlödik
            
        }
        
        void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e) // ezt kell végrehajtania - Müködik
        {

            passzivIdo = IdleTimeFinder.GetIdleTime();

            if (atmeneti>passzivIdo && aktiv_kovetkezik_e==true) //passziv idöszak végetér. csak az elsö ellenörzésnél írd ki hogy aktív kezdödik
            {
                _ = new Aktivitate
                { Valtozas = false };
                kapcsolo = true;
                summa += maxpasszivIdo;//zsákold be a visszaesés elötti max állást
                //ShowMessageBox("aktiv Signal:" + e.SignalTime.ToString("HH:mm:ss") + "\n Now:" + DateTime.Now.ToString("HH:mm:ss") + "  summa +=maxpasszivIdo:" + summa + "/" + maxpasszivIdo);
                maxpasszivIdo = 0;
                aktiv_kovetkezik_e = false;
            }
            atmeneti = passzivIdo;

            if (passzivIdo > limit)
            {
                if (kapcsolo)//csak az elsö ellenörzésnél írd ki hogy passziv a többinél ne
                {
                    //ShowMessageBox("Inaktiv kezdödik. Signal:" + e.SignalTime.ToString("HH:mm:ss") + "\n Now:" + DateTime.Now.ToString("HH:mm:ss") + "  summa +=maxpasszivIdo:" + summa + "/" + maxpasszivIdo);
                    kapcsolo =false;
                    _ = new Aktivitate
                    { Valtozas = true };
                }
                
                if (maxpasszivIdo < passzivIdo)
                {
                    maxpasszivIdo = passzivIdo;
                }
                aktiv_kovetkezik_e = true;
            }
        }
        
        private void Button1_Click(object sender, EventArgs e)

        {
            aktiv_kovetkezik_e = false;

            if (button1.Text == "ongoing") //ha  kikapcsolom a munkamenetet
            {
                aTimer.Stop();
                newActivity = false;// nem aktiv állapot
                button1.BackColor = Color.LightPink;
                button1.Text = "stopped";

            }
            else //ha  bekapcsolom a munkamenetet
            {
                aTimer.Start();
                newActivity = true; // aktív állapot
                button1.BackColor = Color.LightGreen;
                button1.Text = "ongoing";

            }
            Export(path);
            
        }

        public void Export(string path)
        {
            
            inputFeld = "";
            loginfo = "";

            sw = new StreamWriter(path, true);

            if (newActivity) //Start vagy cancel nyomása esetén
            {
                maxpasszivIdo = 0; atmeneti = 0; summa = 0;
                workOut = TimeSpan.FromMilliseconds(0);
                EtapStartTime = DateTime.Now;
                InputBox("new activity", "Description:", ref inputFeld);
                loginfo = "in :" + EtapStartTime.ToString("HH:mm:ss") + "  " + inputFeld;
             
            }

            else //a go megnyomása esetén
            {
     
                summa += maxpasszivIdo; 
                EtapEndTime = DateTime.Now;
                deltaEtap = EtapEndTime.Subtract(EtapStartTime);
                deltaTread = deltaTread.Add(deltaEtap);
                workOut = TimeSpan.FromMilliseconds(summa);
                SworkOut +=  workOut;
                loginfo = "out:" + EtapEndTime.ToString("HH:mm:ss") + "   " +
                    "\u0394:" + deltaEtap.ToString(@"hh\:mm") + " \u0394\u0394:" + deltaTread.ToString(@"hh\:mm") + "   Pause:" + workOut.ToString(@"hh\:mm");
                maxpasszivIdo = 0; summa = 0;
            }

            sw.WriteLine(loginfo);
            sw.Close();
            Felrak(loginfo);

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            EtapEndTime = DateTime.Now;
            TimeSpan bruttoTime = StartTime - EtapEndTime;
            if (button1.Text == "ongoing")
            {
                
                summa += maxpasszivIdo;
                workOut = TimeSpan.FromMilliseconds(summa);
                SworkOut += workOut;
                deltaEtap = EtapEndTime.Subtract(EtapStartTime);
                deltaTread = deltaTread.Add(deltaEtap);
            }

            sw = new StreamWriter(path, true);

            sw.WriteLine("esc: \u0394:" + deltaEtap.ToString(@"hh\:mm") + " \u0394\u0394:" + deltaTread.ToString(@"hh\:mm") + "   Pause:" + workOut.ToString(@"hh\:mm"));
            sw.WriteLine("Brutto Time:" + bruttoTime.ToString(@"hh\:mm") + "  Netto Time: " + (deltaTread- SworkOut).ToString(@"hh\:mm")+ " Summ Pause:" + SworkOut.ToString(@"hh\:mm"));
            sw.WriteLine("Work finished:" + EtapEndTime.ToString("HH:mm:ss"));

            sw.Close();

        }

        private void Label4_Click(object sender, EventArgs e)
        {
            aTimer.AutoReset = true;
            limit = (uint) ((Convert.ToInt32(Pause.Text)+1) * 60000);
            Pause.Text=(limit/60000).ToString();
        }

        private void Label3_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(Pause.Text)==1)
            {
                return;
            }
            aTimer.AutoReset = true;
            limit = (uint)((Convert.ToInt32(Pause.Text) - 1) * 60000);
            Pause.Text = (limit / 60000).ToString();
        }

        private void Timestamp_DoubleCklick(object sender, EventArgs e)
        {
            aTimer.AutoReset = true;

            if (kiskepernyo)
            {
                Width = panel1.Width;//580
                Height = panel1.Height + 50;//150
                kiskepernyo = false;
            }
            else
            {
                Width = 144;
                Height = 38;
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

            lbl = new Label
            {
                AutoSize = true,
                Font = font,
                Location = new Point(5, 11),
                Text = loginfo,
            };
            panel1.Controls.Add(lbl);
            panel1.Height += 25;
            
        }

        public static DialogResult InputBox( string title, string promptText, ref string value)
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

        private void Label2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            TimeSpan eredmeny, deltaEtap1, workOut1;
            buttonIdeiglenes = button1.Text;
            if (newActivity)
            {
                deltaEtap1 = DateTime.Now.Subtract(EtapStartTime);
                workOut1 = TimeSpan.FromMilliseconds(summa);
                eredmeny = deltaTread - SworkOut + deltaEtap1 - workOut1;
            }
            else
            {
                eredmeny = deltaTread - SworkOut;
            }
                    
            
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
         //használd ha szöveget kellene kiiaratni
        /*public void ShowMessageBox(string vmi)
        {

        
            var thread = new Thread(
              () =>
              {
                  MessageBox.Show(vmi);
              });
            thread.Start();
        }*/
        //ShowMessageBox("-activ:" + e.SignalTime.ToString("HH:mm:ss") + "  pI/lim:" + passzivIdo + "/" + limit);

    }
}
