using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Web.API.Application.Models;
using Web.API.Application.Repository;

namespace Web.API.Infrastructure.Data
{
    public class OutOfOfficeRepository : IOutOfOfficeRepository
    {
        private readonly string connectionString = string.Empty;

        public OutOfOfficeRepository(string connectionString)
        {
            this.connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<IEnumerable<OutOfOffice>> GetAllOutOfOfficeForUser(User user)
        {
            var sql = @"
                select * from OutOfOffice where ResourceId = "+user.Id+";";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<OutOfOffice>(sql);
        }
    }
}
