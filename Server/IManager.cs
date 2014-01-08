namespace Server
{
	using System.Net.Sockets;

	public interface IManager
	{
		string ManagerType { get; }
		void Start();
		Socket GetSocket();
		void Stop();
		IClient GetClient();
		Socket ServerSocket { get; }
	}
}
