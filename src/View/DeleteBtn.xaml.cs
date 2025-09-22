using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TourPlanner.View
{
    public partial class DeleteBtn : UserControl
    {
        public DeleteBtn() => InitializeComponent();

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(DeleteBtn));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(DeleteBtn));

        public static readonly DependencyProperty IsButtonEnabledProperty =
            DependencyProperty.Register(nameof(IsButtonEnabled), typeof(bool), typeof(DeleteBtn), new PropertyMetadata(true));

        public ICommand Command { get => (ICommand)GetValue(CommandProperty); set => SetValue(CommandProperty, value); }
        public object CommandParameter { get => GetValue(CommandParameterProperty); set => SetValue(CommandParameterProperty, value); }
        public bool IsButtonEnabled { get => (bool)GetValue(IsButtonEnabledProperty); set => SetValue(IsButtonEnabledProperty, value); }
    }
}
