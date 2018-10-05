namespace MaestroPanel.MsDnsZoneManager
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    internal class MsDnsZoneParser
    {
        private const string SOA_PATTERN = @"@\s+IN\s{2}SOA\s([a-z0-9\-\.]*).\s{2}([a-z0-9\-\.]*).\s\(\s*(\d+).+\s*(\d+).+\s*(\d+).+\s*(\d+).+\s*(\d+).+";
        private const string NS_PATTERN = @"^@\s.+NS\t(.+)\.";
        private const string A_PATTERN = @"(^[a-zA-Z0-9@\*]+)\s.+A\t(.+)\s";
        private const string MX_PATTERN = @"(^[a-zA-Z0-9@\*\s]+)\s.+MX\t(\d+)\s(.+)\.";
        private const string TXT_PATTERN = @"(^[a-zA-Z0-9@\*]+)\s.+TXT\t\(\s\""(.*)\""\s\)";
        private const string CNAME_PATTERN = @"(^[a-zA-Z0-9@\*]+)\s.+CNAME\t(.+)\.";
        private const string ZONE_NAME_PATTERN = @"for\s(.+)\szone.";

        private string _zoneDirectory;

        public MsDnsZoneParser()
        {
            this._zoneDirectory = String.Format("{0}\\dns",System.Environment.SystemDirectory);

            if (!Directory.Exists(this._zoneDirectory))
                throw new DirectoryNotFoundException(this._zoneDirectory);
        }

        private List<string> GetFiles()
        {
            return Directory.GetFiles(_zoneDirectory, "*.dns").ToList();
        }

        public List<DnsZone> Start()
        {
            var _tmp = new List<DnsZone>();
            var dnsZoneText = String.Empty;

            foreach (var item in GetFiles())
            {
                dnsZoneText = File.ReadAllText(item);
                _tmp.Add(GetZoneFromText(dnsZoneText));
            }

            return _tmp;
        }

        public DnsZone GetZoneFromText(string zoneFileText)
        {
            var zone = new DnsZone();
            zone.Name = GetZoneNameFromText(zoneFileText);
            zone.SOA = GetSoaFromText(zoneFileText);            

            var A_Records = GetZoneRecords(zoneFileText, A_PATTERN, RecordTypes.A, 1, 2, -1);
            var CNAME_Records = GetZoneRecords(zoneFileText, CNAME_PATTERN, RecordTypes.CNAME, 1, 2, -1);
            var MX_Records = GetZoneRecords(zoneFileText, MX_PATTERN, RecordTypes.MX, 1, 3, 2);
            var NS_Records = GetZoneRecords(zoneFileText, NS_PATTERN, RecordTypes.NS, -1, 1, -1);
            var TXT_Records = GetZoneRecords(zoneFileText, TXT_PATTERN, RecordTypes.TXT, 1, 2, -1);

            zone.Records.AddRange(A_Records);
            zone.Records.AddRange(CNAME_Records);
            zone.Records.AddRange(MX_Records);
            zone.Records.AddRange(NS_Records);
            zone.Records.AddRange(TXT_Records);

            return zone;
        }

        private SOAType GetSoaFromText(string zoneFileText)
        {
            var soa = new SOAType();
            var match = Regex.Match(zoneFileText, SOA_PATTERN, RegexOptions.Multiline);

            if (match.Success)
            {
                soa.PrimaryServer = ReadGroupValue(match.Groups, 1);
                soa.ResponsibleParty = ReadGroupValue(match.Groups, 2);
                soa.SerialNumber = Convert.ToUInt32(ReadGroupValueLong(match.Groups, 3));
                soa.RefreshInterval = Convert.ToUInt32(ReadGroupValueInt(match.Groups, 4));
                soa.RetryDelay = Convert.ToUInt32(ReadGroupValueInt(match.Groups, 5));
                soa.ExpireLimit = Convert.ToUInt32(ReadGroupValueInt(match.Groups, 6));
                soa.MinimumTTL = Convert.ToUInt32(ReadGroupValueInt(match.Groups, 7));
            }

            return soa;
        }

        private List<DnsZoneRecord> GetZoneRecords(string zoneFileText, string regexPattern,
                                    RecordTypes recordType, int nameIndex, int valueIndex, int priorityIndex)
        {
            var _tmp = new List<DnsZoneRecord>();
            var matches = Regex.Matches(zoneFileText, regexPattern, RegexOptions.Multiline);

            foreach (Match item in matches)
            {
                var match = new DnsZoneRecord();
                match.Name = ReadGroupValue(item.Groups, nameIndex);                
                match.Data = ReadGroupValue(item.Groups, valueIndex);
                match.RecordType = recordType.ToString();
                match.Priority = Convert.ToUInt16(ReadGroupValueInt(item.Groups, priorityIndex));

                _tmp.Add(match);
            }

            return _tmp;
        }

        private string GetZoneNameFromText(string zoneFileText)
        {
            var zoneName = String.Empty;
            var match = Regex.Match(zoneFileText, ZONE_NAME_PATTERN, RegexOptions.Multiline);

            if (match.Success)
                zoneName = match.Groups[1].Value;

            return zoneName;
        }

        private string ReadGroupValue(GroupCollection groups, int gIndex)
        {
            var gResult = String.Empty;

            if (groups == null)
                return String.Empty;

            if (groups[gIndex] != null)
                if (gIndex <= groups.Count)
                    gResult = groups[gIndex].Value;

            return gResult.Trim();

        }

        private long ReadGroupValueLong(GroupCollection groups, int gIndex)
        {
            long rResult = 0;
            var gValue = ReadGroupValue(groups, gIndex);

            long.TryParse(gValue, out rResult);

            return rResult;
        }

        private int ReadGroupValueInt(GroupCollection groups, int gIndex)
        {
            int rResult = 0;
            var gValue = ReadGroupValue(groups, gIndex);

            int.TryParse(gValue, out rResult);

            return rResult;
        }
    }
}
