using System.Drawing;
using System.Text;

namespace SvgX {

    public class Polygon : ElementBase {

        public double[] X { get; set; } = [];

        public double[] Y { get; set; } = [];

        public double StrokeWidth { get; set; } = 1;

        public ColorHtml? StrokeColor { get; set; } = null;

        public ColorHtml? FillColor { get; set; } = null;

        public override string Serialize(Canvas canvas) {
            StringBuilder sb = new StringBuilder();
            sb.Append($"<polygon ");
            sb.Append($"points=`{string.Join(" ", X.Select((x, i) => $"{canvas.PosX(x):0.##},{canvas.PosY(Y[i]):0.##}"))}` ");
            if (StrokeColor.HasValue) sb.Append($"stroke=`{StrokeColor.Value.Value}` stroke-width=`{StrokeWidth:0.##}` ");
            sb.Append($"fill=`{FillColor?.Value ?? "none"}` ");
            sb.Append($"/>");
            return sb.ToString();
        }
    }
}
