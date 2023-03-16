//using Microsoft.Maui.Graphics.Skia;
using Speakers.UI.Models;
using Speakers.UI.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Speakers.UI.ViewModels {
    public class SpeakerViewModel : BaseViewModel {
        private Guid speakerId;
        private string firstName;
        private string lastName;
        private string title;
        private string profilePictureFileName;
        private byte[] profilePicture;
        private ProfilePictureWithPictureName profilePictureWithPictureName;

        public SpeakerViewModel(Speaker speaker = null) {
            Debug.WriteLine("SpeakerViewModel ctor");
            if (speaker != null) {

                this.SpeakerId = speaker.SpeakerId;
                this.FirstName = speaker.FirstName;
                this.LastName = speaker.LastName;
                this.Title = speaker.Title;
                this.ProfilePictureWithPictureName = new() { ProfilePicture = speaker.ProfilePicture, ProfilePictureFileName = speaker.ProfilePictureFileName };
                this.IsNew = false;
            }
            else {
                this.speakerId = Guid.NewGuid();
                this.IsNew = true;
            }
        }

        public bool IsNew { get; set; } = true;


        public Guid SpeakerId {
            get => speakerId;
            set => SetProperty(ref speakerId, value);
        }
        public string FirstName {
            get => firstName;
            set {
                SetProperty(ref firstName, value);
                base.OnPropertyChanged(nameof(FullName));
            }
        }

        public string LastName {
            get => lastName;
            set {
                SetProperty(ref lastName, value);
                base.OnPropertyChanged(nameof(FullName));
            }
        }

        public string FullName => $"{FirstName} {LastName}";

        public string Title {
            get => title;
            set => SetProperty(ref title, value);
        }

        //public byte[] ProfilePicture {
        //    get => profilePicture;
        //    set {
        //        SetProperty(ref profilePicture, value);
        //    }
        //}
        //public string ProfilePictureFileName {
        //    get => profilePictureFileName;
        //    set => SetProperty(ref profilePictureFileName, value);
        //}

        public ProfilePictureWithPictureName ProfilePictureWithPictureName {
            get => profilePictureWithPictureName;
            set => SetProperty(ref profilePictureWithPictureName, value);
        }

    }

    public class ProfilePictureWithPictureName {

        public byte[] ProfilePicture { get; set; }
        public string ProfilePictureFileName { get; set; }

    }

}
