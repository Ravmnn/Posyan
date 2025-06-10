using System.Xml.Linq;


namespace Posyan.Words;


public enum GrammaticalClass : byte
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


public class Word
{
    public string Orthography { get; private set; }
    public string Definition { get; private set; }
    public GrammaticalClass GrammaticalClass { get; private set; }
    public WordEtymology Etymology { get; private set;}


    public Word(string orthography, string definition = "", GrammaticalClass grammaticalClass = GrammaticalClass.Unknown, WordEtymology etymology = new WordEtymology())
    {
        Orthography = orthography.ToLower(); // default to lower
        Definition = definition;
        GrammaticalClass = grammaticalClass;
        Etymology = etymology;
    }


    protected Word()
    {
        Orthography = Definition = "";
        GrammaticalClass = GrammaticalClass.Unknown;
        Etymology = new WordEtymology();
    }



    public override string ToString()
        => $"Gramatical Class: {GrammaticalClass}\nOrthography: {Orthography}\nDefinition: {Definition}\nEtymology: {Etymology}";


    protected static string? StringOrNull(string source)
        => source == "null" ? null : source;

    // note: don't write empty strings to binary.
    protected static string StringifyString(string? source)
        => source is null || source.Length == 0 ? "null" : source;


    public virtual void ReadFromBinary(BinaryReader reader)
    {
        GrammaticalClass = (GrammaticalClass)reader.ReadByte();
        Orthography = reader.ReadString();
        Definition = reader.ReadString();

        Etymology = new WordEtymology(
            StringOrNull(reader.ReadString()),
            StringOrNull(reader.ReadString())
        );
    }

    public static Word FromBinary(BinaryReader reader)
    {
        var word = new Word();

        word.ReadFromBinary(reader);

        return word;
    }


    public virtual void WriteToBinary(BinaryWriter writer)
    {
        // the first data of a word binary data sequence must be its
        // class, so we know how to read it.

        writer.Write((byte)GrammaticalClass);
        writer.Write(StringifyString(Orthography));
        writer.Write(StringifyString(Definition));
        writer.Write(StringifyString(Etymology.Origin));
        writer.Write(StringifyString(Etymology.Literal));
    }


    public static Word FromXml(XDocument xml)
    {
        var data = xml.Element("entry")!;
        var form = data.Element("form")!;
        var sense = data.Element("sense")!;
        var etym = data.Element("etym");

        return new Word(
        form.Element("orth")!.Value,
        sense.Element("def")!.Value,
        sense.Element("gramGrp")?.Value.ToGramaticalClass() ?? GrammaticalClass.Unknown,
        new WordEtymology(etym?.Attribute("orig")?.Value, etym?.Value)
        );
    }
}
