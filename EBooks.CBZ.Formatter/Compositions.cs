using EBooks.CBZ.Formatter.DataAccess;
using EBooks.CBZ.Formatter.Domain;
using EBooks.CBZ.Formatter.Models;

namespace EBooks.CBZ.Formatter
{
    // an attempt at following the Dependency Rejection pattern from https://blog.ploeh.dk/2017/02/02/dependency-rejection/
    public static class Compositions
    {
        public static async Task FormatCbzCompositionAsync(FileInfo input, FileInfo output, CancellationToken cancellationToken)
        {
            List<InputFile> inputFiles = await InputFilesRepository.GetInputFilesAsync(input.FullName, cancellationToken);

            Stream formatedFile = await CbzFormatter.FormatCbzAsync(inputFiles, cancellationToken);
            formatedFile.Seek(0, SeekOrigin.Begin);

            Directory.CreateDirectory(output.DirectoryName);
            using FileStream outputStream = File.Create(output.FullName);
            await formatedFile.CopyToAsync(outputStream, cancellationToken);
        }
    }
}
