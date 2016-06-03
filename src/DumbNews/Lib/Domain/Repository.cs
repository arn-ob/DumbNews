namespace DumbNews.Lib.Domain
{
    using Microsoft.WindowsAzure.Storage.Table;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Entity;

    public class Repository<T> where T : TableEntity, new()
    {
        public CloudTable Table { get; private set; }

        internal Repository(CloudTableClient tableClient)
        {
            var type = typeof(T);
            this.Table = tableClient.GetTableReference(string.Concat(type.Name, "s"));
        }

        internal virtual async Task<TableResult> Insert(T obj)
        {

            TableOperation insertOperation = TableOperation.Insert(obj, echoContent: true);
            var result = await Table.ExecuteAsync(insertOperation);
            return result;

        }

        internal virtual async Task<IList<TableResult>> InsertBatch(IEnumerable<T> objects)
        {
            TableBatchOperation insertOperation = new TableBatchOperation();
            foreach (var obj in objects)
            {
                insertOperation.Insert(obj);
            }

            return await Table.ExecuteBatchAsync(insertOperation);
        }

        internal virtual async Task<TableQuerySegment<T>> Get(string partitionKey)
        {
            TableQuery<T> query = new TableQuery<T>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));

            TableContinuationToken continuationToken = null;

            TableQuerySegment<T> tableQueryResult = await Table.ExecuteQuerySegmentedAsync<T>(query, continuationToken);
            return tableQueryResult;
        }

        internal virtual async Task<TableResult> Get(string partitionKey, string rowKey)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            return await this.Table.ExecuteAsync(retrieveOperation);
        }

        internal virtual async Task<TableResult> Update(Feed feed)
        {
            if (string.IsNullOrEmpty(feed.ETag))
            {
                feed = (await Get(feed.PartitionKey, feed.RowKey)).Result as Feed;
            }
            TableOperation updateOperation = TableOperation.Replace(feed);
            return await this.Table.ExecuteAsync(updateOperation);
        }

        internal virtual async Task<TableResult> Delete(string partitionKey, string rowKey)
        {
            var feed = (await Get(partitionKey, rowKey)).Result as Feed;
            TableOperation deleteOperation = TableOperation.Delete(feed);
            return await this.Table.ExecuteAsync(deleteOperation);
        }
    }
}
