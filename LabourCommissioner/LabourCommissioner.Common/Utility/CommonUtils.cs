﻿using System.Net.Mail;
using System.Text;
using SmtpClient = System.Net.Mail.SmtpClient;
using LabourCommissioner.Abstraction.ViewDataModels;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Data;
using System.Reflection;
using System.Security.Cryptography;

namespace LabourCommissioner.Common.Utility
{
    public class CommonUtils
    {
        private readonly IConfiguration appConfig;
        private readonly IHttpClientFactory _clientFactory;
        private readonly string _couchDbUrl;
        private readonly string _couchDbName;
        private readonly string _couchDbUser;

        private readonly string _sMTPConfigSenderAddress;
        private readonly string _sMTPConfigSenderDisplayName;
        private readonly string _sMTPConfigUserName;
        private readonly string _sMTPConfigPassword;
        private readonly string _sMTPConfigHost;
        private readonly int _sMTPConfigPort;
        private readonly bool _sMTPConfigEnableSSL;
        private readonly string _sMTPConfigUseDefaultCredentials;
        private readonly string _sMTPConfigIsBodyHTML;
        //IConfiguration config;
        public CommonUtils(IConfiguration config)
        {
            appConfig = config ?? throw new ArgumentNullException(nameof(config));
            _couchDbUrl = appConfig["CouchDB:URL"];
            _couchDbName = appConfig["CouchDB:DbName"];
            _couchDbUser = appConfig["CouchDB:User"];

            _sMTPConfigSenderAddress = appConfig["SMTPConfig:_SenderAddress"];
            _sMTPConfigSenderDisplayName = appConfig["SMTPConfig:_SenderDisplayName"];
            _sMTPConfigUserName = appConfig["SMTPConfig:_UserName"];
            _sMTPConfigPassword = appConfig["SMTPConfig:_Password"];
            _sMTPConfigHost = appConfig["SMTPConfig:_Host"];
            _sMTPConfigPort = Convert.ToInt32(appConfig["SMTPConfig:_Port"]);
            _sMTPConfigEnableSSL = Convert.ToBoolean(appConfig["SMTPConfig:_EnableSSL"]);
            _sMTPConfigUseDefaultCredentials = appConfig["SMTPConfig:_UseDefaultCredentials"];
            _sMTPConfigIsBodyHTML = appConfig["SMTPConfig:_IsBodyHTML"];



        }
        public CommonUtils(IConfiguration config, IHttpClientFactory clientFactory)
        {
            appConfig = config ?? throw new ArgumentNullException(nameof(config));
            _clientFactory = clientFactory;
            _couchDbUrl = appConfig["CouchDB:URL"];
            _couchDbName = appConfig["CouchDB:DbName"];
            _couchDbUser = appConfig["CouchDB:User"];
        }

        #region Sent Email
        public bool SendPasswordMail(string DisplayName, string emailAddress, string OTP_Code, string UserName, string Password, int UserType, string ImagePath, string URL)
        {
            string strMailBody = "";

            string path = UserType == 1
                    ? Path.Combine(ImagePath + "\\MailTemplate\\MailVerification.html")
                    : Path.Combine(ImagePath + "\\MailTemplate\\PasswordRecovery.html");

            StreamReader IDDSbody = new StreamReader(path);



            strMailBody = IDDSbody.ReadToEnd();
            strMailBody = strMailBody.Replace("{Name}", DisplayName);
            strMailBody = strMailBody.Replace("{OTPCode}", OTP_Code);
            strMailBody = strMailBody.Replace("{URL}", URL);
            strMailBody = strMailBody.Replace("{username}", UserName);
            strMailBody = strMailBody.Replace("{password}", Password);
            strMailBody = strMailBody.Replace("{ImagePath}", ImagePath);

            bool Chk = SendMail(_sMTPConfigSenderAddress, _sMTPConfigSenderDisplayName, emailAddress, _sMTPConfigSenderAddress, strMailBody, "Password Recovery Mail", "", ""
                , true, "", _sMTPConfigUserName, _sMTPConfigPassword, _sMTPConfigHost, _sMTPConfigPort, _sMTPConfigEnableSSL);

            return Chk;
        }

        public static bool SendMail(string FromEmail, string FromName, string ToEmails, string ReplyEmail, string MailBody, string MailSubject, string CcEmails, string BccEmails
            , bool IsHtml, string attPath, string UserName, string Password, string Host, int Port, bool EnableSSL)
        {
            SmtpClient smtpClient = new SmtpClient(FromEmail);
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new System.Net.NetworkCredential(UserName, Password);
            smtpClient.Port = Port;
            smtpClient.EnableSsl = EnableSSL;
            smtpClient.Host = Host;

            MailMessage objMail = new MailMessage();

            if (!string.IsNullOrEmpty(FromName))
            {
                objMail.From = new MailAddress(FromEmail, FromName);
            }
            else
            {
                objMail.From = new MailAddress(FromEmail);
            }
            if (!string.IsNullOrEmpty(ReplyEmail))
            {
                //objMail.ReplyToList.Add(ReplyEmail);
                objMail.ReplyTo = new MailAddress(ReplyEmail);

            }
            if (!string.IsNullOrEmpty(ToEmails))
            {
                objMail.To.Add(ToEmails);
            }
            if (!string.IsNullOrEmpty(CcEmails))
            {
                objMail.CC.Add(CcEmails);
            }
            if (!string.IsNullOrEmpty(BccEmails))
            {
                objMail.Bcc.Add(BccEmails);
            }
            else
            {
                //Comment for temporary purpose
                //BccEmails = System.Web.Configuration.WebConfigurationManager.AppSettings["ErrorBCCEmail"];
            }
            if (!string.IsNullOrEmpty(attPath))
            {
                Attachment path = new Attachment(attPath);

                objMail.Attachments.Add(path);
            }
            else
            {
                //Comment for temporary purpose
                //BccEmails = System.Web.Configuration.WebConfigurationManager.AppSettings["ErrorBCCEmail"];
            }
            objMail.Subject = MailSubject;
            objMail.Body = MailBody;
            objMail.IsBodyHtml = IsHtml;
            try
            {
                smtpClient.Send(objMail);
                return true;
            }
            catch (Exception e)
            {
                Exception ex = e;
                return false;
            }

        }
        #endregion

