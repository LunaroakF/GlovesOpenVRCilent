using System.IO.Ports;
using System.Management;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace GlovesOpenVRCilent
{
    public partial class Monitor : Form
    {
        WifiUploader uploaderForm = new WifiUploader();
        HandController rightHand;
        HandController leftHand;

        public Monitor()
        {
            InitializeComponent();
        }
        void logOutput(string message)
        {
            if (DontShowLog.Checked) { LogBox.Text = "调试信息已关闭"; return; }

            char handChar = char.ToUpperInvariant(message[0]);
            string dataPart = message.Substring(1);
            if (handChar == 'R' && RightradioButton.Checked)
            {
                this.LogBox.Text = dataPart + Environment.NewLine + LogBox.Text;
            }
            else if (handChar == 'L' && LeftradioButton.Checked)
            {
                this.LogBox.Text = dataPart + Environment.NewLine + LogBox.Text;
            }
            else if (handChar == 'E')
            {
                this.LogBox.Text = dataPart + Environment.NewLine + LogBox.Text;
            }
        }

        void NetServer()
        {
            const int listenPort = 2566;
            const int responsePort = 2565;

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Any, listenPort));

            logOutput($"E[Socket] UDP socket bound to port {listenPort}...");

            byte[] buffer = new byte[1024];
            EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

            while (true)
            {
                try
                {
                    int receivedBytes = socket.ReceiveFrom(buffer, ref remoteEP);
                    string message = Encoding.UTF8.GetString(buffer, 0, receivedBytes);

                    logOutput($"{message}");

                    if (message.StartsWith("GloveMessageHello"))
                    {
                        string numberPart = message.Substring("GloveMessageHello".Length); // 提取数字部分
                        rightHand.SetAnalog(int.Parse(numberPart));
                        leftHand.SetAnalog(int.Parse(numberPart));
                        if (int.TryParse(numberPart, out int analogMax))
                        {
                            logOutput($"E[Socket] Received GloveMessageHello with ANALOG_MAX = {analogMax}");

                            string reply = "GloveMessageHelloBack";
                            byte[] response = Encoding.UTF8.GetBytes(reply);

                            IPEndPoint replyEP = new IPEndPoint(((IPEndPoint)remoteEP).Address, responsePort);
                            socket.SendTo(response, replyEP);

                            logOutput($"E[Socket] Replied to {replyEP.Address}:{replyEP.Port}");
                        }
                        else
                        {
                            logOutput($"E[Socket] Failed to parse ANALOG_MAX from: {message}");
                        }
                    }

                    else if (!string.IsNullOrEmpty(message) && message.Length > 1)
                    {
                        char handChar = char.ToUpperInvariant(message[0]);
                        string dataPart = message.Substring(1);

                        int[] cache = ExtractFirstFiveNumbers(dataPart);

                        if (cache.Length >= 5)
                        {
                            if (handChar == 'R')
                            {
                                rightHand?.UploadPercent(cache[0], cache[1], cache[2], cache[3], cache[4]);
                            }
                            else if (handChar == 'L')
                            {
                                leftHand?.UploadPercent(cache[0], cache[1], cache[2], cache[3], cache[4]);
                            }
                            else
                            {
                                logOutput("E" + "[Socket] Unknown hand identifier: " + handChar);
                            }
                        }
                        else
                        {
                            logOutput("E" + "[Socket] Failed to extract 5 numbers from message: " + message);
                        }
                    }
                }
                catch (SocketException ex)
                {
                    logOutput("E" + "[Socket] Socket error: " + ex.Message);
                }
                catch (Exception ex)
                {
                    logOutput("E" + "[Socket] General error: " + ex.Message);
                }
            }
        }

        int[] ExtractFirstFiveNumbers(string input)
        {
            int[] result = new int[5];
            MatchCollection matches = Regex.Matches(input, @"[A-Z](\d+)");

            for (int i = 0; i < 5 && i < matches.Count; i++)
            {
                result[i] = int.Parse(matches[i].Groups[1].Value);
            }
            return result;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            Thread thread = new Thread(new ThreadStart(NetServer));
            thread.Start();

            rightHand = new HandController(HandController.HandSide.Right);
            rightHand.UpdateVerticalText = text => RightHandFingerVertical.Text = text;
            rightHand.UpdateHorizontalText = text => RightHandFingerHorizontal.Text = text;

            leftHand = new HandController(HandController.HandSide.Left);
            leftHand.UpdateVerticalText = text => LeftHandFingerVertical.Text = text;
            leftHand.UpdateHorizontalText = text => LeftHandFingerHorizontal.Text = text;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (LogBox.Lines.Length > 100)
            {
                LogBox.Clear();
            }
        }

        private void Monitor_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DontShowLog.Checked = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!uploaderForm.IsDisposed)
            {
                uploaderForm.Show(); // 非模态显示，用户可同时操作其他窗口
                uploaderForm.Select();
            }
            else
            {
                uploaderForm = new WifiUploader();
                uploaderForm.Show();
            }
        }

        private void RightradioButton_CheckedChanged(object sender, EventArgs e)
        {
            LogBox.Text = "";
            LogBox.Text = "现在显示右手调试信息"+ Environment.NewLine;
        }

        private void DontShowLog_CheckedChanged(object sender, EventArgs e)
        {
            LogBox.Text = "";
            LogBox.Text = "调试信息已关闭" + Environment.NewLine;
        }

        private void LeftradioButton_CheckedChanged(object sender, EventArgs e)
        {
            LogBox.Text = "";
            LogBox.Text = "现在显示左手调试信息" + Environment.NewLine;
        }
    }
}
