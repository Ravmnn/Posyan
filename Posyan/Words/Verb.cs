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
    Third
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

    public VerbInflectionData InflectionData { get; private set; }
    public VerbNominalForm NominalForm { get; private set; }


    // note: orthography must be in infinitive form
    public Verb(string orthography, string definition, WordEtymology etymology)
        : base(orthography, definition, GrammaticalClass.Verb, etymology)
    {
        Root = GetInfinitiveVerbRoot(Orthography);
        Conjugation = GetConjugationOfInfinitiveVerb(Orthography);

        InflectionData = VerbInflectionData.Undefined();
        NominalForm = VerbNominalForm.Infinitive;
    }

    public Verb(Word word)
        : this(word.Orthography, word.Definition, word.Etymology) {}


    protected Verb()
    {
        Root = "";
        InflectionData = VerbInflectionData.Undefined();
    }


    public override string ToString()
    {
        var str = base.ToString();

        return str + $"\nRoot: {Root}\nConjugation: {Conjugation}\nPerson: {InflectionData.Person}\nNumber: {InflectionData.Number}\n" +
               $"Mood: {InflectionData.Mood}\nTense: {InflectionData.Tense}\nNominal: {NominalForm}";
    }


    public override void ReadFromBinary(BinaryReader reader)
    {
        base.ReadFromBinary(reader);

        Root = reader.ReadString();
        Conjugation = (VerbConjugation)reader.ReadByte();
        InflectionData = new VerbInflectionData(
            (VerbInflectionMood)reader.ReadByte(),
            (VerbInflectionTense)reader.ReadByte(),
            (VerbInflectionPerson)reader.ReadByte(),
            (VerbInflectionNumber)reader.ReadByte()
        );
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

        InvalidVerbNominalFormException.ThrowIfVerbIsNot(Orthography, VerbNominalForm.Infinitive, "Only infinitive verbs can be written.");

        writer.Write(Root);
        writer.Write((byte)Conjugation);
        writer.Write((byte)InflectionData.Mood);
        writer.Write((byte)InflectionData.Tense);
        writer.Write((byte)InflectionData.Person);
        writer.Write((byte)InflectionData.Number);
        writer.Write((byte)NominalForm);
    }


    public static string GetInfinitiveVerbRoot(string verbInInfinitive)
    {
        InvalidVerbNominalFormException.ThrowIfVerbIsNot(verbInInfinitive, VerbNominalForm.Infinitive);

        return verbInInfinitive[..^2];
    }


    public static bool IsVerbIn(string verb, VerbNominalForm form) => form switch
    {
        VerbNominalForm.Infinitive => IsVerbInInfinitive(verb),
        VerbNominalForm.Gerund => IsVerbInGerund(verb),
        VerbNominalForm.Participle => IsVerbInParticiple(verb),

        _ => false
    };

    public static bool IsVerbInInfinitive(string verb)
        => verb.EndsWith("ar") || verb.EndsWith("er") || verb.EndsWith("ir");

    public static bool IsVerbInGerund(string verb)
        => verb.EndsWith("ando") || verb.EndsWith("endo") || verb.EndsWith("indo");

    public static bool IsVerbInParticiple(string verb)
        => verb.EndsWith("ado") || verb.EndsWith("ido");


    public static VerbConjugation GetConjugationOfInfinitiveVerb(string verb)
    {
        InvalidVerbNominalFormException.ThrowIfVerbIsNot(verb, VerbNominalForm.Infinitive);

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
