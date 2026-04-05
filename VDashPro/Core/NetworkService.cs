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
        void StartServices(int udpPort, int tcpPort);
        event Action<TelemetryPacket> TelemetryReceived;
        event Action<double> ImageDownloadProgressChanged;
    }

    public class NetworkService : INetworkService
    {
        public event Action<TelemetryPacket> TelemetryReceived;
        private UdpClient _udpListener;
        private TcpListener _tcpListener;
        private CancellationTokenSource _cts = new CancellationTokenSource();

        public async void StartServices(int udpPort, int tcpPort)
        {
            Task.Run(() => ListenUdp(udpPort), _cts.Token);
            Task.Run(() => ListenTcp(tcpPort), _cts.Token);
        }

        private async Task ListenUdp(int port)
        {
            _udpListener = new UdpClient(port);
            int packetSize = Marshal.SizeOf<TelemetryPacket>();

            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    var result = await _udpListener.ReceiveAsync();
                    if (result.Buffer.Length == packetSize)
                    {
                        var packet = ByteArrayToStruct<TelemetryPacket>(result.Buffer);

                        // Checksum 검증 로직 (필요 시 추가)
                        if (packet.SyncWord == 0xDEADBEEF)
                        {
                            TelemetryReceived?.Invoke(packet);
                        }
                    }
                }
                catch (Exception ex) { /* 로깅 처리 */ }
            }
        }

        private async Task ListenTcp(int port)
        {
            _tcpListener = new TcpListener(IPAddress.Any, port);
            _tcpListener.Start();

            while (!_cts.Token.IsCancellationRequested)
            {
                using (var client = await _tcpListener.AcceptTcpClientAsync())
                using (var stream = client.GetStream())
                {
                    // [Week 4 상세 구현 예정] 
                    // TCP 청킹(Chunking) 수신 및 MemoryStream 재조립 로직이 들어갈 자리입니다.
                }
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
