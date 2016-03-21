namespace usage
{
    using MaestroPanel.MsDnsZoneManager;

    class Program
    {
        static void Main(string[] args)
        {
            var dns = new ZoneManage();

            foreach (var item in dns.GetAllZones())
            {
                System.Console.WriteLine(item.Name);

                foreach (var r in item.Records)
                {
                    System.Console.WriteLine("\t{0}\t{1}\t{2}\t{3}", r.RecordType, r.Name, r.Data, r.Priority);                    
                }
            }            
        }
    }
}
