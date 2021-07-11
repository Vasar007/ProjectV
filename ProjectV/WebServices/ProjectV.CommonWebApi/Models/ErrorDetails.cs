using Acolyte.Assertions;
using Newtonsoft.Json;

namespace ProjectV.CommonWebApi.Models
{
    public sealed class ErrorDetails
    {
        public int StatusCode { get; }
        public string Message { get; }


        public ErrorDetails(
            int statusCode,
            string message)
        {
            StatusCode = statusCode;
            Message = message.ThrowIfNull(nameof(message));
        }

        public static string AsString(
            int statusCode,
            string message)
        {
            var details = new ErrorDetails(statusCode, message);
            return details.ToString();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
