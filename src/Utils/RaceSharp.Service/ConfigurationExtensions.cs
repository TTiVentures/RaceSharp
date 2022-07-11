namespace RaceSharp
{
	public static class ConfigurationExtensions
	{

		public static T GetConfigurationSection<T>(this IConfiguration configuration, string sectionName, IServiceCollection services)
			where T : class
		{
			IConfigurationSection mbs = configuration.GetSection(sectionName);
			services.Configure<T>(mbs);
			T busSettings = mbs.Get<T>();
			return busSettings;
		}
	}
}
