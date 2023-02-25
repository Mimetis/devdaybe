using Speakers.UI.ViewModels;

namespace Speakers.UI.Views;

public partial class SpeakerEdit : ContentPage
{
	public SpeakerEdit()
	{
		InitializeComponent();
        BindingContext = new SpeakerViewModel();
    }
}