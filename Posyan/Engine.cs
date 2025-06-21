using Fint;

namespace Posyan;


public static class PosyanEngine
{
    public static string PosyanCacheDirectory => $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.cache/Posyan";
    public static string WordCacheFilePath => $"{PosyanCacheDirectory}/words.bin";


    public static void Init()
    {
        if (!Directory.Exists(PosyanCacheDirectory))
            Directory.CreateDirectory(PosyanCacheDirectory);

        if (!File.Exists(WordCacheFilePath))
            File.Create(WordCacheFilePath).Close();
    }


    public static IEnumerable<string> GetWordsIn(string source)
    {
        var scanner = new Scanner(new WordOrDigitRule(0));

        var tokens = new Lexer(source).Tokenize();
        var words = from word in scanner.Scan(tokens) select word.Text.ToLower();

        return words;
    }
}
