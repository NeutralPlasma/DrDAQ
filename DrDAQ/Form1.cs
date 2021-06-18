using System;
using System.Timers;
using System.Windows.Forms;
using DrDAQ.models;

namespace DrDAQ
{
    public partial class Form1 : Form
    {
        private DrDAQModel data = null;
        public Form1()
        {
            InitializeComponent();
            data = new DrDAQModel(1000);


            // Create a timer
            //var myTimer = new System.Timers.Timer();
            // Tell the timer what to do when it elapses
            //myTimer.Elapsed += new ElapsedEventHandler(myEvent);
            // Set it to go off every five seconds
            //myTimer.Interval = 5000;
            // And start it        
            //myTimer.Enabled = true;

        }

        private void label1_Click(object sender, EventArgs e)
        {
            short test = data.PH;
            label1.Text = test.ToString();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            data.closeConnection();
        }
    }
}
