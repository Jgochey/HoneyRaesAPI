using System.ComponentModel;
using HoneyRaesAPI.Models;

List<Customer> customers = new List<Customer>
{
    new Customer { Id = 1,  Name = "Bob", Address = "123 Main St." },
    new Customer { Id = 2,  Name = "Sue", Address = "456 Api Rd." },
    new Customer { Id = 3,  Name = "Terry", Address = "789 Git Blvd." },

    new Customer { Id = 4,  Name = "Jenkins" , Address = "Expired Ticket Street." },
    new Customer { Id = 5,  Name = "Guy" , Address = "Expired Ticket Street." },
 };


List<Employee> employees = new List<Employee>
{
    new Employee { Id = 1, Name = "Frank", Specialty = "Cleaning" },
    new Employee { Id = 2, Name = "Jamie", Specialty = "Baking" },
    new Employee { Id = 3, Name = "Johnny", Specialty = "Gambling" },
};


List<ServiceTicket> serviceTickets = new List<ServiceTicket>
{
    new ServiceTicket { Id = 1, CustomerId = 1, EmployeeId = 1, Description = "Clean the house", Emergency = false, DateCompleted = new DateTime(2025, 1, 21) },
    new ServiceTicket { Id = 2, CustomerId = 2, EmployeeId = 2, Description = "Bake a cake", Emergency = true, DateCompleted = new DateTime(2025, 1, 29) },
    new ServiceTicket { Id = 3, CustomerId = 3, EmployeeId = 2, Description = "Clean the car", Emergency = false, DateCompleted = new DateTime(2025, 1, 29) },
    new ServiceTicket { Id = 4, CustomerId = 3, Description = "Build a castle", Emergency = false, },
    new ServiceTicket { Id = 5, CustomerId = 1, EmployeeId = 2, Description = "Walk the Dog", Emergency = false, DateCompleted = new DateTime(2025, 1, 29) },

    new ServiceTicket { Id = 6, CustomerId = 4, EmployeeId = 1, Description = "Be an emergency but also complete.", Emergency = true, DateCompleted = new DateTime(1999, 1, 21) },
    new ServiceTicket { Id = 7, CustomerId = 2, EmployeeId = 1, Description = "Be on top!", Emergency = true, },
    new ServiceTicket { Id = 8, CustomerId = 3, EmployeeId = 1, Description = "Be an emergency", Emergency = true, },
    new ServiceTicket { Id = 9, CustomerId = 1, EmployeeId = 1, Description = "Be incomplete", Emergency = false, },
    new ServiceTicket { Id = 10, CustomerId = 2, EmployeeId = 2, Description = "Also be incomplete", Emergency = false, },
    new ServiceTicket { Id = 11, CustomerId = 3, Description = "Be an emergency and also be unassigned", Emergency = true, },

    new ServiceTicket { Id = 12, CustomerId = 4, EmployeeId = 1, Description = "Be completed a long time ago.", Emergency = true, DateCompleted = new DateTime(1999, 1, 21) },
    new ServiceTicket { Id = 13, CustomerId = 5, EmployeeId = 1, Description = "Be completed slightly after that.", Emergency = true, DateCompleted = new DateTime(2000, 1, 22) },
    new ServiceTicket { Id = 14, CustomerId = 4, EmployeeId = 1, Description = "A little more.", Emergency = true, DateCompleted = new DateTime(2000, 1, 23) },
    new ServiceTicket { Id = 15, CustomerId = 4, EmployeeId = 1, Description = "Little more.", Emergency = false, DateCompleted = new DateTime(2002, 1, 21) },
    new ServiceTicket { Id = 16, CustomerId = 5, EmployeeId = 2, Description = "Even more than that.", Emergency = false, DateCompleted = new DateTime(2003, 1, 21) },
    new ServiceTicket { Id = 17, CustomerId = 3, Description = "Be completed recently.", Emergency = true, DateCompleted = new DateTime(2025, 1, 21) },
 };

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/servicetickets", () =>
{
    return Results.Ok(serviceTickets);
});


app.MapGet("/servicetickets/{id}", (int id) =>
{
    ServiceTicket serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);
    if (serviceTicket == null)
    {
        return Results.NotFound();
    }
    serviceTicket.Employee = employees.FirstOrDefault(e => e.Id == serviceTicket.EmployeeId);
    serviceTicket.Customer = customers.FirstOrDefault(e => e.Id == serviceTicket.CustomerId);
    return Results.Ok(serviceTicket);
});


app.MapGet("/employees", () =>
{
    return Results.Ok(employees);
});


app.MapGet("/employees/{id}", (int id) =>
{
    Employee employee = employees.FirstOrDefault(e => e.Id == id);
    if (employee == null)
    {
        return Results.NotFound();
    }
    employee.ServiceTickets = serviceTickets.Where(st => st.EmployeeId == id).ToList();
    return Results.Ok(employee);
});


app.MapGet("/customers", () =>
{
    return Results.Ok(customers);
});


app.MapGet("/customers/{id}", (int id) =>
{
    Customer customer = customers.FirstOrDefault(c => c.Id == id);
    if (customer == null)
    {
        return Results.NotFound();
    }
    customer.ServiceTickets = serviceTickets.Where(st => st.CustomerId == id).ToList();

    return Results.Ok(customer);
});


app.MapPost("/servicetickets", (ServiceTicket serviceTicket) =>
{
    // creates a new id (When we get to it later, our SQL database will do this for us like JSON Server did!)
    serviceTicket.Id = serviceTickets.Max(st => st.Id) + 1;
    serviceTickets.Add(serviceTicket);
    return Results.Ok(serviceTicket);
});


