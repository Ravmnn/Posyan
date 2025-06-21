using Posyan.Words.Verbs;


namespace Posyan.Words;


/*
 * A word is stored in binary using the following model:
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


    public static void Write(BinaryWriter writer, Word word)
    {
        word.WriteToBinary(writer);
        writer.Flush(); // make sure to flush
    }
}