        public static string ConcatString(string strtext1, string strtext2, string seperator)
        {
            return string.Concat(strtext1, seperator, strtext2);

        }

        #region Uplod To CouchDB
        public async Task<CouchDBResponse> UplodToCouchDB(CouchDBRequest attachInfo)
        {
            var dbClient = DbHttpClient();
            var jsonData = JsonConvert.SerializeObject(attachInfo);
            var httpContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var postResult = await dbClient.PostAsync(_couchDbName, httpContent).ConfigureAwait(true);

            var result = await postResult.Content.ReadAsStringAsync();

            var savedInfo = JsonConvert.DeserializeObject<CouchDBResponse>(result);
            var requestContent = new ByteArrayContent(attachInfo.AttachmentData);
            requestContent.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");

            var putResult = await dbClient.PutAsync(_couchDbName + "/" + savedInfo.Id + "/" + attachInfo.FileName + "?rev=" + savedInfo.Rev, requestContent);
            CouchDBResponse couchDBResponse = new CouchDBResponse();
            if (putResult.IsSuccessStatusCode)
            {

                couchDBResponse.IsSuccess = true;
                couchDBResponse.Id = savedInfo.Id;
                couchDBResponse.Rev = savedInfo.Rev;
                couchDBResponse.Result = savedInfo.Result;
                //return new { IsSuccess = true, Result = await putResult.Content.ReadAsStringAsync() };
                return couchDBResponse;
            }

            //return new { IsSuccess = false, Result = putResult.ReasonPhrase };
            couchDBResponse.IsSuccess = false;
            couchDBResponse.Result = putResult.ReasonPhrase;
            return couchDBResponse;
        }

        private HttpClient DbHttpClient()
        {
            var httpClient = _clientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Clear();

            httpClient.BaseAddress = new Uri(_couchDbUrl);
            var dbUserByteArray = Encoding.ASCII.GetBytes(_couchDbUser);
            httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(dbUserByteArray));
            return httpClient;
        }

        public async Task<dynamic> GetAttachmentByteArray(string DocId, string AttName)
        {
            var dbClient = DbHttpClient();
            var dbResult = await dbClient.GetAsync(_couchDbName + "/" + DocId + "/" + AttName);
            if (dbResult.IsSuccessStatusCode)
            {
                return new
                {
                    IsSuccess = true,
                    Result = await dbResult.Content.ReadAsByteArrayAsync()
                };
            }
            return new
            {
                IsSuccess = false,
                Result = dbResult.ReasonPhrase
            };
        }

        public async Task<dynamic> UpdateAttachment(CouchDBRequest attachInfo)
        {
            var dbClient = DbHttpClient();
            var jsonData = JsonConvert.SerializeObject(attachInfo);
            var httpContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var postResult = await dbClient.PutAsync(_couchDbName + "/" + attachInfo.Id + "?rev=" + attachInfo.Rev, httpContent);
            var result = await postResult.Content.ReadAsStringAsync();
            var savedInfo = JsonConvert.DeserializeObject<CouchDBResponse>(result);
            attachInfo.Id = savedInfo.Id;
            attachInfo.Rev = savedInfo.Rev;
            var requestContent = new ByteArrayContent(attachInfo.AttachmentData);
            requestContent.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");
            var putResult = await dbClient.PutAsync(_couchDbName + "/" + savedInfo.Id + "/" + attachInfo.FileName + "?rev=" + savedInfo.Rev, requestContent);
            if (putResult.IsSuccessStatusCode)
            {
                return new
                {
                    IsSuccess = true,
                    Result = await putResult.Content.ReadAsStringAsync()
                };
            }
            return new
            {
                IsSuccess = false,
                Result = putResult.ReasonPhrase
            };
        }

        #endregion

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        public static string GetHostName()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            return host.HostName;
        }

        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name.ToLower());
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public static string EncryptCRY(string clearText)
        {
            string EncryptionKey = Convert.ToString("EncDecValue");
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
                0x49,0x76,0x61,0x6e,0x20,0x4d,0x65,0x64,0x76,0x65,0x64,0x65,0x76});

                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray()).Replace('/', '_');
                }
            }
            return clearText;
        }
        public static string DecryptCRY(string cipherText)
        {
            string EncryptionKey = Convert.ToString("EncDecValue");
            if (cipherText != null) cipherText = cipherText.Replace(" ", "+").Replace('_', '/');
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
                0x49,0x76,0x61,0x6e,0x20,0x4d,0x65,0x64,0x76,0x65,0x64,0x65,0x76});
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }


    }

}
