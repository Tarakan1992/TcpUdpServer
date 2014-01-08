namespace Server
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Net.Sockets;
	using System.Text;
	using System.Threading;

	public class TcpUdpServer
	{
		private readonly List<IManager> _managers;


		public TcpUdpServer(string ipAddress, int port)
		{
			_managers = new List<IManager>();
			_managers.Add(new UdpManager(ipAddress, port));
			_managers.Add(new TcpListener(ipAddress, port));
			ListenForClients();
		}

		private void ListenForClients()
		{
			foreach (var manager in _managers)
			{
				manager.Start();
			}

			try
			{
				while (true)
				{
					List<Socket> readSockets = _managers.Select(manager => manager.ServerSocket).ToList();

					Socket.Select(readSockets, null, null, int.MaxValue);

					foreach (var rs in readSockets)
					{
						foreach (var manager in _managers)
						{
							if (rs == manager.ServerSocket)
							{
								ConnectInitializer(manager);
							}
						}
					}
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
				foreach (var manager in _managers)
				{
					manager.Stop();
				}
			}
		}

		private void ConnectInitializer(IManager manager)
		{
			Console.WriteLine(manager.ManagerType + "Initializer");

			var clientThread = new Thread(HandleClientConnection);

			clientThread.Start(manager.GetClient());
		}


		private void HandleClientConnection(object client)
		{
			Console.WriteLine("Start get a file!");
			var _client = (IClient)client;
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
