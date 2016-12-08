using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace CommandTarget
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		//PAUSE COMMAND
		// Static, binding callbacks

		// Executed
		public static ExecutedRoutedEventHandler
			OnButtonPause = (sender, e) =>
			{
				e.Handled = ButtonPauseTarget(e, delegate(ToggleButton target)
				{
					if (!target.IsEnabled) return false;
					var flag = target.IsChecked ?? false;
					target.IsChecked = !flag;
					return true;
				});
			};

		// CanExecute
		public static CanExecuteRoutedEventHandler
			OnPauseCanExecute = (sender, e) => { e.CanExecute = true; };

		// helper to extract the target from the event args
		private static bool ButtonPauseTarget (RoutedEventArgs e,
			Func<ToggleButton, bool> ex)
		{
			var target = e.OriginalSource as ToggleButton;
			if (target == null) return false;
			var handled = ex(target);

			return handled;
		}
	}
}