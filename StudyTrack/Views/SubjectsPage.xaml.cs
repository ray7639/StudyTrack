namespace StudyTrack.Views;

public partial class SubjectsPage : ContentPage
{
    private readonly SubjectsViewModel _viewModel;

    public SubjectsPage(SubjectsViewModel viewModel)
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
