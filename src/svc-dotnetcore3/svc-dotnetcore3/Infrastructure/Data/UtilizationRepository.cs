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

        private async Task<int> CalculateWorkingHoursForSinglePosition(Position position){
            var totalHours = 0;
                foreach (KeyValuePair<string, int> dateHourEntry in position.ProjectedMonthlyHours){
                    DateTime date = DateTime.Parse(dateHourEntry.Key);
                    bool isForThisMonth = DateTime.Today.Month == date.Month && DateTime.Today.Year == date.Year;
                    if (isForThisMonth){
                        return totalHours += dateHourEntry.Value;
                    }
                };

            return await Task.FromResult(totalHours);
        }

        private async Task<int> CalculateWorkingHoursForMonth(IEnumerable<Position> positionsOfUser) {
            var total = 0;
            foreach (Position position in positionsOfUser) {
                if (position.IsConfirmed == true) {
                     total += await CalculateWorkingHoursForSinglePosition(position);
                }
            }
            return total;
        }

        private async Task<double> CalculateHoursOutOfOfficeForMonth(OutOfOffice outOfOffice) 
        {
            double hoursOutOfOffice = 0;
            int workHoursPerDay = 8;

            if (outOfOffice.FromDate.Month == DateTime.Today.Month 
                && outOfOffice.FromDate.Year == DateTime.Today.Year) {
                DateTime firstDayOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddSeconds(-1);

                if ((outOfOffice.FromDate < firstDayOfMonth && lastDayOfMonth < outOfOffice.ToDate)
                    || (outOfOffice.FromDate == firstDayOfMonth && lastDayOfMonth == outOfOffice.ToDate)) 
                {
                    TimeSpan timeSpan = lastDayOfMonth.Subtract(firstDayOfMonth);
                    hoursOutOfOffice += timeSpan.TotalDays * workHoursPerDay;

                }
                else if (outOfOffice.FromDate <= firstDayOfMonth && outOfOffice.ToDate < lastDayOfMonth) 
                {
                    TimeSpan timeSpan = outOfOffice.ToDate.Subtract(firstDayOfMonth);
                    hoursOutOfOffice += timeSpan.TotalDays * workHoursPerDay;

                }
                else if (firstDayOfMonth < outOfOffice.FromDate && lastDayOfMonth <= outOfOffice.ToDate) 
                {
                    TimeSpan timeSpan = lastDayOfMonth.Subtract(outOfOffice.FromDate);
                    hoursOutOfOffice += timeSpan.TotalDays * workHoursPerDay;

                }
            }

            return await Task.FromResult(hoursOutOfOffice);
        }
        
        private async Task<decimal> CalculateAvailableHoursForMonth(IEnumerable<OutOfOffice> outOfOffices) {
            double availableHoursThisMonth = 176;
            foreach (OutOfOffice outOfOffice in outOfOffices) {
                availableHoursThisMonth -= await CalculateHoursOutOfOfficeForMonth(outOfOffice);
            }
            availableHoursThisMonth = availableHoursThisMonth <= 0 ? 1 : availableHoursThisMonth;
            return System.Convert.ToDecimal(availableHoursThisMonth);
        }

        public async Task<int> CalculateUtilizationOfUser(IEnumerable<Position> positionsOfUser, 
                                                          IEnumerable<OutOfOffice> outOfOffice)
        {
            var totalHoursThisMonth = await CalculateWorkingHoursForMonth(positionsOfUser);
            var availableHoursThisMonth = await CalculateAvailableHoursForMonth(outOfOffice);

            decimal utilization = Math.Round(totalHoursThisMonth / availableHoursThisMonth, 2) * 100;
            var utilAsInt = Decimal.ToInt32(utilization);
            return utilAsInt;
        }

    }
}