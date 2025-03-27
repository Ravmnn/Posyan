using System.Xml.Linq;

using Newtonsoft.Json.Linq;

using Fint;

using Posyan.Words;


namespace Posyan;


public class OpenDictApi : HttpClient
{
    public readonly static string Url = "https://api.dicionario-aberto.net";
    public readonly static string WordUrl = $"{Url}/word";


    public async Task<XDocument?> GetWordXmlDataAsync(string word)
    {
        var response = await GetAsync($"{WordUrl}/{word}");
        var jsonString = await response.Content.ReadAsStringAsync();
        var json = JArray.Parse(jsonString).First;
        var xml = json?["xml"]?.ToString().ReplaceLineEndings("");

        return xml is null ? null : XDocument.Parse(xml);
    }


    public async Task<Word?> GetWordAsync(string word)
    {
        if (await GetWordXmlDataAsync(word) is not {} xml)
            return null;

        return Word.FromXml(xml);
    }


    public static async Task<IEnumerable<Word>> GetAllWordsFromText(string text, IProgress<(int, string)>? progress = null)
    {
        var dict = new OpenDictApi();
        var words = new List<Word>();

        var scanner = new Scanner(new WordOrDigitRule(0));
        var tokens = scanner.Scan(new Lexer(text).Tokenize()).ToArray();

        var wordsProcessed = 0;

        await Parallel.ForEachAsync(tokens, new ParallelOptions { MaxDegreeOfParallelism = 8 }, (token, _) =>
        {
            if (dict.GetWordAsync(token.Text).Result is { } word && !words.Contains(word))
                words.Add(word);

            progress?.Report((++wordsProcessed * 100 / tokens.Length, token.Text));

            return ValueTask.CompletedTask;
        });

        return words;
    }
}
