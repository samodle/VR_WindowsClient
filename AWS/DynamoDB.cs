using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2;
 using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;

namespace AWS
{
    public static class DynamoDB
    {
        #region Variables/Properties

        public static AmazonDynamoDBClient client;


        #endregion
        static DynamoDB()
        {
            var resources = new Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView(); ;
           // var token = resources.GetString("secret");
            
            client = new AmazonDynamoDBClient(resources.GetString("AWSKeyID"),resources.GetString("AWSKeySecret"));
        }

  
        public async static void MakeTable()
        {

            Console.WriteLine("Getting list of tables");
          //  List<string> currentTables = client.ListTablesAsync().TableNames;
            ListTablesResponse x = await client.ListTablesAsync();
            List<string> currentTables = x.TableNames;
        //    client.
            Console.WriteLine("Number of tables: " + currentTables.Count);
            if (!currentTables.Contains("AnimalsInventory"))
            {
                var request = new CreateTableRequest
                {
                    TableName = "AnimalsInventory",
                    AttributeDefinitions = new List<AttributeDefinition>
      {
        new AttributeDefinition
        {
          AttributeName = "Id",
          // "S" = string, "N" = number, and so on.
          AttributeType = "N"
        },
        new AttributeDefinition
        {
          AttributeName = "Type",
          AttributeType = "S"
        }
      },
                    KeySchema = new List<KeySchemaElement>
      {
        new KeySchemaElement
        {
          AttributeName = "Id",
          // "HASH" = hash key, "RANGE" = range key.
          KeyType = "HASH"
        },
        new KeySchemaElement
        {
          AttributeName = "Type",
          KeyType = "RANGE"
        },
      },
                    ProvisionedThroughput = new ProvisionedThroughput
                    {
                        ReadCapacityUnits = 10,
                        WriteCapacityUnits = 5
                    },
                };

                var response = client.CreateTable(request);

                Console.WriteLine("Table created with request ID: " + response.ResponseMetadata.RequestId);
            }
        }

        public static void IsTableReadyToModify()
        {
            var status = "";

            do
            {
                // Wait 5 seconds before checking (again).
              //  System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));

                try
                {
                    var response = client.DescribeTable(new DescribeTableRequest
                    {
                        TableName = "AnimalsInventory"
                    });

                    Console.WriteLine("Table = {0}, Status = {1}",
                      response.Table.TableName,
                      response.Table.TableStatus);

                    status = response.Table.TableStatus;
                }
                catch (ResourceNotFoundException)
                {
                    // DescribeTable is eventually consistent. So you might
                    //   get resource not found. 
                }

            } while (status != TableStatus.ACTIVE);
        }

        public static void InsertItemIntoTable()
        {
            var request1 = new PutItemRequest
            {
                TableName = "AnimalsInventory",
                Item = new Dictionary<string, AttributeValue>
  {
    { "Id", new AttributeValue { N = "1" }},
    { "Type", new AttributeValue { S = "Dog" }},
    { "Name", new AttributeValue { S = "Fido" }}
  }
            };

            var request2 = new PutItemRequest
            {
                TableName = "AnimalsInventory",
                Item = new Dictionary<string, AttributeValue>
  {
    { "Id", new AttributeValue { N = "2" }},
    { "Type", new AttributeValue { S = "Cat" }},
    { "Name", new AttributeValue { S = "Patches" }}
  }
            };

            client.PutItem(request1);
            client.PutItem(request2);
        }



    }
}
