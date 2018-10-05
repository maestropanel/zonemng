namespace MaestroPanel.MsDnsZoneManager
{
    using System.Collections.Generic;
    
    public class ZoneManage
    {
        private const string ProviderWMI = "wmi";
        private const string ProviderFILE = "file";
                
        private DataProvider provider;

        public ZoneManage(string provider = "wmi")
        {
            this.provider = selectProvider(provider);
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
            DataProvider p;

            switch (prv)
            {
                case ProviderWMI:
                    p = new DataAccessWMI();
                    break;
                case ProviderFILE:
                    p = new DataAccessFile();
                    break;
                default:
                    p = new DataAccessWMI();
                    break;
            }

            return p;
        }
    }
}