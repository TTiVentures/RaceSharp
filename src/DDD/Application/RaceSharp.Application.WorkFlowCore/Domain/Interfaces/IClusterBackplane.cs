using System.Threading.Tasks;

namespace RaceSharp.Application.WorkFlowCore
{
	public interface IClusterBackplane
	{
		Task Start();
		Task Stop();
		void LoadNewDefinition(string id, int version);
	}
}
