using Microsoft.Extensions.DependencyInjection;

namespace RaceSharp.Application.WorkFlowCore.Scripting
{
	public static class ServiceCollectionExtensions
	{
		public static void ConfigureWorkFlowScripting(this IServiceCollection services)
		{
			services.AddSingleton<IScriptEngineFactory, ScriptEngineFactory>();
			services.AddSingleton<IScriptEngineHost, ScriptEngineHost>();
		}
	}
}
