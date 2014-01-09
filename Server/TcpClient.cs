namespace Server
{
	using System;
	using System.Net.Sockets;

	public class TcpClient: IClient
	{
		private readonly Socket _clientSocket;

		public TcpClient(Socket socket)
		{
			_clientSocket = socket;
			_clientSocket.ReceiveTimeout = 2000;
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			try
			{
				var bytesRead = 0;
				if (_clientSocket.Poll(1, SelectMode.SelectError))
				{
					Console.WriteLine("oob");
					try
					{
						byte[] oobuf = new byte[1];
						_clientSocket.Receive(oobuf, SocketFlags.OutOfBand);
						return -1;
					}
					catch(Exception e)
					{
						Console.WriteLine(e.Message);
						return 0;
					}
				}

				bytesRead = _clientSocket.Receive(buffer, offset,
						count, SocketFlags.None);

				return bytesRead;
			}
			catch
			{
				return 0;
			}
		}

		public bool Write(byte[] buffer, int offset, int count)
		{
			try
			{
				_clientSocket.Send(buffer, offset, count, SocketFlags.None);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public void EndConnection()
		{
			if (_clientSocket != null && _clientSocket.Connected)
			{
				_clientSocket.Shutdown(SocketShutdown.Both);
				_clientSocket.Close();
			}
		}
	}
}
