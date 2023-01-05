using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using DynamoStudentManager.Models;
using Microsoft.AspNetCore.Mvc;

namespace DynamoStudentManager.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SetupController : ControllerBase
{
    private readonly IDynamoDBContext _context;
    private readonly IAmazonDynamoDB _amazonDynamoDb;

    public SetupController(IDynamoDBContext context, IAmazonDynamoDB amazonDynamoDb)
    {
        _context = context;
        _amazonDynamoDb = amazonDynamoDb;
    }

    [HttpPost]
    public async Task<IActionResult> Setup()
    {
        var studentsData = System.IO.File.ReadAllText("./StudentsData.json");
        var students = JsonSerializer.Deserialize<List<Student>>(studentsData);


        var createRequest = new CreateTableRequest
        {
            TableName = "students",
            AttributeDefinitions = new List<AttributeDefinition>
                    {
                        new AttributeDefinition
                        {
                            AttributeName = "id",
                            AttributeType = "N"
                        }
                    },
            KeySchema = new List<KeySchemaElement>
                    {
                        new KeySchemaElement
                        {
                            AttributeName = "id",
                            KeyType = "HASH"
                        }
                    },
            ProvisionedThroughput = new ProvisionedThroughput
            {
                ReadCapacityUnits = 2,
                WriteCapacityUnits = 2
            }
        };

        await _amazonDynamoDb.CreateTableAsync(createRequest);

        foreach (var student in students)
        {
            await _context.SaveAsync(student);
        }

        return Ok(students);
    }
}