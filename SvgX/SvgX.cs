using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml.Linq;

namespace SvgX {

    public partial class SvgX {

        private List<ElementBase> _elements = [];
        //private double _xLeft = 0;
        //private double _xRight = 0;
        //private double _yTop = 0;
        //private double _yBottom = 0;
        private Canvas _canvas = new Canvas();

        public void AddElement(ElementBase element) {
            _elements.Add(element);
        }

        public string Serialize() {
            string body = string.Join(Environment.NewLine, _elements.Select(e => $"    {e.Serialize(_canvas)}"));
            return ($"{body}").Replace('`', '"');
        }

        public void ExportToPdf(string path) {


        }
    }

    public class Line : ElementBase {

        private bool _xRel = false;
        private bool _yRel = false;

        public double X1 { get; set; }

        public double Y1 { get; set; }

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

        public Color StrokeColor { get; set; } = Color.Black;
        public double StrokeWidth { get; set; } = 0;

        public override string Serialize(Canvas canvas) {
            StringBuilder sb = new StringBuilder();
            sb.Append($"<line ");
            sb.Append($"x1=`{canvas.GetX(X1):0.##}` ");
            sb.Append($"y1=`{canvas.GetY(Y1):0.##}` ");
            sb.Append($"x2=`{canvas.GetX(_xRel ? X2rel : X2abs):0.##}` ");
            sb.Append($"y2=`{canvas.GetY(_yRel ? Y1 : Y2abs):0.##}` ");
            sb.Append($"stroke=`{StrokeColor}` ");
            sb.Append($"stroke-width=`{StrokeWidth}` ");
            sb.Append($"/>");
            return sb.ToString(); 
        }
    }

    public abstract class ElementBase {
        public abstract string Serialize(Canvas canvas);
    }

    public class Canvas {
        public double Xleft { get; set; } = 0;
        public double YTop { get; set; } = 0;
        public double XRight { get; set; } = 0;
        public double YBottom { get; set; } = 0;

        public double GetX(double x) => (x - Xleft) * Math.Sign(XRight - Xleft);
        public double GetY(double y) => (y - YTop) * Math.Sign(YTop - YBottom);
    }
}

