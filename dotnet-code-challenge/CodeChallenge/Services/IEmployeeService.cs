using CodeChallenge.Models;
using System;

namespace CodeChallenge.Services
{
    public interface IEmployeeService
    {
        Employee GetById(String id);
        Employee Create(Employee employee);
        Employee Replace(Employee originalEmployee, Employee newEmployee);
        ReportingStructure GetReportingStructureByEmployeeId(String id);
    }
}
