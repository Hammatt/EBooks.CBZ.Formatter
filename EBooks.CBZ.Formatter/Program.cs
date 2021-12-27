using EBooks.CBZ.Formatter;
using System.CommandLine;

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
    await Compositions.FormatCbzCompositionAsync(input, output, CancellationToken.None);
}, inputFileOption, outputFileOption);

await rootCommand.InvokeAsync(args);
