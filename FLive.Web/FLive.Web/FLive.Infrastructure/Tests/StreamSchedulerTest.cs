using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Newtonsoft.Json.Linq;
using NUnit.Framework;


namespace FLive.Infrastructure
{
    [TestFixture]
    public class StreamSchedulerTest
    {
        private readonly string _dbConnection;
        private readonly string _apiEndpoint;
        private readonly string _apiToken;


        public StreamSchedulerTest()
        {
            _dbConnection = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            _apiEndpoint = ConfigurationManager.AppSettings["apiEndpoint"];
            _apiToken = ConfigurationManager.AppSettings["apiToken"];
        }

        [Test]
        public void create_should_return_a_valid_stream_id()
        {
            StreamScheduler streamScheduler = new StreamScheduler(_dbConnection , _apiEndpoint,_apiToken);
            streamScheduler.CreateAndStart().Wait();
            
        }

    }
}
