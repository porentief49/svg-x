using System;
using System.Collections.Generic;
using System.Text;

namespace SvgX {

    public class Canvas {

        public double Left { get; set; } = 0;
        public double Top { get; set; } = 0;
        public double Right { get; set; } = 100;
        public double Bottom { get; set; } = 100;

        public double Width => Math.Abs(Right - Left);
        public double Height => Math.Abs(Top - Bottom);

        public double PosX(double x) => (x - Left) * Math.Sign(Right - Left);
        public double PosY(double y) => (y - Top) * Math.Sign(Bottom - Top);
    }
}
