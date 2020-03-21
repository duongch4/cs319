namespace Web.API.Application.Models
{
    public class PositionRaw
    {
        public int Id { get; set; }
        public int DisciplineId { get; set; }
        public int ProjectId { get; set; }
        public string ProjectedMonthlyHours { get; set; }
        public int ResourceId { get; set; }
        public string PositionName { get; set; }
        public bool IsConfirmed { get; set; }
    }
}
