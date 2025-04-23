using Fint;

using Termbow.Color;
using Termbow.Color.Paint;

using Posyan.Words;
using Spectre.Console;


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
        try
        {
            Init();

            Source = File.ReadAllText("../../../test.txt");

            Analyser.WordsDefinition = WordBinary.ReadAll(WordCacheFilePath).ToList();

            var newWordsResults = Api.GetWordsAsync(Analyser.SearchForNewWords(Source), WordSearchResultCallback);

            var newWords = (from newWord in newWordsResults.Result where !newWord.HasFailed select (Word)newWord.Word).ToArray();

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
}
