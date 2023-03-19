using Speakers.UI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows.Input;


namespace Speakers.UI.ViewModels {
    public class SpeakersViewModel : BaseViewModel {

        private bool isRefreshing;
        private HttpClient httpClient;

        public ObservableCollection<SpeakerViewModel> Speakers { get; private set; } = new ObservableCollection<SpeakerViewModel>();

        public ICommand RefreshCommand => new Command(async () => await RefreshAsync());

        public ICommand DeleteCommand => new Command<SpeakerViewModel>(RemoveSpeaker);
        public ICommand FavoriteCommand => new Command<SpeakerViewModel>(FavoriteSpeaker);

        public SpeakersViewModel(IHttpClientFactory httpClientFactory) {

            this.httpClient = httpClientFactory.CreateClient("api");
        }

        void RemoveSpeaker(SpeakerViewModel speaker) {
            if (Speakers.Contains(speaker)) {
                Speakers.Remove(speaker);
            }
        }


        void FavoriteSpeaker(SpeakerViewModel speaker) { }

        public bool IsRefreshing {
            get => isRefreshing;
            set {
                SetProperty(ref isRefreshing, value);
                this.OnPropertyChanged("IsNotRefreshing");
            }
        }


        public bool IsNotRefreshing {
            get => !isRefreshing;
        }

        public ICommand AppearingCommand => new Command(() => {
            if (this.IsRefreshing)
                return;

            this.IsRefreshing = true;
        });
        public ICommand DisappearingCommand => new Command(() => {
            this.IsRefreshing = false;
        });


        public async Task RefreshAsync() {
            try {

                var speakers = await this.httpClient.GetFromJsonAsync<IEnumerable<Speaker>>("api/Speakers",
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));


                MergeObsevableCollectionWithIEnumerable(speakers);
            }
            catch (Exception ex) {
                await App.Current.MainPage.DisplayAlert("Alert", ex.Message, "OK");
            }

            this.IsRefreshing = false;
        }


        private void MergeObsevableCollectionWithIEnumerable(IEnumerable<Speaker> speakers) {

            foreach (var speaker in speakers) {
                var speakerViewModel = this.Speakers.FirstOrDefault(s => s.SpeakerId == speaker.SpeakerId);
                if (speakerViewModel == null) {
                    this.Speakers.Add(new SpeakerViewModel(speaker));
                }
                else {
                    speakerViewModel.FirstName = speaker.FirstName;
                    speakerViewModel.LastName = speaker.LastName;
                    speakerViewModel.Title = speaker.Title;
                    speakerViewModel.ProfilePictureWithPictureName = new() { ProfilePictureFileName = speaker.ProfilePictureFileName, ProfilePicture = speaker.ProfilePicture };
                }
            }

            var speakersToRemove = this.Speakers.Where(s => !speakers.Any(speaker => speaker.SpeakerId.Equals(s.SpeakerId))).ToList();

            foreach (var speakerToRemove in speakersToRemove)
                this.Speakers.Remove(speakerToRemove);
        }
    }
}
