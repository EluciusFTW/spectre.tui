using Spectre.Tui;

namespace Sandbox;

public sealed class BoxWidget(Color? color = null) : IWidget
{
    public void Render(IRenderContext context)
    {
        var area = context.Viewport;

        // Top/Bottom
        for (var x = 0; x < area.Width; x++)
        {
            if (x == 0)
            {
                context.SetRune(x, 0, '╭');
                context.SetForeground(x, 0, color);
                context.SetRune(x, area.Height - 1, '╰');
                context.SetForeground(x, area.Height - 1, color);
            }
            else if (x == area.Width - 1)
            {
                context.SetRune(x, 0, '╮');
                context.SetForeground(x, 0, color);
                context.SetRune(x, area.Height - 1, '╯');
                context.SetForeground(x, area.Height - 1, color);
            }
            else
            {
                context.SetRune(x, 0, '─');
                context.SetForeground(x, 0, color);
                context.SetRune(x, area.Height - 1, '─');
                context.SetForeground(x, area.Height - 1, color);
            }
        }

        // Sides
        for (var y = 1; y < area.Height - 1; y++)
        {
            context.SetRune(0, y, '│');
            context.SetRune(area.Width - 1, y, '│');
            context.SetForeground(0, y, color);
            context.SetForeground(area.Width - 1, y, color);
        }
    }
}