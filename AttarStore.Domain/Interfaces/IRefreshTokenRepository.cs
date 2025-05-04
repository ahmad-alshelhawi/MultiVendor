using AttarStore.Domain.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Domain.Interfaces
{


    public interface IRefreshTokenRepository
    {
        Task CreateAsync(RefreshToken token);
        Task<RefreshToken> GetByTokenAsync(string token);
        Task UpdateAsync(RefreshToken token);
    }

}
