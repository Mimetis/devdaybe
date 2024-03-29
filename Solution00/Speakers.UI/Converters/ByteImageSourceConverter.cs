﻿using Speakers.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speakers.UI.Converters {
    /// <summary>
    /// Converts the incoming value from <see cref="byte"/>[] and returns the object of a type <see cref="ImageSource"/> or vice versa.
    /// </summary>
    public class ByteArrayToImageSourceConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            if (value is null) {
                return null;
            }

            // Many problems using ImageSource.FromStream:
            // return ImageSource.FromStream(() => new MemoryStream((byte[])value));
            // 1) Seems we have a memory leak but not sure at all
            // 2) Caching is not working correctly as image is saved locally internally with a random id, and really often, the wrong image is shown


            // Eventually, make the save to cache, then returns the image
            var profilePictureWithPictureName = value as ProfilePictureWithPictureName;

            var filePath = Path.Combine(FileSystem.CacheDirectory, profilePictureWithPictureName.ProfilePictureFileName);

            if (!File.Exists(filePath)) {

                // Write the file content to the app data directory
                using FileStream outputStream = File.OpenWrite(filePath);
                outputStream.Write(profilePictureWithPictureName.ProfilePicture, 0, profilePictureWithPictureName.ProfilePicture.Length);
            }

            var imgSource = ImageSource.FromFile(filePath);
            return imgSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return null;
            //if (value is null) {
            //    return null;
            //}

            //if (value is not StreamImageSource streamImageSource) {
            //    throw new ArgumentException("Expected value to be of type StreamImageSource.", nameof(value));
            //}

            //var streamFromImageSource = streamImageSource.Stream(CancellationToken.None).GetAwaiter().GetResult();

            //if (streamFromImageSource is null) {
            //    return null;
            //}

            //using var memoryStream = new MemoryStream();
            //streamFromImageSource.CopyTo(memoryStream);

            //return memoryStream.ToArray();
        }
        ///// <summary>
        ///// Converts the incoming value from <see cref="byte"/>[] and returns the object of a type <see cref="ImageSource"/>.
        ///// </summary>
        ///// <param name="value">The value to convert.</param>
        ///// <param name="culture">The culture to use in the converter. This is not implemented.</param>
        ///// <returns>An object of type <see cref="ImageSource"/>.</returns>
        //[return: NotNullIfNotNull(nameof(value))]
        //public override ImageSource ConvertFrom(byte[] value, CultureInfo? culture = null) {
        //}

        ///// <summary>
        ///// Converts the incoming value from <see cref="StreamImageSource"/> and returns a <see cref="byte"/>[].
        ///// </summary>
        ///// <param name="value">The value to convert.</param>
        ///// <param name="culture">The culture to use in the converter. This is not implemented.</param>
        ///// <returns>An object of type <see cref="ImageSource"/>.</returns>
        //public override byte[]? ConvertBackTo(ImageSource? value, CultureInfo? culture = null) {
        //    if (value is null) {
        //        return null;
        //    }

        //    if (value is not StreamImageSource streamImageSource) {
        //        throw new ArgumentException("Expected value to be of type StreamImageSource.", nameof(value));
        //    }

        //    var streamFromImageSource = streamImageSource.Stream(CancellationToken.None).GetAwaiter().GetResult();

        //    if (streamFromImageSource is null) {
        //        return null;
        //    }

        //    using var memoryStream = new MemoryStream();
        //    streamFromImageSource.CopyTo(memoryStream);

        //    return memoryStream.ToArray();
        //}

    }
}
