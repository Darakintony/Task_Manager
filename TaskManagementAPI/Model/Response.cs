namespace TaskManagementAPI.Model
{
    public class Response
    {
        public string StatusCode { get; set; }
        public string StatusMessage { get; set; }
        //public object Data { get; set; }
    }

    public class Response2<T> : Response
    {
        public T Data { get; set; }
    }
}
