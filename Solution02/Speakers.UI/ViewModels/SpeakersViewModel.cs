using Dotmim.Sync.Sqlite;
using Dotmim.Sync.Web.Client;
using Dotmim.Sync;
using Speakers.UI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Configuration;
using Speakers.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace Speakers.UI.ViewModels {
    public class SpeakersViewModel : BaseViewModel {

        private bool isRefreshing;
        private bool shouldSync;
        private SyncAgent syncAgent;
        private readonly SpeakersContext speakersContext;

        public ObservableCollection<SpeakerViewModel> Speakers { get; private set; } = new ObservableCollection<SpeakerViewModel>();

        public ICommand RefreshCommand => new Command(async () => await RefreshAsync(shouldSync));

        public ICommand DeleteCommand => new Command<SpeakerViewModel>(RemoveSpeaker);
        public ICommand FavoriteCommand => new Command<SpeakerViewModel>(FavoriteSpeaker);

        public SpeakersViewModel(IHttpClientFactory httpClientFactory, IConfiguration configuration, SpeakersContext speakersContext) {

            Debug.WriteLine("SpeakersViewModel ctor");

            var httpClient = httpClientFactory.CreateClient("api");
            var syncUri = new Uri(httpClient.BaseAddress, configuration["SyncEndpoint"]);
            var webRemoteOrchestrator = new WebRemoteOrchestrator(syncUri.AbsoluteUri, client: httpClient);
            var sqliteSyncProvider = new SqliteSyncProvider(configuration["SqliteFilePath"]);

            this.syncAgent = new SyncAgent(sqliteSyncProvider, webRemoteOrchestrator);
            this.speakersContext = speakersContext;
            this.shouldSync = true;
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

        public ICommand SyncCommand => new Command(() => {
            this.shouldSync = true;
            this.IsRefreshing = true;
        }); 

        public bool ShouldSync => this.shouldSync;

        public ICommand AppearingCommand => new Command(() => {
            if (this.IsRefreshing)
                return;

            this.IsRefreshing = true;
        });
        public ICommand DisappearingCommand => new Command(() => {
            this.IsRefreshing = false;
        });


        public async Task RefreshAsync(bool shouldSync) {
            // this.Speakers.Clear(); // for demo purpose

            try {
                if (shouldSync)
                {
                    var syncResult = await syncAgent.SynchronizeAsync();
                    Debug.WriteLine(syncResult);
                    this.shouldSync = false;    
                }

                var speakers = await speakersContext.Speakers.AsNoTracking().ToListAsync();

                MergeObsevableCollectionWithIEnumerable(speakers);
            }
            catch (Exception ex) {
                Debug.WriteLine("Error getting speakers: {Error}", ex);
            }

            this.IsRefreshing = false;
        }


        /// <summary>
        /// This method need to add to ObservableCollection the new speakers from the IENumerable
        /// Then remove from the ObservableCollection the speakers not in the IENumerable
        /// Then update the ObservableCollection with speakers in the IENumerable
        /// </summary>
        /// <param name="speakers"></param>
        private void MergeObsevableCollectionWithIEnumerable(List<Speaker> speakers) {

            try {

                MainThread.BeginInvokeOnMainThread(() => {
                    // Add or Merge speakers 
                    foreach (var speaker in speakers) {
                        var speakerViewModel = this.Speakers.FirstOrDefault(s => s.SpeakerId == speaker.SpeakerId);
                        if (speakerViewModel == null) {
                            try {
                                this.Speakers.Add(new SpeakerViewModel(speaker));
                            }
                            catch (Exception ex) {
                                Debug.WriteLine(ex);
                            }
                        }
                        else {
                            try {
                                speakerViewModel.FirstName = speaker.FirstName;
                                speakerViewModel.LastName = speaker.LastName;
                                speakerViewModel.Title = speaker.Title;
                                //speakerViewModel.ProfilePictureWithPictureName = new() { ProfilePictureFileName = speaker.ProfilePictureFileName, ProfilePicture = speaker.ProfilePicture };
                            }
                            catch (Exception ex) {
                                Debug.WriteLine(ex);
                            }


                        }
                    }

                    var speakersToRemove = this.Speakers.Where(s => !speakers.Any(speaker => speaker.SpeakerId.Equals(s.SpeakerId))).ToList();

                    foreach (var speakerToRemove in speakersToRemove) {
                        try {

                            this.Speakers.Remove(speakerToRemove);
                        }
                        catch (Exception ex) {
                            Debug.WriteLine(ex);
                        }

                    }

                });

            }
            catch (Exception ex) {

                Debug.WriteLine(ex);
            }

        }
    }
}