app.MapDelete("/servicetickets/{id}", (int id) =>
{
    ServiceTicket serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);

    serviceTickets.Remove(serviceTicket);

    return Results.Ok(serviceTicket);
});


app.MapPut("/servicetickets/{id}", (int id, ServiceTicket serviceTicket) =>
{
    ServiceTicket ticketToUpdate = serviceTickets.FirstOrDefault(st => st.Id == id);
    int ticketIndex = serviceTickets.IndexOf(ticketToUpdate);
    if (ticketToUpdate == null)
    {
        return Results.NotFound();
    }
    //the id in the request route doesn't match the id from the ticket in the request body. That's a bad request!
    if (id != serviceTicket.Id)
    {
        return Results.BadRequest();
    }
    serviceTickets[ticketIndex] = serviceTicket;
    return Results.Ok(ticketToUpdate);
});


app.MapPost("/servicetickets/{id}/complete", (int id) =>
{
    ServiceTicket ticketToComplete = serviceTickets.FirstOrDefault(st => st.Id == id);
    if (ticketToComplete == null)
    {
        return Results.NotFound();
    }

    ticketToComplete.DateCompleted = DateTime.Today;
    return Results.Ok(ticketToComplete);

});


// 1. Emergencies
// Create an endpoint to return all of the service tickets that are incomplete and are emergencies

app.MapGet("/servicetickets/emergencies", () =>
{
    var emergencies = serviceTickets.Where(st => st.Emergency == true && st.DateCompleted == null);

    return Results.Ok(emergencies);
});


// 2. Unassigned
// Create an endpoint to return all currently unassigned service tickets

app.MapGet("/servicetickets/unassigned", () =>
{
    var unassigned = serviceTickets.Where(st => st.EmployeeId == null);

    return Results.Ok(unassigned);
});


// 3. Inactive Customers
// Create an endpoint to return all of the customers that haven't had a service ticket closed for them in over a year (refer to the explorer chapter in Book 1 on calculating DateTimes).

app.MapGet("/customers/inactive", () =>
{
    var oneYearAgo = DateTime.Today.AddYears(-1);

    var lastTicketDates = serviceTickets
    .Where(st => st.DateCompleted.HasValue)
    .GroupBy(st => st.CustomerId)
    .Select(g => new { CustomerId = g.Key, LastTicketDate = g.Max(st => st.DateCompleted) });
    // Look only at completed tickets, Group the tickets by customer, then for each customer, get the date of their most recent ticket.

    var inactiveCustomers = customers
    .Where(c => lastTicketDates.Any(lt => lt.CustomerId == c.Id && lt.LastTicketDate <= oneYearAgo))
    .Where(c => !serviceTickets.Any(st => st.CustomerId == c.Id && !st.DateCompleted.HasValue)).ToList();
    // Only get customers who have a ticket older than one year. If the customer does not have any completed tickets, don't include them.

    return Results.Ok(inactiveCustomers);
});


// 4. Available employees
// Create an endpoint to return employees not currently assigned to an incomplete service ticket

app.MapGet("/employees/available", () =>
{
    var activeTickets = serviceTickets.Where(st => st.DateCompleted == null);

    var availableEmployees = employees
    .Where(e => !activeTickets
    .Any(st => st.EmployeeId == e.Id))
    .ToList();

    return Results.Ok(availableEmployees);
});


// 5. Employee's customers
// Create an endpoint to return all of the customers for whom a given employee has been assigned to a service ticket (whether completed or not)

app.MapGet("/employees/{id}/assignedcustomers", (int id) =>
{
    var currentTickets = serviceTickets
    .Where(st => st.EmployeeId == id);

    var assignedCustomers = customers
    .Where(c => currentTickets
    .Any(st => st.CustomerId == c.Id))
    .ToList();


    return Results.Ok(assignedCustomers);
});


// 6. Employee of the month
// Create and endpoint to return the employee who has completed the most service tickets last month.

app.MapGet("/employees/topemployee", () =>
{

    var highestCount = 0;
    var topEmployee = 0;
    var lastMonth = DateTime.Today.AddMonths(-1);

    var monthlyTickets = serviceTickets
    .Where(st => st.DateCompleted.HasValue && st.DateCompleted.Value.Month == lastMonth.Month);

    foreach (var employee in employees)
    {
        var employeeTickets = monthlyTickets
        .Where(st => st.EmployeeId == employee.Id)
        .Count();

        if (employeeTickets > highestCount)
        {
            highestCount = employeeTickets;
            topEmployee = employee.Id;
        }
    }

    var employeeOfTheMonth = employees.FirstOrDefault(e => e.Id == topEmployee);

    return Results.Ok(employeeOfTheMonth);
});


// 7. Past Ticket review
// Create an endpoint to return completed tickets in order of the completion data, oldest first. (This will required a Linq method you haven't learned yet...)
app.MapGet("/servicetickets/review", () =>
{
    var completedTickets = serviceTickets
    .Where(st => st.DateCompleted.HasValue)
    .OrderBy(st => st.DateCompleted)
    .ToList();

    return Results.Ok(completedTickets);
});


// 8. Prioritized Tickets (challenge)
// Create an endpoint to return all tickets that are incomplete, in order first by whether they are emergencies, then by whether they are assigned or not (unassigned first).

app.MapGet("/servicetickets/priority", () =>
{
    var incompleteTickets = serviceTickets.Where(st => !st.DateCompleted.HasValue);

    var priorityTickets = incompleteTickets
    .OrderBy(st => st.Emergency != true)
    .ThenBy(st => st.EmployeeId != null)
    .ToList();

    return Results.Ok(priorityTickets);


});

app.Run();
