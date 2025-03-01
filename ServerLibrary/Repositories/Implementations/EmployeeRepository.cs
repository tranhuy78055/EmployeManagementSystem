using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary.Repositories.Implementations
{
    public class EmployeeRepository(AppDbContext appDbContext) : IGenericRepositoryInterface<Employee>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var dep = await appDbContext.Employees.FindAsync(id);
            if (dep == null) return Notfound();
            appDbContext.Employees.Remove(dep);
            await Commit();
            return Success();
        }

        public async Task<List<Employee>> GetAll()
        {
            var employees = await appDbContext.Employees
                .AsNoTracking()
                .Include(t=> t.Town)
                .ThenInclude(c => c.City)
                .ThenInclude(co => co.Country)
                .Include(b => b.Branch)
                .ThenInclude(d => d.Department)
                .ThenInclude(g => g.GeneralDepartment)
                .ToListAsync();
            return employees;
        }


        public async Task<Employee> GetById(int id)
        {
            var employee = await appDbContext.Employees
                .AsNoTracking()
                .Include(t => t.Town)
                .ThenInclude(c => c.City)
                .ThenInclude(co => co.Country)
                .Include(b => b.Branch)
                .ThenInclude(d => d.Department)
                .ThenInclude(g => g.GeneralDepartment)
                .FirstOrDefaultAsync(x => x.Id == id);
            return employee!;
        }



        public async Task<GeneralResponse> Insert(Employee item)
        {
            var checkIfNull = await CheckName(item.Name!);
            if (!checkIfNull) return new(false, "Employee already Added");
            appDbContext.Employees.Add(item);
            await Commit();
            return Success();
        }


        public async Task<GeneralResponse> Update(Employee employee)
        {
          var findUser= await appDbContext.Employees.FirstOrDefaultAsync(e=>e.Id==employee.Id);
            if (findUser is null) return Notfound();
            findUser.Name = employee.Name;
            findUser.Other = employee.Other;
            findUser.Address = employee.Address;
            findUser.TelephoneNumber = employee.TelephoneNumber;
            findUser.BranchId = employee.BranchId;
            findUser.TownId = employee.TownId;
            findUser.CivilId = employee.CivilId;
            findUser.FileNumber = employee.FileNumber;
            findUser.JobName = employee.JobName;
            findUser.Photo = employee.Photo;
            await Commit();
            return Success();
        }
        private async Task<bool> CheckName(string name)
        {
            var item = await appDbContext.Employees.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name.ToLower()));
            return item is null ? true : false;
        }

        private static GeneralResponse Notfound() => new(false, "Sorry Employee not found");
        private static GeneralResponse Success() => new(true, "Process completed");
        private async Task Commit() => await appDbContext.SaveChangesAsync();
    }
}
