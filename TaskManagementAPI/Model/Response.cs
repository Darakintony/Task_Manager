using System.Text.Json.Serialization;

namespace TaskManagementAPI.Model
{
    public class Response<T>
    {
        public string StatusCode { get; set; }
        public string StatusMessage { get; set; }
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T Data { get; set; }

        //public int TotalCount { get; set; }
        //public int PageSize { get; set; }
        //public int CurrentPage { get; set; }
        //public int TotalPages { get; set; }
        //public bool HasNextPage => CurrentPage < TotalPages;
        //public bool HasPreviousPage => CurrentPage > 1;
    }

    
}
