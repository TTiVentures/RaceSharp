using Microsoft.Scripting.Hosting;

namespace RaceSharp.Application.WorkFlowCore.Scripting
{
	public interface IScriptEngineFactory
	{
		ScriptEngine GetEngine(string contentType);
		ScriptEngine GetExpressionEngine();
	}
}
