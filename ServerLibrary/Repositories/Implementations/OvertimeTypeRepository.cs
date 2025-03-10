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
    public class OvertimeTypeRepository (AppDbContext appDbContext) : IGenericRepositoryInterface<OvertimeType>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var item = await appDbContext.overtimeTypes.FindAsync(id);
            if (item is null) return Notfound();

            appDbContext.overtimeTypes.Remove(item);
            await Commit();
            return Success();
        }
        public async Task<List<OvertimeType>> GetAll() => await appDbContext
         .overtimeTypes
         .AsNoTracking()
         .ToListAsync();

        public async Task<OvertimeType> GetById(int id) =>
            await appDbContext.overtimeTypes.FindAsync(id);

        public async Task<GeneralResponse> Insert(OvertimeType item)
        {
            if(!await CheckName(item.Name!)) 
                return new(false, "OvertimeType already Added");
            appDbContext.overtimeTypes.Add(item);
            await Commit();
            return Success();
        }


        public async Task<GeneralResponse> Update(OvertimeType item)
        {
            var obj = await appDbContext.overtimeTypes.FindAsync(item.Id);
            if (obj is null) return Notfound();
            obj.Name = item.Name;

            await Commit();
            return Success();
        }

        private async Task<bool> CheckName(string name)
        {
            var item = await appDbContext.overtimeTypes.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name.ToLower()));
            return item is null;
        }
        private static GeneralResponse Notfound() => new(false, "Sorry OvertimeType not found");
        private static GeneralResponse Success() => new(true, "Process completed");
        private async Task Commit() => await appDbContext.SaveChangesAsync();
    
    }
}
