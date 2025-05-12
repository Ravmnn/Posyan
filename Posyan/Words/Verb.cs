namespace Posyan.Words;


public enum VerbNumber
{
    Undefined,

    Singular,
    Plural
}


public enum VerbMood
{
    Undefined,

    Indicative,
    Subjective,
    Imperative
}


// public enum VerbVoice // Verb voice is almost all based on semantic.
// {
//     Undefined,
//
//     Active,
//     Passive,
//     Reflective
// }


public enum VerbTense
{
    Undefined,

    Present,
    PastPerfect,
    PastImperfect,
    Pluperfect, // "pretérito mais-que-perfeito"
    PresentFuture,
    PastFuture,
    Future
}


public enum VerbNominalForm
{
    Undefined,

    Infinitive,
    Gerund,
    Participle
}


public class Verb : Word
{
    // Defines whether this verb instance should be treated as
    // a base verb or not. If so, it means it will be used as a base for matching
    // non-base verbs (of the same type as this) conjugation and that it will be
    // stored in the cache.
    public bool IsBase { get; private set; }

    public string Root { get; private set; }

    public byte Conjugation { get; private set; }
    public byte Person { get; private set; }
    public VerbNumber Number { get; private set; }
    public VerbMood Mood { get; private set; }
    public VerbTense Tense { get; private set; }
    public VerbNominalForm NominalForm { get; private set; }


    public Verb(string orthography, string definition, WordEtymology etymology)
        : base(orthography, definition, GrammaticalClass.Verb, etymology)
    {
        IsBase = true;

        Root = GetInfinitiveVerbRoot(Orthography) ?? throw new InvalidOperationException($"Base verb instantiation must be in infinitive form. (\"{orthography}\")");
        Conjugation = GetConjugationOfInfinitiveVerb(Orthography);

        NominalForm = VerbNominalForm.Infinitive;
    }

    public Verb(Word word)
        : this(word.Orthography, word.Definition, word.Etymology) {}

    public Verb()
    {
        IsBase = false;

        Root = "";
    }


    public override string ToString()
    {
        var str = base.ToString();

        return str + $"\nRoot: {Root}\nConjugation: {Conjugation}\nPerson: {Person}\nNumber: {Number}\n" +
               $"Mood: {Mood}\nTense: {Tense}\nNominal: {NominalForm}";
    }


    public override void ReadFromBinary(BinaryReader reader)
    {
        base.ReadFromBinary(reader);

        Root = reader.ReadString();
        Conjugation = reader.ReadByte();
        Person = reader.ReadByte();
        Number = (VerbNumber)reader.ReadByte();
        Mood = (VerbMood)reader.ReadByte();
        Tense = (VerbTense)reader.ReadByte();
        NominalForm = (VerbNominalForm)reader.ReadByte();
    }


    public override void WriteToBinary(BinaryWriter writer)
    {
        base.WriteToBinary(writer);

        if (!IsBase)
            throw new InvalidOperationException("Only base verbs can be written.");

        writer.Write(Root);
        writer.Write(Conjugation);
        writer.Write(Person);
        writer.Write((byte)Number);
        writer.Write((byte)Mood);
        writer.Write((byte)Tense);
        writer.Write((byte)NominalForm);
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
