using EBooks.CBZ.Formatter.Models;
using System.IO.Compression;
using System.Text;

namespace EBooks.CBZ.Formatter.Domain
{
    public static class CbzFormatter
    {
        private class InputFileComparer : IComparer<InputFile>
        {
            public int Compare(InputFile? x, InputFile? y)
            {
                return string.CompareOrdinal(x?.Filename, y?.Filename);
            }
        }

        public static async Task<Stream> FormatCbzAsync(List<InputFile> inputFiles, CancellationToken cancellationToken)
        {
            Stream outputStream = new MemoryStream();

            inputFiles.Sort(new InputFileComparer());

            using ZipArchive formatedCbz = new ZipArchive(outputStream, ZipArchiveMode.Create, true);

            for (int inputFileIndex = 0; inputFileIndex < inputFiles.Count; inputFileIndex++)
            {
                InputFile inputFile = inputFiles[inputFileIndex];

                ZipArchiveEntry entry = formatedCbz.CreateEntry(GetFileName(inputFileIndex, inputFile.Filename));
                using Stream entryStream = entry.Open();
                await inputFile.Content.CopyToAsync(entryStream, cancellationToken);
                await entryStream.FlushAsync(cancellationToken);
            }

            return outputStream;
        }

        private static string GetFileName(int index, string originalFilename)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(index.ToString("0000"));
            stringBuilder.Append(Path.GetExtension(originalFilename));

            return stringBuilder.ToString();
        }
    }
}
