using Dapper;
using Microsoft.Data.SqlClient;
using myapi_minimals.infra.Models;
using System.Data;
namespace myapi_minimals.Repository
{
    public class DapperRepository
    {
        private readonly string _configuration;

        public DapperRepository(IConfiguration configuration)
        {
            _configuration = configuration.GetConnectionString("DefaultConnection");
        }
        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_configuration);
        }

        public Task<IEnumerable<Employee>> GetEmployeesAsync(string search = "")
        {
            var connection = CreateConnection();
            var sql = "SELECT * FROM Employees";
            if (!string.IsNullOrEmpty(search))
            {
                sql += " WHERE Name LIKE @Search OR Position LIKE @Search";
                search = $"%{search}%";
            }
            return connection.QueryAsync<Employee>(sql, new { Search = search });
        }
    }
}
