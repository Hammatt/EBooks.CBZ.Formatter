using EBooks.CBZ.Formatter.Models;
using System.IO.Compression;

namespace EBooks.CBZ.Formatter.DataAccess
{
    public class InputFilesRepository
    {
        public async Task<List<InputFile>> GetInputFilesAsync(string fileLocation, CancellationToken cancellationToken)
        {
            List<InputFile> result = new List<InputFile>();

            using FileStream inputStream = File.OpenRead(fileLocation);

            using ZipArchive inputFileArchive = new ZipArchive(inputStream, ZipArchiveMode.Read);

            foreach(ZipArchiveEntry zipArchiveEntry in inputFileArchive.Entries)
            {
                using Stream entryStream = zipArchiveEntry.Open();
                MemoryStream memoryStream = new MemoryStream();
                await entryStream.CopyToAsync(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                result.Add(new InputFile(memoryStream, zipArchiveEntry.Name));
            }

            return result;
        }
    }
}
