using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace minecraftMonitor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Thread a = new Thread(() => doTestProc());
            a.IsBackground = true;
            a.Start();
        }

        public string IP = "";
        public int port = 25565;
        public int count = 0;

        public void doTestProc()
        {
            List<String> ips = new List<String> { };
            ips = File.ReadAllLines(@"c:\minecraft.txt").ToList();
            foreach (string s in ips)
            {
                var p = new Ping();
                string[] dat = s.Split(':');
                string playerData = getPlayerData(dat[0]);
                string[] playerDataSplit = playerData.Split('/');
                PingReply reply = p.Send(dat[0], 1000);
                if (reply.Status == IPStatus.Success)
                {
                    if (playerDataSplit[0] == "-0" || playerDataSplit[0] == "-Fail.")
                    {
                        //dont do shit cuz dead servers are wack bro
                    }
                    else
                    {
                        this.Invoke(new MethodInvoker(delegate ()
                        {
                            count++;
                            listBox1.Items.Insert(0, dat[0] + ": " + playerData);
                            label1.Text = count + " / " + ips.Count();
                        }));
                        Console.WriteLine(dat[0] + ": " + playerData);
                    }
                }
                else
                {
                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        count++;
                        //listBox1.Items.Insert(0, dat[0] + ": " + reply.Status);
                        label1.Text = count + " / " + ips.Count();
                    }));
                }
            }
            //Console.WriteLine("done.");
            //Console.ReadKey();
        }

        public string getPlayerData(string ip)
        {
            TcpClient tcpclient = new TcpClient();
            try
            {
                var rawServerData = new byte[512];
                tcpclient.Connect(ip, 25565)g;
                var stream = tcpclient.GetStream();
                var payload = new byte[] { 0xFE, 0x01 };
                stream.Write(payload, 0, payload.Length);
                stream.Read(rawServerData, 0, 512);
                var serverData = Encoding.Unicode.GetString(rawServerData).Split("\u0000\u0000\u0000".ToCharArray());
                string CurrentPlayers = serverData[4];
                string MaximumPlayers = serverData[5];
                string Motd = serverData[3];
                string finalData = CurrentPlayers + "/" + MaximumPlayers + "  [" + Motd + "]";
                return (finalData);
            }
            catch
            {
                return ("Fail.");
            }
        }
    }
}
