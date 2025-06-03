using Fint;

using Spectre.Console;

using Termbow.Color;
using Termbow.Color.Paint;

using Posyan.Words;
using Posyan.Analysis;


namespace Posyan;


// TODO: add verb inflection detection.
// TODO: be able to inflect verbs based on raw inflection data.
// TODO: be able to extract inflection data based on the verb orthography.


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

    public static Scanner Scanner { get; }

    public static string Source { get; set; }


    static PosyanProgram()
    {
        Api = new OpenDictApi();
        Analyser = new TextAnalyser();

        Scanner = new Scanner(new WordOrDigitRule(0));

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
        // AnsiConsole.WriteLine(VerbInflector.GetInflectionDataFromVerb("comer", "comeriam").ToString());
        //
        // return;

        try
        {
            Init();

            Source = File.ReadAllText("test.txt");

            var tokens = new Lexer(Source).Tokenize();
            var words = from word in Scanner.Scan(tokens) select word.Text;

            // load already known words from cache
            Analyser.WordsDefinition = WordBinary.ReadAll(WordCacheFilePath).ToList();

            // search for new words
            var newStringWords = Analyser.SearchForNewWords(words);
            var newWordsResults = Api.GetWordsAsync(newStringWords, WordSearchResultCallback);

            // get the new words data using the dictionary API
            var newWords = OpenDictApi.WordSearchResult.GetSuccessfullyWords(newWordsResults.Result);
            newWords = VerbAnalyser.InstantiateBaseVerbs(newWords).ToArray();

            // add the new words to the analyser
            Analyser.WordsDefinition.AddRange(newWords);

            // save the new words data in the binary database
            WordBinary.WriteAll(WordCacheFilePath, newWords);


            // prints everything beautifully
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
