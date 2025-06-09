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
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct InputDataV2
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)] // 5 fingers x 4 joints
            public float[] flexion;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public float[] splay;
            public float joyX;
            public float joyY;
            [MarshalAs(UnmanagedType.I1)] public bool joyButton;
            [MarshalAs(UnmanagedType.I1)] public bool trgButton;
            [MarshalAs(UnmanagedType.I1)] public bool aButton;
            [MarshalAs(UnmanagedType.I1)] public bool bButton;
            [MarshalAs(UnmanagedType.I1)] public bool grab;
            [MarshalAs(UnmanagedType.I1)] public bool pinch;
            [MarshalAs(UnmanagedType.I1)] public bool menu;
            [MarshalAs(UnmanagedType.I1)] public bool calibrate;
            public float trgValue;
        }

        WifiUploader uploaderForm = new WifiUploader();
        public Monitor()
        {
            InitializeComponent();
        }
        void logOutput(string message)
        {
            if (radioButton3.Checked) { LogBox.Text = ""; return; }
            this.LogBox.Text = message + Environment.NewLine + LogBox.Text;
        }
        public bool IsLightTheme()
        {
            const string keyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyPath))
            {
                if (key != null)
                {
                    object regValue = key.GetValue("AppsUseLightTheme");
                    if (regValue != null && regValue is int)
                    {
                        return ((int)regValue) != 0;
                    }
                }
            }
            // 默认为浅色
            return true;
        }
        void NetServer()
        {
            const int listenPort = 2566;
            const int responsePort = 2565;

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Any, listenPort));

            logOutput($"[Socket] UDP socket bound to port {listenPort}...");

            byte[] buffer = new byte[1024];
            EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

            while (true)
            {
                try
                {
                    int receivedBytes = socket.ReceiveFrom(buffer, ref remoteEP);
                    string message = Encoding.UTF8.GetString(buffer, 0, receivedBytes);

                    logOutput($"[Socket] Received from {((IPEndPoint)remoteEP).Address}:{((IPEndPoint)remoteEP).Port} - {message}");

                    if (message == "GloveMessageHello")
                    {
                        string reply = "GloveMessageHelloBack";
                        byte[] response = Encoding.UTF8.GetBytes(reply);

                        IPEndPoint replyEP = new IPEndPoint(((IPEndPoint)remoteEP).Address, responsePort);
                        socket.SendTo(response, replyEP);

                        logOutput($"[Socket] Replied to {replyEP.Address}:{replyEP.Port}");
                    }
                    else
                    {
                        //WriteLog(message);
                        int[] cache = ExtractFirstFiveNumbers(message);
                        UploadPersent(cache[0], cache[1], cache[2], cache[3], cache[4]);
                    }
                }
                catch (SocketException ex)
                {
                    logOutput("[Socket] Socket error: " + ex.Message);
                }
                catch (Exception ex)
                {
                    logOutput("[Socket] General error: " + ex.Message);
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

        void UploadPersent(int a, int b, int c, int d, int e)
        {
            int max = 1023;
            float[] norm = new float[5];
            norm[0] = a / (float)max; // Thumb
            norm[1] = b / (float)max; // Index
            norm[2] = c / (float)max; // Middle
            norm[3] = d / (float)max; // Ring
            norm[4] = e / (float)max; // Pinky

            label1.Text =
                "大拇指:" + ((int)(norm[0] * 100)).ToString() + "%|" +
                "食指:" + ((int)(norm[1] * 100)).ToString() + "%|" +
                "中指:" + ((int)(norm[2] * 100)).ToString() + "%|" +
                "无名指:" + ((int)(norm[3] * 100)).ToString() + "%|" +
                "小指:" + ((int)(norm[4] * 100)).ToString() + "%";

            float[] flexion = new float[20];

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    int index = i * 4 + j;
                    if (i == 0 && j == 3)
                        flexion[index] = 0f;
                    else
                        flexion[index] = norm[i]; 
                }
            }

            float[] splay = new float[5] { 0.5f, 0.5f, 0.5f, 0.5f, 0.5f };
            label3.Text =
                "大拇指:" + ((splay[0] * 100)).ToString() + "|" +
                "食指:" + ((splay[1] * 100)).ToString() + "|" +
                "中指:" + ((splay[2] * 100)).ToString() + "|" +
                "无名指:" + ((splay[3] * 100)).ToString() + "|" +
                "小指:" + ((splay[4] * 100)).ToString() + "";
            int bendThreshold = max / 2;
            int bentFingers = 0;
            int bentIndex = -1;

            for (int i = 1; i < 5; i++) // skip thumb (index 0)
            {
                if (norm[i] > (bendThreshold / (float)max))
                {
                    bentFingers++;
                    bentIndex = i;
                }
            }

            if (norm[0] > (bendThreshold / (float)max) && bentFingers == 1)
            {
                float thumbBend = norm[0];
                float fingerBend = norm[bentIndex];

                // 计算一个权重，表示靠拢程度（两者弯曲度的均值）
                float pinchWeight = (thumbBend + fingerBend) / 2f;

                // splay 目标值，从 thumb（0）到 pinky（1）远离
                float[] targetSplay = new float[5] { 0.0f, 0.0f, 0.25f, 0.75f, 1.0f };

                // 插值模拟靠拢
                splay[0] = 0.5f + (targetSplay[bentIndex] - 0.5f) * pinchWeight;
                splay[bentIndex] = 0.5f + (targetSplay[0] - 0.5f) * pinchWeight;
            }
            label3.Text =
                "大拇指:" + ((splay[0] * 100)).ToString() + "|" +
                "食指:" + ((splay[1] * 100)).ToString() + "|" +
                "中指:" + ((splay[2] * 100)).ToString() + "|" +
                "无名指:" + ((splay[3] * 100)).ToString() + "|" +
                "小指:" + ((splay[4] * 100)).ToString() + "";

            var input = new InputDataV2
            {
                flexion = flexion,
                splay = splay,
                joyX = 0f,
                joyY = 0f,
                joyButton = false,
                trgButton = false,
                aButton = false,
                bButton = false,
                grab = false,
                pinch = false,
                menu = false,
                calibrate = false,
                trgValue = 0f
            };

            SendToPipe(input, @"\\.\pipe\vrapplication\input\glove\v2\right");
        }

        void WriteLog(string mess)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
            try
            {
                File.AppendAllText(path, mess);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to write log: " + ex.Message);
            }
        }

        void SendToPipe(InputDataV2 data, string pipeName)
        {
            try
            {
                using (var pipe = new FileStream(pipeName, FileMode.Open, FileAccess.Write, FileShare.Read))
                {
                    byte[] bytes = StructToBytes(data);
                    pipe.Write(bytes, 0, bytes.Length);
                    pipe.Flush();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Pipe write failed: {ex.Message}");
            }
        }

        byte[] StructToBytes(InputDataV2 input)
        {
            int size = Marshal.SizeOf(typeof(InputDataV2));
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(input, ptr, false);
            byte[] bytes = new byte[size];
            Marshal.Copy(ptr, bytes, 0, size);
            Marshal.FreeHGlobal(ptr);
            return bytes;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            Thread thread = new Thread(new ThreadStart(NetServer));
            thread.Start();
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

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
            radioButton3.Checked = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            radioButton3.Checked = true;
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
            //uploaderForm.ShowDialog();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
        }
    }
}
