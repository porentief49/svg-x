using System.Drawing;
using System.Text;

namespace SvgX {

    public class Ellipse : ElementBase {

        //public void DrawEllipse(double cx, double cy, double rx, double ry, string strokeColor, double strokeWidth) {
        //    _sb.AppendLine($"    <ellipse cx=`{cx:0.##}` cy=`{cy:0.##}` rx=`{rx:0.##}` ry=`{ry:0.##}` stroke=`{strokeColor}` stroke-width=`{strokeWidth}` fill=`none`/>");
        //}

        //public void DrawEllipse(double cx, double cy, double rx, double ry, Color strokeColor, double strokeWidth) {
        //    DrawEllipse(cx, cy, rx, ry, ColorTranslator.ToHtml(strokeColor), strokeWidth);
        //}

        //public void FillEllipse(double cx, double cy, double rx, double ry, string fillColor) {
        //    _sb.AppendLine($"    <ellipse cx=`{cx:0.##}` cy=`{cy:0.##}` rx=`{rx:0.##}` ry=`{ry:0.##}` fill=`{fillColor}`/>");
        //}

        //public void FillEllipse(double cx, double cy, double rx, double ry, Color fillColor) {
        //    FillEllipse(cx, cy, rx, ry, ColorTranslator.ToHtml(fillColor));
        //}

        //private bool _xxIsRelative = true;
        //private bool _yyIsRelative = true;
        //private double _xx = 1;
        //private double _yy = 1;

        public double X { get; set; } = 0;

        public double Y { get; set; } = 0;

        public double RadiusX { get; set; } = 1;

        public double RadiusY { get; set; } = 1;

        //public double? CornerRadius { get; set; } = null;

        public double StrokeWidth { get; set; } = 1;

        public ColorHtml? StrokeColor { get; set; } = null;

        public ColorHtml? FillColor { get; set; } = null;

        public override string Serialize(Canvas canvas) {
            StringBuilder sb = new StringBuilder();
            sb.Append($"<ellipse ");
            sb.Append($"cx=`{canvas.PosX(X):0.##}` ");
            sb.Append($"cy=`{canvas.PosY(Y):0.##}` ");
            sb.Append($"rx=`{RadiusX:0.##}` ");
            sb.Append($"ry=`{RadiusY:0.##}` ");
            if (StrokeColor.HasValue) sb.Append($"stroke=`{StrokeColor.Value.Value}` stroke-width=`{StrokeWidth:0.##}` ");
            //if (CornerRadius.HasValue) sb.Append($"rx=`{CornerRadius.Value:0.##}` ry=`{CornerRadius.Value:0.##}` ");
            sb.Append($"fill=`{FillColor?.Value ?? "none"}` ");
            sb.Append($"/>");
            return sb.ToString();
        }
    }
}
