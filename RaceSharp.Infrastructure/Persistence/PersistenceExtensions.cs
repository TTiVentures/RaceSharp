using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace RaceSharp.Infrastructure.Persistence
{
	public static class PersistenceExtensions
	{
		public static async Task ApplyMigrationsOnStartupAsync(this IServiceProvider serviceProvider, Type dbContextType)
		{
			try
			{
				using (IServiceScope scope = serviceProvider.CreateScope())
				{
					using (IDisposable service = (IDisposable)scope.ServiceProvider.GetRequiredService(dbContextType))
					{
						if (service is DbContext db)
						{
							await db.Database.MigrateAsync();
							//if (db.Database.GetPendingMigrations().Count() > 0)
							//{
							//	db.Database.Migrate(); // apply the migrations
							//}
						}
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}
	}
}
