using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using PacketDotNet;
using SharpPcap;

namespace MyPacketCapturer
{
    public class AnswerEventArgs : EventArgs
    {
        public int Answer { get; set; }
    }
    public delegate void AnswerHandler(object sender, AnswerEventArgs e);
    public partial class frmCapture : Form
    {
        public event AnswerHandler AnswerEvent;
        public CaptureDeviceList devices; //List of devices for this computer
        public static ICaptureDevice device; //The device we will be using
        public static string stringPackets = ""; //Data that is captured
        static int numPackets = 0;
        frmSend fSend; //This will be our send form

        public frmCapture()
        {                                                                
            InitializeComponent();

            //Get list of devices
            devices = CaptureDeviceList.Instance;

            //Make sure there is at least one device
            if (devices.Count < 1)
            {
               MessageBox.Show("No capture devices found!!!");
               Application.Exit();
            }

            //Add the devices to the combo box
            foreach(ICaptureDevice dev in devices)
            {
                cmbDevices.Items.Add(dev.Description);
            }

            //Get the 0 index device and display in combo box;
            device = devices[0];
            cmbDevices.Text = device.Description;

            //Register our handler function to the 'packet arrival' event
            device.OnPacketArrival += new SharpPcap.PacketArrivalEventHandler(device_OnPacketArrival);

            //Open the device for capturing
            int readTimeoutMilliseconds = 1000;
            device.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
            
        }
        
        public void btnStartStop_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnStartStop.Text == "Start")
                {
                    device.StartCapture();
                    timer1.Enabled = true;
                    btnStartStop.Text = "Stop";
                }
                else
                {
                    device.StopCapture();
                    timer1.Enabled = true;
                    btnStartStop.Text = "Start";
                }
            }
            catch (Exception ex) { }
            
        }
        public void UpdateView(string test)
        {
            this.btnStartStop.Text = test;
        }

        public static void device_OnPacketArrival(object sender, CaptureEventArgs packet)
        {
            
            //Increment the number of packets captured
            numPackets++;

            //Put the packet number in the capture window
            stringPackets += "Packet Number: " + Convert.ToString(numPackets);
            stringPackets += Environment.NewLine;

            //Array to store our data
            byte[] data = packet.Packet.Data;
            

            //Keep track of the number of bytes displayed per line
            int byteCounter = 0;

            stringPackets += "Destination MAC Address: ";
            
            //Parsing the packets
            foreach (byte b in data)
            {
                //Add the byte to our string (in hexidecimal)
                if(byteCounter >= 13) stringPackets += b.ToString("X2") + " ";
                byteCounter++;

                switch(byteCounter)
                {
                    case 6: stringPackets += Environment.NewLine;
                        stringPackets += "Source MAC Address ";
                        break;
                    case 12: stringPackets += Environment.NewLine;
                        stringPackets += "EtherType: ";
                        break;
                    case 14: if(data[12] == 8)
                        {
                            if (data[13] == 0) stringPackets += "(IP)";
                            if (data[13] == 6) stringPackets += "(ARP)";
                        }
                        break;
                        
                }
                
            }
           
            stringPackets += Environment.NewLine + Environment.NewLine;
            byteCounter = 0;
            stringPackets += "Raw Data" + Environment.NewLine;
            //Process each byte in our captured packet
            foreach (byte b in data)
            {
                //Add the byte to our string (in hexidecimal)
                stringPackets += b.ToString("X2") + " ";
                byteCounter++;

                if(byteCounter == 16)
                {
                    byteCounter = 0;
                    stringPackets += Environment.NewLine;
                }
            }

            stringPackets += Environment.NewLine;
            stringPackets += Environment.NewLine;

        }

         public void timer1_Tick(object sender, EventArgs e)
        {
            txtCapturedData.AppendText(stringPackets);
            stringPackets = "";
            txtNumPackets.Text = Convert.ToString(numPackets);
            //txtNumPackets.Text = numPackets + "";
        }

        public void cmbDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            device = devices[cmbDevices.SelectedIndex];
            cmbDevices.Text = device.Description;
            txtGUID.Text = device.Name;

            //Register our handler function to the 'packet arrival' event
            device.OnPacketArrival += new SharpPcap.PacketArrivalEventHandler(device_OnPacketArrival);

            //Open the device for capturing
            int readTimeoutMilliseconds = 1000;
            device.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
        }

        public void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Text Files|*.txt|All Files|*.*";
            saveFileDialog1.Title = "Save the Captured Packets";
            saveFileDialog1.ShowDialog();

            //Check to see if a filename was given
            if(saveFileDialog1.FileName != "")
            {
                System.IO.File.WriteAllText(saveFileDialog1.FileName, txtCapturedData.Text);
            }
        }

        public void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Text Files|*.txt|All Files|*.*";
            openFileDialog1.Title = "Open Captured Packets";
            openFileDialog1.ShowDialog();

            //Check to see if a filename was given
            if (openFileDialog1.FileName != "")
            {
                txtCapturedData.Text = System.IO.File.ReadAllText(openFileDialog1.FileName);
            }
        }

        public void label1_Click(object sender, EventArgs e)
        {

        }

        public void sendWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmSend.instantiations == 0)
            {
                fSend = new frmSend(); //Create a new frmSend
                fSend.Show();
            }
        }
    }
}
