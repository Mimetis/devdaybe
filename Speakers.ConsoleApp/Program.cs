using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Speakers.API.Data;
using Speakers.API.Models;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Speakers.ConsoleApp
{
    internal class Program
    {
        private const string connectionString = "Server=(localdb)\\mssqllocaldb;Database=devdaybe;Trusted_Connection=True;MultipleActiveResultSets=true";
        private const string rootPath = @"C:\PROJECTS\devdaybe\Speakers.ConsoleApp";
        static async Task Main(string[] args)
        {
            //Console.WriteLine("Click Enter to proceed");
            //Console.ReadLine();

            await GetAllSpeakersAsync();


        }

        private static async Task GetAllSpeakersAsync()
        {
            using var speakerContext = new SpeakersContext(connectionString);
            var speakers = await speakerContext.Speakers.ToListAsync();

            foreach (var speaker in speakers)
            {
                if (speaker.ProfilePicture != null)
                    continue;

                Console.WriteLine($"{speaker.FirstName} {speaker.LastName}");
                Console.WriteLine("FileName ? ");
                var fileName = Console.ReadLine();

                var fullPath = Path.Combine(rootPath, fileName);
                if (File.Exists(fullPath))
                {
                    var fileInfo = new FileInfo(fullPath);
                    var pictureName = fileInfo.Name;
                    var pictureImageType = fileInfo.Extension switch
                    {
                        "png" => "image/png",
                        "jpg" or "jpeg" or _ => "image/jpeg",

                    };

                    var pictureBytes = await ReadPictureAndReturnBytesAsync(fullPath);
                    speaker.ProfilePicture = pictureBytes;
                    speaker.ProfilePictureFileName = pictureName;
                    speaker.ProfilePictureImageType = pictureImageType;
                }
            }

            await speakerContext.SaveChangesAsync();
        }

        private static async Task UploadSpeakerAsync()
        {

            try
            {



                var pictureBytes = await ReadPictureAndReturnBytesAsync(@"C:\PROJECTS\devdaybe\Speakers.ConsoleApp\elprofessor.jpg");

                var speaker = new Speaker
                {
                    SpeakerId = new Guid("7EC6B882-CC6B-4ECA-8563-B8D6E4D71F50"),
                    FirstName = "Sergio",
                    LastName = "Marquina",
                    Title = "Professor",
                    Created = DateTime.Now,
                    Description = "Casa de Papel El Professor",
                    ProfilePicture = pictureBytes,
                    ProfilePictureFileName = "elprofessor",
                    ProfilePictureImageType = "image/jpg",
                };

                HttpClient httpClient = new HttpClient();

                var jsonSpeaker = JsonConvert.SerializeObject(speaker);
                var content = new StringContent(jsonSpeaker);

                var response = await httpClient.PutAsJsonAsync(
                    $"https://localhost:7170/api/Speakers/{speaker.SpeakerId}", speaker);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// This method is reading a picture from a local file and then save it to the database
        /// </summary>
        /// <returns></returns>
        private static async Task<byte[]> ReadPictureAndReturnBytesAsync(string filePath)
        {
            using var pictureStream = File.OpenRead(filePath);
            using var stream = new MemoryStream();
            await pictureStream.CopyToAsync(stream);
            var bytes = stream.ToArray();

            return bytes;
        }

    }
}