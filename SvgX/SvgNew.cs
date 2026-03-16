using System;
using System.Collections.Generic;
using System.Text;

namespace SvgX {

    public class SvgNew {

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
}
