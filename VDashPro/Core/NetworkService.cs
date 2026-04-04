using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using VDashPro.Models;

namespace VDashPro.Core
{
    public interface INetworkService
    {
        void StartUdpListener(int port);
        event Action<TelemetryPacket> TelemetryReceived;
    }

    public class NetworkService : INetworkService
    {
        public event Action<TelemetryPacket> TelemetryReceived;
        private UdpClient _udpClient;

        public async void StartUdpListener(int port)
        {
            _udpClient = new UdpClient();
            while (true) 
            {
                try
                {
                    var result = await _udpClient.ReceiveAsync();
                    if (result.Buffer.Length == Marshal.SizeOf<TelemetryPacket>())
                    {
                        
                    }
                }
                catch { }

            }
        }

        private T ByteArrayToStruct<T>(byte[] bytes) where T : struct
        {
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try { return Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject()); }
            finally { handle.Free(); }
        }
    }
}
