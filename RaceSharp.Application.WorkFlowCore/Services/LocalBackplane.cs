using System.Threading.Tasks;

namespace RaceSharp.Application.WorkFlowCore
{
    public class LocalBackplane : IClusterBackplane
    {
        public Task Start() => Task.CompletedTask;

        public Task Stop() => Task.CompletedTask;

        public void LoadNewDefinition(string id, int version)
        {
        }
    }
}
