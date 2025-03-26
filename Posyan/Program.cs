namespace Posyan;


// TODO: add text highlight based on the word grammatical class
// TODO: add verb conjugation detector
// TODO: store already searched words and its data (in disc) for faster look ups.


class Program
{
    static void Main(string[] args)
    {
        var dict = new OpenDictApi();
        var xml = dict.GetWordXmlData("gostar");
        var word = Word.FromXml(xml);

        Console.WriteLine(word.ToString());
    }
}
