using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TourPlanner.View
{
    public partial class CreateBtn : UserControl
    {
        public CreateBtn() => InitializeComponent();

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(CreateBtn));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(CreateBtn));

        public static readonly DependencyProperty IsButtonEnabledProperty =
            DependencyProperty.Register(nameof(IsButtonEnabled), typeof(bool), typeof(CreateBtn), new PropertyMetadata(true));

        public ICommand Command { get => (ICommand)GetValue(CommandProperty); set => SetValue(CommandProperty, value); }
        public object CommandParameter { get => GetValue(CommandParameterProperty); set => SetValue(CommandParameterProperty, value); }
        public bool IsButtonEnabled { get => (bool)GetValue(IsButtonEnabledProperty); set => SetValue(IsButtonEnabledProperty, value); }
    }
}
