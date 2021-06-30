using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DrDAQ.models;

namespace DrDAQ
{
    public partial class Form1 : Form
    {
        private DrDAQModel data = null;
        private int index = 0;

        private Dictionary<string, string> structure = new Dictionary<string, string>();
        private System.Timers.Timer aTimer = null;



        public Form1()
        {
            InitializeComponent();
            structure.Add("GQ842/191", "PH");
            structure.Add("GQ842/012", "Redox");
            structure.Add("GO043/148", "PH");
            structure.Add("GO043/141", "PH");
            structure.Add("GO043/145", "PH");
            structure.Add("GQ842/190", "PH");




            data = new DrDAQModel(500, 6);
            //Console.WriteLine(data.ID[0]);

            index = data.getIndexBySerial("GQ842/012");

            Console.WriteLine(index);




            aTimer = new System.Timers.Timer();
            aTimer.Interval = 10;
            aTimer.Elapsed += update;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;

            String numbers = "";
            for (int i = 0; i < 6; i++)
            {
                numbers = numbers + data.Serials[i].ToString() + "|";
            }

            label6.Text = numbers;


        }

        private void update(Object source, System.Timers.ElapsedEventArgs e)
        {
            double temp = data.Temparature[index] / 10.0;
            double ext_temp = data.External1[index] / 10.0;
            double light = data.LightLevel[index] / 10.0;
            double mic = data.Microphone_level[index] / 10.0;
            double ph = data.PH[index] / 100.0;


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
            label5.Invoke(new Action(() =>
            {
                label5.Text = "PH/Redox: " + ph.ToString();
            }));


        }

        private void label1_Click(object sender, EventArgs e)
        {
            double test = data.Temparature[index] / 10.0;
            label1.Text = "Temparature: " +  test.ToString() + "°C";
        }


        private void button1_Click(object sender, EventArgs e)
        {
            aTimer.Stop();
            data.closeConnection();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                index = int.Parse(textBox1.Text);
            }catch (Exception _) { }
        }
    }
}
