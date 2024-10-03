using System;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Inspirit.IDAS.Data;
using Inspirit.IDAS.Data.IDAS;
using Microsoft.AspNetCore.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.NetworkInformation;
using NSwag.Annotations;
using Inspirit.IDAS.WebApplication.Services;

namespace Inspirit.IDAS.WebApplication
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : Controller
    {
        [DllImport("Iphlapi.dll")]
        private static extern int SendARP(int dest, int ScrIP, byte[] pMacAddr, ref int PhyAddrLen);
        //private static extern int SendARP(Int32 dest, Int32 host, ref Int64 mac, ref Int32 length);
        [DllImport("Ws2_32.dll")]
        private static extern Int32 inet_addr(string ip);
        SecurityService _securityService;
        private IHostingEnvironment _hostingEnvironment;
        IDASDbContext _context;
        GoogleRecaptchaService _googleRecaptchaService;
        public SecurityController(SecurityService securityService, IHostingEnvironment hostingEnvironment, IDASDbContext iDASDbContext, GoogleRecaptchaService googleRecaptchaService)
        {
            _securityService = securityService;
            _hostingEnvironment = hostingEnvironment;
            _context = iDASDbContext;
            _googleRecaptchaService = googleRecaptchaService;
        }
        public static PhysicalAddress GetMacAddress(IPAddress ipaddress)
        {
            const int MacAddressLength = 6;
            int length = MacAddressLength;
            var macBytes = new byte[MacAddressLength];
            SendARP(BitConverter.ToInt32(ipaddress.GetAddressBytes(), 0), 0, macBytes, ref length);
            return new PhysicalAddress(macBytes);
        }
        //public string getMAC_AddressTest()
        //{
        //    // var data = "";
        //    string userip;
        //    try
        //    {
        //        userip = this.HttpContext.Connection.RemoteIpAddress.ToString();//HttpContext.Connection.RemoteIpAddress.ToString();

        //        var strClientIP = HttpContext.Connection.RemoteIpAddress.ToString().Trim();

        //        Int32 ldest = inet_addr(strClientIP.ToString());

        //        Int32 lhost = inet_addr("");
        //        Int64 macinfo = new Int64();
        //        Int32 len = 6;

        //        int res = SendARP(-1, 0, ref macinfo, ref len);
        //        string mac_src = macinfo.ToString("X");
        //        var data = "";

        //        if (mac_src == "0")
        //        {
        //            if (userip.ToString() == "0")
        //            {
        //                data += "visited localGhost";
        //            }
        //            else
        //            {
        //                data += "the IP from" + userip + "" + "<br>";
        //            }
        //        }


        //        while (mac_src.Length < 12)
        //        {
        //            mac_src = mac_src.Insert(0, "0");
        //        }


        //        string mac_dest = "";

        //        for (int i = 0; i < 11; i++)
        //        {
        //            if (0 == (i % 2))
        //            {
        //                if (i == 10)
        //                {
        //                    mac_dest = mac_dest.Insert(0, mac_src.Substring(i, 2));
        //                }
        //                else
        //                {
        //                    mac_dest = "-" + mac_dest.Insert(0, mac_src.Substring(i, 2));
        //                }
        //            }
        //        }


        //        data = "ip address = " + userip.ToString() + " mac address : " + mac_dest + " ";
        //        return data;
        //    }
        //    catch (Exception e)
        //    {
        //        return " test ";// + userip + "    " + XYZ + "     " + e.Message;
        //    }
        //}


        [HttpPost("[action]")]
        public async Task<ActionResult<LoginReponse>> Login([FromBody]LoginRequest request)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }
            else
            {
                //PhysicalAddress mac = GetMacAddress(this.HttpContext.Connection.LocalIpAddress);
                IPAddress iPAddress = this.HttpContext.Connection.RemoteIpAddress;
                iPAddress = CheckLocalOrClientAddress(iPAddress);
                LoginReponse res = await _securityService.AuthUser(request, iPAddress);

                return res;
            }
        }
        //XdsLoginRequest
        [HttpPost("[action]")]
        public async Task<ActionResult<LoginReponse>> XdsLogin([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
            {

                return BadRequest(token);
            }
            else
            {
                //PhysicalAddress mac = GetMacAddress(this.HttpContext.Connection.LocalIpAddress);
                //IPAddress iPAddress = this.HttpContext.Connection.RemoteIpAddress;
                //iPAddress = CheckLocalOrClientAddress(iPAddress);
                //LoginReponse res = await _securityService.AuthUser(request, iPAddress);
                LoginReponse res = await _securityService.AuthXdsUser(token);
                return res;
                //return null;
            }
        }
        //public async Task<ActionResult<LoginReponse>> XdsLogin(string token)
        
        /*
        public async Task<ActionResult<LoginReponse>> XdsLoginNew([FromBody] XdsLoginRequest request)
        {
            if (string.IsNullOrEmpty(request.token))
            {

                return BadRequest(request.token);
            }
            else
            {
                //PhysicalAddress mac = GetMacAddress(this.HttpContext.Connection.LocalIpAddress);
                //IPAddress iPAddress = this.HttpContext.Connection.RemoteIpAddress;
                //iPAddress = CheckLocalOrClientAddress(iPAddress);
                //LoginReponse res = await _securityService.AuthUser(request, iPAddress);
                LoginReponse res = await _securityService.AuthXdsUser(request.token);
                return res;
                //return null;
            }
        }
        */
        private IPAddress CheckLocalOrClientAddress(IPAddress iPAddress)
        {
            string temporary = iPAddress.ToString();
            if (iPAddress.ToString() == "::1")
            {
                return this.HttpContext.Connection.LocalIpAddress;
            }

            return iPAddress;
        }

        public string getMachAddress()
        {
            var ipRemote = this.HttpContext.Connection.RemoteIpAddress;
            IPAddress ipLocal = null;

            try
            {

                ipLocal = this.HttpContext.Connection.LocalIpAddress;

            }
            catch (Exception e)
            {
                ipLocal = null;
            }
            if (!CheckIfIPAddressLocal(ipRemote, ipLocal))
            {
                //getMAC_AddressTest();
            }

            return "";
        }

        private bool CheckIfIPAddressLocal(IPAddress ipRemote, IPAddress ipLocal)
        {
            if (ipRemote == ipLocal)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        [HttpGet("[action]")]

        public async Task<ActionResult<string>> Logout([FromQuery] Guid Id, bool istrailuser)
        {
            string msg = string.Empty;
            try
            {

                string userexist = _securityService.Logout(Id, istrailuser);
                if (userexist != null)
                {
                    msg = "User successfully logged off";
                }
            }
            catch (Exception ex)
            {
                _securityService.WriteLog(ex.Message, "Exception Session");
            }
            return msg;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<string>> GetAdminlink()
        {
            return await _securityService.GetAdminUrl();

        }
        [HttpPost("[action]")]
        public async Task<ActionResult<LoginReponse>> TrailuserLogin([FromBody]LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                LoginReponse res = await _securityService.TrailuserAuth(request);
                return res;
            }
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<SignUpResponse>> SignUp([FromBody]SignUpRequest request)
        {
            SignUpResponse sr = new SignUpResponse();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {
                    int index = 0;
                    request.customer.Id = Guid.NewGuid();
                    string tempfolder = "TempCostomerdocs";
                    string permanentfolder = "Signupcustomers";
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    string temppath = Path.Combine(webRootPath, tempfolder);
                    string newPath = Path.Combine(webRootPath, permanentfolder);
                    string mergePdftarget = Path.Combine(newPath, "Customer" + request.customer.Id + ".pdf");

                    //For Admin start
                    string adminfolder = "Customerdocuments";
                    string adminRootPath = _hostingEnvironment.WebRootPath.Replace("WebApplication", "Admin");
                    string parentpath = Path.Combine(adminRootPath, adminfolder);
                    //For Admin end

                    if (!Directory.Exists(newPath))
                    {
                        Directory.CreateDirectory(newPath);
                    }

                    if (!Directory.Exists(parentpath))
                    {
                        Directory.CreateDirectory(parentpath);
                    }
                    string custname = request.customer.Id.ToString();
                    string childpath = Path.Combine(parentpath, custname);
                    if (!Directory.Exists(childpath))
                    {
                        Directory.CreateDirectory(childpath);
                    }
                    string adminfullpath = Path.Combine(childpath, "signupdocument" + "_" + request.customer.RegistrationNumber + ".pdf");

                    foreach (var base64 in request.base64Array)
                    {
                        string fileName = "";
                        fileName = "Customer" + (index) + ".pdf";
                        if (index == 2)
                            fileName = "Customer" + (index == 2 ? 3 : index) + ".pdf";
                        if (index == 3)
                            fileName = "Customer" + (index == 3 ? 4 : index) + ".pdf";

                        string fullPath = Path.Combine(temppath, fileName);
                        if (!Directory.Exists(temppath))
                        {
                            Directory.CreateDirectory(temppath);
                        }
                        byte[] imageBytes = Convert.FromBase64String(base64);
                        Image image = Image.GetInstance(imageBytes);
                        using (FileStream fs = new FileStream(fullPath, FileMode.Create))
                        {
                            Document document = new Document(PageSize.A4,
                                request.floats[0], request.floats[1], request.floats[2], request.floats[3]);
                            PdfWriter writer = PdfWriter.GetInstance(document, fs);
                            image.ScaleToFit(request.floats[4], request.floats[5]);
                            document.Open();
                            document.Add(image);
                            document.Close();
                        }
                        index++;
                    }
                    if (!String.IsNullOrEmpty(request.htmlString))
                    {
                        string fileName = "Customer" + 2 + ".pdf";
                        string fullPath = Path.Combine(temppath, fileName);
                        if (!Directory.Exists(temppath))
                        {
                            Directory.CreateDirectory(temppath);
                        }
                        StringBuilder sb = new StringBuilder();
                        sb.Append(request.htmlString);
                        StringReader strReader = new StringReader(sb.ToString());
                        Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                        HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                        using (FileStream fileStream = new FileStream(fullPath, FileMode.Create))
                        {
                            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, fileStream);
                            pdfDoc.Open();

                            htmlparser.Parse(strReader);
                            pdfDoc.Close();
                        }
                    }
                    DirectoryInfo d = new DirectoryInfo(temppath);
                    FileInfo[] Files = d.GetFiles("*.pdf");
                    string[] mergedtargetFiles = new string[2];
                    mergedtargetFiles[0] = mergePdftarget;
                    mergedtargetFiles[1] = adminfullpath;
                    string attachFilePath = MergePDFs(Files, mergedtargetFiles);
                    sr.isSucsess = true;
                    foreach (var file in Files)
                    {
                        if (System.IO.File.Exists(Path.Combine(newPath, file.FullName)))
                        {
                            System.IO.File.Delete(Path.Combine(newPath, file.FullName));
                        }
                    }
                    Directory.Delete(temppath);
                    if (sr.isSucsess == true)
                        sr = await _securityService.Register(request);

                    if (sr.Message == "successfully registered.")
                    {
                        string adminmailid = _context.ApplicationSetting.Where(t => t.SettingName == "IDAS Admin Email").Select(t => t.SettingValue).FirstOrDefault();
                        string msg = await _securityService.SendMail(adminmailid, attachFilePath);
                        sr.Message = await _securityService.SendMail(request.toMail, attachFilePath);
                    }
                    else
                    {
                        sr.isSucsess = false;
                        sr.Message = sr.Message;
                    }
                }
                catch (Exception ex)
                {
                    sr.isSucsess = false;
                    sr.Message = ex.Message;
                }
                return sr;
            }
        }
        [NonAction]
        public static string MergePDFs(FileInfo[] Files, string[] targetPdf)
        {
            string merged = "";
            foreach (var f in targetPdf)
            {
                using (FileStream stream = new FileStream(f, FileMode.Create))
                {
                    Document document = new Document();
                    PdfCopy pdf = new PdfCopy(document, stream);
                    PdfReader reader = null;
                    try
                    {
                        document.Open();
                        foreach (var file in Files)
                        {
                            reader = new PdfReader(file.FullName);
                            pdf.AddDocument(reader);
                            reader.Close();
                        }
                        merged = f;
                    }
                    catch (Exception)
                    {
                        merged = "";
                        if (reader != null)
                        {
                            reader.Close();
                        }
                    }
                    finally
                    {
                        if (document != null)
                        {
                            document.Close();
                        }
                    }
                }
            }
            return merged;
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<DonoCallRegistryResponse>> callregistry([FromBody]DonotCallRegistryRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                DonoCallRegistryResponse dr = await _securityService.callregistry(request);
                return dr;
            }
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<LoginReponse>> ForgotPassword(string email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                LoginReponse res = await _securityService.ForgotPassword(email);
                return res;
            }
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<DataServiceResponse>> GetDsa(string userid)
        {
            DataServiceResponse dataServiceResponse = new DataServiceResponse();
            try
            {
                var dataServicesAgreement = await _securityService.GetDSA(userid);
                dataServiceResponse.Version = dataServicesAgreement.Version;
                dataServiceResponse.FileName = dataServicesAgreement.FilePath;
                dataServiceResponse.Description = dataServicesAgreement.Description;
                dataServiceResponse.IsSuccess = true;
                dataServiceResponse.Message = "";
            }
            catch (Exception ex)
            {
                dataServiceResponse.IsSuccess = false;
                dataServiceResponse.Message = ex.Message;
            }
            return dataServiceResponse;
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<SignUpResponse>> RegistrationNumberVerify([FromBody]SignUpRequest request)
        {
            SignUpResponse vrify = await _securityService.RegistrationNumberVerify(request);
            return vrify;
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<List<CustomerLog>>> GetLogs(string userid)
        {
            var res = await _securityService.GetLogs(userid);
            return res;
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<List<TrailUserLog>>> GetTrailLogs(string userid)
        {
            var res = await _securityService.GetTrailUserLogs(userid);
            return res;
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<List<Menu>>> GetUserMenu(Guid Id, int screensize)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                var res = await _securityService.GetUserMenu(Id, screensize);
                return res;
            }
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<List<LookupData>>> GetLookupDatas()
        {
            return await _securityService.GetLookupDatas();
        }
        [HttpPost("[action]")]
        public ActionResult<LoginReponse> ResetPassword(string username, string oldpassword, string code)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
             {
                LoginReponse res = _securityService.ResetPassword(username, oldpassword, code);
                return res;
            }
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<List<ApplicationMessages>>> GetMessages(Guid Id)
        {
            var res = _securityService.GetApplicationMessage(Id);
            return res;
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<string>> RemoveAppMessages(Guid Id)
        {
            var res = _securityService.RemoveMessage(Id);
            return res;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<IActionResult>> DownloadHelpPdf()
        {
            string folderName = "HelpDocument";
            string webRootPath = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            string filepath = newPath + @"\" + "Helpdocument.pdf";
            var memory = new MemoryStream();
            try
            {
                using (var stream = new FileStream(filepath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
            }
            catch (System.Exception ex) { }
            return File(memory, "application/pdf", "Helpdocument.pdf");
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<string>> GetLastPasswordResetDateCustomerUser(Guid Id)
        {
            // krishna
            return null;
            //return _securityService.getLastPasswordResetDateCustomerUser(Id);
        }


        [HttpPost("[action]")]
        public async Task<ActionResult<GoogleRensponse>> getRECAPTCHAResponse(string token)
        {
            return await _googleRecaptchaService.VerifyResponse(token);
        }

    }
}