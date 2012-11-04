using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TVRemoteGUI.Windows.Utils {

    public static class GraphicsExtention {

        /// <summary>
        /// Clippings the bounds.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <returns>Rectangle containing the ClipBounds</returns>
        public static Rectangle ClippingBounds(this Graphics graphics) {
            RectangleF bounds = graphics.VisibleClipBounds;
            return new Rectangle {
                X = Convert.ToInt32(bounds.X),
                Y = Convert.ToInt32(bounds.Y),
                Height = Convert.ToInt32(bounds.Height),
                Width = Convert.ToInt32(bounds.Width)
            };
        }

        /// <summary>
        /// Draws a rounded rectangle.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="radius">The radius.</param>
        public static void DrawRoundedRectangle(this Graphics graphics, Pen pen, Rectangle rectangle, int radius) {
            graphics.DrawRoundedRectangle(pen, new RectangleF {
                X = Convert.ToSingle(rectangle.X),
                Y = Convert.ToSingle(rectangle.Y),
                Height = Convert.ToSingle(rectangle.Height),
                Width = Convert.ToSingle(rectangle.Width),
            }, Convert.ToSingle(radius));
        }

        /// <summary>
        /// Fills a rounded rectangle.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="radius">The radius.</param>
        public static void FillRoundedRectangle(this Graphics graphics, Brush brush, Rectangle rectangle, int radius) {
            graphics.FillRoundedRectangle(brush, new RectangleF {
                X = Convert.ToSingle(rectangle.X),
                Y = Convert.ToSingle(rectangle.Y),
                Height = Convert.ToSingle(rectangle.Height),
                Width = Convert.ToSingle(rectangle.Width),
            }, Convert.ToSingle(radius));
        }

        /// <summary>
        /// Draws a rounded rectangle.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="radius">The radius.</param>
        public static void DrawRoundedRectangle(this Graphics graphics, Pen pen, RectangleF rectangle, float radius) {
            GraphicsPath layerOne = graphics.GetRoundedRectangle(rectangle, radius);
            graphics.DrawPath(pen, layerOne);
        }

        /// <summary>
        /// Fills a rounded rectangle.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="radius">The radius.</param>
        public static void FillRoundedRectangle(this Graphics graphics, Brush brush, RectangleF rectangle, float radius) {
            GraphicsPath path = graphics.GetRoundedRectangle(rectangle, radius);
            graphics.FillPath(brush, path);
        }

        /// <summary>
        /// Gets the rounded rectangle.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="rect">The rect.</param>
        /// <param name="raduis">The raduis.</param>
        /// <returns></returns>
        public static GraphicsPath GetRoundedRectangle(this Graphics graphics, RectangleF rect, float raduis) {

            var graphicsPath = new GraphicsPath();

            if (raduis <= 0f) {
                graphicsPath.AddRectangle(rect);
                graphicsPath.CloseFigure();
                return graphicsPath;
            }

            if (raduis >= Math.Min(rect.Width, rect.Height) / 2) {
                return graphics.GetCapsule(rect);
            }

            float diam = raduis * 2f;
            SizeF size = new SizeF(diam, diam);

            RectangleF arc = new RectangleF(rect.Location, size);

            graphicsPath.AddArc(arc, 180, 90);

            arc.X = rect.Right - diam;
            graphicsPath.AddArc(arc, 270, 90);

            arc.Y = rect.Bottom - diam;
            graphicsPath.AddArc(arc, 0, 90);

            arc.X = rect.Left;
            graphicsPath.AddArc(arc, 90, 90);

            graphicsPath.CloseFigure();

            return graphicsPath;

        }

        /// <summary>
        /// Gets the capsule.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="rect">The rect.</param>
        /// <returns>A capsulated rectangle</returns>
        public static GraphicsPath GetCapsule(this Graphics graphics, RectangleF rect) {

            float rad;
            RectangleF arc;
            GraphicsPath gPath = new GraphicsPath();

            try {
                if (rect.Width < rect.Height) {
                    rad = rect.Height;
                    SizeF size = new SizeF(rad, rad);
                    arc = new RectangleF(rect.Location, size);
                    gPath.AddArc(arc, 90, 180);
                    arc.X = rect.Right - rad;
                    gPath.AddArc(arc, 0, 180);
                }
                else if (rect.Width > rect.Height) {

                    rad = rect.Height;
                    SizeF sizeF = new SizeF(rad, rad);
                    arc = new RectangleF(rect.Location, sizeF);
                    gPath.AddArc(arc, 90, 180);
                    arc.X = rect.Right - rad;
                    gPath.AddArc(arc, 270, 180);

                }
                else {
                    gPath.AddEllipse(rect);
                }
            }
            catch {
                gPath.AddEllipse(rect);
            }
            finally {
                gPath.CloseFigure();
            }
            return gPath;

        }

    }
}
