using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Verifier.Application.Interfaces.Services.User
{
    public interface ICurrentUserService
    {
        string UserId { get; }
    }
    
}
