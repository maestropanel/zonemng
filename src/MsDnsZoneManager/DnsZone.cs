namespace MaestroPanel.MsDnsZoneManager
{
    using System;
    using System.Collections.Generic;

    public class DnsZone
    {
        public string Name { get; set; }
        public SOAType SOA { get; set; }
        public List<DnsZoneRecord> Records { get; set; }

        public DnsZone()
        {
            Records = new List<DnsZoneRecord>();
        }        
    }

    public class SOAType
    {
        public uint SerialNumber { get; set; }
        public string PrimaryServer { get; set; }
        public string ResponsibleParty { get; set; }
        public uint RefreshInterval { get; set; }
        public uint RetryDelay { get; set; }
        public uint ExpireLimit { get; set; }
        public uint MinimumTTL { get; set; }
        public string TextRepresentation { get; set; }
    }
    
    public class DnsZoneRecord
    {
        public string Name { get; set; }
        public string RecordType { get; set; }
        public string Data { get; set; }
        public UInt16 Priority { get; set; }
        public string TextRepresentation { get; set; }
    }

    internal enum RecordTypes
    {
        None,
        A,
        AAAA,
        CNAME,
        MX,
        NS,
        TXT
    }
}
