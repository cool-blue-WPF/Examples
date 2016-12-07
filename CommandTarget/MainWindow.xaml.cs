using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace CommandTarget
{
	/// <summary>
	///     Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			View = new ButtonPanelView(Buttons, Target, Emitter);
		}

		public ButtonPanelView View { get; set; }
	}

	public class ButtonPanelView : INotifyPropertyChanged
	{
		private static CanExecuteRoutedEventHandler _pauseCanExecute;


		//CONSTRUCTOR

		public ButtonPanelView (Panel sp, ComboBox cb, ItemsControl itemsControl)
		{
			CommandTargets = new List<ToggleButton>();	
			ComboBoxItemSource = new List<string>(); 

			foreach (var ui in sp.Children)
			{
				var tb = ui as ToggleButton;
				if (tb == null) continue;
				CommandTargets.Add(tb);
				ComboBoxItemSource.Add(tb.Name);
			}

			Cb = cb;
			cb.ItemsSource = ComboBoxItemSource;
			cb.SelectionChanged += OnSelectionChanged;

			_pauseCanExecute += (s, e) => e.CanExecute = Cb.SelectedIndex > -1;
			OnSelectionChanged(itemsControl, new RoutedEventArgs());
		}

		public List<ToggleButton> CommandTargets { get; set; }
		public List<string> ComboBoxItemSource { get; set; }
		public ComboBox Cb { get; set; }

		// set the CommandTarget acording to the ComboBox selection
		private ToggleButton _target;
		public ToggleButton Target
		{
			get { return _target; }
			set
			{
				if (Equals(_target, value)) return;
				_target = value;
				OnPropertyChanged();
			}
		}

		//PAUSE COMMAND
		// Static binding callbacks

		// Executed
		public static ExecutedRoutedEventHandler OnButtonPause
		{
			get
			{
				return (sender, e) =>
				{
					e.Handled = ButtonPauseTargets(e, delegate(ToggleButton target)
					{
						if (!target.IsEnabled) return false;
						var flag = target.IsChecked ?? false;
						target.IsChecked = !flag;
						return true;
					});
				};
			}
		}

		// CanExecute - dissable if no item selected
		public static CanExecuteRoutedEventHandler OnPauseCanExecute
		{
			get
			{
				return (sender, e) =>
				{
					if (_pauseCanExecute == null) return;
					_pauseCanExecute(sender, e);
				};
			}
		}

		// helper to extract the target from the event args
		private static bool ButtonPauseTargets (RoutedEventArgs e,
			Func<ToggleButton, bool> ex)
		{
			var target = e.OriginalSource as ToggleButton;
			if (target == null) return false;
			var handled = ex(target);

			return handled;
		}

		// INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged (
			[CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}

		private void OnSelectionChanged (object s, RoutedEventArgs e)
		{
			if (Cb.SelectedIndex < 0) return;
			Target = CommandTargets[Cb.SelectedIndex];
		}
	}
}