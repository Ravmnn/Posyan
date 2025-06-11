namespace Posyan.Words.Verbs;


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


    public Verb(Verb baseVerb, string variantVerb)
        : base(variantVerb, baseVerb.Definition, GrammaticalClass.Verb, baseVerb.Etymology)
    {
        Root = baseVerb.Root;
        Conjugation = baseVerb.Conjugation;

        NominalForm = GetVerbNominalForm(variantVerb);

        // verb isn't in nominal form, so it is inflected.

        if (NominalForm == VerbNominalForm.Undefined)
            InflectionData = VerbInflector.GetInflectionDataFromVerb(baseVerb.Orthography, variantVerb);
        else
            InflectionData = VerbInflectionData.Undefined();
    }


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

        // there are no public constructors creating a Verb object without defining "Root",
        // so no need for using "StringifyString".
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
        InvalidVerbNominalFormException.ThrowIfVerbIsNot(Orthography, VerbNominalForm.Infinitive, "Only infinitive verbs (base verbs) can be written.");

        base.WriteToBinary(writer);

        writer.Write(StringifyString(Root));
        writer.Write((byte)Conjugation);
        writer.Write((byte)InflectionData.Mood);
        writer.Write((byte)InflectionData.Tense);
        writer.Write((byte)InflectionData.Person);
        writer.Write((byte)InflectionData.Number);
        writer.Write((byte)NominalForm);
    }


    public static bool IsVerbRootEqual(string firstVerb, string secondVerb)
    {
        var baseVerbRoot = GetInfinitiveVerbRoot(firstVerb);
        return firstVerb != secondVerb && secondVerb.StartsWith(baseVerbRoot);
    }


    public static bool IsVerbInflectionOf(string baseVerb, string inflectedVerb)
    {
        if (!IsVerbRootEqual(baseVerb, inflectedVerb))
            return false;

        if (GetVerbNominalForm(inflectedVerb) != VerbNominalForm.Undefined)
            return false;

        return true;
    }


    public static string GetInfinitiveVerbRoot(string verbInInfinitive)
    {
        InvalidVerbNominalFormException.ThrowIfVerbIsNot(verbInInfinitive, VerbNominalForm.Infinitive);

        return verbInInfinitive[..^2];
    }


    public static VerbNominalForm GetVerbNominalForm(string nominalVerb)
    {
        if (IsVerbInInfinitive(nominalVerb)) return VerbNominalForm.Infinitive;
        if (IsVerbInGerund(nominalVerb)) return VerbNominalForm.Gerund;
        if (IsVerbInParticiple(nominalVerb)) return VerbNominalForm.Participle;

        return VerbNominalForm.Undefined;
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
