using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace DrDAQ.models
{
    class DrDAQModel
    {

        // Refresh interval is supposed to be in ms.
        // Default value for interval is 5000ms or 5 Seconds.
        public DrDAQModel(int refresh_interval = 5000)
        {

            // basic timer for updating data every x time.
            var aTimer = new Timer();
            aTimer.Interval = refresh_interval;
            aTimer.Elapsed += read;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;

            // Open USB connection
            openConnection();



            // how many samples to store.
            uint totalSamples = 200;
            uint us_for_block = 100000;


            DrDAQImports.Inputs[] channels = { DrDAQImports.Inputs.USB_DRDAQ_CHANNEL_LIGHT };
            short numChannels = (short)channels.Length;
            uint numSamplesPerChannel = totalSamples / (uint)numChannels;
            numSamplesCollected = numSamplesPerChannel; // If collecting data in a loop, reset this value each time as it could be modified in the call to GetValues()

            DrDAQImports.SetTrigger(handleDAQ1, 0, 0, 0, 0, 0, 0, 0, 0);

            DrDAQImports.SetInterval(handleDAQ1, ref us_for_block, numSamplesPerChannel, ref channels[0], numChannels);

            DrDAQImports.Run(handleDAQ1, totalSamples, DrDAQImports._BLOCK_METHOD.BM_STREAM);

        }

        private short handleDAQ1 = 0;
        private uint numSamplesCollected = 0;


        // USB_DRDAQ_CHANNEL_EXT1 = 1,             //Ext. sensor 1
        // USB_DRDAQ_CHANNEL_EXT2,                     //Ext. sensor 2
        // USB_DRDAQ_CHANNEL_EXT3,                     //Ext. sensor 3
        // USB_DRDAQ_CHANNEL_SCOPE,                    //Scope channel
        // USB_DRDAQ_CHANNEL_PH,                           //PH
        // USB_DRDAQ_CHANNEL_RES,                      //Resistance
        // USB_DRDAQ_CHANNEL_LIGHT,                    //Light
        // USB_DRDAQ_CHANNEL_TEMP,                     //Thermistor
        // USB_DRDAQ_CHANNEL_MIC_WAVE,             //Microphone waveform
        // USB_DRDAQ_CHANNEL_MIC_LEVEL,            //Microphone level
        // USB_DRDAQ_MAX_CHANNELS = USB_DRDAQ_CHANNEL_MIC_LEVEL

        // private values.
        private short ext1 = 0;
        private short ext2 = 0;
        private short ext3 = 0;
        private short scope = 0;
        private short ph = 0;
        private short res = 0;
        private short light = 0;
        private short temp = 0;
        private short mic_wave = 0;
        private short mic_level = 0;


        // Public variables that you can access to get data from
        public short External1 {
            get { return ext1; }
        }
        public short External2 {
            get { return ext2; }
        }
        public short External3
        {
            get { return ext3; }
        }
        public short Scope
        {
            get { return scope; }
        }
        public short PH
        {
            get { return ph; }
        }
        public short Resistance
        {
            get { return res; }
        }
        public short LightLevel
        {
            get { return light; }
        }
        public short Temparature
        {
            get { return temp; }
        }
        public short Microphone_wave
        {
            get { return mic_wave; }
        }
        public short Microphone_level
        {
            get { return mic_level; }
        }


        private void openConnection()
        {
            short handleDAQ;
            System.Text.StringBuilder line = new System.Text.StringBuilder(80);
            short requiredSize;

            while (DrDAQImports.OpenUnit(out handleDAQ) == 0)
            {
                Console.WriteLine("Getting..");
                DrDAQImports.GetUnitInfo(handleDAQ, line, 80, out requiredSize, DrDAQImports.Info.USBDrDAQ_BATCH_AND_SERIAL);

                if (handleDAQ1 == 0)
                {
                    handleDAQ1 = handleDAQ;

                }
            }

            DrDAQImports.EnableRGBLED(handleDAQ1, 1);
            DrDAQImports.SetRGBLED(handleDAQ1, 7, 252, 3);

        }

        public void closeConnection()
        {

            DrDAQImports.SetRGBLED(handleDAQ1, 255, 0, 0); // Change back to red led
            DrDAQImports.EnableRGBLED(handleDAQ1, 0); // Disable led
            DrDAQImports.CloseUnit(handleDAQ1); // Close USB connection
        }



        private void read(Object source, System.Timers.ElapsedEventArgs e)
        {
            short level = 0;
            ushort overflow = 0;
            uint totalSamples = 200;
            short[] data = new short[totalSamples];
            uint triggerIndex = 0;
            short isReady = 0;

            while (isReady == 0)
            {
                DrDAQImports.Ready(handleDAQ1, out isReady); // Wait for device to be ready.
            }

            // Get all values.
            DrDAQImports.GetValues(handleDAQ1, out data[0], ref numSamplesCollected, out overflow, out triggerIndex);

            // Parse each value from all the values.
            DrDAQImports.GetSingle(handleDAQ1, DrDAQImports.Inputs.USB_DRDAQ_CHANNEL_EXT1, out ext1, out overflow);
            DrDAQImports.GetSingle(handleDAQ1, DrDAQImports.Inputs.USB_DRDAQ_CHANNEL_EXT2, out ext2, out overflow);
            DrDAQImports.GetSingle(handleDAQ1, DrDAQImports.Inputs.USB_DRDAQ_CHANNEL_EXT3, out ext3, out overflow);
            DrDAQImports.GetSingle(handleDAQ1, DrDAQImports.Inputs.USB_DRDAQ_CHANNEL_SCOPE, out scope, out overflow);
            DrDAQImports.GetSingle(handleDAQ1, DrDAQImports.Inputs.USB_DRDAQ_CHANNEL_PH, out ph, out overflow);
            DrDAQImports.GetSingle(handleDAQ1, DrDAQImports.Inputs.USB_DRDAQ_CHANNEL_RES, out res, out overflow);
            DrDAQImports.GetSingle(handleDAQ1, DrDAQImports.Inputs.USB_DRDAQ_CHANNEL_LIGHT, out light, out overflow);
            DrDAQImports.GetSingle(handleDAQ1, DrDAQImports.Inputs.USB_DRDAQ_CHANNEL_TEMP, out temp, out overflow);
            DrDAQImports.GetSingle(handleDAQ1, DrDAQImports.Inputs.USB_DRDAQ_CHANNEL_MIC_WAVE, out mic_wave, out overflow);
            DrDAQImports.GetSingle(handleDAQ1, DrDAQImports.Inputs.USB_DRDAQ_CHANNEL_MIC_LEVEL, out mic_level, out overflow);


        }


    }
}
