using System.Drawing;
using System.Windows.Forms;

public class NoHighlightRenderer : ToolStripProfessionalRenderer
{
    private readonly Color hoverColor = Color.FromArgb(70, 70, 70); // Change this color to make it darker or lighter

    protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
    {
        if (e.Item.Selected)
        {
            // Apply a custom color when the item is hovered or selected
            e.Graphics.FillRectangle(new SolidBrush(hoverColor), new Rectangle(Point.Empty, e.Item.Size));
        }
        else
        {
            // Default rendering for unselected items
            base.OnRenderMenuItemBackground(e);
        }
    }
}
