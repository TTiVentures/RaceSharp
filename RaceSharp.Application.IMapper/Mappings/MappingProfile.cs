using AutoMapper;
using System.Reflection;

namespace RaceSharp.Application
{
	public class BaseMappingProfile : Profile
	{
		public BaseMappingProfile() : this(Assembly.GetExecutingAssembly())
		{
		}

		public BaseMappingProfile(Assembly assembly)
		{
			ApplyMappingsFromAssembly(assembly);
			ApplyMappingsToAssembly(assembly);
			ApplyMappingsDoubleAssembly(assembly);
		}

		private void ApplyMappingsAssembly(Assembly assembly, Type mapType, Action<Type, Type> callback)
		{
			System.Collections.Generic.List<Type> types = assembly.GetExportedTypes()
				.Where(t => t.GetInterfaces().Any(i =>
					i.IsGenericType && i.GetGenericTypeDefinition() == mapType))
				.ToList();

			foreach (Type type in types)
			{
				object instance = Activator.CreateInstance(type)!;
				MethodInfo? methodInfo = type.GetMethod("Mapping");
				if (methodInfo != null)
				{
					// If a Mapping method exist in class
					methodInfo.Invoke(instance, new object[] { this });
				}
				else
				{
					// Else make the default map
					foreach (Type i in type.GetInterfaces())
					{
						// https://stackoverflow.com/questions/1121834/finding-out-if-a-type-implements-a-generic-interface
						if (i.IsGenericType && i.GetGenericTypeDefinition() == mapType)
						{
							Type secondType = i.GetGenericArguments()[0];
							callback(secondType, type);
						}
					}
				}
			}
		}

		private void FromMap(Type src, Type dest)
		{
			CreateMap(src, dest);
		}

		private void ToMap(Type dest, Type src)
		{
			CreateMap(src, dest);
		}

		private void DoubleMap(Type a, Type b)
		{
			CreateMap(a, b);
			CreateMap(b, a);
		}

		private void ApplyMappingsFromAssembly(Assembly assembly)
		{
			ApplyMappingsAssembly(assembly, typeof(IMapFrom<>), FromMap);
		}

		private void ApplyMappingsToAssembly(Assembly assembly)
		{
			ApplyMappingsAssembly(assembly, typeof(IMapTo<>), ToMap);
		}

		private void ApplyMappingsDoubleAssembly(Assembly assembly)
		{
			ApplyMappingsAssembly(assembly, typeof(IMapDouble<>), DoubleMap);
		}
	}
}