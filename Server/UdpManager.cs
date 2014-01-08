namespace Server
{
	using System;
	using System.Net;
	using System.Net.Sockets;

	public class UdpManager
	{
		private readonly IPEndPoint _endPoint;
		public Socket _mainUdpSocket;

		public UdpManager(string ipAddress, int port)
		{
			_endPoint = new IPEndPoint(Dns.Resolve(ipAddress)
				.AddressList[0], port);
			_mainUdpSocket = new Socket(AddressFamily.InterNetwork, 
				SocketType.Dgram, ProtocolType.Udp);
		}

		public void Start()
		{
			_mainUdpSocket.Bind(_endPoint);
		}

		public Socket GetSocket(IPEndPoint remoteEndPoint)
		{
			Socket newSocket = new Socket(AddressFamily.InterNetwork, 
				SocketType.Dgram, ProtocolType.Udp);

			newSocket.Bind(new IPEndPoint(_endPoint.Address,0));

			int port = ((IPEndPoint)newSocket.LocalEndPoint).Port;

			Console.WriteLine(newSocket.LocalEndPoint.ToString());

			byte[] bytes = BitConverter.GetBytes(port);

			_mainUdpSocket.SendTo(bytes, 0, bytes.Length, SocketFlags.None, remoteEndPoint);

			return newSocket;
		}

		public void Stop()
		{
			_mainUdpSocket.Close();
			_mainUdpSocket.Shutdown(SocketShutdown.Both);
		}
	}
}
