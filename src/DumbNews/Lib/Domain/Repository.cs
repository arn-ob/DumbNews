namespace DumbNews.Lib.Domain
{
    using Microsoft.WindowsAzure.Storage.Table;

    public class Repository<T> where T : TableEntity
    {
        public Repository(CloudTableClient tableClient)
        {
            this.Table = tableClient.GetTableReference(string.Concat(nameof(T), "s"));
        }

        public CloudTable Table { get; private set; }
    }
}
