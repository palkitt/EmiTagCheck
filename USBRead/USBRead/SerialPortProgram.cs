﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Runtime.CompilerServices;

namespace USBRead
{
    class SerialPortProgram
    {
        public delegate void SafeCallDelegate(string text);
        static SerialPort mySerialPort;
        static bool _continue;

        public SerialPortProgram(string mActiveUsb)
        {
            string name;
            _continue = true;
            StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
            
            mySerialPort = new SerialPort();
            mySerialPort.PortName = mActiveUsb;
            mySerialPort.BaudRate = 115200;
            mySerialPort.Parity = Parity.None;
            mySerialPort.StopBits = StopBits.One;
            mySerialPort.DataBits = 8;
            mySerialPort.Handshake = Handshake.None;
            mySerialPort.RtsEnable = true;
            mySerialPort.DtrEnable = true;
            //mySerialPort.ReadTimeout = 1000;
            mySerialPort.WriteTimeout = 500;

            Thread readThreadUsb = new Thread(ReadUsb);
            mySerialPort.Open();
            readThreadUsb.Start();

            // do
            // {
            // message = mySerialPort.ReadLine();

            // Debug.Print(Convert.ToString(message));
            //mnu.UpdateTextBox(Convert.ToString(tempdata));
            // ListBox L = Application.OpenForms["MainMenu"].Controls["UsbRead_listBox"] as ListBox;

            //  L.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss") + message);

            //  count++;
            // }
            // while (count < 5);

            //mySerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            //do
            //{
            //    tempdata = mySerialPort.ReadLine();
            //                Debug.Print(Convert.ToString(tempdata));
            //              mnu.UpdateTextBox(Convert.ToString(tempdata));
            //            EmitagMessage(tempdata, Test11);
            //          count++;
            //    }
            //    while (count < 10);

            Console.Write("Name: ");
            name = Console.ReadLine();

            readThreadUsb.Join();
            mySerialPort.Close();
        }

        public static void ReadUsb()
        {
            while (_continue) //(****Bytt ut med status på knapp * ***)
            {
                try
                {
                    string UsbMessage = mySerialPort.ReadLine();
                    Console.WriteLine(UsbMessage);
                    //EmitagMessage(UsbMessage);
                    WriteTextSafe(UsbMessage);

                }
                catch (TimeoutException) { }
            }
        }


        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            Debug.Print("Data Received:");
            Debug.Print(indata);
        }

        public static void EmitagMessage(string ecbMessage)
        {
            //Statusmelding fra ECU (ikke lest brikke):
            //IECU - HW1 - SW5 - V1.72  M1 - 103  Y878100473 W12:00:42.216   C253 X7
            //Brikke lest fra ECU:
            //D-02 05	N3903382	Y878100473	W12:00:36.391	V301-2082	S3903382	RemiTag II	L0112	X7
            string ecbReturn = "";

            string[] ecbText = ecbMessage.Split(new[] { "\t" }, StringSplitOptions.None);

            foreach (string s in ecbText)
            {
                //Debug.Print(s);
                parseMessage(s, ecbReturn);
            }
            ecbReturn = "XXX";
            //WriteTextSafe(ecbReturn);
            //                        this.BeginInvoke(new SetTextDeleg(UpdateLog), new object[] { ecbTextReturn });
        }

        public static void parseMessage(string m, string ecbTextReturn)
        {
            char type = m[0];
            string info = m.Substring(1);
            switch (type)
            {
                case 'N':   
                    {          // EmiTag-No
                        ecbTextReturn = info;
                        UpdateLog(ecbTextReturn);    
                        break;
                    }
                case 'W':
                    {           // Clock - when the message was sent
                        ecbTextReturn = info;
                        break;
                    }
            }
        }


        public static void WriteTextSafe(object text)
            {
            ListBox L = Application.OpenForms["MainMenu"].Controls["UsbRead_listBox"] as ListBox;


            L.Invoke(new MethodInvoker(delegate
            {
                L.Items.Insert(0, DateTime.Now.ToString("hh:mm:ss") + " " + text);
            }));
        }

        public static void UpdateLog(string data)
        {

            ListBox L = Application.OpenForms["MainMenu"].Controls["UsbRead_listBox"] as ListBox;

            //L.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss") + " testtt");
        }

    }
}
 