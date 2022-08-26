using Dapper;
using LabourCommissioner.Abstraction.DataModels;
using LabourCommissioner.Abstraction.Repositories;
using LabourCommissioner.Common;
using LabourCommissioner.Common.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabourCommissioner.DataRepository.Repositories
{
    public class HomeRepository : BaseRepository<Registration>, IHomeRepository
    {
        public IConfiguration appConfig;
        private readonly UserCookies cookies;
        public HomeRepository(IConfiguration config, IHttpContextAccessor _httpContextAccessor) : base(config)
        {
            appConfig = config ?? throw new ArgumentNullException(nameof(config));
            this.cookies = new UserCookies(_httpContextAccessor);
        }
        public async Task<IEnumerable<ServiceMaster>> BindServicesUserWiseFilter()
        {
            using (var conn = GetConnection())
            {
                var queryParameters = new DynamicParameters();
                queryParameters.Add("_usertype", Convert.ToInt32(UserCookies.UserType));
                queryParameters.Add("_benificiarytypeid", Convert.ToInt32(UserCookies.BeneficiaryType));
                queryParameters.Add("_postid", Convert.ToInt32(UserCookies.PostId));
                queryParameters.Add("_roleids", UserCookies.RoleIds);
                queryParameters.Add("_servicesubtypeid", 0);
                var result = (await conn.QueryAsync<ServiceMaster>(Procedures.BindServicesUserWiseFilter, queryParameters, commandType: System.Data.CommandType.StoredProcedure)).ToList();
                return result;
            }
        }
        public async Task<ServiceMaster> GetSchemeByServiceId(long ServiceId)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    var queryParameters = new DynamicParameters();
                    queryParameters.Add("_serviceid", ServiceId);
                    var result = await conn.QueryFirstOrDefaultAsync<ServiceMaster>(Procedures.GetSchemeByServiceId, queryParameters, commandType: System.Data.CommandType.StoredProcedure);
                    return result;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        //public async Task<ServiceMaster> GetServicesByHodId(int HodId)
        //{
        //    using (var conn = GetConnection())
        //    {
        //        var queryParameters = new DynamicParameters();
        //        queryParameters.Add("@HodId", HodId);
        //        var result = await conn.QueryFirstOrDefaultAsync<ServiceMaster>(Procedures.GetServicesByHodId, queryParameters, commandType: System.Data.CommandType.StoredProcedure);
        //        return result;
        //    }
        //}
    }
}
