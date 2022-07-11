using System.Threading.Tasks;

namespace RaceSharp.Application.WorkFlowCore
{
	public class LocalBackplane : IClusterBackplane
	{
		public Task Start()
		{
			return Task.CompletedTask;
		}

		public Task Stop()
		{
			return Task.CompletedTask;
		}

		public void LoadNewDefinition(string id, int version)
		{
		}
	}
}
