namespace Spectre.Tui;

[Flags]
[PublicAPI]
public enum Decoration : ushort
{
    None = 0,
    Bold = 1 << 1,
    Dim = 1 << 2,
    Italic = 1 << 3,
    Underlined = 1 << 4,
    SlowBlink = 1 << 5,
    RapidBlink = 1 << 6,
    Invert = 1 << 7,
    Conceal = 1 << 8,
    Strikethrough = 1 << 9,
}

internal static class DecorationTable
{
    private static readonly Dictionary<string, Decoration?> _lookup;
    private static readonly Dictionary<Decoration, string> _reverseLookup;

    static DecorationTable()
    {
        _lookup = new Dictionary<string, Decoration?>(StringComparer.OrdinalIgnoreCase)
            {
                { "none", Decoration.None },
                { "bold", Decoration.Bold },
                { "b", Decoration.Bold },
                { "dim", Decoration.Dim },
                { "italic", Decoration.Italic },
                { "i", Decoration.Italic },
                { "underline", Decoration.Underlined },
                { "u", Decoration.Underlined },
                { "invert", Decoration.Invert },
                { "reverse", Decoration.Invert },
                { "conceal", Decoration.Conceal },
                { "blink", Decoration.SlowBlink },
                { "slowblink", Decoration.SlowBlink },
                { "rapidblink", Decoration.RapidBlink },
                { "strike", Decoration.Strikethrough },
                { "strikethrough", Decoration.Strikethrough },
                { "s", Decoration.Strikethrough },
            };

        _reverseLookup = new Dictionary<Decoration, string>();
        foreach (var (name, decoration) in _lookup)
        {
            // Cannot happen, but the compiler thinks so...
            if (decoration == null)
            {
                continue;
            }

            if (!_reverseLookup.ContainsKey(decoration.Value))
            {
                _reverseLookup[decoration.Value] = name;
            }
        }
    }

    public static Decoration? GetDecoration(string name)
    {
        _lookup.TryGetValue(name, out var result);
        return result;
    }

    public static List<string> GetMarkupNames(Decoration decoration)
    {
        var result = new List<string>();

        Enum.GetValues<Decoration>()
            .Where(flag => (decoration & flag) != 0)
            .ForEach(flag =>
            {
                if (flag != Decoration.None && _reverseLookup.TryGetValue(flag, out var name))
                {
                    result.Add(name);
                }
            });

        return result;
    }
}