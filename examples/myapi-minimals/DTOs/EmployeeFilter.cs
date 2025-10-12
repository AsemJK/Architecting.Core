
namespace myapi_minimals.DTOs
{
    public class EmployeeFilter : IEndpointFilter
    {
        public string? Name { get; set; }
        public decimal? Salary { get; set; }

        public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            // You can add custom filtering logic here if needed
            var filter = context.Arguments.OfType<EmployeeFilter>().FirstOrDefault();
            if (filter != null)
            {
                // Example: Log the filter criteria
                Console.WriteLine($"Filtering employees with Name: {filter.Name}, Salary: {filter.Salary}");
                if (filter.Salary.HasValue && filter.Salary < 0)
                {
                    return ValueTask.FromResult<object?>(Results.BadRequest("Salary must be non-negative"));
                }
            }
            return next(context);
        }
    }
}
