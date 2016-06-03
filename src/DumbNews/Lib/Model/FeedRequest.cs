namespace DumbNews.Lib.Model
{
    using System.ComponentModel.DataAnnotations;

    public class FeedRequest
    {
        [Required(AllowEmptyStrings = true, ErrorMessage = "Url not provided")]
        public string Url { get; set; }
        [Required(AllowEmptyStrings = true, ErrorMessage = "Name not provided")]
        public string Name { get; set; }
        [Required(AllowEmptyStrings = true, ErrorMessage = "Type not provided")]
        public string Type { get; set; }
    }
}
