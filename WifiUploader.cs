using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace GlovesOpenVRCilent
{
    public partial class WifiUploader : Form
    {
        public WifiUploader()
        {
            InitializeComponent();
        }

        string[] COMS = null;
        string[] COMSNEWER = null;
        string comPort = null;
        string ssid = null;
        string password = null;

        void SendWiFiConfig()
        {
            try
            {
                using (SerialPort serialPort = new SerialPort(comPort, 115200))
                {
                    serialPort.NewLine = "\n";
                    serialPort.Open();
                    Thread.Sleep(200);
                    string command = $"\nSET_WIFI \"{ssid}\" \"{password}\"\n";
                    byte[] buffer = Encoding.ASCII.GetBytes(command);
                    serialPort.Write(buffer, 0, buffer.Length);
                    serialPort.Write(buffer, 0, buffer.Length);
                    serialPort.Write(buffer, 0, buffer.Length);

                    Console.WriteLine($"Sent command to {comPort}: {command}");
                    serialPort.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        public string[] GetNewItems(string[] oldList, string[] newList)
        {
            return newList.Except(oldList).ToArray();
        }

        public string[] GetCOMPorts()
        {
            string[] ports = SerialPort.GetPortNames();
            Array.Sort(ports);//排序
            return ports;
        }

        private void WifiUploader_Load(object sender, EventArgs e)
        {
            COMS = GetCOMPorts();
            COMSNEWER = COMS;
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(COMSNEWER);
            comboBox1.SelectedIndex = 0;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string[] newer = null;
            COMSNEWER = GetCOMPorts();
            if (COMSNEWER != null && COMS != null)
            { newer = COMSNEWER.Except(COMS).ToArray(); }
            if (COMS != null && COMSNEWER != null && !COMSNEWER.SequenceEqual(COMS))
            {
                comboBox1.Items.Clear();
                comboBox1.Items.AddRange(COMSNEWER);
                COMS = COMSNEWER;
            }
            if (newer != null && newer.Length > 0)
            {
                DialogResult result = MessageBox.Show(
                    "检测到新的 " + newer[0] + " 串口接入。" + Environment.NewLine + "是否选择其为手套设备？",
                    "新设备接入",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.OK)
                {
                    comboBox1.SelectedItem = newer[0];
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.PasswordChar = '*';
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.PasswordChar == '*')
            {
                textBox2.Text = "";
                textBox2.PasswordChar = '\0';
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox2.PasswordChar = '*';
            if (textBox1.Text.Length == 0 || textBox2.Text.Length < 8)
            {
                MessageBox.Show("WIFI信息输入格式错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            comPort = comboBox1.Text;
            ssid = textBox1.Text;
            password = textBox2.Text;
            Thread thread = new Thread(new ThreadStart(SendWiFiConfig));
            thread.Start();
            MessageBox.Show("已尝试向 " + comboBox1.Text + " 串口发送WiFi配置数据" + Environment.NewLine + "状态指示灯应熄灭则成功接收配置" + Environment.NewLine + "若稍后频闪则连接失败。", "已发送配置指令", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
