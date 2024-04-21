namespace DotnetJobs.Lib.Routing;

public static class RegisterRoutesExtension
{
	public static void RegisterRoutes(this WebApplication app)
	{
		var endpointDefinitions = typeof(Program).Assembly
			.GetTypes()
			.Where(t => t.IsAssignableTo(typeof(IEndpointDefinition))
			            && !t.IsAbstract && !t.IsInterface)
			.Select(Activator.CreateInstance)
			.Cast<IEndpointDefinition>();

		foreach (var endpointDef in endpointDefinitions)
		{
			endpointDef.Map(app);
		}
	}
}