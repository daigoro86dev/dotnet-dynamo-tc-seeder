using Amazon.DynamoDBv2.DataModel;

namespace DynamoStudentManager.Models;

[DynamoDBTable("students")]
public class Student
{
    [DynamoDBHashKey("id")]
    public int Id { get; set; }

    [DynamoDBProperty("firstName")]
    public string FirstName { get; set; }

    [DynamoDBProperty("lastName")]
    public string LastName { get; set; }

    [DynamoDBProperty("class")]
    public int Class { get; set; }

    [DynamoDBProperty("country")]
    public string Country { get; set; }
}