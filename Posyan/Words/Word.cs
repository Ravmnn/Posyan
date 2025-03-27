using System.Xml.Linq;


namespace Posyan.Words;


public enum GramaticalClass
{
    Unknown,

    MascNoun,
    FemNoun,
    Verb,
    Adjective,
    Adverb,
    Preposition,
    Conjunction,
    Pronoun,
    Numeral,
    Interjection
}


public static class GramaticalClassExtensions
{
    public static GramaticalClass ToGramaticalClass(this string gramaticalClass) => gramaticalClass switch
    {
        "m." => GramaticalClass.MascNoun,
        "f." => GramaticalClass.FemNoun,
        "adj." => GramaticalClass.Adjective,
        "adv." => GramaticalClass.Adverb,
        "v." or "v. i." or "v. t." => GramaticalClass.Verb,
        "prep." => GramaticalClass.Preposition,
        "conj." => GramaticalClass.Conjunction,
        "pron." => GramaticalClass.Pronoun,
        "num." => GramaticalClass.Numeral,
        "interj." => GramaticalClass.Interjection,

        _ => GramaticalClass.Unknown
    };
}


public readonly record struct WordEtymology(string? Origin, string? Literal);


public readonly record struct Word
{
    public string Orthography { get; init; }
    public string Definition { get; init; }
    public GramaticalClass GramaticalClass { get; init; }
    public WordEtymology? Etymology { get; init; }


    public override string ToString()
        => $"Orthography: {Orthography}\nDefinition: {Definition}\nGramatical Class: {GramaticalClass}\nEtymology: {Etymology}";


    public static Word FromXml(XDocument xml)
    {
        var data = xml.Element("entry")!;
        var form = data.Element("form")!;
        var sense = data.Element("sense")!;
        var etym = data.Element("etym");

        return new Word
        {
            Orthography = form.Element("orth")!.Value.ToLower(),
            Definition = sense.Element("def")!.Value,
            GramaticalClass = sense.Element("gramGrp")?.Value.ToGramaticalClass() ?? GramaticalClass.Unknown,
            Etymology = new WordEtymology(etym?.Attribute("orig")?.Value, etym?.Value)
        };
    }
}
