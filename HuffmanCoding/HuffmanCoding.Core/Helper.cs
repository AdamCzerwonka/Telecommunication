namespace HuffmanCoding.Core;

public static class Helper
{
    public static byte[] ConvertBinaryString(this string text)
    {
        var nBytes = (int)Math.Ceiling(text.Length / 8m);
        text = text.PadLeft(nBytes * 8, '0');
        return Enumerable
            .Range(0, nBytes)
            .Select(i => text.Substring(8 * i, 8))
            .Select(s => Convert.ToByte(s, 2))
            .ToArray();
    }

    public static string JoinString(this IEnumerable<string> list)
    {
        return string.Concat(list);
    }
}