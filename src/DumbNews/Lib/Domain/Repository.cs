namespace DumbNews.Lib.Domain
{
    using Microsoft.WindowsAzure.Storage.Table;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class Repository<T> where T : TableEntity
    {
        public Repository(CloudTableClient tableClient)
        {
            this.Table = tableClient.GetTableReference(string.Concat(nameof(T), "s"));
        }

        public virtual async Task<TableResult> Insert(T obj)
        {
            TableOperation insertOperation = TableOperation.Insert(obj, echoContent: true);
            return await Table.ExecuteAsync(insertOperation);
        }

        public virtual async Task<IList<TableResult>> InsertBatch(IEnumerable<T> objects)
        {
            TableBatchOperation insertOperation = new TableBatchOperation();
            foreach (var obj in objects)
            {
                insertOperation.Insert(obj);
            }

            return await Table.ExecuteBatchAsync(insertOperation);
        }

        public CloudTable Table { get; private set; }
    }
}
