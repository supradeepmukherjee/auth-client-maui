using System;
using System.Collections.Generic;
using System.Text;

namespace client_maui.Controls
{
    public partial class FloatingLabelEntry : ContentView
    {
        public FloatingLabelEntry()
        {
            InitializeComponent();
            InnerEntry.TextChanged += async (s, e) => await UpdateLabelState(false);
            this.Loaded += async (s, e) => await UpdateLabelState(false);
        }

        public static readonly BindableProperty LabelProperty =
            BindableProperty.Create(nameof(Label), typeof(string), typeof(FloatingLabelEntry), default(string));
        public string Label { get => (string)GetValue(LabelProperty); set => SetValue(LabelProperty, value); }

        public static readonly BindableProperty PlaceholderProperty =
            BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(FloatingLabelEntry), default(string));
        public string Placeholder { get => (string)GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(FloatingLabelEntry), default(string), BindingMode.TwoWay);
        public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

        private async void Entry_Focused(object sender, FocusEventArgs e) => await AnimateLabelUp();
        private async void Entry_Unfocused(object sender, FocusEventArgs e) => await UpdateLabelState(true);

        async Task AnimateLabelUp()
        {
            await FloatingLabel.TranslateToAsync(0, -18, 120, Easing.CubicOut);
            FloatingLabel.FontSize = 10;
        }

        async Task AnimateLabelDown()
        {
            await FloatingLabel.TranslateToAsync(0, 18, 120, Easing.CubicIn);
            FloatingLabel.FontSize = 12;
        }

        async Task UpdateLabelState(bool animate)
        {
            if (!string.IsNullOrEmpty(Text))
            {
                if (animate) await AnimateLabelUp();
                else { FloatingLabel.TranslationY = -18; FloatingLabel.FontSize = 10; }
            }
            else
            {
                if (InnerEntry.IsFocused) await AnimateLabelUp();
                else
                {
                    if (animate) await AnimateLabelDown();
                    else { FloatingLabel.TranslationY = 18; FloatingLabel.FontSize = 12; }
                }
            }
        }
    }
}
