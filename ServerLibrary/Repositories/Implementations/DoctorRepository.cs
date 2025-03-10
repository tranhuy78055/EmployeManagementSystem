﻿using BaseLibrary.Entities;
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
    public class DoctorRepository(AppDbContext appDbContext): IGenericRepositoryInterface<Doctor>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var item = await appDbContext.Doctors.FirstOrDefaultAsync(eid=> eid.EmployeeId == id);
            if (item is null) return Notfound();
            appDbContext.Doctors.Remove(item);
            await Commit();
            return Success();
        }
        public async Task<List<Doctor>> GetAll() => await appDbContext
         .Doctors
         .AsNoTracking()
         .ToListAsync();

        public async Task<Doctor> GetById(int id) =>
            await appDbContext.Doctors.FirstOrDefaultAsync(eid => eid.EmployeeId == id);

        public async Task<GeneralResponse> Insert(Doctor item)
        {
     
            appDbContext.Doctors.Add(item);
            await Commit();
            return Success();
        }


        public async Task<GeneralResponse> Update(Doctor item)
        {
            var dep = await appDbContext.Doctors.FirstOrDefaultAsync(eid => eid.EmployeeId == item.EmployeeId);
            if (dep is null) return Notfound();
            dep.MedicalRecommendation = item.MedicalRecommendation;
            dep.MedicalDiagnose = item.MedicalDiagnose;
            dep.Date = item.Date;

            await Commit();
            return Success();
        }

        private static GeneralResponse Notfound() => new(false, "Sorry Doctor not found");
        private static GeneralResponse Success() => new(true, "Process completed");
        private async Task Commit() => await appDbContext.SaveChangesAsync();
    }
}
