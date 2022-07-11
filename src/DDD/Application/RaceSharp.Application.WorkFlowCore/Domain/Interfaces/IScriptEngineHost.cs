using System.Collections.Generic;

namespace RaceSharp.Application.WorkFlowCore
{
	public interface IScriptEngineHost
	{
		void Execute(Resource resource, IDictionary<string, object> inputs);
		dynamic EvaluateExpression(string expression, IDictionary<string, object> inputs);
		T EvaluateExpression<T>(string expression, IDictionary<string, object> inputs);
	}
}
