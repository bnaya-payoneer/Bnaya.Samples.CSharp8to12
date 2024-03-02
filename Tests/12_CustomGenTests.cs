using Bnaya.CodeGeneration.BuilderPatternGeneration;
using Riok.Mapperly.Abstractions;
using System.Text.Json;

using Xunit.Abstractions;

namespace Tests;

public class CustomGenTests
{
    private readonly ITestOutputHelper _logger;

    public CustomGenTests(ITestOutputHelper logger)
    {
        _logger = logger;
    }


    [Fact]
    public void GenTest()
    {
        Person person = Person.CreateBuilder()
            .AddId(1)
            .AddName("Max")
            .AddEmail("max@gmail.com")
            .AddBirthday(DateTime.Now.AddDays(365 * 70))
            .Build();

        PersonDto dto = PersonMapper.Default.ToDto(person);
    }
}

[GenerateBuilderPattern]
public partial record Person(int Id, string Name)
{
    public required string Email { get; init; }
    public DateTime Birthday { get; init; }
}

public  class PersonDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public string Email { get; init; }
    public DateTime Birthday { get; init; }
}

[Mapper]
public partial class PersonMapper
{
    public static readonly PersonMapper Default = new();
    public partial Person ToPerson(PersonDto dto);
    public partial PersonDto ToDto(Person person);
}
