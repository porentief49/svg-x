using System.Drawing;
using System.Drawing.Text;
using System.Text;
using SkiaSharp;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System;

namespace SvgLib {

    public enum HorizontalTextAnchor {
        Start,
        Middle,
        End
    }

    public enum VerticalTextAnchor {
        NominalHeight,
        Ascent,
        Gravity,
        Midway,
        Baseline,
        Descent
    }

    public class Svg {

        private readonly StringBuilder _sb;
        private int _xLeft;
        private int _yTop;
        private readonly int _width;
        private readonly int _height;
        private Dictionary<string, FontMetrics> _fontMetrics;

        public static bool DarkMode { get; set; } = true;

        public Svg(int xLeft, int yTop, int xRight, int yBottom) {
            _sb = new();
            _xLeft = xLeft;
            _yTop = yTop;
            _width = xRight - xLeft;
            _height = yBottom - yTop;
            _fontMetrics = new();
            _fontMetrics.Add("Exo 2", new FontMetrics(.717, -.205, .3152));
        }

        public FontMetrics GetFontMetrics(string fontName) {
            if (!_fontMetrics.TryGetValue(fontName, out FontMetrics? value)) throw new ArgumentException($"Font '{fontName}' is not defined in FontMetrics. Run TypographyAnalyzer to get these.");
            return value;
        }

        public void Export(string filePath) => File.WriteAllText(filePath, Serialize());

        //public void ExportToPdf(string filePath) {
        //    using var stream = File.OpenWrite(filePath);
        //    using var svg = new SkiaSharp.Extended.Svg.SKSvg();
        //    svg.FromSvg(Serialize());
        //    if (svg.Picture == null) throw new InvalidOperationException("Failed to create SVG picture");
        //    using var pdfDocument = SKDocument.CreatePdf(stream);
        //    using var pdfCanvas = pdfDocument.BeginPage(_width, _height);
        //    pdfCanvas.DrawPicture(svg.Picture);
        //    pdfDocument.EndPage();
        //    pdfDocument.Close();
        //}

        //public void ExportToPdf(string filePath) {
        //    using var stream = File.OpenWrite(filePath);
        //    var svgDocument = global::Svg.SvgDocument.FromSvg<global::Svg.SvgDocument>(Serialize());
        //    if (svgDocument == null) throw new InvalidOperationException("Failed to create SVG document");

        //    using var pdfDocument = SKDocument.CreatePdf(stream);
        //    using var pdfCanvas = pdfDocument.BeginPage(_width, _height);
        //    //svgDocument.Draw(pdfCanvas);

        //    // Render the SVG to the canvas
        //    //using var skPicture = svgDocument.ToSKPicture();
        //    //pdfCanvas.DrawPicture(skPicture);

        //    // Use the Svg.Skia extension method to render directly to canvas
        //    using var skSvg = new global::Svg.Skia.SKSvg();
        //    skSvg.Load(svgDocument);
        //    if (skSvg.Picture != null) {
        //        pdfCanvas.DrawPicture(skSvg.Picture);
        //    }

        //    pdfDocument.EndPage();
        //    pdfDocument.Close();
        //}

        public void ExportToPdf(string filePath) {

            float widthInches = 210 / 25.4f; // 8.5f;
            float heightInches = 297 / 25.4f; // 11f;

            // Define your target DPI
            float targetDpi = 25.4f;
            float defaultDpi = 72f;

            float pdfWidth = widthInches * defaultDpi;
            float pdfHeight = heightInches * defaultDpi;

            using var stream = File.OpenWrite(filePath);

            // Create a memory stream with the SVG string
            string svgString = Serialize();
            using var svgStream = new MemoryStream(Encoding.UTF8.GetBytes(svgString));

            // Load SVG from stream using Svg.Skia
            using var skSvg = new global::Svg.Skia.SKSvg();
            skSvg.Load(svgStream);

            if (skSvg.Picture == null) {
                throw new InvalidOperationException("Failed to create SVG picture");
            }

            using var pdfDocument = SKDocument.CreatePdf(stream);
            //using var pdfCanvas = pdfDocument.BeginPage(_width, _height);
            using var pdfCanvas = pdfDocument.BeginPage(pdfWidth, pdfHeight);
            float scale = defaultDpi / targetDpi;
            pdfCanvas.Scale(scale);
            pdfCanvas.DrawPicture(skSvg.Picture);
            pdfDocument.EndPage();
            pdfDocument.Close();
        }

