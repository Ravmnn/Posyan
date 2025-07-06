using System;
using System.Collections.Generic;
using System.Linq;

using Fint;


namespace Posyan.Words;


public class WordGrammaticalClassRule(IEnumerable<Word> words) : Rule(null)
{
    public IEnumerable<Word> Words { get; } = words;


    public override bool Pass(Token token) => token.Text.All(char.IsLetterOrDigit);


    public override IEnumerable<Token> Process(Token[] tokens, ref int index)
    {
        var token = tokens[index];

        // Posyan works in lowercase
        var word = Words.FirstOrDefault(word => word.Orthography.Equals(token.Text, StringComparison.CurrentCultureIgnoreCase));

        token = token with { Id = word == default ? (int)GrammaticalClass.Unknown : (int)word.GrammaticalClass };

        return [token];
    }
}
