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

    public void InstantiateInflectedVerbs()
        => Words = InstantiateInflectedVerbs(GetBaseVerbs(), Words).ToList();


    public static Word InstantiateBaseVerb(Word word)
        => word.GrammaticalClass == GrammaticalClass.Verb ? new Verb(word) : word;


    public static IEnumerable<Word> InstantiateBaseVerbs(IEnumerable<Word> words)
    {
        List<Word> result = [];

        foreach (var word in words)
            result.Add(InstantiateBaseVerb(word));

        return result;
    }


    public static Word InstantiateInflectedVerb(IEnumerable<Verb> baseVerbs, Word word)
    {
        // the dictionary API won't find inflected verbs, so their grammatical class are
        // always going to be unknown
        if (word.GrammaticalClass != GrammaticalClass.Unknown)
            return word;

        foreach (var baseVerb in baseVerbs)
            if (VerbInflector.IsVerbInflectionOf(baseVerb.Orthography, word.Orthography))
                return new Verb(baseVerb, word.Orthography);

        return word;
    }


    public static IEnumerable<Word> InstantiateInflectedVerbs(IEnumerable<Verb> baseVerbs, IEnumerable<Word> words)
    {
        baseVerbs = baseVerbs.ToArray();

        var result = new List<Word>();

        foreach (var word in words)
            result.Add(InstantiateInflectedVerb(baseVerbs, word));

        return result;
    }
}
