using Speakers.UI.ViewModels;

namespace Speakers.UI.Views;


[QueryProperty(nameof(Speaker), "Speaker")]
public partial class SpeakerEdit : ContentPage {

    public SpeakerEdit() {
        InitializeComponent();
        BindingContext = new SpeakerViewModel();
    }

    public SpeakerViewModel Speaker {
        get => BindingContext as SpeakerViewModel;
        set => BindingContext = value;
    }

}