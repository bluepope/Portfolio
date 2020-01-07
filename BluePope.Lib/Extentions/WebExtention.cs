using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace BluePope
{
    /// <summary>
    /// 웹 확장, 나중에 윈도우 개발들어가면 별도의 프로젝트로 나누는게 맞을거같음
    /// </summary>
    public static class WebExtention
    {
        public enum CustomClaimType
        {
            U_Id,
            Email,
            UserName,
            NextCheckTime
        }


        public static void AddClaim(this ClaimsIdentity claimIdentity, CustomClaimType customClaimType, string value, string type)
        {
            claimIdentity.AddClaim(new Claim(GetCustomClaimTypeString(customClaimType), value, type));
        }

        public static string GetCustomClaimTypeString(CustomClaimType customClaimType)
        {
            return customClaimType switch
            {
                CustomClaimType.U_Id => ClaimTypes.NameIdentifier,
                CustomClaimType.Email => "Email",
                CustomClaimType.UserName => "UserName",
                CustomClaimType.NextCheckTime => "NextCheckTime",
                _ => null
            };
        }

        public static Claim GetClaim(this ClaimsPrincipal principal, string claimType)
        {
            return principal.Claims.FirstOrDefault(x => x.Type == claimType);
        }
        public static string GetClaimValue(this ClaimsPrincipal principal, CustomClaimType claimType)
        {
            return principal.Claims.FirstOrDefault(x => x.Type == GetCustomClaimTypeString(claimType))?.Value;
        }
        public static string GetClaimValue(this ClaimsPrincipal principal, string claimType)
        {
            return principal.Claims.FirstOrDefault(x => x.Type == claimType)?.Value;
        }
        public static bool IsInRoleOrAdmin(this IPrincipal principal, params string[] roles)
        {
            if (principal.IsInRole("ADMIN"))
                return true;

            foreach (string role in roles)
            {
                if (principal.IsInRole(role))
                    return true;
            }

            return false;
        }

        public static HtmlString DrawSelectValue(this HtmlHelper htmlHelper, string value, string text, string selectedValue)
        {
            return new HtmlString($"<option value=\"{HttpUtility.HtmlEncode(value)}\" {(value == selectedValue ? "selected=\"selected\"" : "")}>{HttpUtility.HtmlEncode(text)}</option>");
        }

        public static bool IsLocal(this ConnectionInfo connection)
        {
            if (connection.RemoteIpAddress != null)
            {
                if (connection.LocalIpAddress != null)
                {
                    return connection.RemoteIpAddress.Equals(connection.LocalIpAddress);
                }
                else
                {
                    return IPAddress.IsLoopback(connection.RemoteIpAddress);
                }
            }

            // for in memory TestServer or when dealing with default connection info
            if (connection.RemoteIpAddress == null && connection.LocalIpAddress == null)
            {
                return true;
            }

            return false;
        }

        public static string GetRemoteIpAddress(this ConnectionInfo connection)
        {
            if (connection.IsLocal())
            {
                return "127.0.0.1";
            }
            else
            {
                return connection.RemoteIpAddress.MapToIPv4().ToString();
            }
        }
    }
}