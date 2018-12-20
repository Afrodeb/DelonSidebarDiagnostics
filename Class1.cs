using System;
using System.Diagnostics;
using System.Management;

namespace SidebarDiagnostics
{
    public class Class1
    {
        public Class1()
        {
        }
       public void getCpuInfo()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * Win32_Processor");
            foreach (ManagementObject obj in searcher.Get())
            {
                if (!(obj == null))
                    Debug.Print(obj.Properties["CpuStatus"].Value.ToString());
            }
        }
    }
}
