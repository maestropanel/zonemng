namespace MaestroPanel.MsDnsZoneManager
{
    using System;
    using System.Collections.Generic;
    using System.Management;

    internal class DataAccess
    {
        public List<DnsZoneRecord> GetDnsZoneRecords(string zoneName)
        {            
            var _tm = new List<DnsZoneRecord>();

            var a = GetDnsRecords(zoneName, "A", "OwnerName", "IPAddress");
            var aaaa = GetDnsRecords(zoneName, "AAAA", "OwnerName", "IPv6Address");
            var cname = GetDnsRecords(zoneName, "CNAME", "OwnerName", "RecordData");
            var mx = GetDnsRecords(zoneName, "MX", "OwnerName", "MailExchange");
            var ns = GetDnsRecords(zoneName, "NS", "OwnerName", "NSHost");
            var txt = GetDnsRecords(zoneName, "TXT", "OwnerName", "RecordData");
            var srv = GetDnsRecords(zoneName, "SRV", "OwnerName", "TextRepresentation");

            _tm.AddRange(a);
            _tm.AddRange(aaaa);
            _tm.AddRange(cname);
            _tm.AddRange(mx);
            _tm.AddRange(ns);
            _tm.AddRange(txt);
            _tm.AddRange(srv);

            return _tm;
        }

        public List<DnsZone> GetDnsZones(bool withRecords = false, string where = "")
        {
            var _tm = new List<DnsZone>();
            var _query = "SELECT * FROM MicrosoftDNS_Zone";

            if (!String.IsNullOrEmpty(where))
                _query += String.Format(" WHERE ContainerName = '{0}'", where);
            
            using (var query = GetProperties(_query))
            {
                foreach (ManagementObject item in query)
                {
                    var z = new DnsZone();
                    z.Name = GetValue<String>(item, "ContainerName");
                    z.SOA = GetSOAType(z.Name);

                    if (withRecords)
                        z.Records = GetDnsZoneRecords(z.Name);                    

                    _tm.Add(z);
                }
            }

            return _tm;
        }

        private SOAType GetSOAType(string zoneName)
        {
            var s = new SOAType();

            using (var query = GetProperties(String.Format("SELECT * FROM MicrosoftDNS_SOAType WHERE ContainerName = '{0}'", zoneName)))
            {
                foreach (ManagementObject item in query)
                {
                    s.ExpireLimit = GetValue<uint>(item,"ExpireLimit");
                    s.MinimumTTL = GetValue<uint>(item, "MinimumTTL");
                    s.PrimaryServer = GetValue<string>(item, "PrimaryServer");
                    s.RefreshInterval = GetValue<uint>(item, "RefreshInterval");
                    s.ResponsibleParty = GetValue<string>(item, "ResponsibleParty");
                    s.RetryDelay = GetValue<uint>(item, "RetryDelay");
                    s.SerialNumber = GetValue<uint>(item, "SerialNumber");
                    s.TextRepresentation = GetValue<string>(item, "TextRepresentation");
                    
                    break;
                }
            }

            return s;
        }

        private List<DnsZoneRecord> GetDnsRecords(string zoneName, string recordType, string recordName, string recordData)
        {
            var _tm = new List<DnsZoneRecord>();
            var _query = String.Format("SELECT * FROM MicrosoftDNS_{1}Type WHERE ContainerName='{0}'", zoneName, recordType);

            using (var query = GetProperties(_query))
            {
                foreach (ManagementObject item in query)
                {
                    var r = new DnsZoneRecord();
                    r.RecordType = recordType;
                    r.Name = GetValue<string>(item, recordName);
                    r.Data = GetValue<string>(item, recordData);
                    r.TextRepresentation = GetValue<string>(item, "TextRepresentation");
                    r.Priority = GetValue<UInt16>(item, "Preference");

                    if (recordType == "TXT")
                        r.Data = clearQuotesTXTRecord(r.Data);

                    _tm.Add(r);
                }
            }

            return _tm;
        }

        private ManagementObjectCollection GetProperties(string query)
        {
            using (var s = GetSearcher(query))            
                return s.Get();
        }

        private ManagementObjectSearcher GetSearcher(string query)
        {
            return new ManagementObjectSearcher("root\\MicrosoftDNS", query);
        }

        private T GetValue<T>(ManagementObject source, string name)
        {                        
            var def = default(T);

            if (String.IsNullOrEmpty(name))
                return def;

            if (source == null)
                return def;

            if (!isExists(source, name))
                return def;            

            def = (T)source[name];
            
            return def;
        }

        private bool isExists(ManagementObject source, string name)
        {
            var exists = false;

            foreach (var item in source.Properties)
            {
                if (item.Name == name)
                {
                    exists = true;
                    break;
                }
            }

            return exists;
        }

        private string clearQuotesTXTRecord(string dataValue)
        {
            if (String.IsNullOrEmpty(dataValue))
                return dataValue;

            if (dataValue.StartsWith("\"") && dataValue.EndsWith("\""))
            {                
                dataValue = dataValue.Remove(0, 1);
                dataValue = dataValue.Remove(dataValue.Length - 1, 1);
            }

            return dataValue;
        }
    }
}
