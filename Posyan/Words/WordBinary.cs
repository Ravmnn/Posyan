namespace Posyan.Words;


public static class WordBinary
{
    private static string? StringOrNull(string source)
        => source == "null" ? null : source;


    public static Word Read(BinaryReader reader)
    {
        return new Word
        {
            Orthography = reader.ReadString(),
            Definition = reader.ReadString(),
            GrammaticalClass = (GrammaticalClass)reader.ReadInt32(),
            Etymology = new WordEtymology(
                StringOrNull(reader.ReadString()),
                StringOrNull(reader.ReadString())
            )
        };
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
            words.Add(Read(reader));

        return words;
    }


    public static void Write(BinaryWriter writer, Word word)
    {
        writer.Write(word.Orthography);
        writer.Write(word.Definition);
        writer.Write((int)word.GrammaticalClass);
        writer.Write(word.Etymology.Origin ?? "null");
        writer.Write(word.Etymology.Literal ?? "null");
    }


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
