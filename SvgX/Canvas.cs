using System;
using System.Collections.Generic;
using System.Text;

namespace SvgX {
    public class Canvas {
        public double Xleft { get; set; } = 0;
        public double YTop { get; set; } = 0;
        public double XRight { get; set; } = 100;
        public double YBottom { get; set; } = 100;

        public double Width => Math.Abs(XRight - Xleft);
        public double Height => Math.Abs(YTop - YBottom);

        public double PosX(double x) => (x - Xleft) * Math.Sign(XRight - Xleft);
        public double PosY(double y) => (y - YTop) * Math.Sign(YBottom - YTop);
    }
}
