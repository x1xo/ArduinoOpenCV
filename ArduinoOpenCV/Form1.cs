using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArduinoOpenCV
{
    public partial class Form1 : Form
    {
        public SerialPort port = new SerialPort("COM3", 9600);

        public Form1()
        { 
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            VideoCapture cap = new VideoCapture(0);

            if (!cap.IsOpened())
            {
                MessageBox.Show("Greska pri otvaranje na kamerata.");
                return;
            }

            if (!port.IsOpen)
            {
                port.Open();
            }

            while (true)
            {
                Mat frame = new Mat();
                cap.Read(frame);

                Cv2.Resize(frame, frame, new OpenCvSharp.Size(1280, 720));

                string a = port.ReadExisting();
                if (!string.IsNullOrEmpty(a) && a == "1")
                {
                    Rect[] faces = Utils.DetectFacesOnImage(frame);
                    Utils.DrawFaceRects(frame, faces);
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

        }

        private void izbrisiBtn_Click(object sender, EventArgs e)
        {

        }
    }
}
