using System.Collections.ObjectModel;
using TabDemo.MVVM.Models;
using TabDemo.MVVM.ViewModels;
using TabDemo.Views;


namespace TabDemo;

public partial class MainPage : ContentPage
{
	private MainPageViewModel ViewModel => BindingContext as MainPageViewModel;

	public MainPage()
	{
		InitializeComponent();
		BindingContext = new MainPageViewModel();
	}

	private void OnNewTabClicked(object sender, EventArgs e)
	{
		ViewModel.AddNewTab();
		RebuildTabHeaders();
	}

	private void RebuildTabHeaders()
	{
		TabHeaderBar.Children.Clear();

		foreach (var tab in ViewModel.OpenTabs)
		{
			var button = CreateTabButton(tab);
			TabHeaderBar.Children.Add(button);
		}

		// ✅ Always add the "+" button at the end
		var addButton = new Button
		{
			Text = "+",
			BackgroundColor = Colors.LightGray,
			WidthRequest = 40
		};
		addButton.Clicked += OnNewTabClicked;

		TabHeaderBar.Children.Add(addButton);
	}


	private View CreateTabButton(TabInfo tab)
	{
		var label = new Label
		{
			Text = tab.Title,
			VerticalOptions = LayoutOptions.Center,
			Padding = new Thickness(5, 0),
		};

		var closeButton = new Button
		{
			Text = "×",
			WidthRequest = 30,
			HeightRequest = 30,
			Padding = 0,
			BackgroundColor = Colors.Transparent,
			FontSize = 16,
			TextColor = Colors.Black
		};

		closeButton.Clicked += (s, e) =>
		{
			ViewModel.CloseTab(tab.Id);
			RebuildTabHeaders();
		};

		var tapGesture = new TapGestureRecognizer();
		tapGesture.Tapped += (s, e) =>
		{
			ViewModel.ActivateTab(tab.Id);
		};

		var layout = new Frame
		{
			BackgroundColor = Colors.LightBlue,
			Padding = new Thickness(5, 0),
			Margin = new Thickness(0, 0, 5, 0),
			Content = new HorizontalStackLayout
			{
				Spacing = 5,
				Children = { label, closeButton }
			}
		};

		layout.GestureRecognizers.Add(tapGesture);
		layout.ClassId = tab.Id.ToString();

		return layout;
	}
}

