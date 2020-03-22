using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Web.API.Application.Models;
using Web.API.Application.Repository;
using Web.API.Resources;
using Serilog;

namespace Web.API.Infrastructure.Data
{
    public class UtilizationRepository : IUtilizationRepository
    {
        private readonly string connectionString = string.Empty;

        public UtilizationRepository(string connectionString)
        {
            this.connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString));
        }
        public async Task<IEnumerable<rawUtilization>> GetUtilizationOfUser(string userId)
        {
            var sql = @"select isConfirmed, ceiling(sum(ProjectedMonthlyHours)/176.0 * 100) as Utilization
                        from positions as pos, projects as p
                        where pos.ProjectId = p.Id
                        and (p.ProjectStartDate between CURRENT_TIMESTAMP and DATEADD(year, 2, CURRENT_TIMESTAMP) 
                                or ProjectEndDate between CURRENT_TIMESTAMP and DATEADD(year, 2, CURRENT_TIMESTAMP))
                                and (ResourceId = @UserId)
                        group by IsConfirmed;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var resultOfQuery = await connection.QueryAsync<rawUtilization>(sql, new {UserId = userId});
            return resultOfQuery;
        }

    }
}