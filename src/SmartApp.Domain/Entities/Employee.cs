namespace SmartApp.Domain.Entities;

public sealed class Employee
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Department { get; private set; } = string.Empty;
    public string Position { get; private set; } = string.Empty;
    public decimal Salary { get; private set; }
    public DateTime JoinedAt { get; private set; }
    public bool IsActive { get; private set; }

    private Employee() { }

    public static Employee Create(
        string firstName,
        string lastName,
        string email,
        string department,
        string position,
        decimal salary,
        DateTime joinedAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(department);
        ArgumentException.ThrowIfNullOrWhiteSpace(position);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(salary);

        return new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName,
            Email = email.ToLowerInvariant(),
            Department = department,
            Position = position,
            Salary = salary,
            JoinedAt = joinedAt,
            IsActive = true
        };
    }

    public void Deactivate() => IsActive = false;
}