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
    public class VacationRepository(AppDbContext appDbContext) : IGenericRepositoryInterface<Vacation>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var item = await appDbContext.Vacations.FirstOrDefaultAsync(eid => eid.EmployeeId == id);
            if (item is null) return Notfound();
            appDbContext.Vacations.Remove(item);
            await Commit();
            return Success();
        }
        public async Task<List<Vacation>> GetAll() => await appDbContext
         .Vacations
         .AsNoTracking()
         .Include(t => t.VacationType)
         .ToListAsync();

        public async Task<Vacation> GetById(int id) =>
            await appDbContext.Vacations.FirstOrDefaultAsync(eid => eid.EmployeeId == id);

        public async Task<GeneralResponse> Insert(Vacation item)
        {

            appDbContext.Vacations.Add(item);
            await Commit();
            return Success();
        }


        public async Task<GeneralResponse> Update(Vacation item)
        {
            var obj = await appDbContext.Vacations.FirstOrDefaultAsync(eid => eid.EmployeeId == item.EmployeeId);
            if (obj is null) return Notfound();
            obj.StartDate = item.StartDate;
            obj.NumberofDays = item.NumberofDays;
            obj.VacationTypeId = item.VacationTypeId;
            await Commit();
            return Success();
        }

        private static GeneralResponse Notfound() => new(false, "Sorry Vacation not found");
        private static GeneralResponse Success() => new(true, "Process completed");
        private async Task Commit() => await appDbContext.SaveChangesAsync();
    }
}
