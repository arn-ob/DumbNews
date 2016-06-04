namespace DumbNews.Aggregator
{
    using Microsoft.Azure.Search.Models;

    public class IndexItem
    {

        public IndexItem()
        {
        }
        public string guid { get; set; }
        public string title { get; set; }


    }
}