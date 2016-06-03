namespace DumbNews.Lib.Entity
{
    using Microsoft.WindowsAzure.Storage.Table;

    public class Feed: TableEntity
    {
        public Feed(string url, string lastReadIndex)
        {
            Url = url;
            LastReadIndex = lastReadIndex;
        }
        public Feed()
        {

        }
        
        public string Url { get; set; }
        public string LastReadIndex { get; set; }
    }
}
