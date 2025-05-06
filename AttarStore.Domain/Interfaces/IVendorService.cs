using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Domain.Interfaces
{
    public interface IVendorService
    {
        Task SuspendVendorAsync(int vendorId);
        Task ActivateVendorAsync(int vendorId);
    }
}
