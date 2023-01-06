using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using DynamoStudentManager.Models;
using Microsoft.AspNetCore.Mvc;

namespace DynamoStudentManager.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StudentsController : ControllerBase
{
    private readonly IDynamoDBContext _context;

    public StudentsController(IDynamoDBContext context)
    {
        _context = context;
    }

    [HttpGet("{studentId}")]
    public async Task<IActionResult> GetBydId(int studentId)
    {
        var student = await _context.LoadAsync<Student>(studentId);
        if (student == null) return NotFound();

        return Ok(student);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllStudents()
    {
        var msUrl = Environment.GetEnvironmentVariable("MS_URL");
        var response = await new HttpClient().GetAsync($"{msUrl}/test");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var students = await _context.ScanAsync<Student>(default).GetRemainingAsync();
            return Ok(students);
        }
        return ValidationProblem();
    }

    [HttpPost]
    public async Task<IActionResult> CreateStudent(Student studentRequest)
    {
        var student = await _context.LoadAsync<Student>(studentRequest.Id);
        if (student != null) return BadRequest($"Student with {studentRequest.Id} already exists");

        await _context.SaveAsync(studentRequest);
        return Ok(studentRequest);
    }

    [HttpDelete("{studentId}")]
    public async Task<IActionResult> DeleteStudent(int studentId)
    {
        var student = await _context.LoadAsync<Student>(studentId);
        if (student == null) return NotFound();
        await _context.DeleteAsync(student);
        return NoContent();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateStudent(Student studentRequest)
    {
        var student = await _context.LoadAsync<Student>(studentRequest.Id);
        if (student == null) return NotFound();
        await _context.SaveAsync(studentRequest);
        return Ok(studentRequest);
    }
}