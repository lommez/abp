﻿//using Microsoft.Azure.Documents;
//using Microsoft.Azure.Documents.Client;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace Volo.Abp.CosmosDB
//{
//    public class CosmosDBClientFactory : ICosmosDBClientFactory
//    {
//        private readonly string _databaseName;
//        private readonly List<string> _collectionNames;
//        private readonly IDocumentClient _documentClient;

//        public CosmosDBClientFactory(string databaseName, List<string> collectionNames, IDocumentClient documentClient)
//        {
//            _databaseName = databaseName ?? throw new ArgumentNullException(nameof(databaseName));
//            _collectionNames = collectionNames ?? throw new ArgumentNullException(nameof(collectionNames));
//            _documentClient = documentClient ?? throw new ArgumentNullException(nameof(documentClient));
//        }

//        public ICosmosDBClient GetClient(string collectionName)
//        {
//            if (!_collectionNames.Contains(collectionName))
//            {
//                throw new ArgumentException($"Unable to find collection: {collectionName}");
//            }

//            return new CosmosDBClient(_databaseName, collectionName, _documentClient);
//        }

//        public async Task EnsureDbSetupAsync()
//        {
//            await _documentClient.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(_databaseName));

//            foreach (var collectionName in _collectionNames)
//            {
//                await _documentClient.ReadDocumentCollectionAsync(
//                    UriFactory.CreateDocumentCollectionUri(_databaseName, collectionName));
//            }
//        }
//    }
//}