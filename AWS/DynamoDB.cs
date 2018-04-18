using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWS
{
    public static class DynamoDB
    {
        private const string accessKey = "AKIAJ56HXAFX3LRSLLHQ";
        private const string secretKey = "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";


        public static async Task TaskMainAsync()
        {
            string tableName = "TableNumberOneYo";
            string hashKey = "UserId";

            //   Console.WriteLine("Creating credentials and initializing DynamoDB client");
            var credentials = new BasicAWSCredentials(accessKey, secretKey);
            var client = new AmazonDynamoDBClient(credentials, RegionEndpoint.USEast1);

            //  Console.WriteLine("Verify table => " + tableName);
            var tableResponse = await client.ListTablesAsync();
            if (!tableResponse.TableNames.Contains(tableName))
            {
                //  Console.WriteLine("Table not found, creating table => " + tableName);
                await client.CreateTableAsync(new CreateTableRequest
                {
                    TableName = tableName,
                    ProvisionedThroughput = new ProvisionedThroughput
                    {
                        ReadCapacityUnits = 3,
                        WriteCapacityUnits = 1
                    },
                    KeySchema = new List<KeySchemaElement>
                    {
                        new KeySchemaElement
                        {
                            AttributeName = hashKey,
                            KeyType = KeyType.HASH
                        }
                    },
                    AttributeDefinitions = new List<AttributeDefinition>
                    {
                        new AttributeDefinition { AttributeName = hashKey, AttributeType=ScalarAttributeType.S }
                    }
                });

                bool isTableAvailable = false;
                while (!isTableAvailable)
                {
                    //  Console.WriteLine("Waiting for table to be active...");
                    await Task.Delay(TimeSpan.FromSeconds(2));
                    var tableStatus = await client.DescribeTableAsync(tableName);
                    isTableAvailable = tableStatus.Table.TableStatus == "ACTIVE";
                }
            }

            //     Console.WriteLine("Set a local DB context");
            var context = new DynamoDBContext(client);

            //    Console.WriteLine("Create an AlexaAudioState object to save");
            AlexaAudioState currentState = new AlexaAudioState
            {
                UserId = "MOOSEsomeAwesomeUser"
            };

            //     Console.WriteLine("Save an AlexaAudioState object");
            await context.SaveAsync<AlexaAudioState>(currentState);

            //    Console.WriteLine("Getting an AlexaAudioState object");
            List<ScanCondition> conditions = new List<ScanCondition>();
            conditions.Add(new ScanCondition("UserId", ScanOperator.Equal, currentState.UserId));
            var allDocs = await context.ScanAsync<AlexaAudioState>(conditions).GetRemainingAsync();
            var savedState = allDocs.FirstOrDefault();

            //    Console.WriteLine("Verifying object...");
            if (JsonConvert.SerializeObject(savedState) == JsonConvert.SerializeObject(currentState))
                Console.WriteLine("Object verified");
            else
                Console.WriteLine("oops, something went wrong");

            //Console.WriteLine("Delete table => " + tableName);
            //context.Dispose();
            //await client.DeleteTableAsync(new DeleteTableRequest() { TableName = tableName });
        }
    }

    internal class AlexaAudioState
    {
        public string UserId { get; set; }
    }
}
