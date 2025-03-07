﻿using Dapper;
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
        // private readonly System.Data.SqlClient.SqlConnection connection;


        public UsersRepository(string connectionString)
        {
            this.connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString));
            // connection = new SqlConnection(connectionString);
            // connection.Open();
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            var sql = @"
                select
                    Id, FirstName, LastName, Username, LocationId, Utilization, IsAdmin, IsManager
                from
                    Users
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<User>(sql);
        }

        public async Task<User> GetAUser(string userId)
        {
            var sql = @"
                select
                    *
                from
                    Users
                where 
                    Id = @Id";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var sqlRes = await connection.QueryFirstOrDefaultAsync<User>(sql, new { Id = userId });
            return sqlRes;
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

        public async Task<UserResource> GetAUserResource(string userId)
        {
            var sql = @"
                SELECT
                    u.Id, u.FirstName, u.LastName, u.Username, u.LocationId, u. Utilization,
                    l.Province, l.City
                FROM
                    Users u, Locations l
                WHERE
                    u.LocationId = l.Id
                    AND u.Id = @UserId
            ";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryFirstOrDefaultAsync<UserResource>(sql, new
            {
                UserId = userId
            });
        }

        public async Task<IEnumerable<UserResource>> GetAllUserResourcesOnProject(int projectId, string projectManagerId)
        {
            var sql = @"
                SELECT
                    u.Id, u.FirstName, u.LastName, u.Username, u.LocationId, u.Utilization,
                    l.Province, l.City,
                    p.IsConfirmed, p.DisciplineId, p.YearsOfExperience,
                    d.Name AS DisciplineName,
                    STRING_AGG (s.Name, ',') as Skills
                FROM
                    Users u
                INNER JOIN Locations l
                    ON
                        u.LocationId = l.Id
                INNER JOIN Positions p
                    ON
                        p.ResourceId = u.Id
                INNER JOIN Disciplines d
                    ON
                        d.Id = p.DisciplineId
                LEFT JOIN PositionSkills ps
                    ON
                        p.Id = ps.PositionId
                LEFT JOIN Skills s
                    ON
                        ps.SkillId = s.Id
                        AND ps.SkillDisciplineId = s.DisciplineId
                WHERE
                    p.ProjectId = @ProjectId
                GROUP BY
                    u.Id, u.FirstName, u.LastName, u.Username, u.LocationId, u.Utilization,
                    l.Province, l.City,
                    p.IsConfirmed, p.DisciplineId, p.YearsOfExperience,
                    d.Name
                ORDER BY
                    u.Utilization ASC
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<UserResource>(sql, new
            {
                ProjectId = projectId,
                ProjectManagerId = projectManagerId
            });
        }

        public async Task<IEnumerable<UserResource>> GetAllUserResources(string searchWord, string orderKey, string order, int page, int rowsPerPage)
        {
            var sql = @"
                WITH Data_CTE AS
                (
                    SELECT
                        u.Id, u.FirstName, u.LastName, u.Username, u.LocationId, u. Utilization,
                        l.Province, l.City,
                        rd.DisciplineId, rd.YearsOfExperience,
                        d.Name AS DisciplineName
                    FROM
                        Users u
                    INNER JOIN Locations l
                        ON u.LocationId = l.Id
                    LEFT JOIN ResourceDiscipline rd
                        ON rd.ResourceId = u.Id
                    LEFT JOIN Disciplines d
                        ON rd.DisciplineId = d.Id
                    WHERE
                        LOWER(TRIM(u.FirstName)) LIKE @SearchWord
                        OR LOWER(TRIM(u.LastName)) LIKE @SearchWord
                ), 
                Count_CTE AS 
                (
                    SELECT CEILING(CAST(COUNT(*) AS FLOAT) / @RowsPerPage) AS MaxPages FROM Data_CTE
                )

                SELECT *
                FROM Data_CTE
                CROSS JOIN Count_CTE
                ORDER BY
                    CASE WHEN (@OrderKey = 'utilization' AND @Order = 'asc') THEN Data_CTE.Utilization END ASC,
                    CASE WHEN (@OrderKey = 'utilization' AND @Order = 'desc') THEN Data_CTE.Utilization END DESC,

                    CASE WHEN (@OrderKey = 'province' AND @Order = 'asc') THEN Data_CTE.Province END ASC,
                    CASE WHEN (@OrderKey = 'province' AND @Order = 'desc') THEN Data_CTE.Province END DESC,

                    CASE WHEN (@OrderKey = 'city' AND @Order = 'asc') THEN Data_CTE.City END ASC,
                    CASE WHEN (@OrderKey = 'city' AND @Order = 'desc') THEN Data_CTE.City END DESC,

                    CASE WHEN (@OrderKey = 'discipline' AND @Order = 'asc') THEN Data_CTE.DisciplineName END ASC,
                    CASE WHEN (@OrderKey = 'discipline' AND @Order = 'desc') THEN Data_CTE.DisciplineName END DESC,

                    CASE WHEN (@OrderKey = 'lastname' AND @Order = 'asc') THEN Data_CTE.LastName END ASC,
                    CASE WHEN (@OrderKey = 'lastname' AND @Order = 'desc') THEN Data_CTE.LastName END DESC,

                    CASE WHEN (@OrderKey = 'firstname' AND @Order = 'asc') THEN Data_CTE.FirstName END ASC,
                    CASE WHEN (@OrderKey = 'firstname' AND @Order = 'desc') THEN Data_CTE.FirstName END DESC
                OFFSET (@PageNumber - 1) * @RowsPerPage ROWS
                FETCH NEXT (@RowsPerPage + 1) ROWS ONLY
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var users = await connection.QueryAsync<UserResource>(sql, new
            {
                SearchWord = GetFilteredSearchWord(searchWord),
                PageNumber = page,
                RowsPerPage = rowsPerPage,
                OrderKey = GetFilteredOrderKey(orderKey),
                Order = GetFilteredOrder(order)
            });
            connection.Close();
            return users;
            // return GetSorted(users, orderKey, order);
        }

        private IEnumerable<UserResource> GetSorted(IEnumerable<UserResource> users, string orderKey, string order)
        {
            orderKey = String.IsNullOrEmpty(orderKey) ? "utilization" : orderKey.ToLower();
            order = String.IsNullOrEmpty(order) ? "desc" : order.ToLower();
            switch (order)
            {
                case "asc":
                    switch (orderKey)
                    {
                        case "firstname":
                            return users.OrderBy(user => user.FirstName);
                        case "lastname":
                            return users.OrderBy(user => user.LastName);
                        case "province":
                            return users.OrderBy(user => user.Province);
                        case "city":
                            return users.OrderBy(user => user.City);
                        case "discipline":
                            if (users.First().DisciplineName == null) return users.OrderBy(user => user.Utilization);
                            return users.OrderBy(user => user.DisciplineName);
                        case "yearsofexp":
                            if (users.First().YearsOfExperience == null) return users.OrderBy(user => user.Utilization);
                            return users.OrderBy(user =>
                            {
                                if (user.YearsOfExperience == "10+") return 10;
                                else
                                {
                                    char[] sep = { '-' };
                                    return Int32.Parse(user.YearsOfExperience.Split(sep)[0]);
                                }
                            });
                        default:
                            return users.OrderBy(user => user.Utilization);
                    }
                default:
                    switch (orderKey)
                    {
                        case "firstname":
                            return users.OrderByDescending(user => user.FirstName);
                        case "lastname":
                            return users.OrderByDescending(user => user.LastName);
                        case "province":
                            return users.OrderByDescending(user => user.Province);
                        case "city":
                            return users.OrderByDescending(user => user.City);
                        case "discipline":
                            if (users.First().DisciplineName == null) return users.OrderByDescending(user => user.Utilization);
                            return users.OrderByDescending(user => user.DisciplineName);
                        case "yearsofexp":
                            if (users.First().YearsOfExperience == null) return users.OrderByDescending(user => user.Utilization);
                            return users.OrderByDescending(user =>
                            {
                                if (user.YearsOfExperience == "10+") return 10;
                                else
                                {
                                    char[] sep = { '-' };
                                    return Int32.Parse(user.YearsOfExperience.Split(sep)[0]);
                                }
                            });
                        default:
                            return users.OrderByDescending(user => user.Utilization);
                    }
            }
        }

        public async Task<IEnumerable<UserResource>> GetAllUserResourcesOnFilter(RequestSearchUsers req, int rowsPerPage)
        {
            if (req.Filter == null || (IsNoDisciplinesAndNoYearsOfExpFilters(req) && IsNoLocationFilter(req)))
            {
                return await GetAllUserResources(req.SearchWord, req.OrderKey, req.Order, req.Page, rowsPerPage);
            }

            using var connection = new SqlConnection(connectionString);
            
            var filteredLocations = await GetFilteredLocations(connection, req.Filter.Locations);
            var filteredUtilization = GetFilteredUtilization(req.Filter.Utilization);
            var filteredDisciplinesSkills = await GetFilteredDisciplines(connection, req.Filter.Disciplines);
            var filteredYearsOfExps = await GetFilteredYearsOfExps(connection, req.Filter.YearsOfExps);

            var sql = IsNoDisciplinesAndNoYearsOfExpFilters(req) ? GetSqlForOnlyLocationFilter() : GetSqlForMoreThanTwoFilters();

            connection.Open();
            var users = await connection.QueryAsync<UserResource>(sql, new
            {
                Provinces = filteredLocations["provinces"],
                Cities = filteredLocations["cities"],
                Disciplines = filteredDisciplinesSkills["disciplines"],
                Skills = filteredDisciplinesSkills["skills"],
                SearchWord = GetFilteredSearchWord(req.SearchWord),
                YearsOfExps = filteredYearsOfExps,
                EndDate = GetFilteredEndDate(req.Filter.EndDate),
                StartDate = GetFilteredStartDate(req.Filter.StartDate),
                UtilMin = filteredUtilization.Min,
                UtilMax = filteredUtilization.Max,
                PageNumber = (req.Page == 0) ? 1 : req.Page,
                RowsPerPage = rowsPerPage,
                OrderKey = GetFilteredOrderKey(req.OrderKey),
                Order = GetFilteredOrder(req.Order)
            });
            connection.Close();
            return users;
            // return GetSorted(users, req.OrderKey, req.Order);
        }

        private bool IsNoDisciplinesAndNoYearsOfExpFilters(RequestSearchUsers req)
        {
            return (
                req.Filter.Disciplines == null &&
                (req.Filter.YearsOfExps == null || !req.Filter.YearsOfExps.Any())
            );
        }

        private bool IsNoLocationFilter(RequestSearchUsers req)
        {
            return (req.Filter.Locations == null || !req.Filter.Locations.Any());
        }

        private string GetSqlForMoreThanTwoFilters()
        {
            var sql = @"
                WITH Data_CTE AS
                (
                    SELECT
                        u.Id, u.FirstName, u.LastName, u.Username, u.LocationId, u.Utilization,
                        l.Province, l.City,
                        rd.DisciplineId, rd.YearsOfExperience, 
                        d.Name AS DisciplineName,
                        STRING_AGG (s.Name, ',') AS Skills
                    FROM 
                        Users u
                    INNER JOIN Locations l
                        ON
                            u.LocationId = l.Id
                            AND l.Province IN @Provinces
                            AND l.City IN @Cities
                    INNER JOIN ResourceDiscipline rd
                        ON
                            rd.ResourceId = u.Id
                            AND rd.YearsOfExperience IN @YearsOfExps
                    INNER JOIN Disciplines d
                        ON
                            rd.DisciplineId = d.Id
                            AND d.Name IN @Disciplines
                    LEFT JOIN Skills s
                        ON
                            s.DisciplineId = d.Id
                            AND s.Name IN @Skills
                    LEFT JOIN OutOfOffice o
                        ON
                            o.ResourceId = u.Id
                            AND
                            (
                                o.FromDate > @EndDate
                                OR o.ToDate < @StartDate
                            )
                    WHERE
                        (
                            LOWER(TRIM(u.FirstName)) LIKE @SearchWord
                            OR LOWER(TRIM(u.LastName)) LIKE @SearchWord
                        )
                        AND u.Utilization >= @UtilMin
                        AND u.Utilization <= @UtilMax
                    GROUP BY
                        u.Id, u.FirstName, u.LastName, u.Username, u.LocationId, u.Utilization,
                        l.Province, l.City,
                        rd.DisciplineId, rd.YearsOfExperience,
                        d.Name
                ), 
                Count_CTE AS 
                (
                    SELECT CEILING(CAST(COUNT(*) AS FLOAT) / @RowsPerPage) AS MaxPages FROM Data_CTE
                )

                SELECT *
                FROM Data_CTE
                CROSS JOIN Count_CTE
                ORDER BY
                    CASE WHEN (@OrderKey = 'utilization' AND @Order = 'asc') THEN Data_CTE.Utilization END ASC,
                    CASE WHEN (@OrderKey = 'utilization' AND @Order = 'desc') THEN Data_CTE.Utilization END DESC,

                    CASE WHEN (@OrderKey = 'province' AND @Order = 'asc') THEN Data_CTE.Province END ASC,
                    CASE WHEN (@OrderKey = 'province' AND @Order = 'desc') THEN Data_CTE.Province END DESC,

                    CASE WHEN (@OrderKey = 'city' AND @Order = 'asc') THEN Data_CTE.City END ASC,
                    CASE WHEN (@OrderKey = 'city' AND @Order = 'desc') THEN Data_CTE.City END DESC,

                    CASE WHEN (@OrderKey = 'discipline' AND @Order = 'asc') THEN Data_CTE.DisciplineName END ASC,
                    CASE WHEN (@OrderKey = 'discipline' AND @Order = 'desc') THEN Data_CTE.DisciplineName END DESC,

                    CASE WHEN (@OrderKey = 'lastname' AND @Order = 'asc') THEN Data_CTE.LastName END ASC,
                    CASE WHEN (@OrderKey = 'lastname' AND @Order = 'desc') THEN Data_CTE.LastName END DESC,

                    CASE WHEN (@OrderKey = 'firstname' AND @Order = 'asc') THEN Data_CTE.FirstName END ASC,
                    CASE WHEN (@OrderKey = 'firstname' AND @Order = 'desc') THEN Data_CTE.FirstName END DESC
                OFFSET (@PageNumber - 1) * @RowsPerPage ROWS
                FETCH NEXT (@RowsPerPage + 1) ROWS ONLY
            ;";
            return sql;
        }

        private string GetSqlForOnlyLocationFilter()
        {
            var sql = @"
                WITH Data_CTE AS
                (
                    SELECT
                        u.Id, u.FirstName, u.LastName, u.Username, u.LocationId, u.Utilization,
                        l.Province, l.City,
                        rd.DisciplineId, rd.YearsOfExperience,
                        d.Name AS DisciplineName
                    FROM 
                        Users u
                    INNER JOIN Locations l
                        ON
                            u.LocationId = l.Id
                            AND l.Province IN @Provinces
                            AND l.City IN @Cities
                    LEFT JOIN ResourceDiscipline rd
                        ON rd.ResourceId = u.Id
                    LEFT JOIN Disciplines d
                        ON rd.DisciplineId = d.Id
                    LEFT JOIN OutOfOffice o
                        ON
                            o.ResourceId = u.Id
                            AND
                            (
                                o.FromDate > @EndDate
                                OR o.ToDate < @StartDate
                            )
                    WHERE
                        (
                            LOWER(TRIM(u.FirstName)) LIKE @SearchWord
                            OR LOWER(TRIM(u.LastName)) LIKE @SearchWord
                        )
                        AND u.Utilization >= @UtilMin
                        AND u.Utilization <= @UtilMax
                ), 
                Count_CTE AS 
                (
                    SELECT CEILING(CAST(COUNT(*) AS FLOAT) / @RowsPerPage) AS MaxPages FROM Data_CTE
                )

                SELECT *
                FROM Data_CTE
                CROSS JOIN Count_CTE
                ORDER BY
                    CASE WHEN (@OrderKey = 'utilization' AND @Order = 'asc') THEN Data_CTE.Utilization END ASC,
                    CASE WHEN (@OrderKey = 'utilization' AND @Order = 'desc') THEN Data_CTE.Utilization END DESC,

                    CASE WHEN (@OrderKey = 'province' AND @Order = 'asc') THEN Data_CTE.Province END ASC,
                    CASE WHEN (@OrderKey = 'province' AND @Order = 'desc') THEN Data_CTE.Province END DESC,

                    CASE WHEN (@OrderKey = 'city' AND @Order = 'asc') THEN Data_CTE.City END ASC,
                    CASE WHEN (@OrderKey = 'city' AND @Order = 'desc') THEN Data_CTE.City END DESC,

                    CASE WHEN (@OrderKey = 'discipline' AND @Order = 'asc') THEN Data_CTE.DisciplineName END ASC,
                    CASE WHEN (@OrderKey = 'discipline' AND @Order = 'desc') THEN Data_CTE.DisciplineName END DESC,

                    CASE WHEN (@OrderKey = 'lastname' AND @Order = 'asc') THEN Data_CTE.LastName END ASC,
                    CASE WHEN (@OrderKey = 'lastname' AND @Order = 'desc') THEN Data_CTE.LastName END DESC,

                    CASE WHEN (@OrderKey = 'firstname' AND @Order = 'asc') THEN Data_CTE.FirstName END ASC,
                    CASE WHEN (@OrderKey = 'firstname' AND @Order = 'desc') THEN Data_CTE.FirstName END DESC
                OFFSET (@PageNumber - 1) * @RowsPerPage ROWS
                FETCH NEXT (@RowsPerPage + 1) ROWS ONLY
            ;";
            return sql;
        }

        private string GetFilteredOrder(string order)
        {
            return String.IsNullOrEmpty(order) ? "desc" : order.ToLower();
        }

        private string GetFilteredOrderKey(string orderKey)
        {
            return String.IsNullOrEmpty(orderKey) ? "utilization" : orderKey.ToLower();
        }

        private string GetFilteredSearchWord(string searchWordReq)
        {
            return String.IsNullOrEmpty(searchWordReq) ? "%" : $"%{searchWordReq.ToLower()}%";
        }

        private DateTime GetFilteredStartDate(DateTime startDateReq)
        {
            return (startDateReq == null || startDateReq == new DateTime()) ? DateTime.Today : startDateReq;
        }

        private DateTime GetFilteredEndDate(DateTime endDateReq)
        {
            return (endDateReq == null || endDateReq == new DateTime()) ? DateTime.Today.AddYears(2) : endDateReq;
        }

        private async Task<Dictionary<string, HashSet<string>>> GetFilteredLocations(SqlConnection connection, IEnumerable<LocationResource> locationsReq)
        {
            var provinces = new HashSet<string>();
            var cities = new HashSet<string>();
            if (locationsReq == null || !locationsReq.Any())
            {
                var sqlGetAllLocations = @"
                    SELECT *
                    FROM Locations
                ";
                connection.Open();
                locationsReq = await connection.QueryAsync<LocationResource>(sqlGetAllLocations);
                connection.Close();
            }

            foreach (var location in locationsReq)
            {
                provinces.Add(location.Province);
                cities.Add(location.City);
            }

            return new Dictionary<string, HashSet<string>>()
            {
                { "provinces", provinces },
                { "cities", cities }
            };
        }

        private async Task<Dictionary<string, HashSet<string>>> GetFilteredDisciplines(SqlConnection connection, Dictionary<string, IEnumerable<string>> disciplinesReq)
        {
            var disciplines = new HashSet<string>();
            var skills = new HashSet<string>();
            if (disciplinesReq == null)
            {
                var sqlGetAllDisciplines = @"
                    SELECT Name
                    FROM Disciplines
                ";
                connection.Open();
                disciplines = (await connection.QueryAsync<Discipline>(sqlGetAllDisciplines)).Select(x => x.Name).ToHashSet();
                connection.Close();

                var sqlGetAllSkills = @"
                    SELECT Name
                    FROM Skills
                ";
                connection.Open();
                skills = (await connection.QueryAsync<Skill>(sqlGetAllSkills)).Select(x => x.Name).ToHashSet();
                connection.Close();
            }
            else
            {
                disciplines = disciplinesReq.Keys.ToHashSet();
                skills = disciplinesReq.Values.SelectMany(x => x).ToHashSet();
                foreach (var discipline in disciplines)
                {
                    if (disciplinesReq[discipline] == null || !disciplinesReq[discipline].Any())
                    {
                        var sqlGetAllSkillsForDiscipline = @"
                            SELECT
                                s.Name
                            FROM
                                Skills s, Disciplines d
                            WHERE
                                s.DisciplineId = d.Id
                                AND d.name = @DisciplineName
                        ";
                        connection.Open();
                        skills.UnionWith(
                            (await connection.QueryAsync<Skill>(sqlGetAllSkillsForDiscipline, new
                            {
                                DisciplineName = discipline
                            })).Select(x => x.Name).ToHashSet()
                        );
                        connection.Close();
                    }
                }
            }

            return new Dictionary<string, HashSet<string>>()
            {
                { "disciplines", disciplines },
                { "skills", skills }
            };
        }

        private async Task<IEnumerable<string>> GetFilteredYearsOfExps(SqlConnection connection, IEnumerable<string> yearsOfExpsReq)
        {
            if (yearsOfExpsReq == null || !yearsOfExpsReq.Any())
            {
                yearsOfExpsReq = new HashSet<string>();
                var sqlGetAllUsersYearsOfExps = @"
                    SELECT DISTINCT YearsOfExperience
                    FROM ResourceDiscipline
                ";
                connection.Open();
                yearsOfExpsReq = (await connection.QueryAsync<ResourceDiscipline>(sqlGetAllUsersYearsOfExps)).Select(x => x.YearsOfExperience);
                connection.Close();
            }
            return yearsOfExpsReq;
        }

        private Utilization GetFilteredUtilization(Utilization utilizationReq)
        {
            if (utilizationReq == null)
            {
                utilizationReq = new Utilization()
                {
                    Min = Int32.MinValue,
                    Max = Int32.MaxValue
                };
                return utilizationReq;
            }
            else return utilizationReq;
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

        public async Task<string> UpdateAUser(UserSummary user, Location location)
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
                Id = user.UserID,
                FirstName = user.FirstName,
                LastName = user.LastName,
                LocationId = location.Id
            });
            return result == 1 ? user.UserID : "-1";
        }

        public async Task<User> UpdateUtilizationOfUser(int util, string userId)
        {
            var sql = @"
                update Users
                set Utilization = @Util
                where Id = @Id;

                select 
                    *
                from Users
                where Id = @Id;
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QuerySingleAsync<User>(sql, new
            {
                Util = util,
                Id = userId
            });
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
