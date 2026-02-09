using Spectre.Console;
using Spectre.Tui;

namespace Sandbox;

public static class Program
{
    public static void Main(string[] args)
    {
        var running = true;

        using var terminal = Terminal.Create();
        var renderer = new Renderer(terminal);
        renderer.SetTargetFps(144);
        Console.Title = "Spectre.Tui Sandbox";

        var widgets = new List<(bool, WriteWidget)>
        {
            (true, new WriteWidget(Color.Aqua)),
            (false, new WriteWidget(Color.Purple))
        };

        var inputWidget = new WriteWidget(Color.Aqua);

        while (running)
        {

            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                var activePair = widgets.FirstOrDefault(pair => pair.Item1);
                var activeWidget = activePair.Item2;
                switch (key)
                {
                    case ConsoleKey.LeftArrow:
                        var active = widgets.IndexOf(activePair);
                        widgets = [.. widgets.Select((pair, index) => (index == (active + 1) % widgets.Count, pair.Item2))];
                        break;
                    case ConsoleKey.RightArrow:
                        running = false;
                        break;
                    case ConsoleKey.Q:
                        running = false;
                        break;
                    case ConsoleKey.Backspace:
                        activeWidget.DeleteChar();
                        break;
                    case ConsoleKey.Spacebar:
                        activeWidget.AppendChar(' ');
                        break;
                    case ConsoleKey.Escape:
                        activeWidget.Clear();
                        break;
                    case ConsoleKey.Tab:
                        activeWidget.Reverse();
                        break;
                    default:
                        activeWidget.AppendChar(key.ToString()[0]);
                        break;
                }
            }
            renderer.Draw((ctx, elapsed) =>
            {
                var vp = ctx.Viewport;
                var x = (int)Math.Floor(vp.Width * 0.4);
                var left = new Rectangle(0, 0, x, vp.Height);
                var right = new Rectangle(x + 1, 0, vp.Width - x - 1, vp.Height);

                ctx.Render(new BoxWidget(Color.Red), left);
                ctx.Render(new BoxWidget(Color.Gray), right);


                ctx.Render(widgets.First().Item2, Inner(left));
                ctx.Render(widgets.Last().Item2, Inner(right));
            });
        }
    }

    private static Rectangle Inner(Rectangle r) => r.Inflate(-1, -1);
}