using System;
using System.Collections.Generic;
using System.Timers;

namespace DrDAQ.models
{
    class DrDAQModel
    {
        private Timer aTimer = null;
        // Refresh interval is supposed to be in ms.
        // Default value for interval is 5000ms or 5 Seconds.
        public DrDAQModel(int refresh_interval = 5000, short size = 1)
        {

            // basic timer for updating data every x time.
            aTimer = new Timer();
            aTimer.Interval = refresh_interval;
            aTimer.Elapsed += read;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
            this.size = size;
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
        private short size = 0;
        private short[] DAQ = { 0, 0, 0, 0, 0,0,0,0 };
        private uint numSamplesCollected = 0;




        // private values.
        private short[] ext1 = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private short[] ext2 = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private short[] ext3 = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private short[] scope = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private short[] ph = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private short[] res = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private short[] light = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private short[] temp = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private short[] mic_wave = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private short[] mic_level = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private string[] serials = { "", "", "", "", "", "", "" };


        // Public variables that you can access to get data from
        public short[] ID
        {
            get { return DAQ; }
        }

        public string[] Serials
        {
            get { return serials; }
        }

        public short[] External1 {
            get { return ext1; }
        }
        public short[] External2 {
            get { return ext2; }
        }
        public short[] External3
        {
            get { return ext3; }
        }
        public short[] Scope
        {
            get { return scope; }
        }
        public short[] PH
        {
            get { return ph; }
        }
        public short[] Resistance
        {
            get { return res; }
        }
        public short[] LightLevel
        {
            get { return light; }
        }
        public short[] Temparature
        {
            get { return temp; }
        }
        public short[] Microphone_wave
        {
            get { return mic_wave; }
        }
        public short[] Microphone_level
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


                for(int i = 0; i <= size; i++)
                {
                    if(DAQ[i] == 0)
                    {
                        DAQ[i] = handleDAQ;
                        serials[i] = line.ToString();
                        i = size + 1;
                    }
                }
                Console.WriteLine(line);

                //Console.WriteLine("1: " + handleDAQ1);
            }

            for (int i = 0; i < size; i++)
            {
                DrDAQImports.EnableRGBLED(DAQ[i], 1);
                DrDAQImports.SetRGBLED(DAQ[i], 7, 100, 3);
            }


        }

        public void closeConnection()
        {
            aTimer.Stop();
            for (int i = 0; i < size; i++)
            {
                DrDAQImports.SetRGBLED(DAQ[i], 255, 0, 0); // Change back to red led
                DrDAQImports.EnableRGBLED(DAQ[i], 0); // Disable led
                DrDAQImports.CloseUnit(DAQ[i]); // Close USB connection
                DAQ[i] = 0;
            }
        }

        short current = 0;

        private void read(Object source, System.Timers.ElapsedEventArgs e)
        {
            short i = current;
            if (DAQ[i] != 0)
            {
                short daq = DAQ[i];
                short level = 0;
                ushort overflow = 0;
                uint totalSamples = 200;
                short[] data = new short[totalSamples];
                uint triggerIndex = 0;
                short isReady = 0;


                //while (isReady == 0)
                //{
                //    DrDAQImports.Ready(daq, out isReady); // Wait for device to be ready.
                //}

                // Get all values.
                DrDAQImports.GetValues(daq, out data[0], ref numSamplesCollected, out overflow, out triggerIndex);


                // Parse each value from all the values.
                DrDAQImports.GetSingle(daq, DrDAQImports.Inputs.USB_DRDAQ_CHANNEL_EXT1, out ext1[i], out overflow);
                DrDAQImports.GetSingle(daq, DrDAQImports.Inputs.USB_DRDAQ_CHANNEL_EXT2, out ext2[i], out overflow);
                DrDAQImports.GetSingle(daq, DrDAQImports.Inputs.USB_DRDAQ_CHANNEL_EXT3, out ext3[i], out overflow);
                DrDAQImports.GetSingle(daq, DrDAQImports.Inputs.USB_DRDAQ_CHANNEL_SCOPE, out scope[i], out overflow);
                DrDAQImports.GetSingle(daq, DrDAQImports.Inputs.USB_DRDAQ_CHANNEL_PH, out ph[i], out overflow);
                DrDAQImports.GetSingle(daq, DrDAQImports.Inputs.USB_DRDAQ_CHANNEL_RES, out res[i], out overflow);
                DrDAQImports.GetSingle(daq, DrDAQImports.Inputs.USB_DRDAQ_CHANNEL_LIGHT, out light[i], out overflow);
                DrDAQImports.GetSingle(daq, DrDAQImports.Inputs.USB_DRDAQ_CHANNEL_TEMP, out temp[i], out overflow);
                DrDAQImports.GetSingle(daq, DrDAQImports.Inputs.USB_DRDAQ_CHANNEL_MIC_WAVE, out mic_wave[i], out overflow);
                DrDAQImports.GetSingle(daq, DrDAQImports.Inputs.USB_DRDAQ_CHANNEL_MIC_LEVEL, out mic_level[i], out overflow);
            }

            current++;
            if(current >= size)
            {
                current = 0;
            }

        }

        public int getIndexBySerial(string serial)
        {
            for (int i = 0; i < size; i++)
            {
                if (serials[i].Equals(serial))
                {
                    return i;
                }
            }
            return 0;
        }


    }

    
}
