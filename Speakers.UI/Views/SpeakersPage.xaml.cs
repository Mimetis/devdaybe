using Speakers.UI.ViewModels;

namespace Speakers.UI.Views;

public partial class SpeakersPage : ContentPage
{
	public SpeakersPage(SpeakersViewModel speakersViewModel)
	{
		InitializeComponent();
        BindingContext = speakersViewModel;
    }
}