using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ArduinoOpenCV
{
    public partial class Form1 : Form
    {
        public SerialPort port = new SerialPort("COM4", 9600);

        public bool prevDetected = false;

        public Form1()
        { 
            InitializeComponent();
            listView1.View = View.Details;
            listView1.Sorting = SortOrder.None;
            listView1.FullRowSelect = true;
            listView1.Columns.Add("Email", 216);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count == 0)
            {
                MessageBox.Show("Nema dodadeno nikakov email.");
                return;
            }
            if (!port.IsOpen)
            {
                port.Open();
            }
            VideoCapture cap = new VideoCapture(1);
            Mat frame = new Mat();
            if (!cap.IsOpened())
            {
                MessageBox.Show("Greska pri otvaranje na kamerata.");
                return;
            }

            while (true)
            {
                cap.Read(frame);

                Cv2.Resize(frame, frame, new OpenCvSharp.Size(1280, 720));
                string a = port.ReadExisting();
                label4.Text = a.ToString();
                if (!string.IsNullOrEmpty(a) && a.Contains("1"))
                {
                    Rect[] faces = Utils.DetectFacesOnImage(frame);
                    if (faces.Length > 0)
                    {
                        if (!prevDetected)
                        {
                            new Thread(delegate ()
                            {
                                List<Mat> faci = Utils.GetFaceFromRects(frame, faces);
                                SendMail(faci);
                            }).Start();

                            prevDetected = true;
                        }
                        Utils.DrawFaceRects(frame, faces);
                    }
                }
                else
                {
                    prevDetected = false;
                }
                
                Cv2.ImShow("Camera", frame);

                if (Cv2.WaitKey(1) == 32)
                {
                    break;
                }
            }
        }

        private void dodajBtn_Click(object sender, EventArgs e)
        {
            MimeKit.MailboxAddress addr = null;
            try
            {
                addr = MimeKit.MailboxAddress.Parse(email.Text);
            }catch {
                MessageBox.Show("Nevalidna email addresa.");
            }
            if (addr == null) return;
                
            if (!listView1.Items.Contains(new ListViewItem(addr.ToString())))
            {
                listView1.Items.Add(new ListViewItem(addr.ToString()));
                email.Text = "";
            }
        }

        private void izbrisiBtn_Click(object sender, EventArgs e)
        {
            if(listView1.SelectedItems[0].SubItems[0] != null)
            {
                int i = listView1.SelectedItems[0].SubItems.IndexOf(listView1.SelectedItems[0].SubItems[0]);
                if (i == -1) return;
                listView1.Items.RemoveAt(i);
            }
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            int[] values = { 30, 40, 50, 60, 70, 80, 90, 100 };
            //MessageBox.Show(values[trackBar1.Value-1].ToString());
            
            if(!port.IsOpen) { port.Open(); }
            port.Write(values[trackBar1.Value - 1].ToString());
        }

        private void SendMail(List<Mat> faci)
        {
            var email = new MimeKit.MimeMessage();
            email.From.Add(new MimeKit.MailboxAddress("Alarm System", "hgliguroski@gmail.com"));
            foreach (ListViewItem emailString in listView1.Items)
            {
                email.To.Add(new MimeKit.MailboxAddress("", emailString.SubItems[0].Text));
            }

            email.Subject = "Warning! Object Detected";

            var body = new MimeKit.BodyBuilder();
            int num = 0;
            foreach (Mat faca in faci)
            {
                num++;
                body.Attachments.Add("face_" + num + ".png", faca.ToBytes(".png"));
            }
            email.Body = body.ToMessageBody();

            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                smtp.Connect("smtp.gmail.com", 587, false);

                smtp.Authenticate("hgliguroski@gmail.com", "apppassword");

                smtp.Send(email);
                smtp.Disconnect(true);
            }   
        }
    }
}
