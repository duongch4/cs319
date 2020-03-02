using Dapper;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Web.API.Application.Models;
using Web.API.Application.Repository;
using Web.API.Application.Communication;
using Web.API.Resources;

using Serilog;

namespace Web.API.Infrastructure.Data
{
    public class UsersRepository : IUsersRepository
    {
        private readonly string connectionString = string.Empty;

        public UsersRepository(string connectionString)
        {
            this.connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            var sql = @"
                select
                    Id, FirstName, LastName, Username, LocationId, IsAdmin, IsManager
                from
                    Users
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<User>(sql);
        }

        public async Task<User> GetAUser(int userId)
        {
            var sql = @"
                select
                    Id, FirstName, LastName, Username, LocationId, IsAdmin, IsManager
                from
                    Users
                where 
                    Id = @Id";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Id = userId });
        }

        public async Task<IEnumerable<User>> GetAllUsersAtLocation(Location location)
        {
            var sql = @"
                select 
                    Id, FirstName, LastName, Username, LocationId, IsAdmin, IsManager
                from Users
                where LocationId = @LocationId
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<User>(sql, new { LocationId = location.Id });
        }

        public async Task<IEnumerable<User>> GetAllUsersWithDiscipline(Discipline discipline)
        {
            var sql = @"
                select 
                    u.Id, u.FirstName, u.LastName, u.Username, u.LocationId, u.IsAdmin, u.IsManager
                from Users as u, ResourceDiscipline rd
                where rd.DisciplineId = @DisciplineId
                    and rd.ResourceId = u.Id
            ;";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<User>(sql, new { DisciplineId = discipline.Id });
        }

        public async Task<IEnumerable<User>> GetAllUsersWithSkill(Skill skill)
        {
            var sql = @"
                select 
                    u.Id, u.FirstName, u.LastName, u.Username, u.LocationId, u.IsAdmin, u.IsManager
                from Users as u, ResourceSkill as rs
                where rs.SkillId = @SkillId
                    and rs.ResourceId = u.Id
            ;";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<User>(sql, new { SkillId = skill.Id });
        }

        // //TODO: @Chi plz
        // public async Task<IEnumerable<User>> GetAllUsersWithAvailability(Availability requestedAvailability)
        // {
        //     return null;
        // }

        // public async Task<IEnumerable<User>> GetAllUsersOverNUtilization(int nUtil) 
        // {
        //     return null;
        // }

        public async Task<IEnumerable<User>> GetAllUsersOnProject(Project project)
        {
            var sql = @"
                select 
                    u.Id, u.FirstName, u.LastName, u.Username, u.LocationId, u.IsAdmin, u.IsManager
                from 
                    Users as u, Positions pos
                where pos.ProjectId = @ProjectId
                    and pos.ResourceId = u.Id
            ;";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<User>(sql, new { ProjectId = project.Id });
        }

        public async Task<IEnumerable<UserResource>> GetAllUserResourcesOnProject(int projectId, int projectManagerId)
        {
            var sql = @"
                SELECT *
                FROM
                (
                    SELECT
                        Utilization,
                        util.Id, FirstName, LastName, Username, LocationId,
                        util.IsConfirmed, d.Name AS DisciplineName,
                        rd.YearsOfExperience,
                        Province, City
                    FROM
                    (
                        SELECT
                            CEILING(SUM(ij.ProjectedMonthlyHours)/176.0*100.0) AS Utilization,
                            ij.Id, ij.FirstName, ij.LastName, ij.Username, ij.LocationId,
                            ij.IsConfirmed,
                            ij.Province, ij.City
                        FROM
                        (
                            SELECT
                                u.Id, u.FirstName, u.LastName, u.Username, u.LocationId,
                                p.IsConfirmed, p.ProjectedMonthlyHours,
                                l.Province, l.City
                            FROM
                                Users u
                            LEFT JOIN Positions p ON p.ResourceId = u.Id
                            LEFT JOIN Projects proj
                                ON
                                    proj.Id = p.ProjectId
                                    AND proj.ProjectEndDate <= @DateInTwoYears
                            INNER JOIN Locations l ON u.LocationId = l.Id
                        ) AS ij
                        GROUP BY
                            ij.Id, ij.FirstName, ij.LastName, ij.Username, ij.LocationId,
                            ij.IsConfirmed,
                            ij.Province, ij.City
                    ) AS util
                    INNER JOIN ResourceDiscipline rd
                        ON
                            rd.ResourceId = util.Id
                    INNER JOIN Disciplines d
                        ON
                            rd.DisciplineId = d.Id
                    INNER JOIN Positions p
                        ON
                            p.ResourceId = util.Id
                            AND p.ProjectId = @ProjectId
                    WHERE
                        util.Id != @ProjectManagerId
                ) AS final
                GROUP BY
                    Utilization,
                    Id, FirstName, LastName, Username, LocationId,
                    IsConfirmed, DisciplineName,
                    YearsOfExperience,
                    Province, City
                ORDER BY
                    CASE WHEN (@OrderKey = 'utilization' AND @Order = 'asc') THEN final.Utilization END ASC,
                    CASE WHEN (@OrderKey = 'utilization' AND @Order = 'desc') THEN final.Utilization END DESC
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<UserResource>(sql, new
            {
                DateInTwoYears = DateTime.Today.AddYears(2),
                ProjectId = projectId,
                ProjectManagerId = projectManagerId,
                OrderKey = "utilization",
                Order = "asc"
            });
        }

