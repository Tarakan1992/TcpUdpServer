namespace Server
{
	using System.Net.Sockets;

	public class TcpClient: IClient
	{
		private readonly Socket _clientSocket;

		public TcpClient(Socket socket)
		{
			_clientSocket = socket;
			_clientSocket.ReceiveTimeout = 5000;
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			try
			{
				 var bytesRead = _clientSocket.Receive(buffer, offset,
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
