namespace Server
{
	public interface IClient
	{
		int Read(byte[] buffer, int offset, int count);
		bool Write(byte[] buffer, int offset, int count);
		void EndConnection();
	}
}
