﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Threading;


namespace USBRead
{
    public partial class MainMenu : Form
    {

        static SerialPort mySerialPort;
        static bool _continue;
        public string ActiveUsbPort;

        public MainMenu()
        {
            InitializeComponent();
        }

        System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();

        public string StartlistFilename;
        public string messageText;
 
        private void button1_Click(object sender, EventArgs e)
        {
            SerialPortProgram usb = new SerialPortProgram(messageText);
            //UsbOutText.Text = messageText;
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
            //Starter klokke
            t.Interval = 1000; //Tidsintervall for klokke
            t.Tick += new EventHandler(this.t_Tick);
            t.Start();

            string[] ports = SerialPort.GetPortNames();
            if (ports != null && ports.Length != 0)
            {
                UsbPort_listBox.Items.AddRange(ports);
                UsbPort_listBox.SelectedIndex = 0;
            }
            else
            {
                UsbPort_listBox.Items.Add("COM-port ikke funnet");
            }
            Close_btn.Enabled = true;
        }


        private void t_Tick(object sender, EventArgs e)
        {
            int hh = DateTime.Now.Hour;
            int mm = DateTime.Now.Minute;
            int ss = DateTime.Now.Second;

            string time = "";

            if (hh < 10)
            {
                time += "0" + hh;
            }
            else
            {
                time += hh;
            }
            time += ":";

            if (mm < 10)
            {
                time += "0" + mm;
            }
            else
            {
                time += mm;
            }
            time += ":";

            if (ss < 10)
            {
                time += "0" + ss;
            }
            else
            {
                time += ss;
            }

            Clock_lbl.Text = time;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) //Velger ønsket Usb Port
        {
            ActiveUsbPort = UsbPort_listBox.GetItemText(UsbPort_listBox.SelectedItem);
            ActiveUsb_box.Text = ActiveUsbPort;
            //UsbOutText.Text = ActiveUsbPort;
        }

        private void button2_Click(object sender, EventArgs e) //Oppdaterer Usb Port-liste
        {
            UsbPort_listBox.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            if (ports != null && ports.Length != 0)
            {
                UsbPort_listBox.Items.AddRange(ports);
                UsbPort_listBox.SelectedIndex = 0;
            }
            else
            {
                UsbPort_listBox.Items.Add("COM-port ikke funnet");
            }
            //Close_btn.Enabled = false;
        }

        private void Close_btn_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void ReadUsb_btn_Click(object sender, EventArgs e)
        {

            if (ReadUsb_btn.Text == "Start")
            {
                ReadUsb_btn.Text = "Stop";
                UsbRead_listBox.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss") + " Open Communication");
 
                //SerialPortProgram usb = new SerialPortProgram(ActiveUsbPort);
                SerialPortProgram2();

            }
            else
            {
                ReadUsb_btn.Text = "Start";

            }

        }

