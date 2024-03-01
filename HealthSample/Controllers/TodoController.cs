using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Dapper;

namespace HealthSample.Controllers;
[ApiController]
[Route("[controller]")]
public class TodoController : ControllerBase
{
    private readonly ILogger<TodoController> _logger;
    private readonly SqlConnection _connection;

    public TodoController(
        ILogger<TodoController> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        var connectionString = configuration.GetConnectionString("SqlServerConnection");
        _connection = new SqlConnection(connectionString);
    }

    [HttpGet]
    [ProducesResponseType<Todo[]>(200)]
    public async Task<IActionResult> GetAsync()
    {
        var todos = await _connection.QueryAsync<Todo>("SELECT * FROM Todos");
        return Ok(todos.ToArray());
    }

    [HttpGet("{id}")]
    [ProducesResponseType<Todo>(200)]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var todo = await _connection.QueryFirstOrDefaultAsync<Todo>(
                                "SELECT * FROM Todos WHERE Id = @Id", new { Id = id });
        return Ok(todo);
    }

    [HttpPost]
    [ProducesResponseType<Todo>(201)]
    public async Task<IActionResult> PostAsync(Todo todo)
    {
        await _connection.ExecuteAsync(
                                           """
                                           INSERT INTO Todos (Id, TaskName, Completion)
                                           VALUES (@Id, @TaskName, @Completion); 
                                           """,
                                           todo);
        return Ok(todo);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> PutAsync(int id, Todo todo)
    {
        await _connection.ExecuteAsync(
                       "UPDATE Todos SET TaskName = @TaskName, Completion = @Completion WHERE Id = @Id",
                                  new { Id = id, todo.TaskName, todo.Completion });
        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        await _connection.ExecuteAsync("DELETE FROM Todos WHERE Id = @Id", new { Id = id });
        return NoContent();
    }
}
