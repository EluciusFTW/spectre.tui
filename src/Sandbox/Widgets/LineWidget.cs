using Spectre.Console;
using Spectre.Tui;

namespace Sandbox;

public sealed class LineWidget(int y, Color? color = null) : IWidget
{
    public void Render(RenderContext context)
    {
        var area = context.Viewport;
        for (var x = 1; x < area.Width - 1; x++)
        {
            context.SetSymbol(x, y % area.Height, '-');
            context.SetForeground(x, y, color);
        }
    }
}

public sealed class WriteWidget(Color? color = null) : IWidget
{
    private List<char> _content = [];
    public void AppendChar(char c) => _content.Add(c);
    public void DeleteChar() => _content.RemoveAt(_content.Count - 1);
    public void Clear() => _content = [];
    public void Reverse() => _content.Reverse();
    public void Activate() => color = Color.Blue;

    public void Render(RenderContext context)
    {
        foreach (var pair in _content.Select((c, i) => (c, i)))
        {
            context.SetSymbol(pair.i, 1, pair.c);
            context.SetForeground(pair.i, 1, color);
        }
    }
}