using Spectre.Console;

using Posyan.Words;


namespace Posyan.Terminal;


public static class StatusMessages
{
    public static void AnalysingWords(Task<IEnumerable<Word>> wordsTask, Progress<WordRegisteringData> progress)
    {
        AnsiConsole.Status()
            .Spinner(Spinner.Known.Arc)
            .Start("Analysing words...", context =>
            {
                var progressFactor = 0;

                progress.ProgressChanged += (_, registerData) =>
                {
                    progressFactor = registerData.WordsProcessed * 100 / registerData.WordCount;

                    if (registerData.Failed)
                        AnsiConsole.MarkupLine($"[bold]>[/] [red3]{registerData.WordBeingProcessed}[/]");
                    else
                        AnsiConsole.MarkupLine($"[bold]>[/] [palegreen1]{registerData.WordBeingProcessed}[/] {(registerData.IsNew ? "*" : "")}");
                };

                while (!wordsTask.IsCompleted)
                    context.Status = $"[italic]Analysing words...[/] [bold]{progressFactor}%[/]";
            });
    }
}
