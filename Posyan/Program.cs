using Fint;

using Spectre.Console;

using Termbow.Color;
using Termbow.Color.Paint;

using Posyan.Words;
using Posyan.Analysis;


namespace Posyan;


// TODO: add verb conjugation detector


class PosyanProgram
{
    public static Dictionary<int, ColorObject> ColorTable { get; } = new Dictionary<int, ColorObject>
    {
        { -1, 243 },

        { (int)GrammaticalClass.MascNoun, 38 },
        { (int)GrammaticalClass.FemNoun, 38 },
        { (int)GrammaticalClass.Verb, 126 },
        { (int)GrammaticalClass.Adjective, 141 },
        { (int)GrammaticalClass.Adverb, 210 },
        { (int)GrammaticalClass.Preposition, 220 },
        { (int)GrammaticalClass.Conjunction, 40 },
        { (int)GrammaticalClass.Pronoun, ColorValue.Italic },
        { (int)GrammaticalClass.Numeral, 150 },
        { (int)GrammaticalClass.Interjection, 63 }
    };

    public static string PosyanCacheDirectory => $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.cache/Posyan";
    public static string WordCacheFilePath => $"{PosyanCacheDirectory}/words.bin";

    public static OpenDictApi Api { get; }
    public static TextAnalyser Analyser { get; }

    public static string Source { get; set; }


    static PosyanProgram()
    {
        Api = new OpenDictApi();
        Analyser = new TextAnalyser();

        Source = "";
    }


    public static void Init()
    {
        if (!Directory.Exists(PosyanCacheDirectory))
            Directory.CreateDirectory(PosyanCacheDirectory);

        if (!File.Exists(WordCacheFilePath))
            File.Create(WordCacheFilePath).Close();
    }


    public static void Main(string[] args)
    {
        // PrintWordCacheFile();
        //
        // return;

        try
        {
            Init();

            Source = File.ReadAllText("test.txt");

            // load already known words from cache
            Analyser.WordsDefinition = WordBinary.ReadAll(WordCacheFilePath).ToList();

            // search for new words and store them
            var newStringWords = Analyser.SearchForNewWords(Source);
            var newWordsResults = Api.GetWordsAsync(newStringWords, WordSearchResultCallback);

            var newWords = OpenDictApi.WordSearchResult.GetSuccessfullyWords(newWordsResults.Result);
            newWords = VerbAnalyser.InstantiateVerbs(newWords).ToArray();

            Analyser.WordsDefinition.AddRange(newWords);

            WordBinary.WriteAll(WordCacheFilePath, newWords);

            var scanner = new Scanner(new NonWordAndDigitRule(-1), new WordGrammaticalClassRule(Analyser.WordsDefinition));
            var painter = new FintPainter(scanner, ColorTable);

            Console.Clear();
            Console.WriteLine(painter.Paint(Source));
        }
        finally
        {
            Console.ReadKey();
        }
    }


    private static void WordSearchResultCallback(OpenDictApi.WordSearchResult result)
    {
        AnsiConsole.Markup($"New word \"[mediumspringgreen]{result.WordText}[/]\".");

        if (result.HasFailed)
            AnsiConsole.Markup("[red] (Failed)[/]");

        AnsiConsole.MarkupLine("");
    }


    private static void PrintWordCacheFile()
    {
        var words = WordBinary.ReadAll(WordCacheFilePath);

        foreach (var word in words)
        {
            AnsiConsole.MarkupLine(word.ToString());
            AnsiConsole.MarkupLine("\n");
        }
    }
}
