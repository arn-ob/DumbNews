using System.Collections.Generic;
using Microsoft.AspNet.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Extensions.OptionsModel;
using DumbNews.Lib.Options;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace DumbNews.Controllers
{
    [Route("api/[controller]")]
    public class FeedController : Controller
    {
        private IOptions<ConnectionStringsSettings> connectionSettings;

        public FeedController(IOptions<ConnectionStringsSettings> connectionSettings)
        {
            this.connectionSettings = connectionSettings;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionSettings.Value.StorageConnection);
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
        public void Post([FromBody]string value)
        {
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
