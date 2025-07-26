using System;
using System.IO;
using System.Runtime.InteropServices;

namespace GlovesOpenVRCilent
{
    public class HandController
    {
        public enum HandSide { Left, Right }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct InputDataV2
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
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

        private readonly string pipeName;
        private readonly HandSide hand;
        private int ANALOG_MAX = 1023;

        public Action<string> UpdateVerticalText;
        public Action<string> UpdateHorizontalText;

        public HandController(HandSide hand)
        {
            this.hand = hand;
            this.pipeName = $@"\\.\pipe\vrapplication\input\glove\v2\{hand.ToString().ToLower()}";
        }

        public void UploadPercent(int a, int b, int c, int d, int e)
        {
            float[] norm = new float[5] {
                a / (float)ANALOG_MAX,
                b / (float)ANALOG_MAX,
                c / (float)ANALOG_MAX,
                d / (float)ANALOG_MAX,
                e / (float) ANALOG_MAX
            };

            float[] flexion = new float[20];
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 4; j++)
                    flexion[i * 4 + j] = (i == 0 && j == 3) ? 0f : norm[i];

            float[] splay = new float[5] { 0.5f, 0.5f, 0.5f, 0.5f, 0.5f };

            int bendThreshold = ANALOG_MAX / 2;
            int bentFingers = 0;
            int bentIndex = -1;

            for (int i = 1; i < 5; i++)
            {
                if (norm[i] > (bendThreshold / (float)ANALOG_MAX))
                {
                    bentFingers++;
                    bentIndex = i;
                }
            }

            if (norm[0] > (bendThreshold / (float)ANALOG_MAX) && bentFingers == 1)
            {
                float pinchWeight = (norm[0] + norm[bentIndex]) / 2f;
                float[] targetSplay = new float[5] { 0.0f, 0.0f, 0.25f, 0.75f, 1.0f };
                splay[0] = 0.5f + (targetSplay[bentIndex] - 0.5f) * pinchWeight;
                splay[bentIndex] = 0.5f + (targetSplay[0] - 0.5f) * pinchWeight;
            }

            UpdateVerticalText?.Invoke(
                $"大拇指:{(int)(norm[0] * 100)}%|食指:{(int)(norm[1] * 100)}%|中指:{(int)(norm[2] * 100)}%|无名指:{(int)(norm[3] * 100)}%|小指:{(int)(norm[4] * 100)}%");

            UpdateHorizontalText?.Invoke(
                $"大拇指:{splay[0]}|食指:{splay[1]}|中指:{splay[2]}|无名指:{splay[3]}|小指:{splay[4]}");

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

            SendToPipe(input);
        }

        private void SendToPipe(InputDataV2 data)
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
                Console.WriteLine($"Pipe write failed ({hand}): {ex.Message}");
            }
        }

        public void SetAnalog(int analog)
        {
            ANALOG_MAX = analog;
        }
        private byte[] StructToBytes(InputDataV2 input)
        {
            int size = Marshal.SizeOf(typeof(InputDataV2));
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(input, ptr, false);
            byte[] bytes = new byte[size];
            Marshal.Copy(ptr, bytes, 0, size);
            Marshal.FreeHGlobal(ptr);
            return bytes;
        }
    }
}