        public string Serialize() => $"<svg width=\"{_width}\" height=\"{_height}\" xmlns=\"http://www.w3.org/2000/svg\">\r\n{_sb.ToString().Replace('`', '"')}</svg>";

        public static bool FontExists(string fontName) {
            //using var typeface = SKTypeface.FromFamilyName(fontName);
            //return typeface != null && typeface.FamilyName.Equals(fontName, StringComparison.InvariantCultureIgnoreCase);
            return true;
        }

        public static double TextWidth(string text, double fontSize, string font) {
            using var typeface = SKTypeface.FromFamilyName(font);
            using var skFont = new SKFont(typeface, (float)fontSize);
            return skFont.MeasureText(text);
        }

        public void DrawLine(double x1, double y1, double x2, double y2, string strokeColor, double strokeWidth) {
            _sb.AppendLine($"    <line x1=`{x1 - _xLeft:0.##}` y1=`{y1 - _yTop:0.##}` x2=`{x2 - _xLeft:0.##}` y2=`{y2 - _yTop:0.##}` stroke=`{strokeColor}` stroke-width=`{strokeWidth}`/>");
        }

        public void DrawLine(double x1, double y1, double x2, double y2, Color strokeColor, double strokeWidth) {
            DrawLine(x1, y1, x2, y2, ColorTranslator.ToHtml(strokeColor), strokeWidth);
        }

        public void DrawHorizontalRuler(double y, string strokeColor, double strokeWidth) {
            DrawLine(_xLeft, y, _xLeft + _width, y, strokeColor, strokeWidth);
        }

        public void DrawHorizontalRuler(double y, Color strokeColor, double strokeWidth) {
            DrawHorizontalRuler(y, ColorTranslator.ToHtml(strokeColor), strokeWidth);
        }

        public void DrawVerticalRuler(double x, string strokeColor, double strokeWidth) {
            DrawLine(x, _yTop, x, _yTop + _height, strokeColor, strokeWidth);
        }

        public void DrawVerticalRuler(double x, Color strokeColor, double strokeWidth) {
            DrawVerticalRuler(x, ColorTranslator.ToHtml(strokeColor), strokeWidth);
        }

        public void FillBackground(string fillColor) {
            _sb.AppendLine($"    <rect x=`{0 - _xLeft:0.##}` y=`{0 - _yTop:0.##}` width=`{_width:0.##}` height=`{_height:0.##}` fill=`{fillColor}`/>");
        }

        public void FillBackground(Color fillColor) {
            FillBackground(ColorTranslator.ToHtml(fillColor));
        }

        public void FillRectangle(double xFrom, double yFrom, double xTo, double yTo, string fillColor) {
            _sb.AppendLine($"    <rect x=`{xFrom - _xLeft:0.##}` y=`{yFrom - _yTop:0.##}` width=`{xTo - xFrom:0.##}` height=`{yTo - yFrom:0.##}` fill=`{fillColor}`/>");
        }

        public void FillRectangle(double xFrom, double yFrom, double xTo, double yTo, Color fillColor) {
            FillRectangle(xFrom, yFrom, xTo, yTo, ColorTranslator.ToHtml(fillColor));
        }

        public void DrawRectangle(double xFrom, double yFrom, double xTo, double yTo, string strokeColor, double strokeWidth) {
            _sb.AppendLine($"    <rect x=`{xFrom - _xLeft:0.##}` y=`{yFrom - _yTop:0.##}` width=`{xTo - xFrom:0.##}` height=`{yTo - yFrom:0.##}` stroke=`{strokeColor}` stroke-width=`{strokeWidth}` fill=`none`/>");
        }

        public void DrawRectangle(double xFrom, double yFrom, double xTo, double yTo, Color strokeColor, double strokeWidth) {
            DrawRectangle(xFrom, yFrom, xTo, yTo, ColorTranslator.ToHtml(strokeColor), strokeWidth);
        }

        public void DrawRectangleRounded(double xFrom, double yFrom, double xTo, double yTo, double radius, string strokeColor, double strokeWidth) {
            _sb.AppendLine($"    <rect x=`{xFrom - _xLeft:0.##}` y=`{yFrom - _yTop:0.##}` width=`{xTo - xFrom:0.##}` height=`{yTo - yFrom:0.##}` rx=`{radius:0.##}` ry=`{radius:0.##}` stroke=`{strokeColor}` stroke-width=`{strokeWidth}` fill=`none`/>");
        }

