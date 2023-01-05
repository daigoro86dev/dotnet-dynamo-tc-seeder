using System.Net;
using System.Threading.Tasks;
using Alba;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DynamoStudentManager.Models;

namespace DynamoStudentManagerTests;

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
        await _host.Scenario(_ =>
       {
           _.Get.Url("/api/students");
           _.StatusCodeShouldBeOk();
       });
    }

    [Fact]
    public async Task Test_Get_Student_By_Id()
    {
        var result = await _host.GetAsJson<Student>("/api/students/1");

        Assert.Equal(1, result.Id);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("Doe", result.LastName);
        Assert.Equal(10, result.Class);
        Assert.Equal("Ireland", result.Country);
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