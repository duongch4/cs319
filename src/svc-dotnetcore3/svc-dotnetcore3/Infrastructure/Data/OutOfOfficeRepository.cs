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

        public async Task<IEnumerable<OutOfOffice>> GetAllOutOfOfficeForUser(string userId)
        {
            var sql = @"
                select * from OutOfOffice where ResourceId = @Id;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<OutOfOffice>(sql, new {Id = userId});
        }

        public async Task<OutOfOffice> DeleteOutOfOffice(OutOfOffice avail) {
            var sql = @"
                delete from OutOfOffice where ResourceId = @ResourceId AND FromDate = @FromDate AND ToDate = @ToDate";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            await connection.ExecuteAsync(sql, new {
                ResourceId = avail.ResourceId,
                FromDate = avail.FromDate,
                ToDate = avail.ToDate
            });
            return avail;
        }

        public async Task<OutOfOffice> InsertOutOfOffice(OutOfOffice avail) {
            var sql = @"
                insert into OutOfOffice (ResourceId, FromDate, ToDate, Reason)
                values
                (@ResourceId, @FromDate, @ToDate, @Reason)
            ";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            await connection.QueryFirstOrDefaultAsync(sql, new {
                ResourceId = avail.ResourceId,
                FromDate = avail.FromDate,
                ToDate = avail.ToDate,
                Reason = avail.Reason
            });
            return avail;
        }
    }
}
