using System.Text;
using System.Text.RegularExpressions;

namespace HuffmanCoding.Core;

public partial class HuffmanEncoding
{
    public HuffmanEncoding(string text)
    {
        Generate(text);
    }
    
    private HuffmanEncoding() {}

    public static HuffmanEncoding CreateFromEncoding(string dict)
    {
        var obj = new HuffmanEncoding();
        var regex = DictRegex();
        var matches = regex.Matches(dict).ToArray().Select(x =>
        {
            var character = x.Value[0];
            var code = x.Value[1..^1];
            var pair = new KeyValuePair<char, string>(character, code);
            return pair;
        }).ToDictionary(x => x.Key, x => x.Value);
        obj.EncodedCharacters = matches;

        return obj;
    }

    public Dictionary<char, string> EncodedCharacters { get; set; } = new();

    public string GetEncoding()
    {
        var payload = string.Empty;
        foreach (var encEncodedCharacter in EncodedCharacters)
        {
            payload += $"{encEncodedCharacter.Key}{encEncodedCharacter.Value} ";
        }

        return payload;
    }

    public string EncodeMessage(string message)
    {
        var builder = new StringBuilder();
        foreach (var c in message)
        {
            builder.Append(EncodedCharacters[c]);
        }

        return builder.ToString();
    }

    public string DecodeMessage(string encdodedMessage)
    {
        var message = string.Empty;
        var reversedDict = EncodedCharacters.ToDictionary(x => x.Value, x => x.Key);
        string current = string.Empty;
        for (int i = 0; i < encdodedMessage.Length; i++)
        {
            current += encdodedMessage[i];
            if (reversedDict.TryGetValue(current, out var value))
            {
                message += value;
                current = string.Empty;
            }
        }

        return message;
    }


    private void Generate(string text)
    {
        var contentLenght = text.Length;

        var freqs = new Dictionary<char, int>();

        foreach (var c in text)
        {
            var isPresent = freqs.TryGetValue(c, out var value);
            freqs[c] = isPresent ? value + 1 : 1;
        }

        var sortedFreqs = freqs.OrderBy(x => x.Value);

        var trees = new List<Node>();
        foreach (var pair in sortedFreqs)
        {
            var node = new Node()
            {
                Character = pair.Key,
                Freq = (int)(pair.Value / (float)contentLenght * 100)
            };

            trees.Add(node);
        }

        while (trees.Count != 1)
        {
            var first = trees[0];
            var second = trees[1];

            trees.Remove(first);
            trees.Remove(second);

            var newNode = new Node()
            {
                Left = first,
                Right = second,
                Character = null,
                Freq = first.Freq + second.Freq
            };

            trees.Add(newNode);
            trees.Sort((x, y) =>
            {
                if (x.Freq > y.Freq)
                {
                    return 1;
                }

                if (x.Freq == y.Freq)
                {
                    return 0;
                }

                return -1;
            });
        }

        var endTree = trees[0];

        Traverse(endTree);
    }

    private void Traverse(Node node, string currentText = "")
    {
        var text = currentText;

        if (node.Character is not null)
        {
            EncodedCharacters.Add(node.Character.Value, text);
            return;
        }

        //go left
        Traverse(node.Left, text + '0');
        Traverse(node.Right, text + '1');
    }

    [GeneratedRegex(".\\d+\\s")]
    private static partial Regex DictRegex();
}