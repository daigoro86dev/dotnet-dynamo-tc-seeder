using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Alba;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DynamoStudentManager.Models;
using Snapshooter.Xunit;

namespace DynamoStudentManagerTests;

[Collection("StudentTests")]
public class StudentsTests : IAsyncLifetime
{
    private IAlbaHost _host;

    private readonly TestcontainersContainer TestContainer = new TestcontainersBuilder<TestcontainersContainer>()
               .WithImage("localstack/localstack")
               .WithCleanUp(true)
               .WithEnvironment("DEFAULT_REGION", "us-east-1")
               .WithEnvironment("SERVICES", "dynamodb")
               .WithEnvironment("DOCKER_HOST", "unix:///var/run/docker.sock")
               .WithEnvironment("DEBUG", "1")
               .WithPortBinding(4566, 4566).Build();

    public async Task InitializeAsync()
    {
        await TestContainer.StartAsync();
        _host = await AlbaHost.For<Program>();

        await _host.Scenario(_ =>
        {
            _.Post.Url("/api/setup");
            _.StatusCodeShouldBeOk();
        });
    }

    [Fact]
    public async Task Test_Get_All_Students()
    {
        var result = await _host.GetAsJson<List<Student>>("/api/students");

        Snapshot.Match(result);
    }

    [Fact]
    public async Task Test_Get_Student_By_Id()
    {
        var result = await _host.GetAsJson<Student>("/api/students/1");

        Snapshot.Match(result);
    }

    [Fact]
    public async Task Test_Delete_Student()
    {
        await _host.Scenario(_ =>
       {
           _.Delete.Url("/api/students/1");
           _.StatusCodeShouldBe(HttpStatusCode.NoContent);
       });
    }

    public async Task DisposeAsync()
    {
        await TestContainer.DisposeAsync();
    }
}