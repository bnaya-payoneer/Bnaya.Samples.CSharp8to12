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
        var newTodo = await _connection.QueryFirstOrDefaultAsync<Todo>(
                                           """
                                           INSERT INTO Todos (TaskName, Completion)
                                           VALUES (@TaskName, @Completion); 
                                           SELECT * FROM Todos WHERE Id = SCOPE_IDENTITY();
                                           """, 
                                           todo); 
        return CreatedAtAction(nameof(GetByIdAsync), new { id = newTodo.Id }, newTodo);
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
