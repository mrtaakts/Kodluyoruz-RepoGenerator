namespace Kodluyoruz_RepoGenerator.Models
{
    public class ResponseModel<T> where T : class
    {
        public int counter { get; set; }
        public T? Data { get; set; }
    }
}
