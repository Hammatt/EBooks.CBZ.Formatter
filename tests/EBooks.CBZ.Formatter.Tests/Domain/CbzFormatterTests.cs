using EBooks.CBZ.Formatter.Domain;
using EBooks.CBZ.Formatter.Models;
using System.IO.Compression;
using Xunit;

namespace EBooks.CBZ.Formatter.Tests.Domain
{
    public class CbzFormatterTests
    {
        [Theory]
        [InlineData("file.png", ".png")]
        [InlineData("file.PNG", ".PNG")]
        [InlineData("file.jpg", ".jpg")]
        [InlineData("file.jpeg", ".jpeg")]
        public async Task OriginalFileExtensionIsKept(string inputFilename, string expectedExtension)
        {
            List<InputFile> testInput = new List<InputFile>();
            testInput.Add(new InputFile(new MemoryStream(), inputFilename));

            using MemoryStream testOutput = new MemoryStream();
            await CbzFormatter.FormatCbzAsync(testInput, testOutput, CancellationToken.None);
            
            testOutput.Seek(0, SeekOrigin.Begin);
            using ZipArchive outputArchive = new ZipArchive(testOutput);
            ZipArchiveEntry entry = outputArchive.Entries[0];
            Assert.Equal($"0000{expectedExtension}", entry.Name);
        }

        [Fact]
        public async Task OutputFilesAreSortedByInputName()
        {
            List<InputFile> testInput = new List<InputFile>();
            testInput.Add(new InputFile(new MemoryStream(new byte[] { 2 }), "z.png"));
            testInput.Add(new InputFile(new MemoryStream(new byte[] { 1 }), "a.png"));

            using MemoryStream testOutput = new MemoryStream();
            await CbzFormatter.FormatCbzAsync(testInput, testOutput, CancellationToken.None);

            testOutput.Seek(0, SeekOrigin.Begin);
            using ZipArchive outputArchive = new ZipArchive(testOutput);
            Assert.Equal($"0000.png", outputArchive.Entries[0].Name);
            byte[] actualValue = new byte[1];
            await outputArchive.Entries[0].Open().ReadAsync(actualValue);
            Assert.Equal(1, actualValue[0]);
            Assert.Equal($"0001.png", outputArchive.Entries[1].Name);
            await outputArchive.Entries[1].Open().ReadAsync(actualValue);
            Assert.Equal(2, actualValue[0]);
        }

        [Fact]
        public async Task OutputStreamIsAZipArchive()
        {
            List<InputFile> testInput = new List<InputFile>();
            testInput.Add(new InputFile(new MemoryStream(new byte[] { 1 }), "blah.png"));
            uint zipLeadBytes = 0x04034b50; //note, this is 0x06054b50 if it is an empty zip, which is why we have had to add a dummy file to be more realistic to the actual use case

            using MemoryStream testOutput = new MemoryStream();
            await CbzFormatter.FormatCbzAsync(testInput, testOutput, CancellationToken.None);

            testOutput.Seek(0, SeekOrigin.Begin);
            byte[] leadBytes = new byte[4];
            await testOutput.ReadAsync(leadBytes);
            Assert.Equal(zipLeadBytes, BitConverter.ToUInt32(leadBytes));
        }
    }
}
