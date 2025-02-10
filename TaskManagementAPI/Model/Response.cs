using System.Text.Json.Serialization;

namespace TaskManagementAPI.Model
{
    public class Response<T>
    {
        public string StatusCode { get; set; }
        public string StatusMessage { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T Data { get; set; }
    }

    //public class Response2<T> : Response
    //{
    //    public T Data { get; set; }
    //}
}
