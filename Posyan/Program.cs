using Fint;

using Termbow.Color;
using Termbow.Color.Paint;

using Spectre.Console;

using Posyan.Words;


namespace Posyan;


// TODO: add verb conjugation detector
// TODO: store already searched words and its data (in disc) for faster look ups.


class PosyanProgram
{
    public static Dictionary<int, ColorObject> ColorTable => new Dictionary<int, ColorObject>
    {
        { -1, 243 },

        { (int)GramaticalClass.MascNoun, 38 },
        { (int)GramaticalClass.FemNoun, 38 },
        { (int)GramaticalClass.Verb, 126 },
        { (int)GramaticalClass.Adjective, 141 },
        { (int)GramaticalClass.Adverb, 210 },
        { (int)GramaticalClass.Preposition, 220 },
        { (int)GramaticalClass.Conjunction, 40 },
        { (int)GramaticalClass.Pronoun, ColorValue.Italic },
        { (int)GramaticalClass.Numeral, 150 },
        { (int)GramaticalClass.Interjection, 63 }
    };



    static void Main(string[] args)
    {
        var source = File.ReadAllText("../../../test.txt");

        var progress = new Progress<(int, string)>();
        var wordsTask = OpenDictApi.GetAllWordsFromText(source, progress);

        ProcessingStatus(progress, wordsTask);
        wordsTask.Wait();

        var scanner = new Scanner(new NonWordAndDigitRule(-1), new WordGrammaticalClassRule(wordsTask.Result));
        var painter = new FintPainter(scanner, ColorTable);

        Console.Clear();
        Console.WriteLine(painter.Paint(source));

        Console.ReadKey();
    }


    public static void ProcessingStatus(Progress<(int, string)> progress, Task<IEnumerable<Word>> wordsTask)
    {
        AnsiConsole.Status()
            .Spinner(Spinner.Known.Arc)
            .Start("Analysing words...", context =>
            {
                var progressFactor = 0;

                progress.ProgressChanged += (_, value) =>
                {
                    progressFactor = value.Item1;
                    AnsiConsole.MarkupLine($"[bold]>[/] [palegreen1]{value.Item2}[/]");
                };

                while (!wordsTask.IsCompleted)
                    context.Status = $"[italic]Analysing words...[/] [bold]{progressFactor}%[/]";
            });
    }
}
