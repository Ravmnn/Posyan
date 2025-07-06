using System;
using System.Numerics;


namespace Posyan.Words.Verbs;


public enum VerbInflectionMood
{
    Undefined, // Verbs in a nominal form aren't inflected (mood, tense, person, number)

    Indicative,
    Subjective,
    Imperative
}


public enum VerbInflectionTense
{
    Undefined, // Verbs in a nominal form aren't inflected (mood, tense, person, number)

    Present,
    PastPerfect,
    PastImperfect,
    Pluperfect, // "pretérito mais-que-perfeito"
    PresentFuture,
    PastFuture
}


public enum VerbInflectionPerson
{
    Undefined, // Verbs in a nominal form aren't inflected (mood, tense, person, number)

    First,
    Second,
    Third
}


public enum VerbInflectionNumber
{
    Undefined, // Verbs in a nominal form aren't inflected (mood, tense, person, number)

    Singular,
    Plural
}


// doesn't work when using a record struct, search why
public record VerbInflectionData(
    VerbInflectionMood Mood = VerbInflectionMood.Indicative,
    VerbInflectionTense Tense = VerbInflectionTense.Present,
    VerbInflectionPerson Person = VerbInflectionPerson.First,
    VerbInflectionNumber Number = VerbInflectionNumber.Singular
)
{
    public static VerbInflectionData Undefined()
        => new VerbInflectionData(VerbInflectionMood.Undefined, VerbInflectionTense.Undefined, VerbInflectionPerson.Undefined,
            VerbInflectionNumber.Undefined);
}


public static class VerbInflector
{
    // TODO: finish Subjunctive and Imperative... Indicative finished by now.


    public static string Inflect(string infinitiveForm, VerbInflectionData inflectionData)
    {
        var verbRoot = Verb.GetInfinitiveVerbRoot(infinitiveForm);
        var verbConjugation = Verb.GetConjugationOfInfinitiveVerb(infinitiveForm);

        var ending = GetEnding(verbConjugation, inflectionData);

        return verbRoot + ending;
    }


    public static string GetEnding(VerbConjugation verbConjugation, VerbInflectionData inflectionData)
        => verbConjugation switch
        {
            VerbConjugation.First => GetFirstConjugationIndicativeEnding(inflectionData),
            VerbConjugation.Second => GetSecondConjugationIndicativeEnding(inflectionData),
            VerbConjugation.Third => GetThirdConjugationIndicativeEnding(inflectionData),

            _ => throw new ArgumentException("Invalid conjugation.")
        };


    public static VerbInflectionData GetInflectionDataFromVerb(string baseVerb, string inflectedVerb)
    {
        var verbRoot = Verb.GetInfinitiveVerbRoot(baseVerb);
        var verbConjugation = Verb.GetConjugationOfInfinitiveVerb(baseVerb);

        if (!Verb.IsVerbInflectionOf(baseVerb, inflectedVerb))
            throw new ArgumentException("Verb is not an inflection of base verb."); // TODO: create a specific exception for this.

        var ending = inflectedVerb[verbRoot.Length..];

        return GetInflectionDataOfEnding(verbConjugation, ending);
    }


    public static VerbInflectionData GetInflectionDataOfEnding(VerbConjugation verbConjugation, string ending)
    {
        var inflectionData = new VerbInflectionData();
        var found = false;

        // go through every inflection possibility to find the specific ending inflection data.

        ForeachEnumItem(typeof(VerbInflectionMood), 1, mood => {
            ForeachEnumItem(typeof(VerbInflectionTense), 1, tense => {
                ForeachEnumItem(typeof(VerbInflectionPerson), 1, person => {
                    ForeachEnumItem(typeof(VerbInflectionNumber), 1, number =>
                    {
                        if (!found)
                            inflectionData = new VerbInflectionData(
                                (VerbInflectionMood)mood,
                                (VerbInflectionTense)tense,
                                (VerbInflectionPerson)person,
                                (VerbInflectionNumber)number
                            );

                        if (GetEnding(verbConjugation, inflectionData) == ending)
                            found = true;
                    });
                });
            });
        });

        if (found)
            return inflectionData;

        throw new InvalidOperationException("Invalid verb inflection.");
    }


    private static void ForeachEnumItem<TEnumItem>(Type enumType, TEnumItem startValue, Action<TEnumItem> action)
        where TEnumItem : INumber<TEnumItem>
    {
        for (var i = startValue;; i++)
        {
            if (!Enum.IsDefined(enumType, i))
                break;

            action(i);
        }
    }