        public async Task<IEnumerable<UserResource>> GetAllUserResources(string orderKey, string order, int page)
        {
            var sql = @"
                SELECT *
                FROM
                (
                    SELECT
                        CEILING(SUM(ij.ProjectedMonthlyHours)/176.0*100.0) AS Utilization,
                        ij.Id, ij.FirstName, ij.LastName, ij.Username, ij.LocationId,
                        ij.IsConfirmed,
                        ij.Province, ij.City
                    FROM
                    (
                        SELECT
                            u.Id, u.FirstName, u.LastName, u.Username, u.LocationId,
                            p.IsConfirmed, p.ProjectedMonthlyHours,
                            l.Province, l.City
                        FROM
                            Users u
                        LEFT JOIN Positions p ON p.ResourceId = u.Id
                        LEFT JOIN Projects proj
                            ON
                                proj.Id = p.ProjectId
                                AND proj.ProjectEndDate <= @DateInTwoYears
                        INNER JOIN Locations l ON u.LocationId = l.Id
                    ) AS ij
                    GROUP BY
                        ij.Id, ij.FirstName, ij.LastName, ij.Username, ij.LocationId,
                        ij.IsConfirmed,
                        ij.Province, ij.City
                ) AS final
                ORDER BY
                    CASE WHEN (@OrderKey = 'utilization' AND @Order = 'asc') THEN final.Utilization END ASC,
                    CASE WHEN (@OrderKey = 'utilization' AND @Order = 'desc') THEN final.Utilization END DESC,

                    CASE WHEN (@OrderKey = 'province' AND @Order = 'asc') THEN final.Province END ASC,
                    CASE WHEN (@OrderKey = 'province' AND @Order = 'desc') THEN final.Province END DESC,

                    CASE WHEN (@OrderKey = 'city' AND @Order = 'asc') THEN final.City END ASC,
                    CASE WHEN (@OrderKey = 'city' AND @Order = 'desc') THEN final.City END DESC
                    
                    OFFSET (@PageNumber - 1) * @RowsPerPage ROWS
                    FETCH NEXT @RowsPerPage ROWS ONLY
            ;";

                    // CASE WHEN @OrderKey = 'startDate' THEN final.ProjectEndDate END ASC,
                    // CASE WHEN @OrderKey = 'endDate' THEN final.ProjectEndDate END ASC

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var users = await connection.QueryAsync<UserResource>(sql, new
            {
                DateInTwoYears = DateTime.Today.AddYears(2),
                OrderKey = orderKey,
                Order = order,
                PageNumber = page,
                RowsPerPage = 50
            });

            return users;
        }

