namespace Speakers.API.Models
{
    public class Speaker
    {
        public Guid SpeakerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string LinkedInUrl { get; set; }
        public string TwitterUrl { get; set; }
        public string WebSiteUrl { get; set; }
        public DateTime? Created { get; set; }
        public string Description { get; set; }
        public byte[] ProfilePicture { get; set; }
        public string ProfilePictureFileName { get; set;}
        public string ProfilePictureImageType{ get; set; }


    }
}
