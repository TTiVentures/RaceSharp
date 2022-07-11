namespace RaceSharp.Application.WorkFlowCore
{
	public interface IDefinitionService
	{
		void LoadDefinitionsFromStorage();
		void RegisterNewDefinition(Definition definition);
		void ReplaceVersion(Definition definition);
		Definition GetDefinition(string id);
	}
}
