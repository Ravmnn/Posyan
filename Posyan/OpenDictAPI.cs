using System.Xml.Linq;

using Newtonsoft.Json.Linq;


namespace Posyan;


public class OpenDictApi : HttpClient
{
    public readonly static string Url = "https://api.dicionario-aberto.net";
    public readonly static string WordUrl = $"{Url}/word";


    public XDocument? GetWordXmlData(string word)
    {
        var response = GetAsync($"{WordUrl}/{word}").Result;
        var jsonString = response.Content.ReadAsStringAsync().Result;
        var json = JArray.Parse(jsonString).First;
        var xml = json?["xml"]?.ToString().ReplaceLineEndings("");

        return xml is null ? null : XDocument.Parse(xml);
    }
}
