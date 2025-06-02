using System.Xml.Linq;

using Newtonsoft.Json.Linq;

using Posyan.Words;


namespace Posyan;


public class OpenDictApi : HttpClient
{
    public record struct WordSearchResult(Word? Word, string WordText, bool HasFailed)
    {
        public static WordSearchResult Failed(string wordText)
            => new WordSearchResult { Word = null, WordText = wordText, HasFailed = true};

        public static WordSearchResult Success(Word word)
            => new WordSearchResult { Word = word, WordText = word.Orthography, HasFailed = false};


        public static IEnumerable<Word> GetSuccessfullyWords(IEnumerable<WordSearchResult> wordSearchResults)
            => from wordSearchResult in wordSearchResults where !wordSearchResult.HasFailed select wordSearchResult.Word;
    }


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


    public async Task<WordSearchResult> GetWordAsync(string word)
    {
        if (await GetWordXmlDataAsync(word) is not {} xml)
            return WordSearchResult.Failed(word);

        return WordSearchResult.Success(Word.FromXml(xml));
    }


    public async Task<IEnumerable<WordSearchResult>> GetWordsAsync(IEnumerable<string> words, Action<WordSearchResult>? callback = null)
    {
        var result = new List<WordSearchResult>();

        foreach (var word in words)
        {
            var wordSearchResult = await GetWordAsync(word);

            result.Add(wordSearchResult);
            callback?.Invoke(wordSearchResult);
        }

        return result;
    }
}
