using System.Collections.Generic;

namespace RaceSharp.Application.WorkFlowCore
{
    public interface ICustomStepService
    {
        void SaveStepResource(Resource resource);
        Resource GetStepResource(string name);
        void Execute(Resource resource, IDictionary<string, object> scope);
    }
}
