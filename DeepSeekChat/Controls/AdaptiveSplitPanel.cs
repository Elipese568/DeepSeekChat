using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;

namespace DeepSeekChat.Controls
{
    public partial class AdaptiveSplitPanel : Panel
    {
        #region Dependency Properties

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(AdaptiveSplitPanel),
                new PropertyMetadata(Orientation.Horizontal, OnLayoutPropertyChanged));

        public static readonly DependencyProperty FirstControlSizeProperty =
            DependencyProperty.Register(nameof(FirstControlSize), typeof(GridLength), typeof(AdaptiveSplitPanel),
                new PropertyMetadata(new GridLength(1, GridUnitType.Star), OnLayoutPropertyChanged));

        public static readonly DependencyProperty SecondControlSizeProperty =
            DependencyProperty.Register(nameof(SecondControlSize), typeof(GridLength), typeof(AdaptiveSplitPanel),
                new PropertyMetadata(new GridLength(1, GridUnitType.Star), OnLayoutPropertyChanged));

        public static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register(nameof(Padding), typeof(Thickness), typeof(AdaptiveSplitPanel),
                new PropertyMetadata(new Thickness(0), OnLayoutPropertyChanged));

        #endregion

        #region Public Properties

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public GridLength FirstControlSize
        {
            get => (GridLength)GetValue(FirstControlSizeProperty);
            set => SetValue(FirstControlSizeProperty, value);
        }

        public GridLength SecondControlSize
        {
            get => (GridLength)GetValue(SecondControlSizeProperty);
            set => SetValue(SecondControlSizeProperty, value);
        }

        public Thickness Padding
        {
            get => (Thickness)GetValue(PaddingProperty);
            set => SetValue(PaddingProperty, value);
        }

        #endregion

        public AdaptiveSplitPanel()
        {
            this.Loaded += (s, e) => AttachVisibilityHandlers();
        }

        private readonly Dictionary<UIElement, long> _callbackTokens = new();

        #region Helper Functions
        private void AttachVisibilityHandlers()
        {
            foreach (var child in this.Children)
            {
                if (child is UIElement el && !_callbackTokens.ContainsKey(el))
                {
                    long token = el.RegisterPropertyChangedCallback(UIElement.VisibilityProperty, (_, __) =>
                    {
                        this.InvalidateMeasure();
                        this.InvalidateArrange();
                    });
                    _callbackTokens[el] = token;
                }
            }
        }

        private static void OnLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AdaptiveSplitPanel panel)
            {
                panel.InvalidateMeasure();
                panel.InvalidateArrange();
            }
        }

        private bool IsVisible(UIElement element) =>
            element != null && element.Visibility != Visibility.Collapsed;

        private double CalculateLength(GridLength length, double total, GridLength otherLength)
        {
            return length.GridUnitType switch
            {
                GridUnitType.Pixel => length.Value,
                GridUnitType.Star =>
                    (length.Value + otherLength.Value) == 0 ? total / 2 : total * (length.Value / (length.Value + otherLength.Value)),
                GridUnitType.Auto => total / 2,
                _ => total / 2,
            };
        }

        #endregion

        protected override Size MeasureOverride(Size availableSize)
        {
            var firstChild = Children.ElementAtOrDefault(0);
            var secondChild = Children.ElementAtOrDefault(1);

            bool firstVisible = IsVisible(firstChild);
            bool secondVisible = IsVisible(secondChild);

            var padding = this.Padding;
            var paddingSize = new Size(padding.Left + padding.Right, padding.Top + padding.Bottom);

            var innerSize = new Size(
                Math.Max(0, availableSize.Width - paddingSize.Width),
                Math.Max(0, availableSize.Height - paddingSize.Height));

            double totalMain = Orientation == Orientation.Horizontal ? innerSize.Width : innerSize.Height;

            if (firstVisible && !secondVisible)
            {
                Size full = Orientation == Orientation.Horizontal
                    ? new Size(totalMain, innerSize.Height)
                    : new Size(innerSize.Width, totalMain);

                firstChild.Measure(full);
                return new Size(full.Width + paddingSize.Width, full.Height + paddingSize.Height);
            }
            else if (!firstVisible && secondVisible)
            {
                Size full = Orientation == Orientation.Horizontal
                    ? new Size(totalMain, innerSize.Height)
                    : new Size(innerSize.Width, totalMain);

                secondChild.Measure(full);
                return new Size(full.Width + paddingSize.Width, full.Height + paddingSize.Height);
            }
            else if (firstVisible && secondVisible)
            {
                double firstLength = CalculateLength(FirstControlSize, totalMain, SecondControlSize);
                double secondLength = totalMain - firstLength;

                if (Orientation == Orientation.Horizontal)
                {
                    firstChild.Measure(new Size(firstLength, innerSize.Height));
                    secondChild.Measure(new Size(secondLength, innerSize.Height));
                    double height = Math.Max(firstChild.DesiredSize.Height, secondChild.DesiredSize.Height);
                    return new Size(innerSize.Width + paddingSize.Width, height + paddingSize.Height);
                }
                else
                {
                    firstChild.Measure(new Size(innerSize.Width, firstLength));
                    secondChild.Measure(new Size(innerSize.Width, secondLength));
                    return new Size(innerSize.Width + paddingSize.Width, totalMain + paddingSize.Height);
                }
            }

            return new Size(paddingSize.Width, paddingSize.Height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var firstChild = Children.ElementAtOrDefault(0);
            var secondChild = Children.ElementAtOrDefault(1);

            bool firstVisible = IsVisible(firstChild);
            bool secondVisible = IsVisible(secondChild);

            var padding = this.Padding;
            double offsetX = padding.Left;
            double offsetY = padding.Top;

            double contentWidth = finalSize.Width - padding.Left - padding.Right;
            double contentHeight = finalSize.Height - padding.Top - padding.Bottom;

            double totalMain = Orientation == Orientation.Horizontal ? contentWidth : contentHeight;

            if (firstVisible && !secondVisible)
            {
                if (Orientation == Orientation.Horizontal)
                    firstChild.Arrange(new Rect(offsetX, offsetY, contentWidth, contentHeight));
                else
                    firstChild.Arrange(new Rect(offsetX, offsetY, contentWidth, contentHeight));
            }
            else if (!firstVisible && secondVisible)
            {
                if (Orientation == Orientation.Horizontal)
                    secondChild.Arrange(new Rect(offsetX, offsetY, contentWidth, contentHeight));
                else
                    secondChild.Arrange(new Rect(offsetX, offsetY, contentWidth, contentHeight));
            }
            else if (firstVisible && secondVisible)
            {
                double totalLength = totalMain;
                double firstLength = CalculateLength(FirstControlSize, totalLength, SecondControlSize);
                double secondLength = totalLength - firstLength;

                if (Orientation == Orientation.Horizontal)
                {
                    firstChild.Arrange(new Rect(offsetX, offsetY, firstLength, contentHeight));
                    secondChild.Arrange(new Rect(offsetX + firstLength, offsetY, secondLength, contentHeight));
                }
                else
                {
                    firstChild.Arrange(new Rect(offsetX, offsetY, contentWidth, firstLength));
                    secondChild.Arrange(new Rect(offsetX, offsetY + firstLength, contentWidth, secondLength));
                }
            }

            return finalSize;
        }
    }
}
