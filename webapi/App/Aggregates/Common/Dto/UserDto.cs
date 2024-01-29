using System;
using System.Collections.Generic;
using webapi.App.Model.User;
using Comm.Commons.Extensions;
using System.Linq;

namespace webapi.App.Aggregates.Common
{
    public class UserDto
    {
        public static Subscriber Subscriber(IDictionary<string, object> data)
        {
            Subscriber o = new Subscriber();
            string result = data["RESULT"].Str();
            string usertype = data["USR_TYP"].Str();
            if (result == "52")
            {
                o.OTPCode = data["OTP"].Str();
                o.MobileNumber = data["MOB_NO"].Str().Replace("+63", "0");
            }
            else if (result == "1")
            {
                o.SubscriberID = data["USR_ID"].Str();
                o.GroupID = data["GRP_CD"].Str();
                o.AccountID = data["ACT_ID"].Str();
                o.ImageUrl = data["IMG_URL"].Str();
                o.DisplayName = data["NCK_NM"].Str();
                o.PresentAddress = data["PRSNT_ADD"].Str();
                o.BirthDate = data["BRT_DT"].Str();
                o.PasswordExpired = data["PSSWRD_EXP_TS"].Str();
                //
                o.IsGeneralCoordinator = (usertype.Equals("1"));
                o.IsCoordinator = (usertype.Equals("2"));
                o.IsPlayer = (usertype.Equals("3"));
                // 
                o.MobileNumber = data["MOB_NO"].Str();
                o.EmailAddress = data["EML_ADD"].Str();

                o.CreditBalance = data["ACT_CRDT_BAL"].Str().ToDecimalDouble();
                if (o.IsGeneralCoordinator || o.IsCoordinator)
                {
                    o.CommissionBalance = data["ACT_COM_BAL"].Str().ToDecimalDouble();
                    o.CommissionRate = data["ACT_COM_RT"].Str().ToDecimalDouble();
                }
                o.IsReseller = (data["S_RSLR"].Str().Equals("1"));
                var winbal = data["ACT_WIN_BAL"].Str().ToDecimalDouble();
                if (winbal > 0) o.WinningBalance = winbal;

                o.IsProduction = (data["S_PRD"].Str().Equals("1"));
                o.IsBlocked = (data["IS_BLCK"].Str().Equals("1"));
                o.LastLogIn = data["LST_LOG_IN"].Str();
                o.SessionID = data["SSSN_ID"].Str().ToLower();

                string gencoor = data["REF_ACT_ID1"].Str().Trim();
                if (!gencoor.IsEmpty())
                {
                    o.GeneralCoordinator = data["REF_ACT_NM1"].Str().Trim();
                    o.GeneralCoordinatorID = gencoor;
                }
                string coor = data["REF_ACT_ID2"].Str().Trim();
                if (!coor.IsEmpty())
                {
                    o.Coordinator = data["REF_ACT_NM2"].Str().Trim();
                    o.CoordinatorID = coor;
                }
            }
            o.BranchZipCode = data["BR_LOC_ZP"].Str();
            o.CompanyID = data["COMP_ID"].Str();
            o.BranchID = data["BR_CD"].Str();
            o.Firstname = data["FRST_NM"].Str().ToUpper();
            o.Lastname = data["LST_NM"].Str().ToUpper();
            o.Fullname = data["FLL_NM"].Str().ToUpper();
            return o;
        }


