namespace StudyTrack.Views;

public partial class AddTaskPage : ContentPage
{
    private readonly AddTaskViewModel _viewModel;

    public AddTaskPage(AddTaskViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
    }
}
