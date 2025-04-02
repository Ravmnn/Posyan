using System.Xml.Linq;

using Newtonsoft.Json.Linq;

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
}
