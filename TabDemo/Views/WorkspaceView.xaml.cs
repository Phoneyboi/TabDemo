namespace TabDemo.Views;

public partial class WorkspaceView : ContentView
{
	public WorkspaceView()
	{
		InitializeComponent();
	}

	public static readonly BindableProperty TitleProperty =
		BindableProperty.Create(nameof(Title), typeof(string), typeof(WorkspaceView), "Workspace");

	public string Title
	{
		get => (string)GetValue(TitleProperty);
		set => SetValue(TitleProperty, value);
	}
}
