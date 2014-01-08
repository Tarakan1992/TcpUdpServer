namespace Server
{
	using System;
	using System.IO;
	using System.Net;
	using System.Net.Sockets;
	using System.Text;
	using System.Threading;

	public class TcpUdpServer
	{
		private readonly TcpListener _listener;
		private readonly UdpManager _udpManager;

		public TcpUdpServer(string ipAddress, int port)
		{
			_udpManager = new UdpManager(ipAddress, port);
			ListenForClients();
		}

		private void ListenForClients()
		{
			_udpManager.Start();

			EndPoint _endPoint = new IPEndPoint(0,0);

			while (true)
			{
				byte[] bytes = new byte[10];

				var bytesRead = _udpManager._mainUdpSocket.ReceiveFrom(bytes, 0, 10, SocketFlags.None, ref _endPoint);

				Console.WriteLine(Encoding.ASCII.GetString(bytes,0,bytesRead));

				var client = new UdpClient((_udpManager.GetSocket((IPEndPoint)_endPoint)));

				var clientThread = new Thread(HandleClientConnection);
				clientThread.Start(client);
			}
		}

		private void HandleClientConnection(object client)
		{
			Console.WriteLine("Start get a file!");
			var _client = (UdpClient)client;
			FileStream file = null;
			byte[] message = new byte[4096];
			int bytesRead;

			bytesRead = _client.Read(message, 0, message.Length);

			if (bytesRead == 0)
			{
				_client.EndConnection();
				return;
			}

			string fileName = Encoding.UTF8.GetString(message, 0, bytesRead);

			Console.WriteLine(fileName);

			try
			{
				file = new FileStream(fileName, FileMode.Append, FileAccess.Write);
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
				if (file != null)
				{
					file.Close();
				}
				_client.EndConnection();
				return;
			}

			byte[] bytes = BitConverter.GetBytes(file.Length);

			if (!_client.Write(bytes, 0, bytes.Length))
			{
				_client.EndConnection();
				return;
			}

			while (true)
			{
				bytesRead = 0;
				
				bytesRead = _client.Read(message, 0, message.Length);
				
				if (bytesRead == 0)
				{
					break;
				}

				file.Write(message, 0, bytesRead);
				Console.WriteLine(file.Length);
			}

			file.Close();
			_client.EndConnection();
		}
	}
}
