namespace Solutions.ExtensionMethods;

public static class StringExtensions
{
    public static void StartWithCapitalLetters(this string name)
    {
        if (name is not null)
        {
            string[] words = name.Split(' ');

            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 0)
                {
                    words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1);
                }
            }

            name = string.Join(" ", words);
        }
    }
}