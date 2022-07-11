using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace RaceSharp.Application.WorkFlowCore
{
	public class CustomStep : StepBodyAsync
	{
		private readonly ICustomStepService _service;

		public Dictionary<string, object> _variables { get; set; } = new Dictionary<string, object>();

		public object this[string propertyName]
		{
			get => _variables[propertyName];
			set => _variables[propertyName] = value;
		}

		public CustomStep(ICustomStepService service)
		{
			_service = service;
		}

		public override Task<ExecutionResult> RunAsync(IStepExecutionContext context)
		{
			Resource resource = _service.GetStepResource(Convert.ToString(_variables["__custom_step__"]));

			_service.Execute(resource, _variables);
			return Task.FromResult(ExecutionResult.Next());
		}
	}
}
