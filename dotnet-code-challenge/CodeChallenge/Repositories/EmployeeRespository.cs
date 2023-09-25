using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CodeChallenge.Data;

namespace CodeChallenge.Repositories
{
    public class EmployeeRespository : IEmployeeRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<IEmployeeRepository> _logger;

        public EmployeeRespository(ILogger<IEmployeeRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        public Employee Add(Employee employee)
        {
            employee.EmployeeId = Guid.NewGuid().ToString();
            _employeeContext.Employees.Add(employee);
            return employee;
        }

        public Employee GetById(string id)
        {
            // Added this Include chain to fix a bug where DirectReports were always null
            //  due to the context getting disposed before they were serialized.
            // Definitely not the ideal solution if this was to go into production, 
            //  since it only covers 3 levels and is hacky. Ideally we could have the db
            //  config specify eager loading on this property, keep the context alive longer,
            //  or possibly change the schema to avoid infinite recursion.
            return _employeeContext.Employees.Include(r => r.DirectReports)
                .ThenInclude(r => r.DirectReports).ThenInclude(r => r.DirectReports)
                .SingleOrDefault(e => e.EmployeeId == id);
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }

        public Employee Remove(Employee employee)
        {
            return _employeeContext.Remove(employee).Entity;
        }
    }
}
