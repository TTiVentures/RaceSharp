using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.OpenApi.Models;
using RaceSharp.Application;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;
using System.Reflection;

namespace RaceSharp.Infrastructure
{
	public class SwaggerSkipPropertyFilterSchema : ISchemaFilter
	{
		public void Apply(OpenApiSchema schema, SchemaFilterContext context)
		{
			if (schema?.Properties == null)
			{
				return;
			}

			var skipProperties = context.Type.GetProperties().Where(t => t.GetCustomAttribute<SwaggerIgnoreAttribute>() != null);

			foreach (var skipProperty in skipProperties)
			{
				var propertyToSkip = schema.Properties.Keys.SingleOrDefault(x => string.Equals(x, skipProperty.Name, StringComparison.OrdinalIgnoreCase));

				if (propertyToSkip != null)
				{
					schema.Properties.Remove(propertyToSkip);
				}
			}
		}
	}

	public class SwaggerSkipPropertyFilterOperation : IOperationFilter
	{
		public void Apply(OpenApiOperation operation, OperationFilterContext context)
		{
			context.ApiDescription.ParameterDescriptions
				.Where(d => d.Source.Id == "Query").ToList()
				.ForEach(param =>
				{
					var toIgnore =
						((DefaultModelMetadata)param.ModelMetadata)
						.Attributes.PropertyAttributes
						?.Any(x => x is SwaggerIgnoreAttribute);

					var toRemove = operation.Parameters
						.SingleOrDefault(p => p.Name == param.Name);

					if (toIgnore ?? false)
						operation.Parameters.Remove(toRemove);
				});
		}
	}
}
