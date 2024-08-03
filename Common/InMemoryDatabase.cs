namespace Common;

internal static class InMemoryDatabase
{
    internal static readonly Dictionary<string, string> DatabaseLocation = new()
    {
        { "yesncf", "{Server=1.1.1.1;DatabaseID=Yesncf}" },
        { "helloexo", "{Server=2.2.2.2;DatabaseID=Helloexo}" },
    };

    internal static readonly Dictionary<string, Payment[]> Payments = new()
    {
        { "{Server=1.1.1.1;DatabaseID=Yesncf}", new Payment[] {
            new Payment{Id = 1,Amount = 100},
            new Payment{Id = 2,Amount = 200}
           }
        },
        { "{Server=2.2.2.2;DatabaseID=Helloexo}", new Payment[] {
            new Payment{Id = 1,Amount = 500},
            new Payment{Id = 2,Amount = 600}
        }},
    };
}
