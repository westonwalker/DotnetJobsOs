using Imagekit.Sdk;
using Microsoft.AspNetCore.Components.Forms;

namespace DotnetJobs.Application.Services
{
    public class ImageKitService
    {

		public ImageKitService() {
		}

        public static bool ValidateImageSize(IFormFile file)
        {
            float fileSize = file.Length / 1024f / 1024f;
            if (fileSize > 1)
            {
                return false;
            }
            return true;
        }

        public static bool ValidateImageType(IFormFile file)
        {
            if (file.ContentType != "image/jpeg" && file.ContentType != "image/png")
            {
                return false;
            }
            return true;
        }

        public async Task<Result> UploadImage(IFormFile file)
        {
            var key = DotNetEnv.Env.GetString("IMAGE_KIT_KEY");
			var secret = DotNetEnv.Env.GetString("IMAGE_KIT_SECRET");
			var url = DotNetEnv.Env.GetString("IMAGE_KIT_URL");

			var imageKit = new ImagekitClient(
				key,
                secret,
                url
            );
            var extension = Path.GetExtension(file.Name);
            var filename = $"{Guid.NewGuid().ToString()}{extension}";
            //var resizedImage = await file.RequestImageFileAsync(file.ContentType, 250, 250);
            var buffer = new byte[file.Length];
            await file.OpenReadStream().ReadAsync(buffer);
            FileCreateRequest imagekitRequest = new FileCreateRequest
            {
                file = buffer,
                fileName = filename,
            };
            imagekitRequest.tags = new List<string>()
                {
                    "dotnetjobs"
                };
            Result result = await imageKit.UploadAsync(imagekitRequest);
            return result;
        }
    }
}
