using PhotoSauce.MagicScaler;

namespace Blog.Data.FileManager
{
	public class FileManager : IFileManager
	{
		private string imagePath;
		public FileManager(IConfiguration config)
		{
			imagePath = config["Path:Images"]!; 
		}

		public FileStream ImageStream(string image)
		{
			string filePath = Path.Combine(imagePath, image);

			if (!File.Exists(filePath))
			{
				throw new FileNotFoundException($"Could not find file '{filePath}'.");
			}

			return new FileStream(filePath, FileMode.Open, FileAccess.Read);
		}

		public bool RemoveImage(string image)
		{
			try
			{
				string file = Path.Combine(imagePath, image);
				if (File.Exists(file))
				{
					File.Delete(file);
				}	
				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				return false;
			}
		}

		public async Task<string> SaveImage(IFormFile image)
		{
			try
			{
				string savePath = Path.Combine(imagePath);
				if (!Directory.Exists(savePath))
				{
					Directory.CreateDirectory(savePath);
				}

				string mime = Path.GetExtension(image.FileName);
				string fileName = $"img_{DateTime.Now:dd-MM-yyyy-HH-mm-ss}{mime}";

				ProcessImageSettings settings = new ProcessImageSettings
				{
					Width = 800,
					Height = 500,
					ResizeMode = CropScaleMode.Crop,
					//EncoderOptions = new JpegEncoderOptions
					//{
					//	Quality = 100,
					//	Subsample = ChromaSubsampleMode.Subsample420,
					//},
					
				};

				using (var fileStream = new FileStream(Path.Combine(savePath, fileName), FileMode.Create))
				{
					MagicImageProcessor.ProcessImage(image.OpenReadStream(), fileStream, settings);
				}

				return fileName;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				return "Error";
			}
		}

	}
}
