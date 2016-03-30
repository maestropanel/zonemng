# Microsoft DNS Service Zone Manager

Microsoft DNS Servisi için .NET yönetim paketi. Bu paket ile DNS servisinideki Zone'ları yönetebilir, okuyabilirsiniz. 

## Kurulum

```powershell
Install-Package MaestroPanel.MsDnsZoneManager
``` 

## Gereksinimler

* .NET Framework 4+
* Microsoft DNS Service 6+ 

## Kullanım

```csharp
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
                    System.Console.WriteLine("\t{0}\t{1}\t{2}\t{3}", 
                                    r.RecordType, r.Name, r.Data, r.Priority);                    
                }
            }            
        }
    }
}
```