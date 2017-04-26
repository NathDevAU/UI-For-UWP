﻿using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Input
{
    /// <summary>
    /// Represents a item in a <see cref="SegmentedItemsControl"/>.
    /// </summary>
    public class Segment : ButtonBase
    {
        /// <summary>
        /// Identifies the <see cref="CornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(Segment), new PropertyMetadata(new CornerRadius(0d)));

        /// <summary>
        /// Identifies the <see cref="IsSelected"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(Segment), new PropertyMetadata(false, OnIsSelectedChanged));

        private const string FocusedName = "Focused";
        private const string PointerFocusedName = "PointerFocused";
        private const string UnfocusedName = "Unfocused";

        private static readonly Dictionary<string, List<Segment>> groups;

        private readonly bool isAutoGenerated;
        private string group;
        private bool isKeyDown;
        private SegmentVisualState visualStateCache;
        private Rect layoutSlot;
        private bool isParentEnabled = true;

        static Segment()
        {
            groups = new Dictionary<string, List<Segment>>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Segment"/> class.
        /// </summary>
        public Segment()
        {
            this.DefaultStyleKey = typeof(Segment);
            this.visualStateCache = SegmentVisualState.Normal;
        }

        internal Segment(bool isAutoGenerated) : this()
        {
            this.isAutoGenerated = isAutoGenerated;
        }

        /// <summary>
        /// Occurs when the <see cref="Segment"/> has been selected.
        /// </summary>
        public event EventHandler Selected;

        /// <summary>
        /// Occurs when the <see cref="LayoutSlot"/> or the VisualStateName of the segment has changed.
        /// </summary>
        public event EventHandler AnimationContextChanged;

        /// <summary>
        /// Gets or sets the corner radius of the outer border of the segment.
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { this.SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the segment is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { this.SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// Gets the name of the current <see cref="VisualState"/> of the control.
        /// </summary>
        public SegmentVisualState VisualState
        {
            get
            {
                return this.visualStateCache;
            }
        }

        /// <summary>
        /// Gets or sets the layout slot where the element has been arranged.
        /// </summary>
        public Rect LayoutSlot
        {
            get
            {
                return this.layoutSlot;
            }
            set
            {
                if (this.layoutSlot != value)
                {
                    this.layoutSlot = value;
                    this.OnAnimationContextChanged();
                }
            }
        }

        internal bool IsParentEnabled
        {
            get
            {
                return this.isParentEnabled;
            }
            set
            {
                if (this.isParentEnabled != value)
                {
                    this.isParentEnabled = value;
                    this.UpdateVisualState(false);
                }
            }
        }

        internal string Group
        {
            get
            {
                return this.group;
            }
            set
            {
                if (this.group != value)
                {
                    this.UpdateGroups(this.group, value);
                    this.group = value;
                }
            }
        }

        internal bool IsAutoGenerated
        {
            get
            {
                return this.isAutoGenerated;
            }
        }

        internal void UpdateVisualState(bool useTransitions)
        {
            var state = this.CreateVisualState();

            if (VisualStateManager.GoToState(this, state.ToString(), useTransitions))
            {
                this.visualStateCache = state;
                this.OnAnimationContextChanged();
            }
        }

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.UpdateVisualState(true);
        }

        /// <inheritdoc/>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            if (this.IsPressed)
            {
                VisualStateManager.GoToState(this, PointerFocusedName, true);
            }
            else
            {
                VisualStateManager.GoToState(this, FocusedName, true);
            }
        }

        /// <inheritdoc/>
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            base.OnKeyDown(e);
            base.OnKeyUp(e);
            switch (e.Key)
            {
                case VirtualKey.Space:
                case VirtualKey.Enter:
                    this.isKeyDown = true;
                    this.UpdateVisualState(true);
                    break;
            }
        }

        /// <inheritdoc/>
        protected override void OnKeyUp(KeyRoutedEventArgs e)
        {
            this.isKeyDown = false;
            base.OnKeyUp(e);
            switch (e.Key)
            {
                case VirtualKey.Space:
                case VirtualKey.Enter:
                    this.IsSelected = true;
                    this.UpdateVisualState(true);
                    break;
            }
        }

        /// <inheritdoc/>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            VisualStateManager.GoToState(this, UnfocusedName, true);
        }

        /// <inheritdoc/>
        protected override void OnPointerCanceled(PointerRoutedEventArgs e)
        {
            base.OnPointerCanceled(e);
            this.UpdateVisualState(true);
        }

        /// <inheritdoc/>
        protected override void OnPointerCaptureLost(PointerRoutedEventArgs e)
        {
            base.OnPointerCaptureLost(e);
            this.UpdateVisualState(true);
        }

        /// <inheritdoc/>
        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            base.OnPointerEntered(e);
            this.UpdateVisualState(true);
        }

        /// <inheritdoc/>
        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            base.OnPointerExited(e);
            this.UpdateVisualState(true);
        }

        /// <inheritdoc/>
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            this.UpdateVisualState(true);
        }

        /// <inheritdoc/>
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            if (this.IsPointerOver && !this.IsSelected)
            {
                this.IsSelected = true;
            }

            base.OnPointerReleased(e);
            this.UpdateVisualState(true);
        }

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var segment = d as Segment;

            if ((bool)e.NewValue)
            {
                if (!string.IsNullOrEmpty(segment.Group))
                {
                    foreach (var seg in groups[segment.Group])
                    {
                        if (seg.Equals(segment))
                        {
                            continue;
                        }

                        seg.IsSelected = false;
                    }
                }

                segment.OnSelected();
            }

            segment.UpdateVisualState(true);
        }

        private void OnSelected()
        {
            var handler = this.Selected;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void OnAnimationContextChanged()
        {
            var handler = this.AnimationContextChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private SegmentVisualState CreateVisualState()
        {
            var state = SegmentVisualState.Normal;

            if (this.IsSelected)
            {
                if (!this.IsEnabled || !this.IsParentEnabled)
                {
                    state = SegmentVisualState.SelectedDisabled;
                }
                else if (this.IsPressed || this.isKeyDown)
                {
                    state = SegmentVisualState.SelectedPressed;
                }
                else if (this.IsPointerOver)
                {
                    state = SegmentVisualState.SelectedPointerOver;
                }
                else
                {
                    state = SegmentVisualState.Selected;
                }
            }
            else
            {
                if (!this.IsEnabled || !this.IsParentEnabled)
                {
                    state = SegmentVisualState.Disabled;
                }
                else if (this.IsPressed || this.isKeyDown)
                {
                    state = SegmentVisualState.Pressed;
                }
                else if (this.IsPointerOver)
                {
                    state = SegmentVisualState.PointerOver;
                }
                else
                {
                    state = SegmentVisualState.Normal;
                }
            }

            return state;
        }

        private void UpdateGroups(string oldGroup, string newGroup)
        {
            if (!string.IsNullOrEmpty(oldGroup))
            {
                groups[oldGroup].Remove(this);
                if (groups[oldGroup].Count == 0)
                {
                    groups.Remove(oldGroup);
                }
            }

            if (!string.IsNullOrEmpty(newGroup))
            {
                if (groups.ContainsKey(newGroup))
                {
                    groups[newGroup].Add(this);
                }
                else
                {
                    groups.Add(newGroup, new List<Segment> { this });
                }
            }
        }
    }
}
