using System;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using CodeChallenge.Data;

namespace CodeChallenge.Repositories
{
    public class CompensationRespository : ICompensationRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<ICompensationRepository> _logger;

        public CompensationRespository(ILogger<ICompensationRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        public Compensation Add(Compensation comp)
        {
            comp.CompensationId = Guid.NewGuid().ToString();
            _employeeContext.Compensations.Add(comp);
            return comp;
        }

        /// <summary>
        /// Returns the most recent Compensation by EffectiveDate, excluding future dates
        /// </summary>
        /// <param name="id">employee ID</param>
        /// <returns></returns>
        public Compensation GetByEmployeeId(string id)
        {
            var comp = _employeeContext.Compensations.Where((c) => c.EffectiveDate <= DateTime.Today)
                .OrderByDescending((c) => c.EffectiveDate).FirstOrDefault((c) => c.EmployeeId == id);
            return comp;
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }

    }
}
