using System.Drawing;
using System.Text;

namespace SvgX {
    public class Rectangle : ElementBase {

        private bool _xxIsRelative = true;
        private bool _yyIsRelative = true;
        private double _xx = 1;
        private double _yy = 1;

        public double X1 { get; set; } = 0;

        public double Y1 { get; set; } = 0;

        public double X2 {
            get => _xxIsRelative ? _xx + X1 : _xx;
            set {
                _xx = value;
                _xxIsRelative = false;
            }
        }

        public double Y2 {
            get => _yyIsRelative ? _yy + Y1 : _yy;
            set {
                _yy = value;
                _yyIsRelative = false;
            }
        }

        public double Width {
            get => _xxIsRelative ? _xx : _xx - X1;
            set {
                _xx = value;
                _xxIsRelative = true;
            }
        }

        public double Height {
            get => _yyIsRelative ? _yy : _yy - Y1;
            set {
                _yy = value;
                _yyIsRelative = true;
            }
        }

        public double StrokeWidth { get; set; } = 1;

        public ColorHtml? StrokeColor { get; set; } = null;

        public ColorHtml? FillColor { get; set; } = null;

        public override string Serialize(Canvas canvas) {
            StringBuilder sb = new StringBuilder();
            sb.Append($"<rect ");
            sb.Append($"x=`{Math.Min(canvas.PosX(X1), canvas.PosX(X2)):0.##}` ");
            sb.Append($"y=`{Math.Min(canvas.PosY(Y1), canvas.PosY(Y2)):0.##}` ");
            sb.Append($"width=`{Math.Abs(Width):0.##}` ");
            sb.Append($"height=`{Math.Abs(Height):0.##}` ");
            if (StrokeColor.HasValue) {
                sb.Append($"stroke=`{StrokeColor.Value.Value}` ");
                sb.Append($"stroke-width=`{StrokeWidth:0.##}` ");
            }
            if (FillColor.HasValue) sb.Append($"fill=`{FillColor.Value.Value}` ");
            sb.Append($"/>");
            return sb.ToString();
        }
    }
}
