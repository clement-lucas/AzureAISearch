using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

// Function to merge <em> tags in a list of strings  
List<string> MergeEmTags(List<string> inputList)
{
    // Create a dictionary to store unique sentences  
    var sentenceDictionary = new Dictionary<string, HashSet<string>>();

    foreach (var sentence in inputList)
    {
        // Extract the text without <em> tags  
        var plainText = Regex.Replace(sentence, @"<em>|</em>", "");

        // If the sentence is not already in the dictionary, add it  
        if (!sentenceDictionary.ContainsKey(plainText))
        {
            sentenceDictionary[plainText] = new HashSet<string>();
        }

        // Add all <em> tags from the current sentence to the dictionary entry  
        foreach (Match match in Regex.Matches(sentence, @"<em>(.*?)</em>"))
        {
            sentenceDictionary[plainText].Add(match.Value);
        }
    }

    var result = new List<string>();

    // Reconstruct each sentence from the dictionary  
    foreach (var entry in sentenceDictionary)
    {
        var plainText = entry.Key;
        var emTags = entry.Value;

        // Insert <em> tags back into the plain text without duplicating  
        foreach (var emTag in emTags)
        {
            var content = Regex.Replace(emTag, @"<em>|</em>", "");
            // Replace only the first occurrence of the plain text  
            int index = plainText.IndexOf(content);
            if (index != -1)
            {
                plainText = plainText.Substring(0, index) + emTag + plainText.Substring(index + content.Length);
            }
        }

        result.Add(plainText);
    }

    return result;
}

// Sample usage  
var inputList = new List<string>
{
    "<em>content</em> file と <em>contents</em> file のヒットの違いについて",
    "<em>content</em> file と contents file のヒットの違いについて",
    "content file と <em>contents</em> file のヒットの違いについて",
    "<em>Test</em> file for content and contents",
    "<em>Test</em> file for <em>content</em> and contents",
    "<em>Test</em> file for <em>content</em> and <em>contents</em>",
    "Test file for content and contents",
    "content file と contents file のヒットの違いについて",
    "content file と contents <em>file</em> のヒットの違い<em>について</em>",
    "Test file for <em>content</em> and <em>contents</em>"
};

var outputList = MergeEmTags(inputList);

Console.OutputEncoding = System.Text.Encoding.UTF8;
foreach (var sentence in outputList)
{
    Console.WriteLine(sentence);
}
