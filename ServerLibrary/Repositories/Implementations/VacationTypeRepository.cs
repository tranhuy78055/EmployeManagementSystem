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
    public class VacationTypeRepository (AppDbContext appDbContext) : IGenericRepositoryInterface<VacationType>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var item = await appDbContext.VacationsTypes.FindAsync(id);
            if (item is null) return Notfound();

            appDbContext.VacationsTypes.Remove(item);
            await Commit();
            return Success();
        }
        public async Task<List<VacationType>> GetAll() => await appDbContext
         .VacationsTypes
         .AsNoTracking()
         .ToListAsync();

        public async Task<VacationType> GetById(int id) =>
            await appDbContext.VacationsTypes.FindAsync(id);

        public async Task<GeneralResponse> Insert(VacationType item)
        {
            if (!await CheckName(item.Name!))
                return new(false, "VacationType already Added");
            appDbContext.VacationsTypes.Add(item);
            await Commit();
            return Success();
        }


        public async Task<GeneralResponse> Update(VacationType item)
        {
            var obj = await appDbContext.VacationsTypes.FindAsync(item.Id);
            if (obj is null) return Notfound();
            obj.Name = item.Name;

            await Commit();
            return Success();
        }

        private async Task<bool> CheckName(string name)
        {
            var item = await appDbContext.VacationsTypes.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name.ToLower()));
            return item is null;
        }
        private static GeneralResponse Notfound() => new(false, "Sorry VacationType not found");
        private static GeneralResponse Success() => new(true, "Process completed");
        private async Task Commit() => await appDbContext.SaveChangesAsync();

    }
}
