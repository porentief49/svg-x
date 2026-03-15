using System;
using System.Collections.Generic;
using System.Text;

namespace SvgLib {
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
}
