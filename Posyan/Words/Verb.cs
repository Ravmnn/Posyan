namespace Posyan.Words;


public class Verb : Word
{
    public string Root { get; private set; }
    public byte Conjugation { get; private set; }


    public Verb(string orthography, string definition, WordEtymology etymology)
        : base(orthography, definition, GrammaticalClass.Verb, etymology)
    {
        Root = GetInfinitiveVerbRoot(Orthography) ?? throw new InvalidOperationException($"Verb instantiation must be in infinitive form. (\"{orthography}\")");
        Conjugation = GetConjugationOfInfinitiveVerb(Orthography);
    }

    public Verb(Word word)
        : this(word.Orthography, word.Definition, word.Etymology) {}

    public Verb()
    {
        Root = "";
        Conjugation = 0;
    }


    public override string ToString()
    {
        var str = base.ToString();

        return str + $"\nRoot: {Root}\nConjugation: {Conjugation}";
    }


    public override void ReadFromBinary(BinaryReader reader)
    {
        base.ReadFromBinary(reader);

        Root = reader.ReadString();
        Conjugation = reader.ReadByte();
    }


    public override void WriteToBinary(BinaryWriter writer)
    {
        base.WriteToBinary(writer);

        writer.Write(Root);
        writer.Write(Conjugation);
    }


    public static string? GetInfinitiveVerbRoot(string verb)
        => !IsVerbInInfinitive(verb) ? null : verb[..^2];


    public static bool IsVerbInInfinitive(string verb)
        => verb.EndsWith("ar") || verb.EndsWith("er") || verb.EndsWith("ir");


    public static byte GetConjugationOfInfinitiveVerb(string verb)
    {
        var suffix = verb[^2..];

        return suffix switch
        {
            "ar" => 1,
            "er" => 2,
            "ir" => 3,

            _ => 0
        };
    }
}
