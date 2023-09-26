using System;

namespace CodeChallenge.Models
{
    public class Compensation
    {
        public string CompensationId { get; set; }
        public string EmployeeId { get; set; }
        public Decimal Salary { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}
