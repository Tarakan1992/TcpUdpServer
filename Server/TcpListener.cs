namespace Server
{
	using System;
	using System.Net;
	using System.Net.Sockets;

	public class TcpListener
	{
		private readonly IPEndPoint _endPoint;
		private readonly Socket _listener;

		public TcpListener(string ipAddress, int port)
		{
			_endPoint = new IPEndPoint(Dns.Resolve(ipAddress)
				.AddressList[0], port);
			_listener = new Socket(AddressFamily.InterNetwork,
				SocketType.Stream, ProtocolType.Tcp);
		}

		public void Start()
		{
			_listener.Bind(_endPoint);
			_listener.Listen(100);
		}

		public Socket GetSocket()
		{
			return _listener.Accept();
		}

		public void Stop()
		{
			_listener.Close();
			_listener.Shutdown(SocketShutdown.Both);
		}
	}
}
