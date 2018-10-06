namespace MaestroPanel.MsDnsZoneManager
{
    using System.Collections.Generic;
    
    public class ZoneManage
    {
        private const string ProviderWMI = "wmi";
        private const string ProviderFILE = "file";
                
        private DataProvider provider;

        public ZoneManage(string providername = "wmi")
        {
            this.provider = selectProvider(providername);
        }

        public List<DnsZone> GetAllZones(bool withRecords = true)
        {
            return provider.GetDnsZones(withRecords);
        }

        public List<DnsZoneRecord> GetZoneRecords(string zoneName)
        {
            return provider.GetDnsZoneRecords(zoneName);
        }

        private DataProvider selectProvider(string prv)
        {
            if (prv == ProviderFILE)
                return new DataAccessFile();   

            if (prv == ProviderWMI)            
                return new DataAccessWMI();

         
            return null;
        }
    }
}