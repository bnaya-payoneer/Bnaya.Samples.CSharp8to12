namespace Bnaya.Samples.Entities;

public record Person (
    int Id,
    string FirstName,
    string LastName,
    [PersonalData]
    string Phone,
    [SensitiveData] 
    string password);


