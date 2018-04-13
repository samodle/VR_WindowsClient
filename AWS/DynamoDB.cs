using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2;
 using Amazon.DynamoDBv2.Model;
namespace AWS
{
    public class DynamoDB
    {
        private AmazonDynamoDBClient client = new AmazonDynamoDBClient();
   
        private void MakeTable()
        {

            Console.WriteLine("Getting list of tables");
            List<string> currentTables = client.ListTables().TableNames;
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

        private void IsTableReadyToModify()
        {
            var status = "";

            do
            {
                // Wait 5 seconds before checking (again).
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));

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

        private void InsertItemIntoTable()
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
