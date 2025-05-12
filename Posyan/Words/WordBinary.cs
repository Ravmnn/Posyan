using Spectre.Console;

namespace Posyan.Words;


public static class WordBinary
{
    private static void ReadWithoutAdvancing(BinaryReader reader, Action<BinaryReader> readingOperation)
    {
        var oldPosition = reader.BaseStream.Position;

        readingOperation(reader);

        reader.BaseStream.Position = oldPosition;
    }


    public static Word Read(BinaryReader reader)
    {
        // gets the grammatical class without advancing the stream position,
        // so we can instantiate the correct object.

        var grammaticalClass = GrammaticalClass.Unknown;
        ReadWithoutAdvancing(reader, binaryReader => grammaticalClass = (GrammaticalClass)binaryReader.ReadByte());

        var word = grammaticalClass == GrammaticalClass.Verb ? new Verb() : new Word();

        word.ReadFromBinary(reader);

        return word;
    }


    public static Word Read(string path)
    {
        using var reader = new BinaryReader(new FileStream(path, FileMode.Open));
        return Read(reader);
    }


    public static IEnumerable<Word> ReadAll(string path)
    {
        using var reader = new BinaryReader(new FileStream(path, FileMode.Open));
        var words = new List<Word>();

        while (reader.BaseStream.Position != reader.BaseStream.Length)
        {
            var word = Read(reader);
            words.Add(word);
        }

        return words;
    }




    public static void Write(BinaryWriter writer, Word word)
        => word.WriteToBinary(writer);


    public static void Write(string path, Word word)
    {
        using var writer = new BinaryWriter(new FileStream(path, FileMode.Append));
        Write(writer, word);
    }


    public static void WriteAll(string path, IEnumerable<Word> words)
    {
        using var writer = new BinaryWriter(new FileStream(path, FileMode.Append));

        foreach (var word in words)
            Write(writer, word);
    }
}
