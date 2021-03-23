using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CardMGTService.API.Infrastructure
{
    public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        public CustomMultipartFormDataStreamProvider(string path)
            : base(path)
        {
        }

        public override string GetLocalFileName(System.Net.Http.Headers.HttpContentHeaders headers)
        {
            var name = !string.IsNullOrWhiteSpace(headers.ContentDisposition.FileName) ? headers.ContentDisposition.FileName : "NoName.jpg";
            var Name =name.Replace("\"", string.Empty);
            var fileExtension = Name.Split('.')[1];
            var newName = $"{Guid.NewGuid().ToString("N")}.{fileExtension}";
            return newName;
        }
    }
}