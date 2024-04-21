using FluentValidation;
using Hydro;

namespace DotnetJobs.Application.Helpers;

public static class HydroValidationExtensions
{
	public static bool Validate<TComponent>(this TComponent component, IValidator<TComponent> validator) where TComponent : HydroComponent
	{
		component.IsModelTouched = true;
		var result = validator.Validate(component);

		foreach (var error in result.Errors) 
		{
			component.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
		}

		return result.IsValid;
	}
}