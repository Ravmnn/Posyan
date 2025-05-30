namespace Posyan.Words;


// public enum VerbVoice // Verb voice is almost totally based on semantic.
// {
//     Undefined,
//
//     Active,
//     Passive,
//     Reflective
// }




public enum VerbConjugation
{
    Undefined,

    First,
    Second,
    Third,
}




public enum VerbMood
{
    Undefined, // Verbs in a nominal form aren't inflected (mood, tense, person, number)

    Indicative,
    Subjective,
    Imperative
}


public enum VerbTense
{
    Undefined, // Verbs in a nominal form aren't inflected (mood, tense, person, number)

    Present,
    PastPerfect,
    PastImperfect,
    Pluperfect, // "pretérito mais-que-perfeito"
    PresentFuture,
    PastFuture,
    Future
}


public enum VerbPerson
{
    Undefined, // Verbs in a nominal form aren't inflected (mood, tense, person, number)

    First,
    Second,
    Third,
}


public enum VerbNumber
{
    Undefined, // Verbs in a nominal form aren't inflected (mood, tense, person, number)

    Singular,
    Plural
}




public enum VerbNominalForm
{
    Undefined, // Verbs inflected (mood, tense, person, number) aren't in a nominal form

    Infinitive,
    Gerund,
    Participle
}


public class Verb : Word
{
    public string Root { get; private set; }

    public VerbConjugation Conjugation { get; private set; }

    public VerbMood Mood { get; private set; }
    public VerbTense Tense { get; private set; }
    public VerbPerson Person { get; private set; }
    public VerbNumber Number { get; private set; }
    public VerbNominalForm NominalForm { get; private set; }


    public Verb(string orthography, string definition, WordEtymology etymology)
        : base(orthography, definition, GrammaticalClass.Verb, etymology)
    {
        Root = GetInfinitiveVerbRoot(Orthography) ?? throw new InvalidOperationException($"Base verb instantiation must be in infinitive form. (\"{orthography}\")");
        Conjugation = GetConjugationOfInfinitiveVerb(Orthography);

        NominalForm = VerbNominalForm.Infinitive;
    }

    public Verb(Word word)
        : this(word.Orthography, word.Definition, word.Etymology) {}


    protected Verb()
    {
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
        Conjugation = (VerbConjugation)reader.ReadByte();
        Mood = (VerbMood)reader.ReadByte();
        Tense = (VerbTense)reader.ReadByte();
        Person = (VerbPerson)reader.ReadByte();
        Number = (VerbNumber)reader.ReadByte();
        NominalForm = (VerbNominalForm)reader.ReadByte();
    }

    public new static Verb FromBinary(BinaryReader reader)
    {
        var verb = new Verb();
        verb.ReadFromBinary(reader);

        return verb;
    }


    public override void WriteToBinary(BinaryWriter writer)
    {
        base.WriteToBinary(writer);

        if (!IsVerbInInfinitive(Orthography))
            throw new InvalidOperationException("Only infinitive verbs can be written.");

        writer.Write(Root);
        writer.Write((byte)Conjugation);
        writer.Write((byte)Mood);
        writer.Write((byte)Tense);
        writer.Write((byte)Person);
        writer.Write((byte)Number);
        writer.Write((byte)NominalForm);
    }


    public static string? GetInfinitiveVerbRoot(string verb)
        => !IsVerbInInfinitive(verb) ? null : verb[..^2];


    public static bool IsVerbInInfinitive(string verb)
        => verb.EndsWith("ar") || verb.EndsWith("er") || verb.EndsWith("ir");


    public static VerbConjugation GetConjugationOfInfinitiveVerb(string verb)
    {
        var suffix = verb[^2..];

        return suffix switch
        {
            "ar" => VerbConjugation.First,
            "er" => VerbConjugation.Second,
            "ir" => VerbConjugation.Third,

            _ => VerbConjugation.Undefined
        };
    }
}
