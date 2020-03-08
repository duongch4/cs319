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

        public async Task<UserResource> GetAUserResource(int userId)
        {
            var sql = @"
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
                HAVING
                    ij.Id = @UserId
            ";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var users = await connection.QueryAsync<UserResource>(sql, new
            {
                DateInTwoYears = DateTime.Today.AddYears(2),
                UserId = userId
            });

            if (users == null || !users.Any() || users.Count() > 2) return null;
            foreach (var user in users)
            {
                if (user.IsConfirmed) return user;
                else user.Utilization = 0;
            }
            return users.First();
        }

        public async Task<IEnumerable<UserResource>> GetAllUserResourcesOnProject(int projectId, int projectManagerId)
        {
            var sql = @"
                SELECT
                    Utilization,
                    util.Id, FirstName, LastName, Username,
                    util.IsConfirmed, p.DisciplineId, p.YearsOfExperience,
                    LocationId, Province, City,
                    d.Name AS DisciplineName,
                    STRING_AGG (s.Name, ',') as Skills
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
                INNER JOIN Positions p
                    ON
                        p.ResourceId = util.Id
                        AND p.ProjectId = @ProjectId
                INNER JOIN Disciplines d
                    ON
                        d.Id = p.DisciplineId
                INNER JOIN Skills s
                    ON
                        s.DisciplineId = d.Id
                WHERE
                    util.Id != @ProjectManagerId
                GROUP BY
                    Utilization,
                    util.Id, FirstName, LastName, Username,
                    util.IsConfirmed, p.DisciplineId, p.YearsOfExperience,
                    LocationId, Province, City,
                    d.Name
                ORDER BY
                    CASE WHEN (@OrderKey = 'utilization' AND @Order = 'asc') THEN util.Utilization END ASC,
                    CASE WHEN (@OrderKey = 'utilization' AND @Order = 'desc') THEN util.Utilization END DESC
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

        public async Task<IEnumerable<UserResource>> GetAllUserResources(string searchWord, string orderKey, string order, int page)
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
                ) AS util
                WHERE
                    LOWER(TRIM(util.FirstName)) LIKE @SearchWord
                    OR LOWER(TRIM(util.LastName)) LIKE @SearchWord 
                ORDER BY
                    util.Id
                    OFFSET (@PageNumber - 1) * @RowsPerPage ROWS
                    FETCH NEXT @RowsPerPage ROWS ONLY
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var users = await connection.QueryAsync<UserResource>(sql, new
            {
                DateInTwoYears = DateTime.Today.AddYears(2),
                SearchWord = GetFilteredSearchWord(searchWord),
                PageNumber = page,
                RowsPerPage = 50
            });

            if (users == null || !users.Any()) return null;

            IEnumerable<UserResource> result = Enumerable.Empty<UserResource>();
            var ids = new HashSet<int>();
            foreach (var user in users)
            {
                if (user.IsConfirmed)
                {
                    ids.Add(user.Id);
                    result = result.Append(user);
                }
            }
            foreach (var user in users)
            {
                if (!ids.Contains(user.Id))
                {
                    ids.Add(user.Id);
                    user.Utilization = 0;
                    result = result.Append(user);
                }
            }
            return GetSorted(result, orderKey, order);
        }

        private IEnumerable<UserResource> GetSorted(IEnumerable<UserResource> users, string orderKey, string order)
        {
            orderKey = (orderKey == null || orderKey == "") ? "utilization" : orderKey.ToLower();
            order = (order == null || order == "") ? "desc" : order.ToLower();
            switch(order)
            {
                case "asc":
                    switch(orderKey)
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
                            return users.OrderBy(user => user.DisciplineName);
                        case "yearsofexp":
                            return users.OrderBy(user => user.YearsOfExperience);
                        default:
                            return users.OrderBy(user => user.Utilization);
                    }
                default:
                    switch(orderKey)
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
                            return users.OrderByDescending(user => user.DisciplineName);
                        case "yearsofexp":
                            return users.OrderByDescending(user => user.YearsOfExperience);
                        default:
                            return users.OrderByDescending(user => user.Utilization);
                    }
            }
        }

        public async Task<IEnumerable<UserResource>> GetAllUserResourcesOnFilter(RequestSearchUsers req)
        {
            if (req.Filter == null) return await GetAllUserResources(req.SearchWord, req.OrderKey, req.Order, req.Page);

            using var connection = new SqlConnection(connectionString);

            var filteredSearchWord = GetFilteredSearchWord(req.SearchWord);

            var filteredLocations = await GetFilteredLocations(connection, req.Filter.Locations);
            var filteredProvinces = filteredLocations["provinces"];
            var filteredCities = filteredLocations["cities"];

            var filteredUtilization = GetFilteredUtilization(req.Filter.Utilization);

            var filteredDisciplinesSkills = await GetFilteredDisciplines(connection, req.Filter.Disciplines);
            var filteredDisciplines = filteredDisciplinesSkills["disciplines"];
            var filteredSkills = filteredDisciplinesSkills["skills"];

            var filteredYearsOfExps = await GetFilteredYearsOfExps(connection, req.Filter.YearsOfExps);
                        
            var filteredStartDate = GetFilteredStartDate(req.Filter.StartDate);
            var filteredEndDate = GetFilteredEndDate(req.Filter.EndDate);

            var sql = @"
                SELECT
                    Utilization,
                    util.Id, FirstName, LastName, Username,
                    util.IsConfirmed, rd.DisciplineId, rd.YearsOfExperience, 
                    LocationId, Province, City,
                    d.Name AS DisciplineName,
                    STRING_AGG (s.Name, ',') as Skills
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
                    (
                        LOWER(TRIM(util.FirstName)) LIKE @SearchWord
                        OR LOWER(TRIM(util.LastName)) LIKE @SearchWord
                    )
                    AND util.Province IN @Provinces
                    AND util.City IN @Cities
                    AND util.Utilization >= @UtilMin
                    AND util.Utilization <= @UtilMax
                GROUP BY
                    Utilization,
                    util.Id, FirstName, LastName, Username,
                    util.IsConfirmed, rd.DisciplineId, rd.YearsOfExperience,
                    LocationId, Province, City,
                    d.Name
                ORDER BY
                    util.Id
                    OFFSET (@PageNumber - 1) * @RowsPerPage ROWS
                    FETCH NEXT @RowsPerPage ROWS ONLY
            ;";

            connection.Open();
            var users = await connection.QueryAsync<UserResource>(sql, new
            {
                DateInTwoYears = DateTime.Today.AddYears(2),
                Provinces = filteredProvinces,
                Cities = filteredCities,
                Disciplines = filteredDisciplines,
                Skills = filteredSkills,
                SearchWord = filteredSearchWord,
                YearsOfExps = filteredYearsOfExps,
                EndDate = filteredEndDate,
                StartDate = filteredStartDate,
                UtilMin = filteredUtilization.Min,
                UtilMax = filteredUtilization.Max,
                PageNumber = (req.Page == 0) ? 1 : req.Page,
                RowsPerPage = 50
            });

            if (users == null || !users.Any()) return null;

            IEnumerable<UserResource> result = Enumerable.Empty<UserResource>();
            var ids = new HashSet<int>();
            var disciplineIds = new HashSet<int>();
            foreach (var user in users)
            {
                if (user.IsConfirmed)
                {
                    ids.Add(user.Id);
                    disciplineIds.Add(user.DisciplineId);
                    result = result.Append(user);
                }
            }
            foreach (var user in users)
            {
                if (!ids.Contains(user.Id) || !disciplineIds.Contains(user.DisciplineId))
                {
                    ids.Add(user.Id);
                    disciplineIds.Add(user.DisciplineId);
                    user.Utilization = 0;
                    result = result.Append(user);
                }
            }
            return GetSorted(result, req.OrderKey, req.Order);
        }

        private string GetFilteredSearchWord(string searchWordReq)
        {
            return (searchWordReq == null || searchWordReq == "") ? "%" : $"%{searchWordReq.ToLower()}%";
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
            }

            return new Dictionary<string, HashSet<string>>()
            {
                { "disciplines", disciplines },
                { "skills", skills }
            };
        }

        private async Task<IEnumerable<string>> GetFilteredYearsOfExps(SqlConnection connection, IEnumerable<string> yearsOfExpsReq)
        {
            var result = new HashSet<string>();
            if (yearsOfExpsReq == null || !yearsOfExpsReq.Any())
            {
                var sqlGetAllUsersYearsOfExps = @"
                    SELECT YearsOfExperience
                    FROM ResourceDiscipline
                ";
                connection.Open();
                result = (await connection.QueryAsync<ResourceDiscipline>(sqlGetAllUsersYearsOfExps)).Select(x => x.YearsOfExperience).ToHashSet();
                connection.Close();
            }
            return result;
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

        public async Task<int> UpdateAUser(UserSummary user, Location location)
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
            return result == 1 ? user.UserID : -1;
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