        public async Task<IEnumerable<UserResource>> GetAllUserResourcesOnFilter(RequestSearchUsers req)
        {
            var provinces = new HashSet<string>();
            var cities = new HashSet<string>();
            foreach (var location in req.Filter.Locations)
            {
                provinces.Add(location.Province);
                cities.Add(location.City);
            }

            var sql = @"
                SELECT *
                FROM
                (
                    SELECT
                        Utilization,
                        util.Id, FirstName, LastName, Username, LocationId,
                        IsConfirmed, d.Name AS DisciplineName,
                        rd.YearsOfExperience,
                        Province, City
                    FROM
                    (
                        SELECT
                            CEILING(SUM(ij.ProjectedMonthlyHours)/176.0*100.0) AS Utilization,
                            ij.Id, ij.FirstName, ij.LastName, ij.Username, ij.LocationId,
                            ij.IsConfirmed,
                            ij.Province, ij.City
                        FROM
                        (
                            SELECT
                                u.Id, u.FirstName, u.LastName, u.Username, u.LocationId,
                                p.IsConfirmed, p.ProjectedMonthlyHours,
                                l.Province, l.City
                            FROM
                                Users u
                            LEFT JOIN Positions p ON p.ResourceId = u.Id
                            LEFT JOIN Projects proj
                                ON
                                    proj.Id = p.ProjectId
                                    AND proj.ProjectEndDate <= @DateInTwoYears
                            INNER JOIN Locations l ON u.LocationId = l.Id
                        ) AS ij
                        GROUP BY
                            ij.Id, ij.FirstName, ij.LastName, ij.Username, ij.LocationId,
                            ij.IsConfirmed,
                            ij.Province, ij.City
                    ) AS util
                    LEFT JOIN OutOfOffice o
                        ON
                            o.ResourceId = util.Id
                            AND (o.FromDate > @EndDate OR o.ToDate < @StartDate)
                    INNER JOIN ResourceDiscipline rd
                        ON
                            rd.ResourceId = util.Id
                            AND rd.YearsOfExperience IN @YearsOfExps
                    INNER JOIN Disciplines d
                        ON
                            rd.DisciplineId = d.Id
                            AND d.Name IN @Disciplines
                    INNER JOIN Skills s
                        ON
                            s.DisciplineId = d.Id
                            AND s.Name IN @Skills
                    WHERE
                        util.Province IN @Provinces
                        AND util.City IN @Cities
                        AND util.Utilization > @UtilMin
                        AND util.Utilization < @UtilMax
                ) AS final
                GROUP BY
                    Utilization,
                    Id, FirstName, LastName, Username, LocationId,
                    IsConfirmed, DisciplineName,
                    YearsOfExperience,
                    Province, City
                ORDER BY
                    CASE WHEN (@OrderKey = 'utilization' AND @Order = 'asc') THEN final.Utilization END ASC,
                    CASE WHEN (@OrderKey = 'utilization' AND @Order = 'desc') THEN final.Utilization END DESC,

                    CASE WHEN (@OrderKey = 'province' AND @Order = 'asc') THEN final.Province END ASC,
                    CASE WHEN (@OrderKey = 'province' AND @Order = 'desc') THEN final.Province END DESC,

                    CASE WHEN (@OrderKey = 'city' AND @Order = 'asc') THEN final.City END ASC,
                    CASE WHEN (@OrderKey = 'city' AND @Order = 'desc') THEN final.City END DESC,

                    CASE WHEN (@OrderKey = 'discipline' AND @Order = 'asc') THEN final.DisciplineName END ASC,
                    CASE WHEN (@OrderKey = 'discipline' AND @Order = 'desc') THEN final.DisciplineName END DESC,
                    
                    CASE WHEN (@OrderKey = 'yearsOfExp' AND @Order = 'asc') THEN final.YearsOfExperience END ASC,
                    CASE WHEN (@OrderKey = 'yearsOfExp' AND @Order = 'desc') THEN final.YearsOfExperience END DESC
                    
                    OFFSET (@PageNumber - 1) * @RowsPerPage ROWS
                    FETCH NEXT @RowsPerPage ROWS ONLY
            ;";

                    // CASE WHEN @OrderKey = 'startDate' THEN final.ProjectEndDate END ASC,
                    // CASE WHEN @OrderKey = 'endDate' THEN final.ProjectEndDate END ASC

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var users = await connection.QueryAsync<UserResource>(sql, new
            {
                DateInTwoYears = DateTime.Today.AddYears(2),
                Provinces = provinces,
                Cities = cities,
                Disciplines = req.Filter.Disciplines.Keys.ToArray(),
                Skills = req.Filter.Disciplines.Values.SelectMany(x => x),
                YearsOfExps = req.Filter.YearsOfExps,
                EndDate = req.Filter.EndDate,
                StartDate = req.Filter.StartDate,
                UtilMin = req.Filter.Utilization.Min,
                UtilMax = req.Filter.Utilization.Max,
                OrderKey = (req.OrderKey == null || req.OrderKey == "") ? "utilization": req.OrderKey,
                Order = (req.Order == null || req.Order == "") ? "desc" : req.Order,
                PageNumber = req.Page,
                RowsPerPage = 50
            });

            return users;
        }

        // public async Task<IEnumerable<User>> GetAllUsersWithYearsOfExp(Discipline discipline, int yrsOfExp)
        // {
        //     return null;
        // }

        public async Task<User> GetPMOfProject(Project project)
        {
            var sql = @"
                select *
                from 
                    User
                where Id = @ProjectManagerId
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QuerySingleAsync<User>(sql, new { ProjectManagerId = project.ManagerId });
        }

        public async Task<User> UpdateAUser(User user)
        {
            var sql = @"
                update
                    Users
                set 
                    FirstName = @FirstName,
                    LastName = @LastName,
                    LocationId = @LocationId
                where 
                    Id = @Id
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            int result = await connection.ExecuteAsync(sql, new
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                LocationId = user.LocationId
            });
            return result == 1 ? user : null;
        }

        public async Task<IEnumerable<UserResource>> GetAllUsersGeneral()
        {
            var sql = @"
                select
                    u.Id, u.FirstName, u.LastName, u.Username, u.LocationId, l.City, l.Province
                from
                    Users as u, Locations as l
                where
                    u.LocationId = l.Id
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<UserResource>(sql);
        }
    }
}
