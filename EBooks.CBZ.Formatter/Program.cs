using EBooks.CBZ.Formatter.DataAccess;
using EBooks.CBZ.Formatter.Domain;
using EBooks.CBZ.Formatter.Models;
using System.CommandLine;

InputFilesRepository inputFilesRepository = new InputFilesRepository();

Option<FileInfo> inputFileOption = new Option<FileInfo>("--input")
{
    IsRequired = true,
};
Option<FileInfo> outputFileOption = new Option<FileInfo>("--output")
{
    IsRequired = true,
};
RootCommand rootCommand = new RootCommand()
{
    inputFileOption,
    outputFileOption,
};
rootCommand.Description = "A quick tool to help format CBZ files.";

rootCommand.SetHandler(async (FileInfo input, FileInfo output) =>
{
    List<InputFile> inputFiles = await inputFilesRepository.GetInputFilesAsync(input.FullName, CancellationToken.None);

    Directory.CreateDirectory(output.DirectoryName);
    using FileStream outputStream = File.Create(output.FullName);

    await CbzFormatter.FormatCbzAsync(inputFiles, outputStream, CancellationToken.None);
}, inputFileOption, outputFileOption);

await rootCommand.InvokeAsync(args);
