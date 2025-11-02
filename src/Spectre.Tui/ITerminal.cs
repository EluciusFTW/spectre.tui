using System.Runtime.InteropServices;
using System.Text;

namespace Spectre.Tui.Terminal;

public interface ITerminal : IDisposable
{
    void Clear();
    Size GetSize();
    void Write(IEnumerable<(int x, int y, Cell cell)> updates);
    void Flush();
}

public sealed class Terminal : ITerminal
{
    private readonly StringBuilder _buffer;

    [DllImport("libc")]
    private static extern int write(int fd, byte[] buf, int n);

    public Terminal()
    {
        _buffer = new StringBuilder();

        WriteAndFlush("\e[?1049h\e[H");
        WriteAndFlush("\e[?25l");
    }

    public void Dispose()
    {
        WriteAndFlush("\e[?1049l");
        WriteAndFlush("\e[?25h");
    }

    public void Write(IEnumerable<(int x, int y, Cell cell)> updates)
    {
        var lastPosition = default(Position?);

        foreach (var (x, y, cell) in updates)
        {
            if (lastPosition == null || !(x == lastPosition.Value.X + 1 && y == lastPosition.Value.Y))
            {
                MoveTo(x, y);
            }

            lastPosition = new Position(x, y);
            Write((char)cell.Rune.Value);
        }
    }

    public void Flush()
    {
        var lol = _buffer.ToString();
        var utf8 = Encoding.UTF8.GetBytes(lol);
        var _ = write(1, utf8, utf8.Length);
        _buffer.Clear();
    }

    public void Clear()
    {
        Write("\e[2J");
    }

    public Size GetSize()
    {
        // TODO: Use ioctl with TIOCGWINSZ
        return new Size(Console.WindowWidth, Console.WindowHeight);
    }

    private void MoveTo(int x, int y)
    {
        Write($"\e[{y+1};{x+1}H");
    }

    private void Write(char text)
    {
        _buffer.Append(text);
    }

    private void Write(ReadOnlySpan<char> text)
    {
        _buffer.Append(text.ToArray());
    }

    private void WriteAndFlush(ReadOnlySpan<char> text)
    {
        Write(text);
        Flush();
    }
}