using System;
using System.ComponentModel.DataAnnotations;
namespace webapi.App.RequestModel.AppRecruiter
{
    public class SignInRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public string DeviceID { get; set; }
        public string DeviceName { get; set; }
        public bool Terminal { get; set; }

        //[Required]
        public string ApkVersion { get; set; }

        //[Required]
        public string CoordinateLocation { get; set; }
        //[Required]
        public string AddressLocation { get; set; }
    }
    public class STLSignInRequest
    {
        public string Username;
        public string Password;
        public string plid;
        public string groupid;
        public string psncd;
    }
    public class RequiredChangePassword
    {
        public string PLID;
        public string PGRPID;
        public string Username;
        public string OldPassword;
        public string Password;
        public string ConfirmPassword;
    }
    public class BIMSSMSIN 
    {
        public string Id;
        public string SMSID;
        public string SendTime;
        public string Messagefrom;
        public string Messageto;
        public string Messagetext;
        public string Messagetype;
        public string MessageRead;
    }
    public class FetchLoadNews
    {
        public string category;
        public string jsonnews;
    }
}
