using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using VDashPro.Models;
using System.Threading.Channels;
using System.Net.Http;

namespace VDashPro.Core
{
    public interface INetworkService
    {
        event Action<TelemetryPacket> TelemetryProcessd;
        event Action<TelemetryPacket> ImageDownloadProgressChanged;
        event Action<byte[]> ImageReassembled;

        void StartServices(int udpPort, int tcpPort);
        void StopServices();
    }

    public class NetworkService : INetworkService
    {
        // Event
        public event Action<TelemetryPacket> TelemetryProcessd;
        public event Action<TelemetryPacket> ImageDownloadProgressChanged;
        public event Action<byte[]> ImageReassembled;

        // Listner, Token
        private UdpClient _udpClient;
        private TcpListener _tcpListener;
        private CancellationTokenSource _cts;

        private readonly Channel<TelemetryPacket> _telemetryChannel;

        public NetworkService()
        {
            // Unbounded Channel
            _telemetryChannel = Channel.CreateUnbounded<TelemetryPacket>();
        }

        public void StartServices(int udpPort, int tcpPort)
        {
            _cts = new CancellationTokenSource();


        }
        public void StopServices()
        {

        }

        private async Task ListenUdpAsync(int port, CancellationToken token)
        {
            _udpClient = new UdpClient(port);
            int packetSize = Marshal.SizeOf<TelemetryPacket>();

            while (!token.IsCancellationRequested)
            {
                try
                {
                    var result = await _udpClient.ReceiveAsync(token);
                    if (result.Buffer.Length == packetSize)
                    {
                        var packet = ByteArrayToStruct<TelemetryPacket>(result.Buffer);
                        if (packet.SyncWord == 0xDEADBEEF)
                        {
                            await _telemetryChannel.Writer.WriteAsync(packet, token);
                        }

                    }
                }
                catch (OperationCanceledException) { break; }
                catch (Exception) { }
            }
        }

        private async Task ConsumeTelemetryAsync(CancellationToken token)
        {
            await foreach (var packet in _telemetryChannel.Reader.ReadAllAsync(token))
            {
                TelemetryProcessd?.Invoke(packet);
            }
        }

        private async Task ListenTcpAsync(int port, CancellationToken token)
        {
            _tcpListener = new TcpListener(IPAddress.Any, port);
            _tcpListener.Start();

            while (!token.IsCancellationRequested)
            {
                try
                {
                    using (var client = await _tcpListener.AcceptTcpClientAsync(token))
                    using(var stream = client.GetStream()) 
                    {

                    }
                }
                catch (OperationCanceledException) { break; }
                catch (Exception) { }
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
