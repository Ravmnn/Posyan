using Fint;

using Posyan.Words;


namespace Posyan.Analysis;


public class TextAnalyser
{
    public List<Word> WordsDefinition { get; set; } = [];


    public IEnumerable<string> SearchForNewWords(IEnumerable<string> words)
        => SearchForNewWords(WordsDefinition, words);


    public static IEnumerable<string> SearchForNewWords(IEnumerable<Word> definedWords, IEnumerable<string> words)
        => from word in words
            where definedWords.All(definedWord => definedWord.Orthography != word)
            select word;
}
