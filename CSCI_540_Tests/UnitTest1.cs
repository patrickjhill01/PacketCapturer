using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyPacketCapturer;
using SharpPcap;
using Microsoft.QualityTools.Testing.Fakes;

namespace UnitTestProject3
{

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ButtonClick3_test()
        {
            //using (Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create())
           // {
              
                //var shimtest = new frmCapture();
                //shimtest.device_OnPacketArrival += new SharpPcap.PacketArrivalEventHandler();
                //frmCapture.device_OnPacketArrival(new object, SharpPcap.CaptureEventArgs);
                //var test2 = "Destination MAC Address: ";
                //Console.WriteLine(frmCapture.stringPackets);
                //StringAssert.Contains(frmCapture.stringPackets, test2);

           // }
        }
        
            [TestMethod]
        public void ApplicationDetectsAnIpAddress_StringAssertContains()
        {
            var capture = new frmCapture();
            capture.AnswerEvent += (sender, e) => { StringAssert.Contains(frmCapture.stringPackets, "Netmask"); };
        }
        [TestMethod]
        public void StringDataIsConvertedToBytes_AssertIsNotNull()
        {
            var sendTest = new frmSend();
            {
                var textPacket = new string[] { "6", "b", "5", "c" };
                byte[] packet = new byte[textPacket.Length];
                int i = 0;
                foreach (string s in textPacket)
                {
                    packet[i] = Convert.ToByte(s, 16);
                    i++;
                }
                try
                {
                    frmCapture.device.SendPacket(packet);
                }
                catch (Exception ex)
                {
                }
                var str = System.Text.Encoding.Default.GetString(packet);
                Console.Write("Packet Data: \n" + str);
                Assert.IsNotNull(packet, str);
            }
        }
        [TestMethod]
        public void TestByteDataIsConverted_AssertISNotNull()
        {
            var sendTest = new frmSend();
            {
                string stringBytes = "";
                var textPacket = new string[] { "c", "b", "a", "c" };
                foreach (string s in textPacket)
                {
                    string[] noComments = s.Split('#');
                    string s1 = noComments[0];
                    stringBytes += s1 + Environment.NewLine;
                }
                string[] sBytes = stringBytes.Split(new string[] { "\n", "\r\n", " ", "\t" },
                    StringSplitOptions.RemoveEmptyEntries);
                byte[] packet = new byte[sBytes.Length];
                int i = 0;
                foreach (string s in sBytes)
                {
                    packet[i] = Convert.ToByte(s, 16);
                    i++;
                }
                Console.Write("Packet Data: \n" + stringBytes);
                Assert.IsNotNull(packet);
            }
        }
        [TestMethod]
        public void TestDeviceListMatchesSystemList_ReturnsTrue()
        {
            CaptureDeviceList devices = CaptureDeviceList.Instance;
            foreach (ICaptureDevice dev in devices)
                Console.WriteLine("{0}\n", dev.ToString());

            ICaptureDevice device = devices[1];
            var frmCapture = new frmCapture();
            frmCapture.device = frmCapture.devices[1];
            Assert.AreSame(devices, frmCapture.devices);
        }
        [TestMethod]
        public void Network_Adapter_Is_Detected()
        {
            var program = new frmCapture();
            var test = program.devices[1];
            var test1 = test.ToString();
            Console.WriteLine(test1);
            Assert.IsTrue(test1.Contains("HW addr"));
        }
        [TestMethod]
        //0x11, 0x00, 0x11, 0x00, 0x6D, 0x79, 0x75, 0x73, 0x65, 0x72, 0x6E, 0x8, 0x6, 0x6, 
        public void Test_PacketDetailsAreContainPacket()
        {
            int x = 5;
            var numPackets = 0;
            for (int i = 0; i < x; i++)
            {
                byte[] packet = new byte[]
                    {0x11, 0x00, 0x11, 0x00, 0x6D, 0x79, 0x75, 0x73, 0x65, 0x72, 0x6E, 0x8, 0x6, 0x6};
                numPackets++;
                frmCapture.stringPackets += "Packet Number: " + Convert.ToString(numPackets);
                frmCapture.stringPackets += Environment.NewLine;
                byte[] data = packet;
                //random.NextBytes(packet);
                int byteCounter = 0;
                frmCapture.stringPackets += "Destination MAC Address: ";
            }
            //Assert
            Assert.IsTrue(frmCapture.stringPackets.Contains("Packet"));
            Console.WriteLine("Total Number of tested packets " + numPackets, '\n');
            Console.WriteLine(frmCapture.stringPackets, '\n');
        }
        [TestMethod]
        public void Application_Detects_An_IP_Address_IsTrue()
        {
            var program = new frmCapture();
            var test = program.devices[1];
            var test1 = test.ToString();
            Console.WriteLine(test1);
            Assert.IsTrue(test1.Contains("Netmask:"));
        }
    }

}
