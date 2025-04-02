using Fint;

using Posyan.Words;


namespace Posyan;


public record struct WordRegisteringData(string WordBeingProcessed, bool IsNew, bool Failed, int WordsProcessed, int WordCount);


public class TextAnalyser(OpenDictApi api)
{
    private readonly Scanner _scanner = new Scanner(new WordOrDigitRule(0));


    public OpenDictApi Api { get; } = api;

    public List<Word> Words { get; set; } = [];


    public async Task<IEnumerable<Word>> RegisterNewWordsFromStringAsync(string source, IProgress<WordRegisteringData>? progress = null)
    {
        var newWords = new List<Word>();
        var tokens = _scanner.Scan(new Lexer(source).Tokenize()).ToArray();

        var wordsProcessed = 0;

        await Parallel.ForEachAsync(tokens, new ParallelOptions { MaxDegreeOfParallelism = 8 }, (token, _) =>
        {
            var isNewWord = Words.All(word => !word.Orthography.Equals(token.Text, StringComparison.CurrentCultureIgnoreCase)) &&
                            newWords.All(word => !word.Orthography.Equals(token.Text, StringComparison.CurrentCultureIgnoreCase));

            var failed = false;

            if (isNewWord)
            {
                if (Api.GetWordAsync(token.Text).Result is { } validWord)
                    newWords.Add(validWord);
                else
                    failed = true;
            }

            progress?.Report(new WordRegisteringData(token.Text, isNewWord, failed, ++wordsProcessed, tokens.Length));

            return ValueTask.CompletedTask;
        });

        Words.AddRange(newWords);

        return newWords;
    }
}
