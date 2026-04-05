using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace VDashPro.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TelemetryPacket
    {
        public uint SyncWord; // 시작 확인
        public uint Timestamp;
        public float PosX;
        public float PosY;
        public float Altitude; // 고도
        public float Roll;
        public float Pitch;
        public float Yaw;
        public float Battery;
        public float Temperature;
        public uint Checksum;
    }
}
