using Fint;

using Posyan.Words;


namespace Posyan;


public class TextAnalyser()
{
    private readonly Scanner _scanner = new Scanner(new WordOrDigitRule(0));

    public List<Word> WordsDefinition { get; set; } = [];


    public bool IsWordNew(string word) => !WordsDefinition.Exists(wordDefinition => wordDefinition.Orthography == word);


    public IEnumerable<string> SearchForNewWords(string source)
    {
        var tokens = _scanner.Scan(new Lexer(source).Tokenize());

        return from token in tokens where IsWordNew(token.Text) select token.Text;
    }
}
