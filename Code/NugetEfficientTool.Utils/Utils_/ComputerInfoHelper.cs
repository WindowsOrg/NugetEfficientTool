using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NugetEfficientTool.Utils
{
    public static class ComputerInfoHelper
    {
        public static string GetMacAddress()
        {
            string macAddress = "";
            ManagementObjectSearcher query = new ManagementObjectSearcher("select * from Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection querylist = query.Get();
            foreach (var managementBaseObject in querylist)
            {
                if (managementBaseObject["IPEnabled"].ToString() == "True")
                {
                    macAddress = managementBaseObject["MacAddress"].ToString();
                }
            }
            return macAddress;
        }
        public static string GetAddressIp()
        {
            string ip = string.Empty;
            var ipHostEntry = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress iPAddress in ipHostEntry.AddressList)
            {
                if (iPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    ip = iPAddress.ToString();
                }
            }
            return ip;
        }
        public static string GetUserName()
        {
            return Environment.UserName;
        }
        public static string GetMachineName()
        {
            return Environment.MachineName;
        }
    }
}
