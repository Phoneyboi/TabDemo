using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TabDemo.MVVM.Models;
using TabDemo.Views;

namespace TabDemo.MVVM.ViewModels;

public class MainPageViewModel : INotifyPropertyChanged
{
	public ObservableCollection<TabInfo> OpenTabs { get; set; } = new();
	public Dictionary<Guid, View> TabContentLookup { get; set; } = new();
	private Guid? _activeTabId;

	private View _activeContent;
	public View ActiveContent
	{
		get => _activeContent;
		set
		{
			_activeContent = value;
			OnPropertyChanged();
		}
	}

	public ICommand NewTabCommand { get; }
	public ICommand CloseTabCommand { get; }

	public MainPageViewModel()
	{
		NewTabCommand = new Command(AddNewTab);
		CloseTabCommand = new Command<Guid>(CloseTab);
	}

	public void AddNewTab()
	{
		var tabId = Guid.NewGuid();
		var title = $"Tab {OpenTabs.Count + 1}";

		var tab = new TabInfo { Id = tabId, Title = title };
		OpenTabs.Add(tab);

		var workspace = new WorkspaceView { Title = title };
		TabContentLookup[tabId] = workspace;

		ActivateTab(tabId);
	}

	public void ActivateTab(Guid tabId)
	{
		_activeTabId = tabId;

		if (TabContentLookup.TryGetValue(tabId, out var content))
		{
			ActiveContent = content;
		}
	}

	public void CloseTab(Guid tabId)
	{
		var tab = OpenTabs.FirstOrDefault(t => t.Id == tabId);
		if (tab != null)
			OpenTabs.Remove(tab);

		TabContentLookup.Remove(tabId);

		if (_activeTabId == tabId)
		{
			if (OpenTabs.Count > 0)
				ActivateTab(OpenTabs.Last().Id);
			else
				ActiveContent = new Label { Text = "No tabs open.", FontSize = 18 };
		}
	}

	// INotifyPropertyChanged implementation
	public event PropertyChangedEventHandler? PropertyChanged;
	protected void OnPropertyChanged([CallerMemberName] string name = null) =>
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
