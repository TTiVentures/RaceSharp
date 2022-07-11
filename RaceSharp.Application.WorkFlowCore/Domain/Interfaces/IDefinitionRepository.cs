using System.Collections.Generic;

namespace RaceSharp.Application.WorkFlowCore
{
	public interface IDefinitionRepository
	{
		IEnumerable<Definition> GetAll();

		Definition Find(string workflowId);
		Definition Find(string workflowId, int version);
		int? GetLatestVersion(string workflowId);

		void Save(Definition definition);
	}
}
