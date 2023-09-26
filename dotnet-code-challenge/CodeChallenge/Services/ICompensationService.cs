using CodeChallenge.Models;
using System;

namespace CodeChallenge.Services
{
    public interface ICompensationService
    {
        Compensation GetByEmployeeId(String id);
        Compensation Create(Compensation comp);
    }
}
