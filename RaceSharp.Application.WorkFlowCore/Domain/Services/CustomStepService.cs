using System;
using System.Collections.Generic;

namespace RaceSharp.Application.WorkFlowCore
{
	public class CustomStepService : ICustomStepService
	{

		private readonly IResourceRepository _resourceRepository;
		private readonly IScriptEngineHost _scriptHost;

		public CustomStepService(IResourceRepository resourceRepository, IScriptEngineHost scriptHost)
		{
			_resourceRepository = resourceRepository;
			_scriptHost = scriptHost;
		}

		public void SaveStepResource(Resource resource)
		{
			if (resource.ContentType != @"text/x-python")
			{
				throw new ArgumentException();
			}

			_resourceRepository.Save(Bucket.Lambda, resource);
		}

		public Resource GetStepResource(string name)
		{
			return _resourceRepository.Find(Bucket.Lambda, name);
		}

		public void Execute(Resource resource, IDictionary<string, object> scope)
		{
			_scriptHost.Execute(resource, scope);
		}
	}
}
