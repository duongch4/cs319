namespace Web.API.Application.Models
{
    public class Position
    {
        public int PositionId {get; set;}
        public int DisciplineId {get; set;}
        public int ProjectId {get; set;}
        public int ProjectedMonthlyHours {get; set;}
        public int ResourceId {get; set;}
        public string PositionName {get; set;}
        public bool IsConfirmed {get; set;}
    }
}
