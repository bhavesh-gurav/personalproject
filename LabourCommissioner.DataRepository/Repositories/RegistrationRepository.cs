using Dapper;
using LabourCommissioner.Abstraction.DataModels;
using LabourCommissioner.Abstraction.Repositories;
using LabourCommissioner.Common;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabourCommissioner.DataRepository.Repositories
{
    public class RegistrationRepository : BaseRepository<Registration>, IRegistrationRepository
    {
        public IConfiguration appConfig;

        public RegistrationRepository(IConfiguration config) : base(config)
        {
            appConfig = config ?? throw new ArgumentNullException(nameof(config));
        }
        public async Task<Registration> AddUpdateRegistration(Registration registration)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    var queryParameters = new DynamicParameters();
                    //queryParameters.Add("@RegistrationNo", registration.RegistrationNo);
                    queryParameters.Add("@Name", registration.Name);
                    queryParameters.Add("@DateOfBirth", registration.DateOfBirth);
                    queryParameters.Add("@Gender", registration.Gender);
                    queryParameters.Add("@MobileNo", registration.MobileNo);
                    queryParameters.Add("@EmailId", registration.EmailId);
                    queryParameters.Add("@Password", registration.Password);
                    queryParameters.Add("@UniqueIDNumber", registration.regunique.UniqueIdnumber);
                    queryParameters.Add("@BeneficiaryType", registration.BeneficiaryType);
                    queryParameters.Add("@HostName", "Host");//registration.regunique.HostName);
                    queryParameters.Add("@IpAddress", "IP");//registration.regunique.IpAddress);
                    queryParameters.Add("@UserId", 1);
                    queryParameters.Add("@FirstCardIssuedDate", registration.FirstCardIssuedDate);
                    queryParameters.Add("@CDistrictId", registration.CDistrictId);
                    queryParameters.Add("@ICardFromDate", registration.ICardFromDate);
                    queryParameters.Add("@ICardToDate", registration.ICardToDate);
                    var result = await conn.QueryFirstOrDefaultAsync<Registration>(Procedures.AddUpdateRegistration, queryParameters, commandType: System.Data.CommandType.StoredProcedure);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
