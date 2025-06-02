namespace Posyan.Words;


/*
 * A word is stored in binary using the following template:
 *
 * 1. Grammatical class (byte)
 * 2. Orthography (string)
 * 3. Definition (string)
 * 4. Word etymology origin (string)
 * 5. Word etymology literal (string)
 *
 * If the word is also a verb, the following data is appended after the
 * previous ones:
 *
 * 6. Root (string)
 * 7. Conjugation (byte)
 * 8. Mood (byte)
 * 9. Tense (byte)
 * 10. Person (byte)
 * 11. Number (byte)
 * 12. Nominal form (byte)
 */


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

        var word = grammaticalClass == GrammaticalClass.Verb ? Verb.FromBinary(reader) : Word.FromBinary(reader);

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
