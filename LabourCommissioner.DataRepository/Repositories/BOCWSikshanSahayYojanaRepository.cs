using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Dapper;
using LabourCommissioner.Abstraction.DataModels;
using LabourCommissioner.Abstraction.Repositories;
using LabourCommissioner.Abstraction.ViewDataModels;
using LabourCommissioner.Common;
using LabourCommissioner.Common.Utility;
using Microsoft.Extensions.Configuration;
using NpgsqlTypes;
using static LabourCommissioner.Abstraction.ViewDataModels.DocumentDetails;

namespace LabourCommissioner.DataRepository.Repositories
{
    public class BOCWSikshanSahayYojanaRepository : BaseRepository<TabModel>, IBOCWSikshanSahayYojanaRepository
    {
        public IConfiguration appConfig;
        public BOCWSikshanSahayYojanaRepository(IConfiguration config) : base(config)
        {
            appConfig = config ?? throw new ArgumentNullException(nameof(config));
        }
        public async Task<List<TabModel>> GetServiceTabByServiceId(int ServiceId)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    var queryParameters = new DynamicParameters();
                    queryParameters.Add("_serviceid", ServiceId);
                    var result = (await conn.QueryAsync<TabModel>(Procedures.GetServiceTabByServiceId, queryParameters, commandType: CommandType.StoredProcedure)).ToList();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<SelectListItem>> GetAllStates()
        {
            try
            {

                using (var conn = GetConnection())
                {

                    var result = (await conn.QueryAsync<SelectListItem>(Procedures.GetAllStates, commandType: CommandType.StoredProcedure)).ToList();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<PersonalDetailsModel> GetPersonalDetailsByRegNo(string RegistrationNo)
        {
            try
            {

                using (var conn = GetConnection())
                {
                    var queryParameters = new DynamicParameters();
                    queryParameters.Add("_regno", RegistrationNo);

                    var result = await conn.QueryFirstOrDefaultAsync<PersonalDetailsModel>(Procedures.GetPersonalDetailsByRegNo, queryParameters, commandType: System.Data.CommandType.StoredProcedure);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<SelectListItem>> GetDistrict()
        {
            try
            {
                using (var conn = GetConnection())
                {
                    var result = (await conn.QueryAsync<SelectListItem>(Procedures.GetDistrict, commandType: CommandType.StoredProcedure));
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<SelectListItem>> GetTalukaByDistrictId(int districtId)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    var queryParameters = new DynamicParameters();
                    queryParameters.Add("_districtid", districtId);
                    var result = (await conn.QueryAsync<SelectListItem>(Procedures.GetTalukaByDistrictId, queryParameters, commandType: CommandType.StoredProcedure)).ToList();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<SelectListItem>> GetVillageByDistrictIdAndTalukaId(int districtId, int talukaId)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    var queryParameters = new DynamicParameters();
                    queryParameters.Add("_districtid", districtId);
                    queryParameters.Add("_talukaid", talukaId);
                    var result = (await conn.QueryAsync<SelectListItem>(Procedures.GetVillageByDistrictIdAndTalukaId, queryParameters, commandType: CommandType.StoredProcedure));
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<SelectListItem>> GetEducation(string ResourceType)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    var queryParameters = new DynamicParameters();
                    queryParameters.Add("_resourcetype", ResourceType);
                    var result = (await conn.QueryAsync<SelectListItem>(Procedures.BindResourceValue, queryParameters, commandType: CommandType.StoredProcedure));
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<DocumentDetails>> GetFileDocuments(int ServiceId)
        {
            try
            {

                using (var conn = GetConnection())
                {
                    var queryParameters = new DynamicParameters();
                    queryParameters.Add("_serviceid", ServiceId);

                    var result = (await conn.QueryAsync<DocumentDetails>(Procedures.GetFileDocuments, queryParameters, commandType: CommandType.StoredProcedure)).ToList();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public async Task<ResponseMessage> AddUpdateApplication(PersonalDetailsModel personalDetailsModel)
        public async Task<ResponseMessage> AddUpdateApplication(PersonalDetailsModel personalDetailsModel)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    var procName = "CALL public.bocw_ssy_insertpersonaldetails(@in_registrationid,@in_userprofileid,@in_photo,@in_name,@in_nameingujarati," +
                        "@in_fatherorhusbandnameingujarati,@in_fatherorhusbandname,@in_dateofbirth,@in_mobileno,@in_phoneno,@in_emailid,@in_casteid,@in_gender," +
                        "@in_caddressineng,@in_caddressinguj,@in_cdistrictid,@in_ctalukaid ,@in_cvillageid,@in_cpincode,@in_paddressineng,@in_paddressinguj," +
                        "@in_pstateid,@in_pdistrictid,@in_ptalukaid,@in_pvillageid,@in_ppincode,@in_createdby,@in_createddate,@in_isdeleted,@in_ipaddress,@in_hostname,@in_serviceid,@in_sequenceno," +
                        "@out_error,@out_msg,@out_id,@out_applicatinno)";

                    ResponseMessage res = new ResponseMessage();
                    var queryParameters = new Dapper.DynamicParameters();
                    queryParameters.Add("@in_registrationid", personalDetailsModel.RegistrationId);
                    queryParameters.Add("@in_userprofileid", personalDetailsModel.UserProfileId);
                    queryParameters.Add("@in_photo", Convert.ToString(personalDetailsModel.Photo.FileName));
                    queryParameters.Add("@in_name", personalDetailsModel.Name);
                    queryParameters.Add("@in_nameingujarati", personalDetailsModel.NameinGujarati);
                    queryParameters.Add("@in_fatherorhusbandnameingujarati", personalDetailsModel.FatherOrHusbandNameinGujarati);
                    queryParameters.Add("@in_fatherorhusbandname", personalDetailsModel.FatherOrHusbandName);
                    queryParameters.Add("@in_dateofbirth", personalDetailsModel.DateOfBirth);
                    queryParameters.Add("@in_mobileno", personalDetailsModel.MobileNo);
                    queryParameters.Add("@in_phoneno", personalDetailsModel.PhoneNo);
                    queryParameters.Add("@in_emailid", personalDetailsModel.EmailId);
                    queryParameters.Add("@in_casteid", personalDetailsModel.CasteId);
                    queryParameters.Add("@in_gender", personalDetailsModel.Gender);
                    queryParameters.Add("@in_caddressineng", personalDetailsModel.CAddressInEng);
                    queryParameters.Add("@in_caddressinguj", personalDetailsModel.CAddressInGuj);
                    queryParameters.Add("@in_cdistrictid", personalDetailsModel.CDistrictId);
                    queryParameters.Add("@in_ctalukaid", personalDetailsModel.CTalukaId);
                    queryParameters.Add("@in_cvillageid", personalDetailsModel.CVillageId);
                    queryParameters.Add("@in_cpincode", personalDetailsModel.CPinCode);
                    queryParameters.Add("@in_paddressineng", personalDetailsModel.PAddressInEng);
                    queryParameters.Add("@in_paddressinguj", personalDetailsModel.PAddressInGuj);
                    queryParameters.Add("@in_pstateid", personalDetailsModel.PStateId);
                    queryParameters.Add("@in_pdistrictid", personalDetailsModel.PDistrictId);
                    queryParameters.Add("@in_ptalukaid", personalDetailsModel.PTalukaId);
                    queryParameters.Add("@in_pvillageid", personalDetailsModel.PVillageId);
                    queryParameters.Add("@in_ppincode", personalDetailsModel.PPinCode);
                    queryParameters.Add("@in_createdby", personalDetailsModel.CreatedBy);
                    queryParameters.Add("@in_createddate", personalDetailsModel.CreatedDate);
                    queryParameters.Add("@in_isdeleted", personalDetailsModel.IsDeleted);
                    queryParameters.Add("@in_ipaddress", personalDetailsModel.IpAddress);
                    queryParameters.Add("@in_hostname", personalDetailsModel.HostName);
                    queryParameters.Add("@in_serviceid", personalDetailsModel.ServiceId);
                    queryParameters.Add("@in_sequenceno", personalDetailsModel.TabSequenceNo);
                    queryParameters.Add("@out_error", 0, direction: ParameterDirection.InputOutput);
                    queryParameters.Add("@out_msg", "", direction: ParameterDirection.InputOutput);
                    queryParameters.Add("@out_id", 0,  direction: ParameterDirection.InputOutput);
                    queryParameters.Add("@out_applicatinno", 0, direction: ParameterDirection.InputOutput);
                    //var result = await conn.QueryFirstOrDefaultAsync<ResponseMessage>(Procedures.BOCW_SSY_AddUpdateApplication, queryParameters);
                    // var result = await conn.QueryFirstOrDefaultAsync<ResponseMessage>(Procedures.BOCW_SSY_AddUpdateApplication, queryParameters);

                    var result = conn.Execute(procName, queryParameters);
                    res.Error = queryParameters.Get<long>("@out_error");
                    res.Msg = queryParameters.Get<string>("@out_msg");
                    res.Id = queryParameters.Get<long>("@out_id");
                    res.ApplicationNo = queryParameters.Get<long>("@out_applicatinno");
                    

                    return res;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<IList<DocumentFileDetails>> GetDocumentFileDetails(int ServiceId)
        {
            try
            {

                using (var conn = GetConnection())
                {
                    var queryParameters = new DynamicParameters();
                    queryParameters.Add("_serviceid", ServiceId);

                    var result = (await conn.QueryAsync<DocumentFileDetails>(Procedures.GetFileDocuments, queryParameters, commandType: CommandType.StoredProcedure)).ToList();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ResponseMessage> AddUpdateDocumentDetails(IList<DocumentFileDetails> lstdocumentFileDetails)
        {
            try
            {
                ResponseMessage responseMessage = new ResponseMessage();
                using (var conn = GetConnection())
                {
                    int iteration = 0;
                    foreach (var item in lstdocumentFileDetails)
                    {
                        var queryParameters = new DynamicParameters();
                        queryParameters.Add("@ApplicationId", item.ApplicationId);
                        queryParameters.Add("@DocumentMasterId", item.DocumentMasterId);
                        queryParameters.Add("@DocumentName", item.DocumentName);
                        queryParameters.Add("@DocumentNumber", item.DocumentNumber);
                        queryParameters.Add("@CreatedBy", item.CreatedBy);
                        queryParameters.Add("@ModifiedBy", item.CreatedBy);
                        queryParameters.Add("@IpAddress", CommonUtils.GetLocalIPAddress());
                        queryParameters.Add("@HostName", CommonUtils.GetHostName());
                        queryParameters.Add("@CouchDBDocId", item.CouchDBDocId);
                        var result = await conn.QueryFirstOrDefaultAsync<ResponseMessage>(Procedures.BOCW_SSY_AddUpdateDocumentDetails, queryParameters, commandType: System.Data.CommandType.StoredProcedure);

                        if (result != null)
                        {
                            responseMessage.Error = result.Error;
                            responseMessage.Msg = result.Msg;
                            responseMessage.Id = result.Id;
                        }

                        iteration++;
                        if (iteration < lstdocumentFileDetails.Count)
                        {
                            var queryParametersTab = new DynamicParameters();
                            queryParametersTab.Add("@ApplicationId", item.ApplicationId);
                            queryParametersTab.Add("@ServiceId", item.ServiceId);
                            queryParametersTab.Add("@UserId", item.CreatedBy);
                            queryParametersTab.Add("@SequenceNo", item.SequenceNo);

                            var tabResult = await conn.QueryFirstOrDefaultAsync<ResponseMessage>(Procedures.BOCW_SSY_InsertTabEntryDocumentDetails, queryParametersTab, commandType: System.Data.CommandType.StoredProcedure);
                        }
                    }



                    return responseMessage;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<ResponseMessage> AddSchemeDetails(SchemeDetails schemeDetails)
        {
            
            try
            {
                using (var conn = GetConnection())
                {
                    var procName = "CALL public.bocw_ssy_insertschemedetails(@in_applicationid,@in_serviceid,@in_userid,@in_sequenceno,@in_schmename," +
                        "@in_syllabus,@in_course,@in_acadmicyearsem,@in_startdate,@in_schoolcollagename,@in_submitteddate,@in_approveddate,@in_createdby"+
                        "@in_createddate,@in_modifiedby,@in_modifieddate,@in_isdeleted,@in_ipaddress,@in_hostname"+
                        "@out_error,@out_msg,@out_id,@out_applicatinno)";
                    ResponseMessage res = new ResponseMessage();
                    var queryParameters = new DynamicParameters();
                    queryParameters.Add("@in_applicationid", schemeDetails.ApplicationId);
                    queryParameters.Add("@in_serviceid", schemeDetails.ServiceId);
                    queryParameters.Add("@in_userid", schemeDetails.UserId);
                    queryParameters.Add("@in_sequenceno", schemeDetails.TabSequenceNo);
                    queryParameters.Add("@in_schmename", schemeDetails.Schmename);
                    queryParameters.Add("@in_syllabus", schemeDetails.Syllabus);
                    queryParameters.Add("@in_course", schemeDetails.Course);
                    queryParameters.Add("@in_acadmicyearsem", schemeDetails.AcadmicYearSem);
                    queryParameters.Add("@in_startdate", schemeDetails.StartDate);
                    queryParameters.Add("@in_schoolcollagename", schemeDetails.SchoolCollageName);
                    queryParameters.Add("@in_submitteddate", schemeDetails.SubmittedDate);
                    queryParameters.Add("@in_approveddate", schemeDetails.ApprovedDate);
                    queryParameters.Add("@in_createdby", schemeDetails.CreatedBy);
                    queryParameters.Add("@in_createddate", schemeDetails.CreatedDate);
                    queryParameters.Add("@in_modifiedby", schemeDetails.ModifiedBy);
                    queryParameters.Add("@in_modifieddate", schemeDetails.ModifiedDate);
                    queryParameters.Add("@in_isdeleted", schemeDetails.IsDeleted);
                    queryParameters.Add("@in_ipaddress", schemeDetails.IpAddress);
                    queryParameters.Add("@in_hostname", schemeDetails.HostName);
                    queryParameters.Add("@out_error", 0, direction: ParameterDirection.InputOutput);
                    queryParameters.Add("@out_msg", "", direction: ParameterDirection.InputOutput);
                    queryParameters.Add("@out_id", 0, direction: ParameterDirection.InputOutput);
                    queryParameters.Add("@out_applicatinno", 0, direction: ParameterDirection.InputOutput);
                    //var result = await conn.QueryFirstOrDefaultAsync<ResponseMessage>(Procedures.BOCW_SSY_AddSchemeDetails, queryParameters, commandType: System.Data.CommandType.StoredProcedure);
                    var result = conn.Execute(procName, queryParameters);
                    res.Error = queryParameters.Get<long>("@out_error");
                    res.Msg = queryParameters.Get<string>("@out_msg");
                    res.Id = queryParameters.Get<long>("@out_id");
                    res.ApplicationNo = queryParameters.Get<long>("@out_applicatinno");
                    return res;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ResponseMessage> AddUpdateDocumentDetailsNew(DataTable dtData)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    var queryParameters = new DynamicParameters();
                    conn.TypeMapper.MapComposite<p_in_document>("ty_documentdetails");
                    var result = await conn.QueryFirstOrDefaultAsync<ResponseMessage>("bocw_ssy_insertdocumentdetails_withtable", new
                    {
                        p_in_document = new p_in_document
                        {
                            couchdbdocid = Convert.ToString(dtData.Rows[0]["couchdbdocid"])
                        }
                    }, commandType: System.Data.CommandType.StoredProcedure);
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
