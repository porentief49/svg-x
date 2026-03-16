using System.Drawing;
using System.Text;
using ExCSS;
using static System.Net.Mime.MediaTypeNames;

namespace SvgX {

    public enum TextAnchor {
        Start,
        Middle,
        End
    }

    public class Text : ElementBase {

        public double X { get; set; } = 0;

        public double Y { get; set; } = 0;

        public string Value { get; set; } = string.Empty;

        public string Font { get; set; } = "Consolas";

        public double Size { get; set; } = 1;

        public ColorHtml Color { get; set; } = "#000000";

        public TextAnchor Anchor { get; set; } = TextAnchor.Start;

        public double? Rotation { get; set; } = null;

        public override string Serialize(Canvas canvas) {
            StringBuilder sb = new StringBuilder();
            sb.Append($"<text ");
            sb.Append($"x=`{canvas.PosX(X):0.##}` ");
            sb.Append($"y=`{canvas.PosY(Y):0.##}` ");
            sb.Append($"font-family=`'{Font}'` ");
            sb.Append($"font-size=`{Size:0.##}` ");
            sb.Append($"fill=`{Color.Value}` ");
            sb.Append($"text-anchor=`{Anchor.ToString().ToLower()}` ");
            if (Rotation.HasValue) {
                double sign = Math.Sign(canvas.Right-canvas.Left) * Math.Sign(canvas.Bottom-canvas.Top);
                sb.Append($"transform=`rotate({Rotation*sign:0.##} {canvas.PosX(X):0.##}, {canvas.PosY(Y):0.##})` ");
            }
            sb.Append($"xml:space=`preserve`>{Value}</text>");
            return sb.ToString();
        }
    }
}
