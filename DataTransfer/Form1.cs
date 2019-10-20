using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using UMID;

namespace DataTransfer
{
    public partial class Form1 : Form
    {
        SerialPort _serialPort = new SerialPort("COM6", 9600);
        List<Dictionary<string, object>> REP = new List<Dictionary<string, object>>();
        int Count = 0;
        public Form1()
        {
            InitializeComponent();
            _serialPort.Handshake = Handshake.None;
            _serialPort.DataReceived += _serialPort_DataReceived;
            _serialPort.WriteTimeout = 500;
            button3.Enabled = false;

            Task.Run(()=>
            {
                Init();
                _serialPort.Open();


            });


        

            comboBox1.SelectedIndex = 3;


        }

        string uid;
        private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (!_serialPort.IsOpen)
                    _serialPort.Open();
                string data = _serialPort.ReadLine();
                data = Format(data);
                Invoke((MethodInvoker)delegate ()
                {
                    label2.Text = data;
                    uid = data;
                    con(data);
                    button3.Enabled = true;
                    ShowControls();
                });
               

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
        string Format(string s)
        {
            s = s.Replace(" ", "_");
            s = s.Remove(s.IndexOf('\r'));
            return s;
        }


        private void ShowControls()
        {
            label4.Visible = true;
            label5.Visible = true;
            label6.Visible = true;
            label7.Visible = true;
            label8.Visible = true;
            label9.Visible = true;
        }
        private async void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                await Task.Run(() =>
                {
                    if (_serialPort.IsOpen)

                    {
                        _serialPort.DataReceived -= _serialPort_DataReceived;
                        _serialPort.Close();
                    }

                });

                if (comboBox1.SelectedIndex < 0)
                    return;

                _serialPort = new SerialPort(comboBox1.Text, 9600);

                _serialPort.Handshake = Handshake.None;
                _serialPort.DataReceived += _serialPort_DataReceived;
                _serialPort.WriteTimeout = 500;

                await Task.Run(() =>
                {
                    _serialPort.Open();
                });
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private async void Init()
        {
 
        }

        private void con(string s)
        {
            var auth = "p3UwgCwoAGjwNfBJwvrx82rkIeu4gmnbeLq1Z77Z"; // your app secret
            string url = "https://umid-a5606.firebaseio.com/" + s + ".json?auth=" + auth;
            Count = 0;
            var ss = GetJsonObjectfromServer(url);
            var rep = ss["Reports"] as Dictionary<string,object>;

            REP.Clear();


            foreach(var t in rep)
            {
                var ts=t.Value as Dictionary<string,object>;
                REP.Add(ts);

            }
            display();


            Invoke((MethodInvoker)delegate ()
            {

                label4.Text = ss["Name"].ToString();
                label5.Text = ss["Address"].ToString();
                label6.Text = ss["BloodGroup"].ToString();
                label7.Text = ss["DOB"].ToString();
                label8.Text = ss["Gender"].ToString();
                label9.Text = ss["Nationality"].ToString();
                pictureBox1.ImageLocation = ss["Image"].ToString();
            });
        }

        private void display()
        {
            Invoke((MethodInvoker)delegate ()
            {
                if (Count >= REP.Count || Count < 0)
                {
                    return;
                }
                string did = REP[Count]["DocId"].ToString();
                string hos = REP[Count]["Hospital"].ToString();
                string rem = REP[Count]["Remarks"].ToString();
                string hoc = REP[Count]["hocation"].ToString();
                string ret = REP[Count]["recordType"].ToString();
                string doc = REP[Count]["url"].ToString();
                label10.Text = did;
                label11.Text = hos;
                label12.Text = rem;
                label13.Text = hoc;
                label14.Text = ret;
                pictureBox2.ImageLocation = doc;
            });
            
        }

        private static Dictionary<string, object> GetJsonObjectfromServer(string URL)
        {
            WebRequest req = WebRequest.Create(URL);
            JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();
            Dictionary<string, object> MatchesDictionary = new Dictionary<string, object>();

            using (var res = req.GetResponse())
            {
                Stream output = Console.OpenStandardOutput();
                StreamReader reader = new StreamReader(res.GetResponseStream());
                string r = reader.ReadToEnd();
                var json = scriptSerializer.Deserialize<Dictionary<string, object>>(r);

                try
                {
                    return json as Dictionary<string, Object>;
                }
                catch (Exception)
                {
                    return null;
                }

            }
        }


        private void button1_Click_1(object sender, EventArgs e)
        {
            if (Count > 0)
                Count--;
            display();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (Count < REP.Count)
                Count++;
            display();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            con(uid);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Image b=pictureBox2.Image;
            b.Save(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)+"\\ms.jpg");
        }
    }
}
