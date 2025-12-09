namespace Spectre.Tui;

public interface IRendererContext
{
    Rectangle Viewport { get; }

    void Render(IWidget widget, Rectangle area);

    /// <summary>
    /// Sets the cell at the specified (viewport) coordinates.
    /// </summary>
    void SetCell(int x, int y, Cell cell);
}

public static class IRenderContextExtensions
{
    extension(IRendererContext context)
    {
        public void Render(IWidget widget)
        {
            context.Render(widget, context.Viewport);
        }

        public void SetRune(int x, int y, char rune)
        {
            context.SetRune(x, y, new Rune(rune));
        }

        public void SetRune(int x, int y, Rune rune)
        {
            context.SetCell(x, y, new Cell { Rune = rune });
        }
    }
}

internal sealed record RenderContext : IRendererContext
{
    public Buffer Buffer { get; }
    public Rectangle Screen { get; private init; }
    public Rectangle Viewport { get; private init; }

    public RenderContext(Buffer buffer, Rectangle screen, Rectangle viewport)
    {
        Buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
        Screen = screen;
        Viewport = viewport;
    }

    public void Render(IWidget widget, Rectangle area)
    {
        if (area.Width == 0 || area.Height == 0)
        {
            return;
        }

        var screen = Screen.Intersect(
            new Rectangle(
                Screen.X + area.X, Screen.Y + area.Y,
                area.Width, area.Height));

        var viewport = new Rectangle(0, 0, screen.Width, screen.Height);
        widget.Render(this with { Screen = screen, Viewport = viewport });
    }

    public void SetCell(int x, int y, Cell cell)
    {
        Buffer.SetCell(Screen.X + x, Screen.Y + y, cell);
    }
}