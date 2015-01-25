using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.GPU;


namespace HumanDetection
{
    public partial class HumanDet : Form
    {
        private Capture _capture = null;
        private bool _captureInProgress;

        public HumanDet()
        {
            InitializeComponent();
            try
            {
                _capture = new Capture();
                _capture.ImageGrabbed += ProcessFrame;
            }
            catch (NullReferenceException excpt)
            {
                MessageBox.Show(excpt.Message);
            }
        }

        private void ProcessFrame(object sender, EventArgs arg)
        {
            Image<Bgr, Byte> frame = _capture.RetrieveBgrFrame().Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR).Copy(); ;
            int detected_human=0;
            long processingTime;
            Rectangle[] results = FindHuman.Find(frame, out processingTime);
            MCvFont f = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_COMPLEX_SMALL, 0.6, 0.7);
            foreach (Rectangle rect in results)
            {
                ++detected_human;
                frame.Draw(rect, new Bgr(Color.Red), 2);
                frame.Draw("[" + detected_human + "]" + rect.Left.ToString() + "," + rect.Top.ToString(), ref f, new Point(rect.Left, rect.Top - 5), new Bgr(Color.Red));
            }
            
            captureImageBox.Image = frame;
            
            lblTime.Invoke((Action)(() => lblTime.Text = processingTime.ToString()));
            lblTime.Invoke((Action)(() => lblCount.Text = detected_human.ToString()));
        }

        private void ReleaseData()
        {
            if (_capture != null)
                _capture.Dispose();
        }

        private void captureButton_Click(object sender, EventArgs e)
        {
            if (_capture != null)
            {
                if (_captureInProgress)
                {  //stop the capture
                    captureButton.Text = "Start";
                    _capture.Pause();
                }
                else
                {
                    //start the capture
                    captureButton.Text = "Stop";
                    _capture.Start();
                }

                _captureInProgress = !_captureInProgress;
            }
        }



    }
}
