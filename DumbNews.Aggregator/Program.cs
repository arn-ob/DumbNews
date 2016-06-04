using DumbNews.Aggregator.Lib.Model;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DumbNews.Aggregator
{
    class Program
    {
        static void Main(string[] args)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["storageConnection"].ConnectionString);

            // Create the queue client
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue
            CloudQueue queue = queueClient.GetQueueReference("feedq");

            // Get the next message
            CloudQueueMessage retrievedMessage = queue.GetMessage();
            Feed feed = JsonConvert.DeserializeObject<Feed>(retrievedMessage.AsString);
            SyndicationFeed syndicationFeed = SyndicationFeed.Load(XmlReader.Create(feed.Url));

            if (retrievedMessage != null)
            {
                //Process the message in less than 30 seconds, and then delete the message
                //queue.DeleteMessage(retrievedMessage);
            }



            string searchServiceName = ConfigurationManager.AppSettings["searchServiceName"];
            string apiKey = ConfigurationManager.AppSettings["searchApiKey"];

            SearchServiceClient searchServiceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(apiKey));
            DeleteFeedsIndex(searchServiceClient);

            Console.WriteLine("{0}", "Creating index...\n");
            CreateFeedsIndex(searchServiceClient);

            Console.WriteLine("{0}", "Uploading index...\n");
            SearchIndexClient indexClient = searchServiceClient.Indexes.GetClient("feeds");
            PopulateIndex(indexClient, syndicationFeed.Items);

            SearchDocuments(indexClient, "U.S. Defence Chief");
            Console.ReadLine();
        }

        private static void SearchDocuments(SearchIndexClient indexClient, string searchText, string filter = null)
        {
            // Execute search based on search text and optional filter
            var sp = new SearchParameters();

            if (!String.IsNullOrEmpty(filter))
            {
                sp.Filter = filter;
            }

            DocumentSearchResult<IndexItem> response = indexClient.Documents.Search<IndexItem>(searchText, sp);
            foreach (SearchResult<IndexItem> result in response.Results)
            {
                Console.WriteLine(result.Document.title);
            }
        }

        private static void PopulateIndex(SearchIndexClient indexClient, IEnumerable<SyndicationItem> items)
        {
            var documents = new List<IndexItem>();

            foreach (var item in items)
            {
                documents.Add(new IndexItem() {
                    guid = Guid.NewGuid().ToString(),
                    title = item.Title.Text
                });
            }

            try
            {
                var batch = IndexBatch.Upload(documents);
                indexClient.Documents.Index(batch);
            }
            catch (IndexBatchException e)
            {
                // Sometimes when your Search service is under load, indexing will fail for some of the documents in
                // the batch. Depending on your application, you can take compensating actions like delaying and
                // retrying. For this simple demo, we just log the failed document keys and continue.
                Console.WriteLine(
                    "Failed to index some of the documents: {0}",
                    String.Join(", ", e.IndexingResults.Where(r => !r.Succeeded).Select(r => r.Key)));
            }
        }

        private static void DeleteFeedsIndex(SearchServiceClient searchServiceClient)
        {
            if (searchServiceClient.Indexes.Exists("feeds"))
            {
                searchServiceClient.Indexes.Delete("feeds");
            }
        }

        private static void CreateFeedsIndex(SearchServiceClient serviceClient)
        {
            var definition = new Index()
            {
                Name = "feeds",
                Fields = new[]
                {
                    new Field("guid", DataType.String)                          { IsKey = true },
                    new Field("title", DataType.String)                         { IsSearchable = true, IsFilterable = true }
                }
            };

            serviceClient.Indexes.Create(definition);
        }
    }
}
