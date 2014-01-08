namespace Server
{
	using System;
	using System.Linq;
	using System.Net;
	using System.Net.Sockets;
	using System.Text;

	public class UdpClient
	{
		private readonly Socket _clientSocket;
		private EndPoint _endPoint;
		private byte _previousPocket = 0xF;

		public UdpClient(Socket socket)
		{
			_endPoint = new IPEndPoint(0,0);
			_clientSocket = socket;
			_clientSocket.ReceiveTimeout = 10000;
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			var tempBuffer = new byte[buffer.Length + 1];

			for (int i = 0; i < 5; i++)
			{
				var bytesRead = 0;
				try
				{
					bytesRead = _clientSocket.ReceiveFrom(tempBuffer, offset,
						count + 1, SocketFlags.None, ref _endPoint);
				}
				catch(Exception e)
				{
					Console.WriteLine(e.Message);
					continue;
				}

				byte[] bytes = Encoding.UTF8.GetBytes(NetworkConstants.Ask);
				_clientSocket.SendTo(bytes, 0, bytes.Length, SocketFlags.None, _endPoint);

				if (tempBuffer[0] == _previousPocket)
				{
					continue;
				}

				_previousPocket = tempBuffer[0];

				Buffer.BlockCopy(tempBuffer,1,buffer,0, buffer.Length);

				if (Encoding.ASCII.GetString(buffer, 0, bytesRead - 1) == NetworkConstants.Fin)
				{
					break;
				}

				return bytesRead - 1;
			}

			return 0;
		}

		public bool Write(byte[] buffer, int offset, int count)
		{
			byte[] bytes = new byte[3];
			for (int i = 0; i < 5; i++)
			{
				_clientSocket.SendTo(buffer, 0, count, SocketFlags.None, _endPoint);

				try
				{
					_clientSocket.ReceiveFrom(bytes, 0, bytes.Length, SocketFlags.None, ref _endPoint);

					if (Encoding.ASCII.GetString(bytes, 0, bytes.Length) != NetworkConstants.Ask)
					{
						continue;
					}
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
					continue;
				}

				return true;
			}
			return false;
		}

		public void EndConnection()
		{
			if (_clientSocket != null)
			{
				_clientSocket.Shutdown(SocketShutdown.Both);
				_clientSocket.Close();
			}
		}
	}
}
