using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml.Linq;

namespace SvgLib {

    public partial class SvgX {

        private List<ElementBase> _elements = [];
        public Canvas Canvas = new();

        public void AddElement(ElementBase element) {
            _elements.Add(element);
        }

        public string Serialize() {
            string header = $"<svg width=`{Canvas.Width}` height=`{Canvas.Height:0.##}` xmlns=`http://www.w3.org/2000/svg`>";
            string body = string.Join(Environment.NewLine, _elements.Select(e => $"    {e.Serialize(Canvas)}"));
            string footer = $"</svg>";
            return ($"{header}\r\n{body}\r\n{footer}").Replace('`', '"');
        }

        public void ExportToPdf(string path) {


        }
    }

    public class Line : ElementBase {

        private bool _xRel = false;
        private bool _yRel = false;
        //private ColorHtml _strokeColor = "0x0";

        public double X1 { get; set; }

        public double Y1 { get; set; }

        public ColorHtml StrokeColor { get; set; } = "#000000";

        public double X2abs {
            get;
            set {
                field = value;
                _xRel = false;
            }
        }

        public double Y2abs {
            get;
            set {
                field = value;
                _yRel = false;
            }
        }

        public double X2rel {
            get;
            set {
                field = value;
                _xRel = true;
            }
        }

        public double Y2rel {
            get;
            set {
                field = value;
                _yRel = true;
            }
        }


        //public string StrokeColorString {
        //    set {
        //        field = value;
        //    }
        //}

        public double StrokeWidth { get; set; } = 0;

        public override string Serialize(Canvas canvas) {
            StringBuilder sb = new StringBuilder();
            sb.Append($"<line ");
            sb.Append($"x1=`{canvas.CalcX(X1):0.##}` ");
            sb.Append($"y1=`{canvas.CalcY(Y1):0.##}` ");
            sb.Append($"x2=`{canvas.CalcX(_xRel ? X1 + X2rel : X2abs):0.##}` ");
            sb.Append($"y2=`{canvas.CalcY(_yRel ? Y1 + Y2rel : Y2abs):0.##}` ");
            sb.Append($"stroke=`{StrokeColor.Value}` ");
            sb.Append($"stroke-width=`{StrokeWidth}` ");
            sb.Append($"/>");
            return sb.ToString();
        }
    }

    public struct ColorHtml {

        public string Value;

        public static implicit operator ColorHtml(string value) => new ColorHtml { Value = value };
        public static implicit operator ColorHtml(Color value) => new ColorHtml { Value = ColorTranslator.ToHtml(value) };
        public static implicit operator ColorHtml(int value) => new ColorHtml { Value = $"#{value:X6}" };
    }

    public abstract class ElementBase {
        public abstract string Serialize(Canvas canvas);
    }

    public class Canvas {
        public double Xleft { get; set; } = 0;
        public double YTop { get; set; } = 0;
        public double XRight { get; set; } = 100;
        public double YBottom { get; set; } = 100;

        public double Width => Math.Abs(XRight - Xleft);
        public double Height => Math.Abs(YTop - YBottom);

        public double CalcX(double x) => (x - Xleft) * Math.Sign(XRight - Xleft);
        public double CalcY(double y) => (y - YTop) * Math.Sign(YBottom - YTop);
    }
}

