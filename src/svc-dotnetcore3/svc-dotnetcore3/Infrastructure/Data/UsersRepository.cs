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
                    Users u, Locations l, Positions p, Disciplines d, Skills s
                WHERE
                    u.LocationId = l.Id
                    AND p.ResourceId = u.Id
                    AND p.ProjectId = @ProjectId
                    AND d.Id = p.DisciplineId
                    AND s.DisciplineId = d.Id
                    AND u.Id != @ProjectManagerId
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

        public async Task<IEnumerable<UserResource>> GetAllUserResources(string searchWord, string orderKey, string order, int page)
        {
            var sql = @"
                SELECT
                    u.Id, u.FirstName, u.LastName, u.Username, u.LocationId, u. Utilization,
                    l.Province, l.City
                FROM
                    Users u, Locations l
                WHERE
                    u.LocationId = l.Id
                    AND
                    (
                        LOWER(TRIM(u.FirstName)) LIKE @SearchWord
                        OR LOWER(TRIM(u.LastName)) LIKE @SearchWord
                    )
                ORDER BY
                    u.Id
                    OFFSET (@PageNumber - 1) * @RowsPerPage ROWS
                    FETCH NEXT @RowsPerPage ROWS ONLY
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var users = await connection.QueryAsync<UserResource>(sql, new
            {
                SearchWord = GetFilteredSearchWord(searchWord),
                PageNumber = page,
                RowsPerPage = 50
            });
            return GetSorted(users, orderKey, order);
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
                    ul.Id, FirstName, LastName, Username,
                    rd.DisciplineId, rd.YearsOfExperience, 
                    LocationId, Province, City,
                    d.Name AS DisciplineName,
                    STRING_AGG (s.Name, ',') as Skills
                FROM
                (
                    SELECT
                        u.Id, u.FirstName, u.LastName, u.Username, u.LocationId, u.Utilization,
                        l.Province, l.City
                    FROM
                        Users u, Locations l
                    WHERE
                        u.LocationId = l.Id
                ) AS ul
                LEFT JOIN OutOfOffice o
                    ON
                        o.ResourceId = ul.Id
                        AND
                        (
                            o.FromDate > @EndDate
                            OR o.ToDate < @StartDate
                        )
                INNER JOIN ResourceDiscipline rd
                    ON
                        rd.ResourceId = ul.Id
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
                        LOWER(TRIM(ul.FirstName)) LIKE @SearchWord
                        OR LOWER(TRIM(ul.LastName)) LIKE @SearchWord
                    )
                    AND ul.Province IN @Provinces
                    AND ul.City IN @Cities
                    AND ul.Utilization >= @UtilMin
                    AND ul.Utilization <= @UtilMax
                GROUP BY
                    Utilization,
                    ul.Id, FirstName, LastName, Username,
                    rd.DisciplineId, rd.YearsOfExperience,
                    LocationId, Province, City,
                    d.Name
                ORDER BY
                    ul.Id
                    OFFSET (@PageNumber - 1) * @RowsPerPage ROWS
                    FETCH NEXT @RowsPerPage ROWS ONLY
            ;";

            connection.Open();
            var users = await connection.QueryAsync<UserResource>(sql, new
            {
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

            return GetSorted(users, req.OrderKey, req.Order);
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
                foreach(var discipline in disciplinesReq.Keys.ToList())
                {
                    if (disciplinesReq[discipline] == null || !disciplinesReq[discipline].Any())
                    {
                        var sqlGetSkillsForDiscipline = @"
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
                            (await connection.QueryAsync<Skill>(sqlGetSkillsForDiscipline, new {
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
                    SELECT YearsOfExperience
                    FROM ResourceDiscipline
                ";
                connection.Open();
                yearsOfExpsReq = (await connection.QueryAsync<ResourceDiscipline>(sqlGetAllUsersYearsOfExps)).Select(x => x.YearsOfExperience).ToHashSet();
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
            return await connection.QuerySingleAsync<User>(sql, new {Util = util, 
                                                                     Id = userId});
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
