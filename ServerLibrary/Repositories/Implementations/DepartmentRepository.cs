using BaseLibrary.Entities;
using BaseLibrary.Responses;
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
    public class DepartmentRepository(AppDbContext appDbContext) : IGenericRepositoryInterface<Department>
    {
          public async Task<GeneralResponse> DeleteById(int id)
        {
            var dep = await appDbContext.departments.FindAsync(id);
            if (dep == null) return Notfound();
            appDbContext.departments.Remove(dep);
            await Commit();
            return Success();
        }

        public async Task<List<Department>> GetAll() => await appDbContext.departments.ToListAsync();


        public async Task<Department> GetById(int id) => await appDbContext.departments.FindAsync(id);



        public async Task<GeneralResponse> Insert(Department item)
        {
            if (!await CheckName(item.Name!)) return new(false, "Department already Added");
            appDbContext.departments.Add(item);
            await Commit();
            return Success();
        }


        public async Task<GeneralResponse> Update(Department item)
        {
            var dep = await appDbContext.departments.FindAsync(item.Id);
            if (dep is null) return Notfound();
            dep.Name = item.Name;
            await Commit();
            return Success();
        }
        private async Task<bool> CheckName(string name)
        {
            var item = await appDbContext.departments.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name.ToLower()));
            return item is null;
        }

        private static GeneralResponse Notfound() => new(false, "Sorry department not found");
        private static GeneralResponse Success() => new(true, "Process completed");
        private async Task Commit() => await appDbContext.SaveChangesAsync();
    }
}
