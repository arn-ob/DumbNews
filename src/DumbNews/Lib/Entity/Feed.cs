namespace DumbNews.Lib.Entity
{
    using Microsoft.WindowsAzure.Storage.Table;

    public class Feed: TableEntity
    {
        public Feed(string url, string name, string type)
        {
            Url = url;
            Name = name;
            Type = type;

            this.PartitionKey = type;
            this.RowKey = name;
        }
        public Feed()
        {

        }
        
        public string Url { get; set; }
        public string LastReadIndex { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
