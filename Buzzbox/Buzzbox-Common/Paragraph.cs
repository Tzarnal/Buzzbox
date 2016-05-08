/// <summary>
/// Represents a paragraph in English
/// Original: https://social.msdn.microsoft.com/Forums/vstudio/en-US/74a113ff-e242-4bec-9b30-7037f0d3650f/sentence-case-string?forum=csharpgeneral
/// </summary>
public abstract class Paragraph
{
    /// <summary>
    /// Convert a string in arbitrary case to English sentence capitalisation.
    /// </summary>
    /// <param name="text">The text to convert</param>
    /// <returns>The paragraph of text</returns>
    public static string ToSentenceCase(string text)
    {
        string temporary = text.Trim();
        string result = "";
        while (temporary.Length > 0)
        {
            string[] splitTemporary = SplitAtFirstSentence(temporary);
            temporary = splitTemporary[1];
            if (splitTemporary[0].Length > 0)
            {
                result += CapitaliseSentence(splitTemporary[0]);
            }
            else
            {
                result += CapitaliseSentence(splitTemporary[1]);
                temporary = "";
            }
        }
        return result;
    }

    private static string CapitaliseSentence(string sentence)
    {
        string result = "";
        while (sentence[0] == ' ')
        {
            sentence = sentence.Remove(0, 1);
            result += " ";
        }
        if (sentence.Length > 0)
        {
            result += sentence.TrimStart().Substring(0, 1).ToUpper();
            result += sentence.TrimStart().Substring(1, sentence.TrimStart().Length - 1);
        }
        return result;
    }

    private static string[] SplitAtFirstSentence(string text)
    {
        //these are the characters to start a new sentence after
        int lastChar = text.IndexOfAny(new[] { '.', ':', '\n', '\r', '!', '?' }) + 1;
        if (lastChar == 1)
        {
            lastChar = 0;
        }
        return new[] { text.Substring(0, lastChar), text.Substring(lastChar, text.Length - lastChar) };
    }
}