public class JsonAccount
{
    public string pass { get; set; }
    public string user { get; set; }
    public int indexChar { get; set; }
    public int indexServer { get; set; }
}

public class Storage
{
    private static Storage instance;
    private JsonAccount account;

    public static Storage gI()
    {
        return instance ??= new Storage();
    }

    public void saveAccount(string user, string pass)
    {
        account = new JsonAccount
        {
            user = user,
            pass = pass
        };
    }

    public JsonAccount getAccount()
    {
        return account;
    }

    public void saveIndexChar(int indexChar)
    {
        account.indexChar = indexChar;
    }

    public void saveServer(int indexServer)
    {
        account.indexServer = indexServer;
    }
}