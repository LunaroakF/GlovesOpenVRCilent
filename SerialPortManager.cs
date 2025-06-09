using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlovesOpenVRCilent
{
    public class SerialPortManager
    {
        private SerialPort serialPort;

        public event Action<string> DataReceived;

        public SerialPortManager(string portName = "COM3", int baudRate = 9600)
        {
            serialPort = new SerialPort
            {
                PortName = portName,
                BaudRate = baudRate,
                DataBits = 8,
                Parity = Parity.None,
                StopBits = StopBits.One
            };

            serialPort.DataReceived += OnDataReceived;
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = serialPort.ReadExisting();
            DataReceived?.Invoke(data); // 触发事件，传出数据
        }

        public void Open()
        {
            if (!serialPort.IsOpen)
                serialPort.Open();
        }

        public void Close()
        {
            if (serialPort.IsOpen)
                serialPort.Close();
        }

        public bool IsOpen => serialPort.IsOpen;

        public void Write(string text)
        {
            if (serialPort.IsOpen)
                serialPort.Write(text);
        }
    }
}
