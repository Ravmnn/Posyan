namespace Posyan.Words;


public class InvalidVerbNominalFormException : Exception
{
    public string VerbOrthography { get; }


    public InvalidVerbNominalFormException(string verb)
    {
        VerbOrthography = verb;
    }

    public InvalidVerbNominalFormException(string verb, string message) : base(message)
    {
        VerbOrthography = verb;
    }

    public InvalidVerbNominalFormException(string verb, string message, Exception inner) : base(message, inner)
    {
        VerbOrthography = verb;
    }


    public static void ThrowIfVerbIsNot(string verb, VerbNominalForm form, string? extraMessage = null)
    {
        if (!Verb.IsVerbIn(verb, form))
            throw new InvalidVerbNominalFormException(verb, $"Verb is not in {form} form." + (extraMessage is not null ? $" {extraMessage}" : ""));
    }


    public override string ToString()
        => Message + $" (\"{VerbOrthography}\")";
}
