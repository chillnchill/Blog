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
            return new FileStream(Path.Combine(imagePath, image), FileMode.Open, FileAccess.Read);   
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

                string mimeType = image.FileName.Substring(image.FileName.LastIndexOf('.'));
                string fileName = $"img{DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss")}{mimeType}";

                using (var fileStream = new FileStream(Path.Combine(savePath, fileName), FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }

                return fileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "Error";
            }
            
        }
    }
}
