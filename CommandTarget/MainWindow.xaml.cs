using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace CommandTarget
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow ()
		{
			InitializeComponent();
			new ButtonPanelView(Buttons, Target, Emmiter);
		}

	}

	public class ButtonPanelView
	{
		public List<ToggleButton> ButtonList { get; set; }
		public List<string> NamesList { get; set; }
		public ComboBox Cb { get; set; }

		private readonly MenuItem _mi;
		private static ButtonPanelView Instance;

		// set the CommandTarget acording to the ComboBox selection
		private void OnSelectionChanged (object s, RoutedEventArgs e)
		{
			var mi = _mi as MenuItem;
			if (mi == null || Cb.SelectedIndex < 0) return;
			mi.CommandTarget = ButtonList[Cb.SelectedIndex];
		}


		//PAUSE COMMAND

		// helper to extract the target from the event args
		private static bool ButtonPauseTargets (RoutedEventArgs e,
			Func<ToggleButton, bool> ex)
		{
			var handled = false;

			var target = e.OriginalSource as ToggleButton;
			if (target == null) return false;
			handled = ex(target);

			return handled;
		}

		// Executed
		public static void _OnButtonPause (object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = ButtonPauseTargets(e, delegate(ToggleButton target)
			{
				if (!target.IsEnabled) return false;
				var flag = target.IsChecked ?? false;
				target.IsChecked = !flag;
				return true;
			});
		}
		public static ExecutedRoutedEventHandler OnButtonPause
		{
			get { return _OnButtonPause; }
		}

		// CanExecute - dissable if no item selected
		private static void _OnPauseCanExecute (object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = Instance != null && Instance.Cb.SelectedIndex > -1;
		}

		public static CanExecuteRoutedEventHandler OnPauseCanExecute
		{
			get { return _OnPauseCanExecute; }
		}


		//CONSTRUCTOR

		public ButtonPanelView (Panel sp, ComboBox cb, MenuItem mi)
		{
			ButtonList = new List<ToggleButton>();
			NamesList = new List<string>();

			foreach (var ui in sp.Children)
			{
				var tb = ui as ToggleButton;
				if (tb == null) continue;
				ButtonList.Add(tb);
				NamesList.Add(tb.Name);
			}

			Cb = cb;
			cb.ItemsSource = NamesList;
			cb.SelectionChanged += OnSelectionChanged;

			_mi = mi;
			Instance = this;
			OnSelectionChanged(_mi, new RoutedEventArgs());
		}
	}
}