        public static IDictionary<string, object> Subscriber(Subscriber user)
        {
            dynamic o = Dynamic.Object;

            o.AccountID = user.AccountID;
            o.ImageUrl = user.ImageUrl;
            o.Firstname = user.Firstname;
            o.Lastname = user.Lastname;
            o.Fullname = user.Fullname;
            o.DisplayName = user.DisplayName;
            //
            o.MobileNumber = user.MobileNumber;
            o.EmailAddress = user.EmailAddress;
            o.PresentAddress = user.PresentAddress;
            o.BirthDate = user.BirthDate;
            //
            o.CreditBalance = user.CreditBalance;
            if (user.IsGeneralCoordinator || user.IsCoordinator)
            {
                o.CommissionBalance = user.CommissionBalance;
                o.CommissionRate = user.CommissionRate;
            }
            if (user.WinningBalance > 0) o.WinningBalance = user.WinningBalance;
            if (user.IsReseller) o.IsReseller = user.IsReseller;
            if (user.IsGeneralCoordinator) o.IsGeneralCoordinator = user.IsGeneralCoordinator;
            if (user.IsCoordinator) o.IsCoordinator = user.IsCoordinator;
            if (user.IsPlayer) o.IsPlayer = user.IsPlayer;
            //

            if (user.IsCoordinator)
            {
                if (!user.GeneralCoordinator.IsEmpty())
                {
                    o.Upline = user.GeneralCoordinator;
                    o.UplineID = user.GeneralCoordinatorID;
                }
            }
            if (user.IsPlayer)
            {
                if (!user.Coordinator.IsEmpty())
                {
                    o.Upline = user.Coordinator;
                    o.UplineID = user.CoordinatorID;
                }
            }
            o.SessionID = user.SessionID;
            o.LastLogIn = user.LastLogIn;
            //
            return o;
        }
        public static IDictionary<string, object> PreSubscriber(Subscriber user)
        {
            dynamic o = Dynamic.Object;

            o.Firstname = user.Firstname;
            o.Lastname = user.Lastname;
            o.Fullname = user.Fullname;
            o.MobileNumber = user.MobileNumber;
            o.OTPCode = user.OTPCode;

            return o;
        }




        public static Operator WebOperator(IDictionary<string, object> data)
        {
            Operator o = new Operator();
            string usertype = data["USR_TYP"].Str().Trim();

            o.Firstname = data["FRST_NM"].Str().ToUpper();
            o.Lastname = data["LST_NM"].Str().ToUpper();
            o.Fullname = data["FLL_NM"].Str().ToUpper();

            o.CompanyID = data["COMP_ID"].Str();
            o.BranchID = data["BR_CD"].Str();
            o.OperatorID = data["USR_ID"].Str();

            o.PresentAddress = data["PRSNT_ADDR"].Str();
            o.EmailAddress = data["EML_ADD"].Str();
            o.MobileNumber = data["MOB_NO"].Str();

            o.IsCompanyAgent = (usertype.Equals("2"));
            o.IsBranchAgent = (usertype.Equals("3"));
            o.IsInAccounting = (usertype.Equals("4"));
            o.IsInTreasury = (usertype.Equals("5"));
            o.IsInOperations = (usertype.Equals("6"));
            o.IsInAccounts = (usertype.Equals("7"));
            o.IsInOperationHead = (usertype.Equals("8"));

            o.IsBlocked = (data["S_BLCK"].Str().Equals("1"));
            o.SessionID = data["SSSN_ID"].Str().ToLower();

            return o;
        }



        public static IDictionary<string, object> WebOperator(Operator user)
        {
            dynamic o = Dynamic.Object;
            o.Firstname = user.Firstname;
            o.Lastname = user.Lastname;
            o.Fullname = user.Fullname;

            o.CompanyID = user.CompanyID;
            o.BranchID = user.BranchID;
            //o.OperatorID = user.Fullname;

            o.PresentAddress = user.PresentAddress;
            o.EmailAddress = user.EmailAddress;
            o.MobileNumber = user.MobileNumber;

            if (user.IsCompanyAgent) o.IsCompanyAgent = user.IsCompanyAgent;
            if (user.IsBranchAgent) o.IsBranchAgent = user.IsBranchAgent;
            if (user.IsInAccounting) o.IsInAccounting = user.IsInAccounting;
            if (user.IsInTreasury) o.IsInTreasury = user.IsInTreasury;
            if (user.IsInOperations) o.IsInOperations = user.IsInOperations;
            if (user.IsInAccounts) o.IsInAccounts = user.IsInAccounts;
            if (user.IsInOperationHead) o.IsInOperationHead = user.IsInOperationHead;

            return o;
        }







    }
}