namespace DumbNews.Controllers
{
    using System.Collections.Generic;
    using Microsoft.AspNet.Mvc;
    using Microsoft.WindowsAzure.Storage.Table;
    using Lib.Domain;
    using Lib.Entity;
    using System.Threading.Tasks;
    using Lib.Model;
    using System.Net;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Newtonsoft.Json;

    [Route("api/[controller]")]
    public class FeedController : Controller
    {
        private CloudTableClient tableClient;
        private Repository<Feed> repository;
        private CloudQueue feedQueue;

        public FeedController(CloudTableClient tableClient, CloudQueue feedQueue)
        {
            this.tableClient = tableClient;
            this.repository = new Repository<Feed>(tableClient);
            this.feedQueue = feedQueue;
        }

        [HttpGet("{partitionKey}")]
        public async Task<IEnumerable<Feed>> Get(string partitionKey)
        {
            return (await repository.Get(partitionKey)).Results;
        }

        [HttpGet]
        public async Task<Feed> Get(string name, string type)
        {
            return (await repository.Get(type, name)).Result as Feed;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]FeedRequest feedReq)
        {
            if(!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }
            var result = await repository.Insert(new Feed(feedReq.Url, feedReq.Name, feedReq.Type));

            CloudQueueMessage message = new CloudQueueMessage(JsonConvert.SerializeObject(result.Result));
            await feedQueue.AddMessageAsync(message);

            return new CreatedResult("/Feed", result.Result);
        }

        [HttpPut]
        public async Task<Feed> Put([FromBody]Feed feed)
        {
            return (await repository.Update(feed)).Result as Feed;
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string name, string type)
        {
            var result = await repository.Delete(type, name);
            return new ObjectResult(result.Result)
            {
                StatusCode = (int)HttpStatusCode.NoContent
            };
        }
    }
}
