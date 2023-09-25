namespace CodeChallenge.Models
{
    public class ReportingStructure
    {
        public Employee Employee { get; set; }
        public int NumberOfReports { get { return this.GetReports(this.Employee); } }

        private int GetReports (Employee employee)
        {
            if (employee.DirectReports != null)
            {
                int numReports = employee.DirectReports.Count;
                foreach (Employee report in employee.DirectReports)
                {
                    numReports += this.GetReports(report);
                }
                return numReports;

            }
            return 0;
        }
    }
}
