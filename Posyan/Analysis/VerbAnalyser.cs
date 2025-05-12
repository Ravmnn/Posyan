using Posyan.Words;


namespace Posyan.Analysis;


public class VerbAnalyser
{
    public static IEnumerable<Word> InstantiateVerbs(IEnumerable<Word> words)
    {
        List<Word> result = [];

        foreach (var word in words)
            result.Add(word.GrammaticalClass == GrammaticalClass.Verb ? new Verb(word) : word);

        return result;
    }
}
