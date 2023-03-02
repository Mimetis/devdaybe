//using Microsoft.Maui.Graphics.Skia;
using Speakers.UI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Speakers.UI.ViewModels
{
    [QueryProperty(nameof(RequestId), nameof(RequestId))]
    public class SpeakerViewModel : BaseViewModel
    {
        private Guid speakerId;
        private string requestId;
        private string firstName;
        private string lastName;
        private string title;
        private string comments;
        private string photoPath;
        private string profilePictureFileName;
        private byte[] profilePicture;
        private DateTime hireDate;

        //public IImageResizer ImageResizer => DependencyService.Get<IImageResizer>();

        public Command SaveCommand { get; }
        public Command TakePhotoCommand { get; }
        public Command CancelCommand { get; }


        public SpeakerViewModel(Speaker speaker = null)
        {
            if (speaker != null)
            {
                this.SpeakerId = speaker.SpeakerId;
                this.FirstName = speaker.FirstName;
                this.LastName = speaker.LastName;
                this.Title = speaker.Title;
                this.ProfilePicture = speaker.ProfilePicture;
                this.ProfilePictureFileName = speaker.ProfilePictureFileName;

            }


            this.SaveCommand = new Command(OnSave, ValidateSave);
            this.CancelCommand = new Command(OnCancel);
            this.TakePhotoCommand = new Command(async () => await OnTakePhotoAsync());

            this.photoPath = "details_place_holder.jpg";
            this.speakerId = Guid.NewGuid();

            // everytime check if we can save
            this.PropertyChanged += (_, __) => SaveCommand.ChangeCanExecute();
        }

        public bool IsNew { get; set; } = true;

        public string RequestId
        {
            get
            {
                return requestId;
            }
            set
            {
                requestId = value;
                LoadItemId(new Guid(value));
            }
        }
        private bool ValidateSave()
        {
            return !String.IsNullOrWhiteSpace(firstName)
                && !String.IsNullOrWhiteSpace(lastName);
        }
    
        public string PhotoPath
        {
            get => photoPath;
            set => SetProperty(ref photoPath, value);

        }

        public Guid SpeakerId
        {
            get => speakerId;
            set => SetProperty(ref speakerId, value);

        }
        public string FirstName
        {
            get => firstName;
            set => SetProperty(ref firstName, value);
        }

        public string LastName
        {
            get => lastName;
            set => SetProperty(ref lastName, value);
        }

        public string FullName => $"{FirstName} {LastName}";

        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }
        public string Comments
        {
            get => comments;
            set => SetProperty(ref comments, value);
        }
        public DateTime HireDate
        {
            get => hireDate;
            set => SetProperty(ref hireDate, value);
        }
        public Byte[] ProfilePicture
        {
            get => profilePicture;
            set => SetProperty(ref profilePicture, value);
        }
        public string ProfilePictureFileName
        {
            get => profilePictureFileName;
            set => SetProperty(ref profilePictureFileName, value);
        }

        public ImageSource ProfilePictureSource
        {
            get
            {
                var ms = new MemoryStream(ProfilePicture);
                var imgSource = ImageSource.FromStream(() => ms);
                return imgSource;
                //return ImageSource.FromStream(() => new MemoryStream(ProfilePicture));
            }
        }

        public async void LoadItemId(Guid speakerId)
        {
            try
            {
                //var item = new Speaker { SpeakerId = speakerId };
                Speaker item = null;
                //var item = await DataStore.GetSpeakerAsync(speakerId);

                this.IsNew = item == null;

                if (!IsNew)
                    await this.FillSpeakerAsync(item);
                else
                    this.SpeakerId = Guid.NewGuid();
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }

        public async Task FillSpeakerAsync(Speaker item)
        {
            this.SpeakerId = item.SpeakerId;
            this.FirstName = item.FirstName;
            this.LastName = item.LastName;
            this.Title = item.Title;
            this.ProfilePicture = item.ProfilePicture;
            this.ProfilePictureFileName = item.ProfilePictureFileName;
            this.PhotoPath = Path.Combine(FileSystem.CacheDirectory, ProfilePictureFileName);

            if (!File.Exists(this.PhotoPath))
            {
                using (var writeStream = File.OpenWrite(PhotoPath))
                {
                    byte[] buffer = new byte[writeStream.Length];
                    await writeStream.WriteAsync(this.ProfilePicture, 0, this.ProfilePicture.Length);
                };

            }

            // no need byte picture array, since we have serialized the photo
            this.ProfilePicture = null;
        }


        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        private async void OnSave()
        {
            Speaker newSpeaker = new Speaker()
            {
                SpeakerId = SpeakerId,
                FirstName = FirstName,
                LastName = LastName,
                Title = Title,
                ProfilePictureFileName = ProfilePictureFileName
            };

            using (var readStream = File.OpenRead(PhotoPath))
            {
                byte[] buffer = new byte[readStream.Length];
                await readStream.ReadAsync(buffer, 0, buffer.Length);
                newSpeaker.ProfilePicture = buffer;
            };

            //if (IsNew)
            //    await DataStore.AddSpeakerAsync(newSpeaker);
            //else
            //    await DataStore.UpdateSpeakerAsync(newSpeaker);

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        async Task OnTakePhotoAsync()
        {
            try
            {

                var photo = await MediaPicker.CapturePhotoAsync();
                await SavePhotoToCacheAsync(photo);
                Console.WriteLine($"CapturePhotoAsync COMPLETED: {PhotoPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CapturePhotoAsync THREW: {ex.Message}");
            }
        }


        async Task SavePhotoToCacheAsync(FileResult photo)
        {
            // canceled
            if (photo == null)
                return;

            // save the file into local storage

            var newFile = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
            //var newFile2 = Path.Combine(FileSystem.CacheDirectory, "thumb.jpg");


            using (Stream sourceStream = await photo.OpenReadAsync())
            {
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


            this.ProfilePictureFileName = photo.FileName;
            this.PhotoPath = newFile;
        }
    }

}
