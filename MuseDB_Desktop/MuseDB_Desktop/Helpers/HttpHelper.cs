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
        public static async void UploadFile(string UploadUrl, string ImagePath, string DestinationFileRename)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                var url = new System.Uri(UploadUrl);
                // Load the file:
                var file = new System.IO.FileInfo(ImagePath);
                if (!file.Exists)
                    throw new ArgumentException($"Unable to access file at: {ImagePath}", nameof(ImagePath));

                using (var stream = file.OpenRead())
                {
                    var multipartContent = new System.Net.Http.MultipartFormDataContent
                    {
                        {
                            new System.Net.Http.StreamContent(stream),
                            "files", // this is the name of FormData field
                            DestinationFileRename
                        }
                    };
                    System.Net.Http.HttpRequestMessage request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Post, url)
                    {
                        Content = multipartContent
                    };
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode(); // this throws an exception on non HTTP success codes
                }
            }
        }

        public static async void DeleteFile(string Url)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                System.Net.Http.HttpRequestMessage request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Delete, Url);
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode(); // this throws an exception on non HTTP success codes
            }
        }
    }
}
