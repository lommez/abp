//using Microsoft.Azure.Documents;
//using Microsoft.Azure.Documents.Client;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Serialization;
//using System;

//namespace Volo.Abp.CosmosDB
//{
//    public class CosmosDBDocumentClientFactory : ICosmosDBDocumentClientFactory
//    {
//        private readonly Uri _serviceEndpoint;
//        private readonly string _authKey;

//        public CosmosDBDocumentClientFactory(Uri serviceEndpoint, string authKey)
//        {
//            _serviceEndpoint = serviceEndpoint;
//            _authKey = authKey;
//        }

//        public IDocumentClient CreateDocumentClient()
//        {
//            var documentClient = new DocumentClient(_serviceEndpoint, _authKey, new JsonSerializerSettings
//            {
//                NullValueHandling = NullValueHandling.Ignore,
//                DefaultValueHandling = DefaultValueHandling.Ignore,
//                ContractResolver = new CamelCasePropertyNamesContractResolver()
//            });

//            documentClient.OpenAsync().GetAwaiter().GetResult();

//            return documentClient;
//        }
//    }
//}