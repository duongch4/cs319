using System.Text.Json;

namespace Web.API.Application.Models
{
    public class Position
    {
        public int Id { get; set; }
        public int DisciplineId { get; set; }
        public int ProjectId { get; set; }
        public JsonElement ProjectedMonthlyHours { get; set; }
        public string ResourceId { get; set; }
        public string PositionName { get; set; }
        public bool IsConfirmed { get; set; }
    }
}
