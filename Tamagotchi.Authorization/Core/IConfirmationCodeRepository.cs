
using System.Threading.Tasks;
using Tamagotchi.Authorization.Models;

namespace Tamagotchi.Authorization.Core
{
    public interface IConfirmationCodeRepository
    {
        Task AddConfirmationCode(int userId, string confirmationCode);
        Task<ConfirmationCode> GetConfirmData(string confirmCode);
        Task SetConfirmCodeNotActive(ConfirmationCode confirmation);
    }
}
