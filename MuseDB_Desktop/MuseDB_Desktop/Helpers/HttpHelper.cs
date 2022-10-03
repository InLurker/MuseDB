using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace MuseDB_Desktop.Helpers
{
    public static class HttpHelper
    {
        public static async Task UploadFile(string UploadUrl, string ImagePath, string DestinationFileRename)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                var uri = new System.Uri(UploadUrl);
                // Load the file:
                var file = new System.IO.FileInfo(ImagePath);
                if (!file.Exists)
                    throw new ArgumentException($"Unable to access file at: {ImagePath}", nameof(ImagePath));

                using (var stream = file.OpenRead())
                {
                    var multipartContent = new System.Net.Http.MultipartFormDataContent();
                    multipartContent.Add(
                        new System.Net.Http.StreamContent(stream),
                        "files", // this is the name of FormData field
                        DestinationFileRename);

                    System.Net.Http.HttpRequestMessage request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Post, uri);
                    request.Content = multipartContent;
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode(); // this throws an exception on non HTTP success codes
                }
            }
        }
    }
}
