namespace DumbNews.Controllers
{
    using System.Collections.Generic;
    using Microsoft.AspNet.Mvc;
    using Microsoft.WindowsAzure.Storage.Table;
    using Lib.Domain;
    using Lib.Entity;
    using System.Threading.Tasks;
    using Lib.Model;
    [Route("api/[controller]")]
    public class FeedController : Controller
    {
        private CloudTableClient tableClient;
        private Repository<Feed> repository;

        public FeedController(CloudTableClient tableClient)
        {
            this.tableClient = tableClient;
            this.repository = new Repository<Feed>(tableClient);
        }
        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public async Task<TableResult> Post([FromBody]FeedRequest feedReq)
        {
            return await repository.Insert(new Feed(feedReq.Url, feedReq.Name));
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
