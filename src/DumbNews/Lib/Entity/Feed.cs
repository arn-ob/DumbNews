namespace DumbNews.Lib.Entity
{
    using Microsoft.WindowsAzure.Storage.Table;

    public class Feed: TableEntity
    {
        public Feed(string url, string name)
        {
            Url = url;
            Name = name;
        }
        public Feed()
        {

        }
        
        public string Url { get; set; }
        public string LastReadIndex { get; set; }
        public string Name { get; set; }
    }
}
