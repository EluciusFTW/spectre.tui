namespace Spectre.Tui;

[PublicAPI]
public sealed class InlineMode : ITerminalMode
{
    private int _reservedLines;
    private int _currentHeight;

    public int Height { get; private set; }

    public InlineMode(int height)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);

        Height = height;
        _currentHeight = height;
    }

    public void SetHeight(int height)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);
        Height = height;
    }

    /// <summary>
    /// Returns the effective size for rendering. Updates <c>_currentHeight</c>
    /// to reflect the clamped height (never exceeds the terminal height).
    /// Called every frame by the renderer.
    /// </summary>
    public Size GetSize(int terminalWidth, int terminalHeight)
    {
        _currentHeight = Math.Min(Height, terminalHeight);
        return new Size(terminalWidth, _currentHeight);
    }

    public void OnAttach(AnsiWriter writer)
    {
        // Reserve scrollback space
        for (var i = 0; i < Height; i++)
        {
            writer.Write("\n");
        }

        // Move back up
        writer.CursorUp(Height);

        // Save cursor position and hide cursor
        writer.SaveCursor();
        writer.HideCursor();

        _reservedLines = Height;
    }

    public void OnDetach(AnsiWriter writer)
    {
        // Restore to top of region
        writer.RestoreCursor();

        // Move past the reserved region
        if (_reservedLines > 0)
        {
            writer.CursorDown(_reservedLines);
        }

        // Show cursor and add newline
        writer.ShowCursor();
        writer.Write("\n");
    }

    public void Clear(AnsiWriter writer)
    {
        if (_currentHeight > _reservedLines)
        {
            // Grow: reserve additional scrollback lines
            writer.RestoreCursor();
            if (_reservedLines > 0)
            {
                writer.CursorDown(_reservedLines);
            }

            var additional = _currentHeight - _reservedLines;
            for (var i = 0; i < additional; i++)
            {
                writer.Write("\n");
            }

            writer.CursorUp(_currentHeight);
            writer.SaveCursor();
        }

        // Clear all lines (covers current region + any excess from shrinking)
        var linesToClear = Math.Max(_currentHeight, _reservedLines);
        writer.RestoreCursor();
        for (var i = 0; i < linesToClear; i++)
        {
            if (i > 0)
            {
                writer.CursorDown(1);
            }

            writer.EraseInLine(2);
        }

        writer.RestoreCursor();

        _reservedLines = _currentHeight;
    }

    public void MoveTo(int x, int y, AnsiWriter writer)
    {
        // Restore to saved position (top-left of region)
        writer.RestoreCursor();

        // Move down if needed
        if (y > 0)
        {
            writer.CursorDown(y);
        }

        // Move to absolute column
        writer.CursorHorizontalAbsolute(x + 1);
    }
}
