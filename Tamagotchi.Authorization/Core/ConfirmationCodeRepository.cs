using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Tamagotchi.Authorization.Models;

namespace Tamagotchi.Authorization.Core
{
    public class ConfirmationCodeRepository : IConfirmationCodeRepository
    {
        private readonly UserContext _db;
        public ConfirmationCodeRepository(UserContext context)
        {
            _db = context;
        }
        public async Task AddConfirmationCode(int userId, string confirmationCode)
        {
            _db.ConfirmationCode.Add(
                new ConfirmationCode
                {
                    UserId = userId,
                    Active = true,
                    CreationTime = DateTime.UtcNow,
                    CodeValue = confirmationCode
                });
            await _db.SaveChangesAsync();
        }

        public async Task<ConfirmationCode> GetConfirmData(string confirmCode) =>
            await _db.ConfirmationCode.FirstOrDefaultAsync(x => x.CodeValue == confirmCode);

        public async Task SetConfirmCodeNotActive(ConfirmationCode confirmation)
        {
            _db.Entry(confirmation).State = EntityState.Modified;
            confirmation.Active = false;
            await _db.SaveChangesAsync();
        }
    }
}
