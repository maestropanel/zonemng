namespace MaestroPanel.MsDnsZoneManager
{
    using System.Collections.Generic;

    internal interface DataProvider
    {
        List<DnsZoneRecord> GetDnsZoneRecords(string zoneName);
        List<DnsZone> GetDnsZones(bool withRecords = false, string where = "");
    }
}
