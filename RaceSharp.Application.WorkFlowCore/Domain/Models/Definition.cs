using System;
using System.Collections.Generic;
using WorkflowCore.Models;

namespace RaceSharp.Application.WorkFlowCore
{
	public class Definition
	{
		public string Id { get; set; }

		public int Version { get; set; }

		public string Description { get; set; }

		//public string DataType { get; set; }

		public WorkflowErrorHandling DefaultErrorBehavior { get; set; }

		public TimeSpan? DefaultErrorRetryInterval { get; set; }

		public List<Step> Steps { get; set; } = new List<Step>();

	}
}
