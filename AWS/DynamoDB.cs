using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWS
{
    public static class DynamoDB
    {
        private const string accessKey = "AKIAJ56HXAFX3LRSLLHQ";
        private const string secretKey = "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";

        private static IAmazonDynamoDB GetDynamoDbClient()
        {
            var credentials = new BasicAWSCredentials(accessKey, secretKey);
            return new AmazonDynamoDBClient(credentials, RegionEndpoint.USEast1);

        }


        public static async Task<List<string>> GetTablesList()
        {
            using (IAmazonDynamoDB client = GetDynamoDbClient())
            {
                ListTablesRequest listTablesRequest = new ListTablesRequest();
                listTablesRequest.Limit = 5;
                ListTablesResponse listTablesResponse = await client.ListTablesAsync(listTablesRequest);
                return listTablesResponse.TableNames;
            }
        }      

    public static void InsertPeopleByDocumentModel(string tableName)
        {
            try
            {
                using (IAmazonDynamoDB client = GetDynamoDbClient())
                {
                    Table peopleTable = Table.LoadTable(client, tableName);
                    Document firstPerson = new Document();
                    firstPerson["Name"] = "John";
                    firstPerson["Birthdate"] = new DateTime(1980, 06, 24);
                    //     firstPerson["Address"] = "34 Wide Street, London, UK";
                    //      firstPerson["Age"] = 34;
                    //      firstPerson["Neighbours"] = new List<String>() { "Jane", "Samantha", "Richard" };
                    peopleTable.PutItemAsync(firstPerson);

                    Document secondPerson = new Document();
                    secondPerson["Name"] = "Jill";
                    secondPerson["Birthdate"] = new DateTime(1981, 02, 26);
                    //        secondPerson["Address"] = "52 Broad Street, Dublin, Ireland";
                    //         secondPerson["Age"] = 33;
                    //         secondPerson["Neighbours"] = new List<String>() { "Alex", "Greg", "Michael" };
                    peopleTable.PutItemAsync(secondPerson);

                    Document thirdPerson = new Document();
                    thirdPerson["Name"] = "George";
                    thirdPerson["Birthdate"] = new DateTime(1979, 11, 4);
                    //        thirdPerson["Address"] = "118 Main Street, Washington";
                    //         thirdPerson["Age"] = 35;
                    //         thirdPerson["Neighbours"] = new List<String>() { "Kathrine", "Kate", "Christine" };
                    peopleTable.PutItemAsync(thirdPerson);

                    Document fourthPerson = new Document();
                    fourthPerson["Name"] = "Carole";
                    fourthPerson["Birthdate"] = new DateTime(1984, 4, 10);
                    //         fourthPerson["Address"] = "5 Short Street, Sydney, Australia";
                    //         fourthPerson["Age"] = 30;
                    //         fourthPerson["Neighbours"] = new List<String>() { "Nadia", "Katya", "Malcolm" };
                    peopleTable.PutItemAsync(fourthPerson);
                }
            }
            catch (AmazonDynamoDBException exception)
            {
                Debug.WriteLine(string.Concat("Exception while inserting records into DynamoDb table: {0}", exception.Message));
                Debug.WriteLine(String.Concat("Error code: {0}, error type: {1}", exception.ErrorCode, exception.ErrorType));
            }
        }

        public static async void DeleteTableDemo(string tableName)
        {
            try
            {
                using (IAmazonDynamoDB client = GetDynamoDbClient())
                {
                    DeleteTableRequest deleteTableRequest = new DeleteTableRequest(tableName);
                    DeleteTableResponse deleteTableResponse = await client.DeleteTableAsync(deleteTableRequest);
                    TableDescription tableDescription = deleteTableResponse.TableDescription;
                    TableStatus tableStatus = tableDescription.TableStatus;
                    Debug.WriteLine(string.Format("Delete table command sent to Amazon for table {0}, status after deletion: {1}", tableName
                        , tableDescription.TableStatus));
                }
            }
            catch (AmazonDynamoDBException exception)
            {
                Debug.WriteLine(string.Concat("Exception while deleting DynamoDb table: {0}", exception.Message));
                Debug.WriteLine(String.Concat("Error code: {0}, error type: {1}", exception.ErrorCode, exception.ErrorType));
            }
        }

        public async static void CreateNewTableDemo(string tableName)
        {
            try
            {
                using (IAmazonDynamoDB client = GetDynamoDbClient())
                {
                    CreateTableRequest createTableRequest = new CreateTableRequest();
                    createTableRequest.TableName = tableName;
                    createTableRequest.ProvisionedThroughput = new ProvisionedThroughput() { ReadCapacityUnits = 1, WriteCapacityUnits = 1 };
                    createTableRequest.KeySchema = new List<KeySchemaElement>()
            {
                new KeySchemaElement()
                {
                    AttributeName = "Name"
                    , KeyType = KeyType.HASH
                },
                new KeySchemaElement()
                {
                    AttributeName = "Birthdate"
                    , KeyType = KeyType.RANGE
                }
            };
                    createTableRequest.AttributeDefinitions = new List<AttributeDefinition>()
            {
                new AttributeDefinition(){AttributeName = "Name", AttributeType = ScalarAttributeType.S}
                , new AttributeDefinition(){AttributeName = "Birthdate", AttributeType = ScalarAttributeType.S}
            };
                    CreateTableResponse createTableResponse = await client.CreateTableAsync(createTableRequest);

                    TableDescription tableDescription = createTableResponse.TableDescription;
                    Debug.WriteLine(string.Format("Table {0} creation command sent to Amazon. Current table status: {1}", tableName, tableDescription.TableStatus));

                    String tableStatus = tableDescription.TableStatus.Value.ToLower();
                    while (tableStatus != "active")
                    {
                        Debug.WriteLine(string.Format("Table {0} not yet active, waiting...", tableName));
                        await Task.Delay(2000);
                        DescribeTableRequest describeTableRequest = new DescribeTableRequest(tableName);
                        DescribeTableResponse describeTableResponse = await client.DescribeTableAsync(describeTableRequest);
                        tableDescription = describeTableResponse.Table;
                        tableStatus = tableDescription.TableStatus.Value.ToLower();
                        Debug.WriteLine(string.Format("Latest status of table {0}: {1}", tableName, tableStatus));
                    }

                    Debug.WriteLine(string.Format("Table creation loop exited for table {0}, final status: {1}", tableName, tableStatus));
                }
            }
            catch (AmazonDynamoDBException exception)
            {
                Debug.WriteLine(string.Concat("Exception while creating new DynamoDb table: {0}", exception.Message));
                Debug.WriteLine(String.Concat("Error code: {0}, error type: {1}", exception.ErrorCode, exception.ErrorType));
            }
        }

        public static async void UpdateTableDemo(string tableName)
        {
            try
            {
                using (IAmazonDynamoDB client = GetDynamoDbClient())
                {
                    UpdateTableRequest updateTableRequest = new UpdateTableRequest();
                    updateTableRequest.TableName = tableName;
                    updateTableRequest.ProvisionedThroughput = new ProvisionedThroughput() { ReadCapacityUnits = 2, WriteCapacityUnits = 2 };
                    UpdateTableResponse updateTableResponse = await client.UpdateTableAsync(updateTableRequest);
                    TableDescription tableDescription = updateTableResponse.TableDescription;
                    Debug.WriteLine(string.Format("Update table command sent to Amazon for table {0}, status after update: {1}", tableName
                        , tableDescription.TableStatus));
                }
            }
            catch (AmazonDynamoDBException exception)
            {
                Debug.WriteLine(string.Concat("Exception while updating DynamoDb table: {0}", exception.Message));
                Debug.WriteLine(String.Concat("Error code: {0}, error type: {1}", exception.ErrorCode, exception.ErrorType));
            }
        }


        public static async void GetTablesDetails()
        {
            List<string> tables = await GetTablesList();
            using (IAmazonDynamoDB client = GetDynamoDbClient())
            {
                foreach (string table in tables)
                {
                    DescribeTableRequest describeTableRequest = new DescribeTableRequest(table);
                    DescribeTableResponse describeTableResponse = await client.DescribeTableAsync(describeTableRequest);
                    TableDescription tableDescription = describeTableResponse.Table;
                    Debug.WriteLine(string.Format("Printing information about table {0}:", tableDescription.TableName));
                    Debug.WriteLine(string.Format("Created at: {0}", tableDescription.CreationDateTime));
                    List<KeySchemaElement> keySchemaElements = tableDescription.KeySchema;
                    foreach (KeySchemaElement schema in keySchemaElements)
                    {
                        Debug.WriteLine(string.Format("Key name: {0}, key type: {1}", schema.AttributeName, schema.KeyType));
                    }
                    Debug.WriteLine(string.Format("Item count: {0}", tableDescription.ItemCount));
                    ProvisionedThroughputDescription throughput = tableDescription.ProvisionedThroughput;
                    Debug.WriteLine(string.Format("Read capacity: {0}", throughput.ReadCapacityUnits));
                    Debug.WriteLine(string.Format("Write capacity: {0}", throughput.WriteCapacityUnits));
                    List<AttributeDefinition> tableAttributes = tableDescription.AttributeDefinitions;
                    foreach (AttributeDefinition attDefinition in tableAttributes)
                    {
                        Debug.WriteLine(string.Format("Table attribute name: {0}", attDefinition.AttributeName));
                        Debug.WriteLine(string.Format("Table attribute type: {0}", attDefinition.AttributeType));
                    }
                    Debug.WriteLine(string.Format("Table size: {0}b", tableDescription.TableSizeBytes));
                    Debug.WriteLine(string.Format("Table status: {0}", tableDescription.TableStatus));
                    Debug.WriteLine("====================================================");

                }
            }
        }










    }
}
