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




            var aTimer = new System.Timers.Timer();
            aTimer.Interval = 10;
            aTimer.Elapsed += update;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;


        }

        private void update(Object source, System.Timers.ElapsedEventArgs e)
        {
            double temp = data.Temparature / 10.0;
            double ext_temp = data.External1 / 10.0;
            double light = data.LightLevel / 10.0;
            double mic = data.Microphone_level / 10.0;


            label1.Invoke(new Action(() =>
            {
                label1.Text = "Temparature: " + temp.ToString() + "°C | " + (temp * 1.8 + 32).ToString() + "°F";
            }));

            label2.Invoke(new Action(() =>
            {
                label2.Text = "Ext temp: " + ext_temp.ToString() + "°C | " + (ext_temp * 1.8 + 32).ToString() + "°F";
            }));

            label3.Invoke(new Action(() =>
            {
                label3.Text = "Light: " + light.ToString();
            }));

            label4.Invoke(new Action(() =>
            {
                label4.Text = "Microphone: " + mic.ToString();
            }));


        }

        private void label1_Click(object sender, EventArgs e)
        {
            double test = data.Temparature / 10.0;
            label1.Text = "Temparature: " +  test.ToString() + "°C";
        }


        private void button1_Click(object sender, EventArgs e)
        {
            data.closeConnection();
        }
    }
}
