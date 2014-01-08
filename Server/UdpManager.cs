namespace Server
{
	using System;
	using System.Net;
	using System.Net.Sockets;

	public class UdpManager: IManager
	{
		private readonly IPEndPoint _endPoint;
		private readonly Socket _mainUdpSocket;
		private const string _managerType = "UDP";

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

		public Socket GetSocket()
		{
			EndPoint endPoint = new IPEndPoint(0, 0);

			byte[] bytes = new byte[10];

			var bytesRead = _mainUdpSocket.ReceiveFrom(bytes, 0, 10, SocketFlags.None, ref endPoint);

			Socket newSocket = new Socket(AddressFamily.InterNetwork, 
				SocketType.Dgram, ProtocolType.Udp);

			newSocket.Bind(new IPEndPoint(_endPoint.Address,0));

			int port = ((IPEndPoint)newSocket.LocalEndPoint).Port;

			Console.WriteLine(newSocket.LocalEndPoint.ToString());

			byte[] portBytes = BitConverter.GetBytes(port);

			_mainUdpSocket.SendTo(portBytes, 0, portBytes.Length, SocketFlags.None, endPoint);

			return newSocket;
		}

		public void Stop()
		{
			_mainUdpSocket.Close();
			_mainUdpSocket.Shutdown(SocketShutdown.Both);
		}

		public Socket ServerSocket
		{
			get { return _mainUdpSocket; }
		}

		public IClient GetClient()
		{
			return new UdpClient(GetSocket());
		}

		public string ManagerType
		{
			get { return _managerType; }
		}
	}
}
