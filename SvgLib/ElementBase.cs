using System;
using System.Collections.Generic;
using System.Text;

namespace SvgLib {
    public abstract class ElementBase {
        public abstract string Serialize(Canvas canvas);
    }
}
