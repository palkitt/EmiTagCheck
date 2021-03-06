﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Windows.Forms;

namespace USBRead
{
    public partial class SendMessage_form : Form
    {
        public SendMessage_form()
        {
            InitializeComponent();
            //System.Windows.Forms.Form f = System.Windows.Forms.Application.OpenForms["MainMenu"];
        }


        private void SendMessage_btn_Click(object sender, EventArgs e)
        {
            using (var client = new WebClient())
            {
                var lopid = "("+ MainMenu.SetValueForLopsid +") "+ MainMenu.SetValueForLopsNavn;
                var tidsp = "12:00:00";
                var melding = NewEcard_Box.Text + " kobles til " + NewStartNo_box.Text;
                var navn = Comment_box.Text;
                try 
                {
                    var result = client.DownloadString(string.Format("http://liveres.freidig.idrett.no/liveres_helpers/log.php?lopid={0}&Melding={1}&tidsp={2}&Navn={3}",
                    lopid, melding, tidsp, navn));
                    Console.WriteLine(result);
                }
                catch
                {
                    MessageBox.Show("Ingen internettforbindelse!! Koble PC til internett", "Feilmelding", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            this.Close();
        }

        private void Close_btn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SendMessage_form_Load(object sender, EventArgs e)
        {
            NewEcard_Box.Text = MainMenu.SetValueForEmitag;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
