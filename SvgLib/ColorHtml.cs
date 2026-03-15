using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace SvgLib {
    public struct ColorHtml {

        public string Value;

        public static implicit operator ColorHtml(string value) => new ColorHtml { Value = value };
        public static implicit operator ColorHtml(Color value) => new ColorHtml { Value = ColorTranslator.ToHtml(value) };
        public static implicit operator ColorHtml(int value) => new ColorHtml { Value = $"#{value:X6}" };
    }
}
