using Microsoft.Extensions.DependencyInjection;

namespace RaceSharp.Application.WorkFlowCore
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureDomainServices(this IServiceCollection services)
        {
            services.AddSingleton<IDefinitionService, DefinitionService>();
            services.AddSingleton<IWorkflowLoader, WorkflowLoader>();
            services.AddSingleton<ICustomStepService, CustomStepService>();
            services.AddSingleton<IExpressionEvaluator, ExpressionEvaluator>();
            services.AddTransient<CustomStep>();
        }
    }
}
