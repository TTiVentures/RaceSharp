using Microsoft.Scripting;
using System.Collections.Generic;

namespace RaceSharp.Application.WorkFlowCore.Scripting
{
	public class ScriptEngineHost : IScriptEngineHost
	{
		private readonly IScriptEngineFactory _engineFactory;

		public ScriptEngineHost(IScriptEngineFactory engineFactory)
		{
			_engineFactory = engineFactory;
		}

		public void Execute(Resource resource, IDictionary<string, object> inputs)
		{
			Microsoft.Scripting.Hosting.ScriptEngine engine = _engineFactory.GetEngine(resource.ContentType);

			Microsoft.Scripting.Hosting.ScriptSource source = engine.CreateScriptSourceFromString(resource.Content, SourceCodeKind.Statements);
			Microsoft.Scripting.Hosting.ScriptScope scope = engine.CreateScope(inputs);
			source.Execute(scope);
			SanitizeScope(inputs);
		}

		public dynamic EvaluateExpression(string expression, IDictionary<string, object> inputs)
		{
			Microsoft.Scripting.Hosting.ScriptEngine engine = _engineFactory.GetExpressionEngine();
			Microsoft.Scripting.Hosting.ScriptSource source = engine.CreateScriptSourceFromString(expression, SourceCodeKind.Expression);
			Microsoft.Scripting.Hosting.ScriptScope scope = engine.CreateScope(inputs);
			return source.Execute(scope);
		}

		public T EvaluateExpression<T>(string expression, IDictionary<string, object> inputs)
		{
			return EvaluateExpression(expression, inputs);
		}

		private void SanitizeScope(IDictionary<string, object> scope)
		{
			scope.Remove("__builtins__");
		}
	}
}
