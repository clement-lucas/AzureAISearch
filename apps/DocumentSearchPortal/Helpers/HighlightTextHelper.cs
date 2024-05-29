using System;
using System.Text;
using System.Text.RegularExpressions;

namespace DocumentSearchPortal.Helpers
{
    public class HighlightTextHelper
    {
        public static string HighlightSearchTerm(string caption, string searchTerm, string highlightPreTag, string highlightPostTag)
        {
            if (string.IsNullOrEmpty(caption) || string.IsNullOrEmpty(searchTerm))
            {
                return caption;
            }

            // Escape special characters in search term for regex  
            string escapedSearchTerm = Regex.Escape(searchTerm);

            // Create a regex pattern to match the search term  
            string pattern = $@"({escapedSearchTerm})";

            // Replace the search term with the highlighted version  
            string highlightedCaption = Regex.Replace(caption, pattern, $"{highlightPreTag}$1{highlightPostTag}", RegexOptions.IgnoreCase);

            return highlightedCaption;
        }

        public static string ProcessText(string sentence, int numChars)
        {
            // This pattern aims to capture text within <em> tags, potentially handling consecutive <em> tags  
            string pattern = @"(<em>.*?</em>){1,}";

            // Use Regex to find all matches  
            MatchCollection matches = Regex.Matches(sentence, pattern, RegexOptions.Singleline);

            List<string> parts = new List<string>(); // To store processed parts  
            int lastIndex = 0; // Track the end of the last processed section  

            bool isFirstMatch = true;

            foreach (Match match in matches)
            {
                int matchStart = match.Index;
                int matchEnd = match.Index + match.Length;

                if (isFirstMatch)
                {
                    lastIndex = matchStart;
                    isFirstMatch = false;
                    int leadingCharsCount = Math.Max(0, matchStart - numChars);

                    if (leadingCharsCount != 0 && leadingCharsCount >= numChars)
                    {
                        string leadingText = sentence.Substring(leadingCharsCount, numChars);
                        leadingText = "***" + leadingText;
                        parts.Add(leadingText);
                    }
                    else if (leadingCharsCount == 0 || leadingCharsCount < numChars)
                    {
                        string leadingText = sentence.Substring(0, matchStart);
                        parts.Add(leadingText);
                    }
                }

                // Handle leading characters  
                if (matchStart > lastIndex)
                {
                    int charctersInMiddle = matchStart - lastIndex;
                    if (charctersInMiddle > numChars)
                    {
                        string leadingText = sentence.Substring(matchStart - numChars, numChars);
                        parts.Add(leadingText);
                    }
                    else
                    {
                        string leadingText = sentence.Substring(lastIndex, charctersInMiddle);
                        parts.Add(leadingText);
                    }
                }

                // Add the <em>...</em> content directly  
                parts.Add(sentence.Substring(matchStart, match.Length));

                // Update lastIndex for the next iteration  
                lastIndex = matchEnd;

                // Look ahead to see if the next match exists  
                if (match.NextMatch().Success)
                {
                    int nextMatchStart = match.NextMatch().Index;
                    if (nextMatchStart - matchEnd > numChars)
                    {
                        string trailingText = sentence.Substring(matchEnd, numChars);
                        parts.Add(trailingText);
                        parts.Add("***");
                    }
                }
            }

            // Handle trailing characters  
            if (lastIndex < sentence.Length)
            {
                //int trailingCharsCount = Math.Min(sentence.Length, lastIndex + numChars);
                int trailingCharsCount = sentence.Length - lastIndex;
                string trailingText = sentence.Substring(lastIndex, Math.Min(numChars, trailingCharsCount));
                if (trailingCharsCount > numChars)
                {
                    trailingText += "***";
                }
                parts.Add(trailingText);
            }

            string highlightedText = string.Join("", parts).Replace("<em>", "<span class=\"highlighted\">").Replace("</em>", "</span>");

            return highlightedText;
        }

    }
}
