using System.Text;

namespace SvgX {

    public enum Orientation {
        Horizontal,
        Vertical
    }

    public class Ruler : ElementBase {

        public double Position { get; set; } = 0;

        public Orientation Orientation { get; set; } = Orientation.Horizontal;

        public double StrokeWidth { get; set; } = 1;

        public ColorHtml StrokeColor { get; set; } = "#000000";

        public override string Serialize(Canvas canvas) {
            StringBuilder sb = new StringBuilder();
            sb.Append($"<line ");
            sb.Append($"x1=`{(Orientation == Orientation.Vertical? canvas.PosX(Position):canvas.Left):0.##}` ");
            sb.Append($"y1=`{(Orientation == Orientation.Horizontal ? canvas.PosY(Position) :canvas.Top):0.##}` ");
            sb.Append($"x2=`{(Orientation == Orientation.Vertical ? canvas.PosX(Position) : canvas.Right):0.##}` ");
            sb.Append($"y2=`{(Orientation == Orientation.Horizontal ? canvas.PosY(Position) : canvas.Bottom):0.##}` ");
            sb.Append($"stroke=`{StrokeColor.Value}` ");
            sb.Append($"stroke-width=`{StrokeWidth:0.##}` ");
            sb.Append($"/>");
            return sb.ToString();
        }
    }
}
