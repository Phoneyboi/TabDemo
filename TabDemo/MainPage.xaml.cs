using System.Collections.ObjectModel;
using TabDemo.Views;


namespace TabDemo;

public partial class MainPage : ContentPage
{
	private ObservableCollection<TabInfo> openTabs = new();
	private Dictionary<Guid, View> tabContentLookup = new();
	private Guid? activeTabId = null;

	public MainPage()
	{
		InitializeComponent();
	}

	private void OnNewTabClicked(object sender, EventArgs e)
	{
		var tabId = Guid.NewGuid();
		var title = $"Tab {openTabs.Count + 1}";

		var tab = new TabInfo { Id = tabId, Title = title };
		openTabs.Add(tab);

		var tabButton = CreateTabButton(tab);
		TabHeaderBar.Children.Insert(TabHeaderBar.Children.Count - 1, tabButton);

		// ✅ Use the custom view here
		var workspace = new Views.WorkspaceView
		{
			Title = title
		};

		tabContentLookup[tabId] = workspace;
		ActivateTab(tabId);
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
			CloseTab(tab.Id);
		};

		var tapGesture = new TapGestureRecognizer();
		tapGesture.Tapped += (s, e) => ActivateTab(tab.Id);

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
		layout.ClassId = tab.Id.ToString(); // Use ClassId to track which tab this represents

		return layout;
	}

	private void ActivateTab(Guid tabId)
	{
		activeTabId = tabId;
		if (tabContentLookup.TryGetValue(tabId, out var content))
		{
			TabContentArea.Content = content;
		}

		// Update visuals
		foreach (var view in TabHeaderBar.Children)
		{
			if (view is Frame frame && frame.ClassId != null)
			{
				frame.BackgroundColor = (frame.ClassId == tabId.ToString())
					? Colors.LightSkyBlue
					: Colors.LightBlue;
			}
		}
	}

	private void CloseTab(Guid tabId)
	{
		// Remove tab info
		var tab = openTabs.FirstOrDefault(t => t.Id == tabId);
		if (tab != null)
			openTabs.Remove(tab);

		// Remove UI
		var frameToRemove = TabHeaderBar.Children
			.FirstOrDefault(v => v is Frame f && f.ClassId == tabId.ToString());
		if (frameToRemove != null)
			TabHeaderBar.Children.Remove(frameToRemove);

		tabContentLookup.Remove(tabId);

		if (activeTabId == tabId)
		{
			if (openTabs.Count > 0)
				ActivateTab(openTabs.Last().Id);
			else
			{
				TabContentArea.Content = new Label
				{
					Text = "No tabs open.",
					FontSize = 18
				};
				activeTabId = null;
			}
		}
	}

	private class TabInfo
	{
		public Guid Id { get; set; }
		public string Title { get; set; }
	}
}
