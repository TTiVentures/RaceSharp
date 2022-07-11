using Microsoft.Scripting.Hosting;
using System.Collections.Generic;

namespace RaceSharp.Application.WorkFlowCore.Scripting
{
	internal class ScriptEngineFactory : IScriptEngineFactory
	{
		private readonly Dictionary<string, ScriptEngine> _engines = new Dictionary<string, ScriptEngine>()
		{
			[@"text/x-python"] = IronPython.Hosting.Python.CreateEngine(),
			[string.Empty] = IronPython.Hosting.Python.CreateEngine()
		};

		public ScriptEngine GetEngine(string contentType)
		{
			return _engines[contentType];
		}

		public ScriptEngine GetExpressionEngine()
		{
			return _engines[string.Empty];
		}
	}
}
