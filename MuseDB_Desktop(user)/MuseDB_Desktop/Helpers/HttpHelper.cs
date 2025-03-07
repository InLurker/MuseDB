﻿using System;
using System.Collections.Generic;
using System.IO;
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
        public static async Task UploadFile(string UploadUrl, string FilePath, string DestinationFileRename)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                var url = new System.Uri(UploadUrl);
                // Load the file:
                var file = new System.IO.FileInfo(FilePath);
                if (!file.Exists)
                    throw new ArgumentException($"Unable to access file at: {FilePath}", nameof(FilePath));

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

        public static async Task UploadFileAndDelete(string UploadUrl, string FilePath, string DestinationFileRename)
        {
            await UploadFile(UploadUrl,FilePath, DestinationFileRename);
            File.Delete(FilePath);
        }


        public static async Task DeleteFile(string Url)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                System.Net.Http.HttpRequestMessage request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Delete, Url);
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode(); // this throws an exception on non HTTP success codes
            }
        }

        public static async Task ReplaceFileAndDelete(string UploadUrl, string FilePath, string DestinationFileRename)
        {
            await DeleteFile(UploadUrl + DestinationFileRename);
            await UploadFileAndDelete(UploadUrl, FilePath, DestinationFileRename);
        }
    }
}
