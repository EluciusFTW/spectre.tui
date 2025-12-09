using System.Diagnostics;

namespace Spectre.Tui;

public sealed class Renderer
{
    private readonly ITerminal _terminal;
    private readonly Stopwatch _stopwatch;
    private readonly Buffer[] _buffers;
    private TimeSpan _lastUpdate;
    private int _bufferIndex;
    private Rectangle _viewport;

    public Renderer(ITerminal terminal)
    {
        _terminal = terminal ?? throw new ArgumentNullException(nameof(terminal));
        _lastUpdate = TimeSpan.Zero;
        _viewport = _terminal.GetSize().ToRectangle();
        _buffers =
        [
            Buffer.Empty(_viewport),
            Buffer.Empty(_viewport)
        ];

        _stopwatch = new Stopwatch();
        _stopwatch.Start();
    }

    public void Draw(Action<IRendererContext, TimeSpan> callback)
    {
        var elapsed = _stopwatch.Elapsed - _lastUpdate;
        _lastUpdate = _stopwatch.Elapsed;

        // Resize the buffers
        Resize();

        // Fill out the current frame
        var frame = new RenderContext(_buffers[_bufferIndex], _viewport, _viewport);
        callback(frame, elapsed);

        // Calculate the diff between the back and front buffer
        var prev = _buffers[1 - _bufferIndex];
        var curr = _buffers[_bufferIndex];
        var diff = prev.Diff(curr);

        // Render the current frame
        _terminal.Write(diff);

        // Swap the buffers
        SwapBuffers();

        // Flush the backend
        _terminal.Flush();
    }

    private void Resize()
    {
        var area = _terminal.GetSize().ToRectangle();
        if (area.Equals(_viewport))
        {
            return;
        }

        // Reset buffer
        _buffers[_bufferIndex].Resize(area);
        _buffers[1 - _bufferIndex].Resize(area);
        _viewport = area;

        // Clear the terminal
        _terminal.Clear();

        // Reset the back buffer
        _buffers[1 - _bufferIndex].Reset();
    }

    private void SwapBuffers()
    {
        _buffers[1 - _bufferIndex].Reset();
        _bufferIndex = 1 - _bufferIndex;
    }
}