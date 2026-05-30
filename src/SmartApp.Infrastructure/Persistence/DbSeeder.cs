namespace SmartApp.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using SmartApp.Domain.Entities;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.Employees.AnyAsync(cancellationToken))
            return;

        var employees = new[]
        {
            Employee.Create("Alice",   "Johnson",  "alice.johnson@smartapp.com",  "Engineering", "Senior Developer",   95000m,  new DateTime(2020, 3,  15)),
            Employee.Create("Bob",     "Smith",    "bob.smith@smartapp.com",      "Finance",     "Financial Analyst",  75000m,  new DateTime(2019, 7,   1)),
            Employee.Create("Carol",   "Williams", "carol.williams@smartapp.com", "HR",          "HR Manager",         80000m,  new DateTime(2018, 11, 20)),
            Employee.Create("David",   "Brown",    "david.brown@smartapp.com",    "Engineering", "DevOps Engineer",    90000m,  new DateTime(2021, 1,  10)),
            Employee.Create("Eva",     "Martinez", "eva.martinez@smartapp.com",   "Marketing",   "Marketing Lead",     70000m,  new DateTime(2022, 5,  30)),
            Employee.Create("Frank",   "Lee",      "frank.lee@smartapp.com",      "Engineering", "Backend Developer",  85000m,  new DateTime(2020, 9,  14)),
            Employee.Create("Grace",   "Taylor",   "grace.taylor@smartapp.com",   "Finance",     "CFO",               150000m, new DateTime(2017, 2,   1)),
            Employee.Create("Henry",   "Anderson", "henry.anderson@smartapp.com", "Engineering", "Tech Lead",         110000m, new DateTime(2019, 4,  22)),
            Employee.Create("Isla",    "Thomas",   "isla.thomas@smartapp.com",    "HR",          "Recruiter",          60000m,  new DateTime(2023, 8,   7)),
            Employee.Create("James",   "Jackson",  "james.jackson@smartapp.com",  "Operations",  "Operations Manager", 88000m,  new DateTime(2020, 12,  3)),
            Employee.Create("Karen",   "White",    "karen.white@smartapp.com",    "Engineering", "QA Engineer",        72000m,  new DateTime(2021, 6,  18)),
            Employee.Create("Liam",    "Harris",   "liam.harris@smartapp.com",    "Marketing",   "SEO Specialist",     65000m,  new DateTime(2022, 3,   5)),
        };

        await context.Employees.AddRangeAsync(employees, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}