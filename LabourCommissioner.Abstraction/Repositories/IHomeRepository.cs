using LabourCommissioner.Abstraction.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabourCommissioner.Abstraction.Repositories
{
    public interface IHomeRepository
    {
        Task<IEnumerable<ServiceMaster>> BindServicesUserWiseFilter();
        Task<ServiceMaster> GetSchemeByServiceId(long ServiceId);
       // Task<ServiceMaster> GetServicesByHodId(int HodId);


    }
}
