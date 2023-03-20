using Speakers.UI.ViewModels;
using System.Diagnostics;

namespace Speakers.UI.Views;


[QueryProperty(nameof(Speaker), "Speaker")]
public partial class SpeakerEdit : ContentPage {

    public SpeakerEdit(SpeakerEditViewModel speakerEditViewModel) {
        InitializeComponent();
        BindingContext = speakerEditViewModel;
    }

    public SpeakerViewModel Speaker {
        get => (BindingContext as SpeakerEditViewModel).SpeakerViewModel;
        set {
            try {
                (BindingContext as SpeakerEditViewModel).SpeakerViewModel = value;
            }
            catch (Exception) { } // If we are going to fast on UI, we can have an exception here
        }
    }

}