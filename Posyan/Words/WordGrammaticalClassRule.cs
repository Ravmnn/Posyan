using Fint;


namespace Posyan.Words;


public class WordGrammaticalClassRule(IEnumerable<Word> words) : Rule(null)
{
    public IEnumerable<Word> Words { get; } = words;


    public override bool Pass(Token token) => token.Text.All(char.IsLetterOrDigit);


    public override IEnumerable<Token> Process(Token[] tokens, ref int index)
    {
        var token = tokens[index];
        var word = Words.FirstOrDefault(word => word.Orthography == token.Text);

        token = token with { Id = word == default ? (int)GramaticalClass.Unknown : (int)word.GramaticalClass };

        return [token];
    }
}
