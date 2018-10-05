namespace MaestroPanel.MsDnsZoneManager
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal class DataAccessFile : DataProvider
    {
        private MsDnsZoneParser parser;

        public DataAccessFile()
        {
            parser = new MsDnsZoneParser();
        }

        public List<DnsZoneRecord> GetDnsZoneRecords(string zoneName)
        {
            var zonePath = String.Format("{0}\\dns\\{1}.dns", System.Environment.SystemDirectory, zoneName);
            var text = File.ReadAllText(zonePath);

            var zn  = parser.GetZoneFromText(text);
            return zn.Records;
        }

        public List<DnsZone> GetDnsZones(bool withRecords = false, string where = "")
        {
            return parser.Start();
        }
    }
}
