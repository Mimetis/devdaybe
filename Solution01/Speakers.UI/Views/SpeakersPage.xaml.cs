using Speakers.UI.ViewModels;

namespace Speakers.UI.Views;

public partial class SpeakersPage : ContentPage {
    public SpeakersPage(SpeakersViewModel speakersViewModel) {
        InitializeComponent();
        BindingContext = speakersViewModel;
    }

    async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e) {
        var speaker = e.CurrentSelection[0] as SpeakerViewModel;

        var navigationParameters = new Dictionary<string, object> { { "Speaker", speaker } };
        await Shell.Current.GoToAsync($"SpeakerEdit", navigationParameters);
    }
}