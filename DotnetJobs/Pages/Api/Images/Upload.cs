using DotnetJobs.Application.Services;
using Spark.Library.Routing;
using Imagekit.Sdk;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Tls;

namespace DotnetJobs.Pages.Api.Jobs;

public class Upload : IRoute
{
	public void Map(WebApplication app)
	{
		app.MapPost("/api/images/upload", async ([FromForm] IFormFile logo, ImageKitService imageService) =>
		{
			var isImageTypeValid = ImageKitService.ValidateImageType(logo);
			var isImageSizeValid = ImageKitService.ValidateImageSize(logo);
			if (!isImageTypeValid || !isImageSizeValid)
			{
				return Results.Ok(new { status = "error", message = "Images must be less than 1 MB and a png or jpg." });
			}
			var result = await imageService.UploadImage(logo);
			return Results.Ok(new { url = result.url, status = "success" });
		}).DisableAntiforgery();
	}
}