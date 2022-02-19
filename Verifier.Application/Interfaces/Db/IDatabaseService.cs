using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Verifier.Domain.Entities.User;

namespace Verifier.Application.Interfaces.Db
{
    public interface IDatabaseService
    {
        DbSet<VerifierUser> TheTerminalUsers { get; set; }

        Task<int> SaveAsync();
    }
}
