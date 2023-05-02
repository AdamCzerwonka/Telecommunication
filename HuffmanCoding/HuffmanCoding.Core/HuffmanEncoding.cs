using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;

namespace HuffmanCoding.Core;

public partial class HuffmanEncoding
{
    public HuffmanEncoding(string text)
    {
        Generate(text);
    }

    private HuffmanEncoding()
    {
    }

    public static HuffmanEncoding CreateFromEncoding(string dict)
    {
        var obj = new HuffmanEncoding();
        // przypisz regex, ktory podzieli slownik kodowy
        var regex = DictRegex();
        // podziel slownik oraz utworz z nich dictionary
        var matches = regex.Matches(dict).ToArray().Select(x =>
        {
            var character = x.Value[0];
            var code = x.Value[1..^1];
            var pair = new KeyValuePair<char, string>(character, code);
            return pair;
        }).ToDictionary(x => x.Key, x => x.Value);
        // przypisz slownik do instancji klasy
        obj.EncodedCharacters = matches;

        return obj;
    }

    public static HuffmanEncoding CreateFromEncoding(byte[] dict)
    {
        var obj = new HuffmanEncoding();
        var characterAmount = dict[0];
        var position = 1;
        var dictSpan = dict.AsSpan();
        for (var i = 0; i < characterAmount; i++)
        {
            var characterBytes = dictSpan.Slice(position, 2);
            var character = 
                Encoding.UTF8.GetString(characterBytes[1] == 0 ? characterBytes[..1] : characterBytes)[0];

            position += 2;
            var bitLenght = dictSpan[position];
            position++;
            var byteLenght = (int)Math.Ceiling(bitLenght / 8.0);
            var encodingBytes = dictSpan
                .Slice(position, byteLenght)
                .ToArray()
                .Select(b => Convert.ToString(b, 2).PadLeft(8, '0'));
            var encoding = string.Concat(encodingBytes)[(byteLenght * 8 - bitLenght)..];
            obj.EncodedCharacters.Add(character, encoding);

            position += byteLenght;
        }

        return obj;
    }

    public Dictionary<char, string> EncodedCharacters { get; set; } = new();

    public string GetEncoding()
    {
        // stworz pustego strniga
        var payload = string.Empty;
        // zapisz wartosci slownika jako jeden string
        foreach (var encEncodedCharacter in EncodedCharacters)
        {
            payload += $"{encEncodedCharacter.Key}{encEncodedCharacter.Value} ";
        }

        return payload;
    }

    public byte[] GetBinaryEncoding()
    {
        var bytes = new List<byte> { (byte)EncodedCharacters.Count };
        foreach (var pair in EncodedCharacters)
        {
            var character = Encoding.UTF8.GetBytes(pair.Key.ToString());
            bytes.AddRange(character);
            if (character.Length == 1)
            {
                bytes.Add(0);
            }

            var encodingLenght = pair.Value.Length;
            bytes.Add((byte)encodingLenght);
            var encoding = pair.Value.ConvertBinaryString();
            bytes.AddRange(encoding);
        }

        return bytes.ToArray();
    }

    public string EncodeMessage(string message)
    {
        // stworz string builder
        var builder = new StringBuilder();
        // dopisz do buildera zakodowane znaki
        foreach (var c in message)
        {
            builder.Append(EncodedCharacters[c]);
        }

        return builder.ToString();
    }

    public string DecodeMessage(string encdodedMessage)
    {
        // stworz pustego stringa
        var message = string.Empty;
        // odwroc slownik
        var reversedDict = EncodedCharacters.ToDictionary(x => x.Value, x => x.Key);
        // stworz pustego stringa
        var current = string.Empty;
        // odkoduj wiadomosc za pomoca slownika
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
        // przypisz dlugosc wiadomosci
        var contentLenght = text.Length;
        // utworz slownik czestotliwosci znakow
        var freqs = new Dictionary<char, int>();

        // dodaj do slownika znaki wraz z czestotliwoscia
        foreach (var c in text)
        {
            var isPresent = freqs.TryGetValue(c, out var value);
            freqs[c] = isPresent ? value + 1 : 1;
        }

        // posortuj slownika
        var sortedFreqs = freqs.OrderBy(x => x.Value);
        // utworz drzewo
        var trees = new List<Node>();
        // utworz node'y z slownika czestotliwosci
        foreach (var pair in sortedFreqs)
        {
            var node = new Node()
            {
                Character = pair.Key,
                Freq = (int)(pair.Value / (float)contentLenght * 100)
            };

            trees.Add(node);
        }

        // polacz nody ze soba
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

    // utworz tekst opisujacy pozyje znaku
    private void Traverse(Node node, string currentText = "")
    {
        var text = currentText;
        if (node.Character is not null)
        {
            EncodedCharacters.Add(node.Character.Value, text);
            return;
        }

        Traverse(node.Left, text + '0');
        Traverse(node.Right, text + '1');
    }

    [GeneratedRegex("(.|\\s)\\d+\\s")]
    private static partial Regex DictRegex();
}