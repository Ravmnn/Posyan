using System.Xml.Linq;


namespace Posyan.Words;


public enum GrammaticalClass
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
    public static GrammaticalClass ToGramaticalClass(this string gramaticalClass) => gramaticalClass switch
    {
        "m." => GrammaticalClass.MascNoun,
        "f." => GrammaticalClass.FemNoun,
        "adj." => GrammaticalClass.Adjective,
        "adv." => GrammaticalClass.Adverb,
        "v." or "v. i." or "v. t." => GrammaticalClass.Verb,
        "prep." => GrammaticalClass.Preposition,
        "conj." => GrammaticalClass.Conjunction,
        "pron." => GrammaticalClass.Pronoun,
        "num." => GrammaticalClass.Numeral,
        "interj." => GrammaticalClass.Interjection,

        _ => GrammaticalClass.Unknown
    };
}


public readonly record struct WordEtymology(string? Origin, string? Literal);


public readonly record struct Word
{
    public string Orthography { get; init; }
    public string Definition { get; init; }
    public GrammaticalClass GrammaticalClass { get; init; }
    public WordEtymology Etymology { get; init; }


    public override string ToString()
        => $"Orthography: {Orthography}\nDefinition: {Definition}\nGramatical Class: {GrammaticalClass}\nEtymology: {Etymology}";


    public static Word FromXml(XDocument xml)
    {
        var data = xml.Element("entry")!;
        var form = data.Element("form")!;
        var sense = data.Element("sense")!;
        var etym = data.Element("etym");

        return new Word
        {
            Orthography = form.Element("orth")!.Value.ToLower(), // default to lower
            Definition = sense.Element("def")!.Value,
            GrammaticalClass = sense.Element("gramGrp")?.Value.ToGramaticalClass() ?? GrammaticalClass.Unknown,
            Etymology = new WordEtymology(etym?.Attribute("orig")?.Value, etym?.Value)
        };
    }
}