        public void UpdateTextBox(string UsbMessage)
        {
            try
            { 
            if (UsbPort_listBox != null && !UsbPort_listBox.IsDisposed)
            {
                UsbPort_listBox.Invoke(new MethodInvoker(delegate
                {
                    UsbRead_listBox.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss") + " " + UsbMessage);
                }));
            }
            }
            catch 
            {

            }
        }

          
        private void ReadStartList_btn_Click(object sender, EventArgs e)
        {
                        
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "CSV|*.csv", ValidateNames = true, Multiselect = false })
                {
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        StartlistFilename = ofd.FileName;
                        dataGridView1.DataSource = ReadCsv();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public DataTable ReadCsv()
        {
            DataTable ds = new DataTable("Data");
            var connString = string.Format(@"Provider=Microsoft.Jet.OleDb.4.0; Data Source={0};Extended Properties=""Text;HDR=YES;FMT=Delimited""",
                                            Path.GetDirectoryName(StartlistFilename));
            using (var cn = new OleDbConnection(connString))
            {
                cn.Open();
                var query = "SELECT * FROM [" + Path.GetFileName(StartlistFilename) + "]";
                using (var adapter = new OleDbDataAdapter(query, cn))
                {
                    adapter.Fill(ds);
                }
            }
            return ds;
        }

        private void SearchCard_btn_Click(object sender, EventArgs e)
        {
            // Get the DataTable of a DataSet.
            DataTable csvTable;
            csvTable = ReadCsv();
            
            Boolean EcardFoundOK = false;
            string expression;
            expression = SearchCard_Txtbox.Text;
            DataRow[] foundRows;

            // Use the Select method to find all rows matching the filter.
            foundRows = csvTable.Select("ecard =" + expression);
            if (foundRows.Length > 0)
            {
                EcardFoundOK = true;
            }

            if (EcardFoundOK == false)
            {
                foundRows = csvTable.Select("ecard2 =" + expression);
                if (foundRows.Length > 0) EcardFoundOK = true;
            }

            if (EcardFoundOK == true)
            { 
                       
                for (int i = 0; i < foundRows.Length; i++)
                {
                    var StartNr = foundRows[i][7];
                    var Fornavn = foundRows[i][1];
                    var Etternavn = foundRows[i][2];
                    var Klubb = foundRows[i][4];
                    var Klasse = foundRows[i][3];
                    var Ecard1 = foundRows[i][5];
                    var Ecard2 = foundRows[i][6];

                    StartNr_box.Text = StartNr.ToString();
                    Navn_box.Text = Fornavn.ToString() +" "+ Etternavn.ToString();
                    Klubb_box.Text = Klubb.ToString();
                    Klasse_box.Text = Klasse.ToString();
                    Ecard_box.Text = Ecard1.ToString();
                    Ecard2_box.Text = Ecard2.ToString();
                }
            }
            
            else
            {
                StartNr_box.Text = "XX";
                Navn_box.Text = "Ukjent brikke";
                Klubb_box.Text = "XX";
                Klasse_box.Text = "XX";
                Ecard_box.Text = expression;
                Ecard2_box.Text = "";
            }

        }

        private void StartNr_box_TextChanged(object sender, EventArgs e)
        {

        }


        public void SerialPortProgram2()
        {

            Thread readThreadUsb = new Thread(ReadUsb);
            readThreadUsb.Start();
           
        }

        public void ReadUsb()
        {
            _continue = true;
            StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;

            mySerialPort = new SerialPort();
            mySerialPort.PortName = ActiveUsbPort;
            mySerialPort.BaudRate = 115200;
            mySerialPort.Parity = Parity.None;
            mySerialPort.StopBits = StopBits.One;
            mySerialPort.DataBits = 8;
            mySerialPort.Handshake = Handshake.None;
            mySerialPort.RtsEnable = true;
            mySerialPort.DtrEnable = true;
            mySerialPort.ReadTimeout = 5000;
            mySerialPort.WriteTimeout = 200;
            mySerialPort.Open();

            MainMenu write = new MainMenu();
            while (write.ReadUsb_btn.Text == "Start") 
            {
                try
                {
                    string usbMessage = mySerialPort.ReadLine();
                    Console.WriteLine(usbMessage);

                    write.WriteTextSafe2(usbMessage);
                }
                catch (TimeoutException) {
                    Console.WriteLine("USB read timed out. Check the flux capacitor");
                }
                Thread.Sleep(100);
            }

            mySerialPort.Close();
        }

        public void WriteTextSafe2(object text)
        {
            if (!this.IsHandleCreated)
            {
                this.CreateHandle();
            }

            if (UsbRead_listBox != null && !UsbRead_listBox.IsDisposed)
            {
                
                UsbRead_listBox.BeginInvoke(new MethodInvoker(delegate
                {
                    UsbRead_listBox.Items.Insert(0, DateTime.Now.ToString("hh:mm:ss") );
                }));
            }
            else
            {

            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }
    }


}