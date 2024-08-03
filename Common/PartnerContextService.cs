namespace Common;


public interface IPartnerContextService
{
    Task<PartnerContext> GetPartnerContextAsync(string partnerName);
}

public class PartnerContextService : IPartnerContextService
{
    public async Task<PartnerContext> GetPartnerContextAsync(string partnerName)
    {
        //Simulate external call to get the Database location information
        await Task.Delay(1);
        var databaseLocation = InMemoryDatabase.DatabaseLocation[partnerName];
        return new PartnerContext(partnerName, databaseLocation);
    }
}
