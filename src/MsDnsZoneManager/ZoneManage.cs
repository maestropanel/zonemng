namespace MaestroPanel.MsDnsZoneManager
{
    using System.Collections.Generic;
    
    public class ZoneManage
    {
        private DataAccess _db;

        public ZoneManage()
        {
            _db = new DataAccess();            
        }

        public List<DnsZone> GetAllZones(bool withRecords = true)
        {
            return _db.GetDnsZones(withRecords);
        }

        public List<DnsZoneRecord> GetZoneRecords(string zoneName)
        {
            return _db.GetDnsZoneRecords(zoneName);
        }
    }
}