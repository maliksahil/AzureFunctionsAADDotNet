using System.Globalization;
using System.Collections.Generic;

namespace Company.Function
{
    // demo code, usually want to pull these from key vault or config etc.
    internal static class Constants
    {
        internal static string audience = "https://sahilcalledfunction.sahilmalikgmail.onmicrosoft.com"; // Get this value from the expose an api, audience uri section example https://appname.tenantname.onmicrosoft.com
        internal static string clientID = "d397a3ab-266e-4743-8aac-9f2440d5f159"; // this is the client id, a GUID
        internal static string tenant = "sahilmalikgmail.onmicrosoft.com"; // this is your tenant name
        internal static string tenantid = "dd1790d8-0aaa-403b-8a1c-43e7cca9b589"; // this is your tenant id (GUID)

        // rest of the values below can be left as is in most circumstances
        internal static string aadInstance = "https://login.microsoftonline.com/{0}/v2.0";
        internal static string authority = string.Format(CultureInfo.InvariantCulture, aadInstance, tenant);
        internal static List<string> validIssuers = new List<string>()
            {
                $"https://login.microsoftonline.com/{tenant}/",
                $"https://login.microsoftonline.com/{tenant}/v2.0",
                $"https://login.windows.net/{tenant}/",
                $"https://login.microsoft.com/{tenant}/",
                $"https://sts.windows.net/{tenantid}/"
            };
    }
}