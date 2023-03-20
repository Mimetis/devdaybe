using Dotmim.Sync.Sqlite;
using Dotmim.Sync.Web.Client;
using Dotmim.Sync;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speakers.UI.Services;
using System.Runtime.CompilerServices;
using Speakers.UI.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.Data.Sqlite;

namespace Speakers.UI.ViewModels {
    public class SpeakerEditViewModel : BaseViewModel {

        private SpeakersContext speakersContext;
        private SpeakerViewModel speakerViewModel;


        public Command SaveCommand { get; }
        public Command TakePhotoCommand { get; }
        public Command CancelCommand { get; }

        public SpeakerEditViewModel(SpeakersContext speakersContext) {

            this.speakersContext = speakersContext;

            this.SaveCommand = new Command(OnSave);
            this.CancelCommand = new Command(OnCancel);
            this.TakePhotoCommand = new Command(async () => await OnTakePhotoAsync());
        }

        public SpeakerViewModel SpeakerViewModel {
            get => speakerViewModel;
            set => SetProperty(ref speakerViewModel, value);

        }
      
        private async void OnCancel() =>await Shell.Current.GoToAsync("..");
        
        private async void OnSave() {

            if (this.SpeakerViewModel == null)
                return;

            var dbSpeaker = await this.speakersContext.Speakers.FirstOrDefaultAsync(s => s.SpeakerId == this.SpeakerViewModel.SpeakerId);

            if (dbSpeaker == null) {
                speakersContext.Speakers.Add(new Speaker() {

                    SpeakerId = this.SpeakerViewModel.SpeakerId,
                    FirstName = this.SpeakerViewModel.FirstName,
                    LastName = this.SpeakerViewModel.LastName,
                    Title = this.SpeakerViewModel.Title,
                    ProfilePictureFileName = this.SpeakerViewModel.ProfilePictureWithPictureName.ProfilePictureFileName,
                    ProfilePicture = this.SpeakerViewModel.ProfilePictureWithPictureName.ProfilePicture
                });
            }
            else {

                dbSpeaker.SpeakerId = this.SpeakerViewModel.SpeakerId;
                dbSpeaker.FirstName = this.SpeakerViewModel.FirstName;
                dbSpeaker.LastName = this.SpeakerViewModel.LastName;
                dbSpeaker.Title = this.SpeakerViewModel.Title;
                dbSpeaker.ProfilePictureFileName = this.SpeakerViewModel.ProfilePictureWithPictureName.ProfilePictureFileName;
                dbSpeaker.ProfilePicture = this.SpeakerViewModel.ProfilePictureWithPictureName.ProfilePicture;
            }

            await speakersContext.SaveChangesAsync();

            await Shell.Current.GoToAsync("..");
        }

        async Task OnTakePhotoAsync() {
            try {

                var photo = await MediaPicker.CapturePhotoAsync();
                await SavePhotoToCacheAsync(photo);
            }
            catch (Exception ex) {
                Console.WriteLine($"CapturePhotoAsync THREW: {ex.Message}");
            }
        }


        async Task SavePhotoToCacheAsync(FileResult photo) {
            // canceled
            if (photo == null)
                return;

            // save the file into local storage

            var newFile = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
            //var newFile2 = Path.Combine(FileSystem.CacheDirectory, "thumb.jpg");


            using (Stream sourceStream = await photo.OpenReadAsync()) {
                using FileStream localFileStream = File.OpenWrite(newFile);
                await sourceStream.CopyToAsync(localFileStream);
            };

            //using (Stream sourceStream = await photo.OpenReadAsync())
            //{
            //    using (var image = SkiaImage.FromStream(sourceStream))
            //    {
            //        var newImage = image.Downsize(150);
            //        using FileStream localFileStream = File.OpenWrite(newFile);
            //        newImage.Save(localFileStream);
            //    }
            //}

            //Microsoft.Maui.Graphics.IImage image;
            //Assembly assembly = GetType().GetTypeInfo().Assembly;
            //using (Stream stream = assembly.GetManifestResourceStream("MauiGraphicsSample.Resources.Images.windows.pngg"))
            //{
            //    image = PlatformImage.FromStream(stream);
            //}

            //if (image != null)
            //{
            //    Microsoft.Maui.Graphics.IImage newImage = image.Downsize(100, true);
            //    canvas.DrawImage(newImage, 10, 10, newImage.Width, newImage.Height);
            //}


            //this.ProfilePictureFileName = photo.FileName;
            //this.PhotoPath = newFile;
        }

    }
}