        public void DrawRectangleRounded(double xFrom, double yFrom, double xTo, double yTo, double radius, Color strokeColor, double strokeWidth) {
            DrawRectangleRounded(xFrom, yFrom, xTo, yTo, radius, ColorTranslator.ToHtml(strokeColor), strokeWidth);
        }

        public void FillRectangleRounded(double xFrom, double yFrom, double xTo, double yTo, double radius, string fillColor) {
            _sb.AppendLine($"    <rect x=`{xFrom - _xLeft:0.##}` y=`{yFrom - _yTop:0.##}` width=`{xTo - xFrom:0.##}` height=`{yTo - yFrom:0.##}` rx=`{radius:0.##}` ry=`{radius:0.##}` fill=`{fillColor}`/>");
        }

        public void FillRectangleRounded(double xFrom, double yFrom, double xTo, double yTo, double radius, Color fillColor) {
            FillRectangleRounded(xFrom, yFrom, xTo, yTo, radius, ColorTranslator.ToHtml(fillColor));
        }

        public void DrawPolyline(double[] xs, double[] ys, string strokeColor, double strokeWidth) {
            _sb.AppendLine($"    <polyline points=`{string.Join(" ", xs.Select((x, i) => $"{x - _xLeft:0.##},{ys[i] - _yTop:0.##}"))}` stroke=`{strokeColor}` stroke-width=`{strokeWidth}` fill=`none`/> ");
        }

        public void DrawPolyline(double[] xs, double[] ys, Color strokeColor, double strokeWidth) {
            DrawPolyline(xs, ys, ColorTranslator.ToHtml(strokeColor), strokeWidth);
        }

        public void FillPolyline(double[] xs, double[] ys, string fillColor) {
            _sb.AppendLine($"    <polygon points=`{string.Join(" ", xs.Select((x, i) => $"{x - _xLeft:0.##},{ys[i] - _yTop:0.##}"))}` fill=`{fillColor}`/> ");
        }

        public void FillPolyline(double[] xs, double[] ys, Color fillColor) {
            FillPolyline(xs, ys, ColorTranslator.ToHtml(fillColor));
        }

        public void DrawText(double x, double y, string text, string color, double fontSize, string font, double? rotation = null,
            HorizontalTextAnchor xAnchor = HorizontalTextAnchor.Start, VerticalTextAnchor yAnchor = VerticalTextAnchor.Baseline) {
            DrawTextCore(x, y, text, color, fontSize, font, rotation, xAnchor, yAnchor);
        }

        public void DrawText(double x, double y, string text, Color color, double fontSize, string font, double? rotation = null,
            HorizontalTextAnchor xAnchor = HorizontalTextAnchor.Start, VerticalTextAnchor yAnchor = VerticalTextAnchor.Baseline) {
            DrawTextCore(x, y, text, ColorTranslator.ToHtml(color), fontSize, font, rotation, xAnchor, yAnchor);
        }

        private void DrawTextCore(double x, double y, string text, string color, double fontSize, string font, double? rotation,
            HorizontalTextAnchor xAnchor, VerticalTextAnchor yAnchor) {
            double yRel = y + CalcOffsetText(font, fontSize, yAnchor);
            _sb.AppendLine($"    <text x=`{x - _xLeft:0.##}` y=`{yRel - _yTop:0.##}`{(rotation.HasValue ? $" transform=`rotate({rotation:0.##} {x - _xLeft:0.##}, {yRel - _yTop:0.##})`" : string.Empty)} text-anchor=`{xAnchor.ToString().ToLower()}` fill=`{color}` font-size=`{fontSize:0.##}` font-family=`'{font}'` xml:space=`preserve`>{text}</text>");
        }

        public void DrawTextEvenOdd(double x, double y, string text, string splitter, string colorEven, double fontSizeEven, string fontEven, string colorOdd,
            double fontSizeOdd, string fontOdd, double? rotation = null, HorizontalTextAnchor xAnchor = HorizontalTextAnchor.Start,
            VerticalTextAnchor yAnchor = VerticalTextAnchor.Baseline) {
            DrawTextEvenOddCore(x, y, text, splitter, colorEven, fontSizeEven, fontEven, colorOdd, fontSizeOdd, fontOdd, rotation, xAnchor, yAnchor);
        }

        public void DrawTextEvenOdd(double x, double y, string text, string splitter, Color colorEven, double fontSizeEven, string fontEven, Color colorOdd,
            double fontSizeOdd, string fontOdd, double? rotation = null, HorizontalTextAnchor xAnchor = HorizontalTextAnchor.Start,
            VerticalTextAnchor yAnchor = VerticalTextAnchor.Baseline) {
            DrawTextEvenOddCore(x, y, text, splitter, ColorTranslator.ToHtml(colorEven), fontSizeEven, fontEven, ColorTranslator.ToHtml(colorOdd), fontSizeOdd, fontOdd, rotation, xAnchor, yAnchor);
        }

        private void DrawTextEvenOddCore(double x, double y, string text, string splitter, string colorEven, double fontSizeEven, string fontEven,
            string colorOdd, double fontSizeOdd, string fontOdd, double? rotation, HorizontalTextAnchor xAnchor, VerticalTextAnchor yAnchor) {
            string[] texts = text.Split(splitter);
            double yRel = y + CalcOffsetText(fontEven, fontSizeEven, yAnchor);
            _sb.Append($"    <text x=`{x - _xLeft:0.##}` y=`{yRel - _yTop:0.##}`{(rotation.HasValue ? $" transform=`rotate({rotation:0.##} {x - _xLeft:0.##}, {yRel - _yTop:0.##})`" : string.Empty)} text-anchor=`{xAnchor.ToString().ToLower()}` xml:space=`preserve`>");
            for (int i = 0; i < texts.Length; i++) {
                bool even = (i % 2) == 0;
                _sb.Append($"<tspan fill=`{(even ? colorEven : colorOdd)}` font-family=`'{(even ? fontEven : fontOdd)}'` font-size=`{(even ? fontSizeEven : fontSizeOdd):0.##}`>{texts[i]}</tspan>");
            }
            _sb.AppendLine($"</text>");
        }

        public double CalcOffsetText(string font, double fontSize, VerticalTextAnchor yAnchor) => fontSize * yAnchor switch {
            VerticalTextAnchor.NominalHeight => GetFontMetrics(font).NominalHeight,
            VerticalTextAnchor.Ascent => GetFontMetrics(font).Ascent,
            VerticalTextAnchor.Gravity => GetFontMetrics(font).GravityCenter,
            VerticalTextAnchor.Midway => GetFontMetrics(font).Midway,
            VerticalTextAnchor.Baseline => 0,
            VerticalTextAnchor.Descent => GetFontMetrics(font).Descent,
            _ => throw new ArgumentException($"Unsupported VerticalTextAnchor: {yAnchor}")
        };

        public void DrawArrowgon(double x, double y, double width, double[] leftStops, double[] leftInclinations, double[] rightStops,
            double[] rightInclinations, string strokeColor, double strokeWidth) {
            ArrowgonSub(x, y, width, leftStops, leftInclinations, rightStops, rightInclinations, out List<double> xx, out List<double> yy);
            DrawPolyline([.. xx], [.. yy], strokeColor, strokeWidth);
        }

        public void DrawArrowgon(double x, double y, double width, double[] leftStops, double[] leftInclinations, double[] rightStops,
            double[] rightInclinations, Color strokeColor, double strokeWidth) {
            DrawArrowgon(x, y, width, leftStops, leftInclinations, rightStops, rightInclinations, ColorTranslator.ToHtml(strokeColor), strokeWidth);
        }

        public void FillArrowgon(double x, double y, double width, double[] leftStops, double[] leftInclinations, double[] rightStops,
            double[] rightInclinations, string fillColor) {
            ArrowgonSub(x, y, width, leftStops, leftInclinations, rightStops, rightInclinations, out List<double> xx, out List<double> yy);
            FillPolyline([.. xx], [.. yy], fillColor);
        }

        public void FillArrowgon(double x, double y, double width, double[] leftStops, double[] leftInclinations, double[] rightStops,
            double[] rightInclinations, Color fillColor) {
            FillArrowgon(x, y, width, leftStops, leftInclinations, rightStops, rightInclinations, ColorTranslator.ToHtml(fillColor));
        }

        public void DrawEllipse(double cx, double cy, double rx, double ry, string strokeColor, double strokeWidth) {
            _sb.AppendLine($"    <ellipse cx=`{cx:0.##}` cy=`{cy:0.##}` rx=`{rx:0.##}` ry=`{ry:0.##}` stroke=`{strokeColor}` stroke-width=`{strokeWidth}` fill=`none`/>");
        }

        public void DrawEllipse(double cx, double cy, double rx, double ry, Color strokeColor, double strokeWidth) {
            DrawEllipse(cx, cy, rx, ry, ColorTranslator.ToHtml(strokeColor), strokeWidth);
        }

        public void FillEllipse(double cx, double cy, double rx, double ry, string fillColor) {
            _sb.AppendLine($"    <ellipse cx=`{cx:0.##}` cy=`{cy:0.##}` rx=`{rx:0.##}` ry=`{ry:0.##}` fill=`{fillColor}`/>");
        }

        public void FillEllipse(double cx, double cy, double rx, double ry, Color fillColor) {
            FillEllipse(cx, cy, rx, ry, ColorTranslator.ToHtml(fillColor));
        }

        private static void ArrowgonSub(double x, double y, double width, double[] leftStops, double[] leftInclinations, double[] rightStops,
            double[] rightInclinations, out List<double> xx, out List<double> yy) {
            List<double> leftX = new([0]);
            List<double> leftY = new([0]);
            for (int i = 0; i < leftInclinations.Length; i++) {
                leftX.Add(leftX.Last() + leftInclinations[i] * (leftStops[i] - leftY.Last()));
                leftY.Add(leftStops[i]);
            }
            List<double> rightX = new([0]);
            List<double> rightY = new([0]);
            for (int i = 0; i < rightInclinations.Length; i++) {
                rightX.Add(rightX.Last() + rightInclinations[i] * (rightStops[i] - rightY.Last()));
                rightY.Add(rightStops[i]);
            }
            xx = new([x]);
            yy = new([y]);
            xx.AddRange(rightX.Select(r => r + x + width));
            yy.AddRange(rightY.Select(r => r + y));
            leftX.Reverse();
            leftY.Reverse();
            xx.AddRange(leftX.Select(l => l + x));
            yy.AddRange(leftY.Select(l => l + y));
        }

        public static Color ColorHsl(double hue, double saturation, double luminosityDarkLightMode) {
            hue = hue % 360; // Normalize hue to [0, 360[
            if (hue < 0) hue += 360;
            double saturationClamped = Clamp(saturation, 0, 1);
            double luminosityClamped;
            if (!DarkMode) luminosityClamped = Clamp(1 - luminosityDarkLightMode, 0, 1);
            else luminosityClamped = Clamp(luminosityDarkLightMode, 0, 1);
            double c = (1.0 - Math.Abs(2.0 * luminosityClamped - 1.0)) * saturationClamped;
            double x = c * (1.0 - Math.Abs((hue / 60.0) % 2.0 - 1.0));
            double m = luminosityClamped - c / 2.0;
            double r1;
            double g1;
            double b1;
            (r1, g1, b1) = hue switch {
                < 60 => (c, x, 0d),
                < 120 => (x, c, 0d),
                < 180 => (0d, c, x),
                < 240 => (0d, x, c),
                < 300 => (x, 0d, c),
                _ => (c, 0d, x),
            };
            int red = ToByte(r1 + m);
            int green = ToByte(g1 + m);
            int blue = ToByte(b1 + m);
            return Color.FromArgb(255, red, green, blue);
        }

        public static Color ColorOklab(double hueDegrees, double saturation, double luminosityDarkLightMode) {
            // Adjust these to change the "feel" of your items
            // lightness: 0.0 (black) to 1.0 (white). 0.75 is good for dark backgrounds.
            // contrastBackground: 0.0 (black in dark mode) to 1.0 (white in dark mode). 0.75 is good for dark backgrounds.
            // chroma: 0.0 (grey) to ~0.4 (very vivid).

            double hueRadians = (hueDegrees + 45) * (Math.PI / 180.0); // Oklab hue is shifted by 45 degrees
            //double lightness = Math.Pow((DarkMode ?contrastBackground :  1 - contrastBackground)/.8,1.2);
            double lightness = (DarkMode ? luminosityDarkLightMode : 1 - luminosityDarkLightMode) / .8;
            double chroma = saturation * .22;

            // 1. Convert Oklab (L, a, b) to Linear LMS
            double ll = lightness + 0.3963377774 * (chroma * Math.Cos(hueRadians)) + 0.2158037573 * (chroma * Math.Sin(hueRadians));
            double mm = lightness - 0.1055613458 * (chroma * Math.Cos(hueRadians)) - 0.0638541728 * (chroma * Math.Sin(hueRadians));
            double ss = lightness - 0.0894841775 * (chroma * Math.Cos(hueRadians)) - 1.2914855480 * (chroma * Math.Sin(hueRadians));

            // 2. Cube the LMS values to get to linear RGB space
            double l = ll * ll * ll;
            double m = mm * mm * mm;
            double s = ss * ss * ss;

            // 3. Transform LMS to Linear RGB
            double rLin = +4.0767416621 * l - 3.3077115913 * m + 0.2309699292 * s;
            double gLin = -1.2684380046 * l + 2.6097574011 * m - 0.3413193965 * s;
            double bLin = -0.0041960863 * l - 0.7034186147 * m + 1.7076147010 * s;

            // 4. Standard Gamma Correction (sRGB)
            return Color.FromArgb(ClampAndGamma(rLin), ClampAndGamma(gLin), ClampAndGamma(bLin));

            static byte ClampAndGamma(double linear) {
                linear = Math.Max(0, Math.Min(1, linear));
                double srgb = linear <= 0.0031308 ? 12.92 * linear : 1.055 * Math.Pow(linear, 1.0 / 2.4) - 0.055;
                return (byte)Math.Round(srgb * 255);
            }
        }

        public static Color ColorGrey(double contrastBackground) {
            double luminositylampoed = Clamp(DarkMode ? contrastBackground : 1 - contrastBackground, 0, 1);
            int grey = ToByte(luminositylampoed);
            return Color.FromArgb(grey, grey, grey);
        }

        private static int ToByte(double value) => (int)Math.Round(Clamp(value * 255, 0, 255));

        private static double Clamp(double value, double min, double max) => (Math.Min(Math.Max(value, min), max));

        public static Color Blend(Color foreground, Color background, double alpha) {
            int r = (int)Math.Round((foreground.R * alpha) + (background.R * (1 - alpha)));
            int g = (int)Math.Round((foreground.G * alpha) + (background.G * (1 - alpha)));
            int b = (int)Math.Round((foreground.B * alpha) + (background.B * (1 - alpha)));
            return Color.FromArgb(255, r, g, b);
        }
        public static Color ColorOkhsl(double hue, double saturation, double luminosityDarkLightMode) {
            RGB rgb = ColorConverter.OkhslToRgb(hue, Math.Pow(saturation, 1.25), DarkMode ? luminosityDarkLightMode : 1 - luminosityDarkLightMode);
            return Color.FromArgb(255, rgb.R, rgb.G, rgb.B);
        }
    }


    public class FontMetrics {

        /// <summary>
        /// The vertical position of the most positive estention above the baseline.
        /// </summary>
        public double Ascent { get; private set; }

        /// <summary>
        /// The vertical position of the most negative estention below the baseline. Value is negative!
        /// </summary>
        public double Descent { get; private set; }

        /// <summary>
        /// The actual height of the glyphs (ascent - descent).
        /// </summary>
        public double ActualHeight { get; private set; }

        /// <summary>
        /// The nominal height of the glyphs (roughly but not exactly ActualHeight).
        /// </summary>
        public double NominalHeight { get; private set; }

        /// <summary>
        /// The vertical position of the midway (half between ascent & descent) relative to the baseline.
        /// </summary>
        public double Midway { get; private set; }

        /// <summary>
        /// The vertical position of the gravity center relative to the baseline.
        /// </summary>
        public double GravityCenter { get; private set; }

        //public FontMetrics(string fontBaseName, double size) {
        //    switch (fontBaseName) {
        //        case "Exo 2": // characterized 2026-02-28 rAiner Gruber
        //            Ascent = .717 * size;
        //            Descent = -.205 * size;
        //            ActualHeight = Ascent - Descent;
        //            NominalHeight = size;
        //            //Midway = .256 * size;
        //            Midway = (Ascent + Descent) / 2;
        //            GravityCenter = .3152 * size;
        //            break;
        //        default:
        //            throw new ArgumentException($"Font '{fontBaseName}' is not defined in FontMetrics. Run TypographyAnalyzer to get these.");
        //    }
        //}

        public FontMetrics(double ascent, double descent, double gravityCenter) {
            Ascent = ascent;
            Descent = descent;
            ActualHeight = ascent - descent;
            NominalHeight = 1;
            Midway = (ascent + descent) / 2;
            GravityCenter = gravityCenter;
        }
    }


    public struct RGB { public int R, G, B; }

    /// <summary>
    /// This class provides a conversion from OKHSL (a perceptually uniform color space) to RGB. Work based
    /// on the algorithm described by Björn Ottosson in his article "OKHSL: A perceptually uniform color space
    /// for saturation and hue" (https://bottosson.github.io/posts/okhsl/). This is a simplified version that
    /// focuses on the needs of our UI colors, and may not cover all edge cases or be fully optimized. The
    /// main goal is to provide a more visually consistent color scheme across different hues and lightness
    /// levels, especially when using the same saturation value.
    ///
    /// Hue Warping is described here: https://bottosson.github.io/posts/colorpicker/.
    /// </summary>
    public static class ColorConverter {

        public static RGB OkhslToRgb(double h, double s, double l) {
            if (l <= 0) return new RGB { R = 0, G = 0, B = 0 };
            if (l >= 1) return new RGB { R = 255, G = 255, B = 255 };

            // Convert Hue to Radians
            //double hRad = (h + 20) * Math.PI / 180.0; // OKhsl hue is off by 20 degrees
            //double hRad = (h) * Math.PI / 180.0; // OKhsl hue is off by 20 degrees
            double hRad = RemapHue(h) * Math.PI / 180.0; // OKhsl hue is off by 20 degrees
            double a_ = Math.Cos(hRad);
            double b_ = Math.Sin(hRad);

            // Pre-defined constants for Okhsl geometry
            // These approximate the 'maximum' saturation for a given lightness
            double ll = l;
            double cc = s * GetMaxChroma(ll, a_, b_);

            double a = cc * a_;
            double b = cc * b_;

            // Step 1: Oklab to LMS
            double l_ = ll + 0.3963377774 * a + 0.2158037573 * b;
            double m_ = ll - 0.1055613458 * a - 0.0638541728 * b;
            double s_ = ll - 0.0894841775 * a - 1.2914855480 * b;

            // Step 2: LMS to Linear RGB
            double l3 = l_ * l_ * l_;
            double m3 = m_ * m_ * m_;
            double s3 = s_ * s_ * s_;

            double rLinear = +4.0767416621 * l3 - 3.3077115913 * m3 + 0.2309699292 * s3;
            double gLinear = -1.2684380046 * l3 + 2.6097574011 * m3 - 0.3413193965 * s3;
            double bLinear = -0.0041960863 * l3 - 0.7034186147 * m3 + 1.7076147010 * s3;

            // Step 3: Linear RGB to sRGB (Gamma correction)
            return new RGB {
                R = ToSRGB(rLinear),
                G = ToSRGB(gLinear),
                B = ToSRGB(bLinear)
            };
        }

        private static double RemapHue(double userHue) {
            // Normalized to 0-360
            userHue = (userHue % 360 + 360) % 360;

            // Define the "User" milestones vs "Actual Oklab" milestones
            double[] input = {
                0, // target red
                60, // target yellow
                120, // target green
                180, // target cyan
                240, // target blue
                300,  // target magenta
                360 };
            double[] output = {
                29, // actual red
                90, // actual yellow
                142, // actual green
                190, // actual cyan
                264, // actual blue
                325, // actual magenta
                389 }; // 389 = 29 + 360

            for (int i = 0; i < input.Length - 1; i++) {
                if (userHue >= input[i] && userHue < input[i + 1]) {
                    // Linear interpolation between points
                    double t = (userHue - input[i]) / (input[i + 1] - input[i]);
                    return (output[i] + t * (output[i + 1] - output[i])) % 360;
                }
            }
            return userHue;
        }

        private static int ToSRGB(double linear) {
            double clipped = Math.Max(0, Math.Min(1, linear));
            double srgb = clipped <= 0.0031308
                ? 12.92 * clipped
                : 1.055 * Math.Pow(clipped, 1.0 / 2.4) - 0.055;
            return (int)Math.Round(srgb * 255);
        }

        // Simplification for the 'Max Chroma' to keep UI colors within gamut
        private static double GetMaxChroma(double l, double a, double b) {
            // This is a simplified boundary; a true implementation involves 
            // solving for the gamut intersection, but 0.4 is a safe 'vivid' limit.
            return 0.4 * (1.0 - Math.Abs(2.0 * l - 1.0));
        }
    }
}