    private static string GetFirstConjugationIndicativeEnding(VerbInflectionData inflectionData)
        => ConjugationEndingTable(inflectionData, new string[,] {
            {"o"   , "ei"   , "ava"   , "ara"   , "arei"  , "aria"},
            {"as"  , "aste" , "avas"  , "aras"  , "arás"  , "arias"},
            {"a"   , "ou"   , "ava"   , "ara"   , "ará"   , "aria"},
            {"amos", "amos" , "ávamos", "áramos", "aremos", "aríamos"},
            {"ais" , "astes", "áveis" , "áreis" , "areis" , "aríeis"},
            {"am"  , "aram" , "avam"  , "aram"  , "arão"  , "ariam"}
        });


    private static string GetSecondConjugationIndicativeEnding(VerbInflectionData inflectionData)
        => ConjugationEndingTable(inflectionData, new string[,] {
            {"o"   , "i"    , "ia"   , "era"   , "erei"  , "eria"},
            {"es"  , "este" , "ias"  , "eras"  , "erás"  , "erias"},
            {"e"   , "eu"   , "ia"   , "era"   , "erá"   , "eria"},
            {"emos", "emos" , "íamos", "êramos", "eremos", "eríamos"},
            {"eis" , "estes", "íeis" , "êreis" , "ereis" , "eríeis"},
            {"em"  , "eram" , "iam"  , "eram"  , "erão"  , "eriam"}
        });


    private static string GetThirdConjugationIndicativeEnding(VerbInflectionData inflectionData)
        => ConjugationEndingTable(inflectionData, new string[,] {
            {"o"   , "i"    , "ia"   , "ira"   , "irei"  , "iria"},
            {"es"  , "iste" , "ias"  , "iras"  , "irás"  , "irias"},
            {"e"   , "iu"   , "ia"   , "ira"   , "irá"   , "iria"},
            {"imos", "imos" , "íamos", "íramos", "iremos", "iríamos"},
            {"is"  , "istes", "íeis" , "íreis" , "ireis" , "iríeis"},
            {"em"  , "iram" , "iam"  , "iram"  , "irão"  , "iriam"}
        });




    // note: be sure to place the endings in the correct order.

    private static string TenseEndingTable(VerbInflectionTense tense, string[] endings)
    {
        if (endings.Length < 6)
            throw new ArgumentException("Must have exactly 6 endings.");

        return tense switch
        {
            VerbInflectionTense.Present => endings[0],
            VerbInflectionTense.PastPerfect => endings[1],
            VerbInflectionTense.PastImperfect => endings[2],
            VerbInflectionTense.Pluperfect => endings[3],
            VerbInflectionTense.PresentFuture => endings[4],
            VerbInflectionTense.PastFuture => endings[5],

            _ => throw new ArgumentException("Invalid tense.")
        };
    }


    private static string ConjugationEndingTable(VerbInflectionData inflectionData, string[,] endings)
    {
        if (endings.GetLength(0) < 6 || endings.GetLength(1) < 6)
            throw new ArgumentException("Ending matrix must be exactly 6x6.");

        return inflectionData.Person switch
        {
            VerbInflectionPerson.First when inflectionData.Number == VerbInflectionNumber.Singular => TenseEndingTable(inflectionData.Tense, [endings[0,0], endings[0,1], endings[0,2], endings[0,3], endings[0,4], endings[0,5]]),
            VerbInflectionPerson.Second when inflectionData.Number == VerbInflectionNumber.Singular => TenseEndingTable(inflectionData.Tense,[endings[1,0], endings[1,1], endings[1,2], endings[1,3], endings[1,4], endings[1,5]]),
            VerbInflectionPerson.Third when inflectionData.Number == VerbInflectionNumber.Singular => TenseEndingTable(inflectionData.Tense,[endings[2,0], endings[2,1], endings[2,2], endings[2,3], endings[2,4], endings[2,5]]),

            VerbInflectionPerson.First when inflectionData.Number == VerbInflectionNumber.Plural => TenseEndingTable(inflectionData.Tense,[endings[3,0], endings[3,1], endings[3,2], endings[3,3], endings[3,4], endings[3,5]]),
            VerbInflectionPerson.Second when inflectionData.Number == VerbInflectionNumber.Plural => TenseEndingTable(inflectionData.Tense,[endings[4,0], endings[4,1], endings[4,2], endings[4,3], endings[4,4], endings[4,5]]),
            VerbInflectionPerson.Third when inflectionData.Number == VerbInflectionNumber.Plural => TenseEndingTable(inflectionData.Tense,[endings[5,0], endings[5,1], endings[5,2], endings[5,3], endings[5,4], endings[5,5]]),

            _ => throw new ArgumentException("Invalid person/number.")
        };
    }
}
