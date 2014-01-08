using System;

namespace Server
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				var tcpServer = new TcpUdpServer(args[0], int.Parse(args[1]));
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}
	}
}
