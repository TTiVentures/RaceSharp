namespace RaceSharp.Application
{
	public class CacheServiceSettings
	{
		public string Host { get; set; } = "localhost";
		public int Port { get; set; } = 6379;
		public string? Password { get; set; } = null;
		public int Database { get; set; } = -1;
		public bool AllowAdmin { get; set; } = true;
		public bool Ssl { get; set; } = false;
		public string ClientName { get; set; } = "Default Client";
	}
}