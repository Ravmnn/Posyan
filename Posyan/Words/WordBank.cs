namespace Posyan.Words;


public class WordBank
{
    public List<Word> Words { get; set; }


    public WordBank()
    {
        Words = [];
    }


    public WordBank(IEnumerable<Word> words)
    {
        Words = words.ToList();
    }


    public bool IsWordNew(string word)
        => Words.All(w => w.Orthography != word);


    public IEnumerable<Verb> GetBaseVerbs()
        => from word in Words
            let verb = word as Verb
            where verb is not null && verb.NominalForm == VerbNominalForm.Infinitive
            select verb;


    public IEnumerable<Verb> GetInflectedVerbs()
        => from word in Words
            let verb = word as Verb
            where verb is not null && verb.NominalForm != VerbNominalForm.Infinitive
            select verb;


    public IEnumerable<string> GetNewWordsIn(IEnumerable<string> words)
        => from word in words
            where IsWordNew(word)
            select word;


    public void InstantiateBaseVerbs()
        => Words = InstantiateBaseVerbs(Words).ToList();

    public void InstantiateVariantVerbs()
        => Words = InstantiateVariantVerbs(GetBaseVerbs(), Words).ToList();


    public void LoadWordsFromBinary(BinaryReader reader)
        => Words = ReadWordsFromBinary(reader).ToList();

    public void LoadWordsFromFile(string path)
        => LoadWordsFromBinary(new BinaryReader(new FileStream(path, FileMode.Open)));

    public void SaveWordsToBinary(BinaryWriter writer)
        => WriteWordsToBinary(writer, Words);

    public void SaveWordsToFile(string path)
        => SaveWordsToBinary(new BinaryWriter(new FileStream(path, FileMode.Append)));


    public static Word InstantiateBaseVerb(Word word)
        => word.GrammaticalClass == GrammaticalClass.Verb ? new Verb(word) : word;


    public static IEnumerable<Word> InstantiateBaseVerbs(IEnumerable<Word> words)
    {
        List<Word> result = [];

        foreach (var word in words)
            result.Add(InstantiateBaseVerb(word));

        return result;
    }


    // variant verbs are the inflected ones or the nominal ones.

    public static Word InstantiateVariantVerb(IEnumerable<Verb> baseVerbs, Word word)
    {
        // the dictionary API won't find inflected verbs, so their grammatical class are
        // always going to be unknown.

        if (word.GrammaticalClass != GrammaticalClass.Unknown)
            return word;

        foreach (var baseVerb in baseVerbs)
            if (Verb.IsVerbRootEqual(baseVerb.Orthography, word.Orthography))
                return new Verb(baseVerb, word.Orthography);

        return word;
    }


    public static IEnumerable<Word> InstantiateVariantVerbs(IEnumerable<Verb> baseVerbs, IEnumerable<Word> words)
    {
        baseVerbs = baseVerbs.ToArray();

        var result = new List<Word>();

        foreach (var word in words)
            result.Add(InstantiateVariantVerb(baseVerbs, word));

        return result;
    }


    public static IEnumerable<Word> ReadWordsFromBinary(BinaryReader reader)
    {
        var words = new List<Word>();

        while (reader.BaseStream.Position != reader.BaseStream.Length)
        {
            Console.WriteLine($"current pos: {reader.BaseStream.Position}; end pos: {reader.BaseStream.Length}");

            var word = WordBinary.Read(reader);
            words.Add(word);
        }

        return words;
    }

    public static IEnumerable<Word> ReadWordsFromFile(string path)
        => ReadWordsFromBinary(new BinaryReader(new FileStream(path, FileMode.Open)));


    public static void WriteWordsToBinary(BinaryWriter writer, IEnumerable<Word> words)
    {
        foreach (var word in words)
        {
            // do not write non-infinitive (non-base) verbs.
            if (word is Verb verb && !Verb.IsVerbInInfinitive(verb.Orthography))
                continue;

            WordBinary.Write(writer, word);
        }
    }

    public static void WriteWordsToFile(string path, IEnumerable<Word> words)
        => WriteWordsToBinary(new BinaryWriter(new FileStream(path, FileMode.Append)), words);
}
