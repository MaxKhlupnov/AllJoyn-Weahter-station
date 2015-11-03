using System;
using System.Collections.Generic;
using System.Linq;
using WinRTXamlToolkit.Tools;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace WinRTXamlToolkit.Controls.Extensions
{
    /// <summary>
    /// Extension methods for DependencyObjects
    /// used for walking the visual tree with
    /// LINQ expressions.
    /// These simplify using VisualTreeHelper to one line calls.
    /// </summary>
    public static class VisualTreeHelperExtensions
    {
        /// <summary>
        /// Gets the window root that is the top level ascendant of the window.Content.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <returns></returns>
        public static UIElement GetRealWindowRoot(Window window = null)
        {
            if (window == null)
            {
                window = Window.Current;
            }

            if (window == null)
            {
                return null;
            }

            var root = window.Content as FrameworkElement;

            if (root != null)
            {
                var ancestors = root.GetAncestors().ToList();

                if (ancestors.Count > 0)
                {
                    root = (FrameworkElement)ancestors[ancestors.Count - 1];
                }
            }

            return root;
        }

        /// <summary>
        /// Gets the bounding rectangle of a given element
        /// relative to a given other element or visual root
        /// if relativeTo is null or not specified.
        /// </summary>
        /// <remarks>
        /// Note that the bounding box is calculated based on the corners of the element relative to itself,
        /// so e.g. a bounding box of a rotated ellipse will be larger than necessary and in general
        /// bounding boxes of elements with transforms applied to them will often be calculated incorrectly.
        /// </remarks>
        /// <param name="dob">The starting element.</param>
        /// <param name="relativeTo">The relative to element.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Element not in visual tree.</exception>
        public static Rect GetBoundingRect(this UIElement dob, UIElement relativeTo = null)
        {
            if (DesignMode.DesignModeEnabled)
            {
                return Rect.Empty;
            }

            if (relativeTo == null)
            {
                relativeTo = Window.Current.Content as FrameworkElement;
            }

            if (relativeTo == null)
            {
                throw new InvalidOperationException("Element not in visual tree.");
            }

            if (dob == relativeTo)
            {
                var fe = relativeTo as FrameworkElement;
                var aw = fe != null ? fe.ActualWidth : 0;
                var ah = fe != null ? fe.ActualHeight : 0;

                return new Rect(0, 0, aw, ah);
            }

            //var ancestors = dob.GetAncestors().ToArray();

            //if (!ancestors.Contains(relativeTo))
            //{
            //    throw new InvalidOperationException("Element not in visual tree.");
            //}


            var fe2 = dob as FrameworkElement;
            var aw2 = fe2 != null ? fe2.ActualWidth : 0;
            var ah2 = fe2 != null ? fe2.ActualHeight : 0;

            var topLeft =
                dob
                    .TransformToVisual(relativeTo)
                    .TransformPoint(new Point());
            var topRight =
                dob
                    .TransformToVisual(relativeTo)
                    .TransformPoint(
                        new Point(
                            aw2,
                            0));
            var bottomLeft =
                dob
                    .TransformToVisual(relativeTo)
                    .TransformPoint(
                        new Point(
                            0,
                            ah2));
            var bottomRight =
                dob
                    .TransformToVisual(relativeTo)
                    .TransformPoint(
                        new Point(
                            aw2,
                            ah2));

            var minX = new[] { topLeft.X, topRight.X, bottomLeft.X, bottomRight.X }.Min();
            var maxX = new[] { topLeft.X, topRight.X, bottomLeft.X, bottomRight.X }.Max();
            var minY = new[] { topLeft.Y, topRight.Y, bottomLeft.Y, bottomRight.Y }.Min();
            var maxY = new[] { topLeft.Y, topRight.Y, bottomLeft.Y, bottomRight.Y }.Max();

            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }
    }
}
