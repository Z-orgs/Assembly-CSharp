using System.Threading;

class AutoReconnect
{
    string user;
    string pass;
    int indexChar;
    public static AutoReconnect instance;
    public static AutoReconnect gI()
    {
        if (instance == null)
        {
            instance = new AutoReconnect();
        }
        return instance;
    }
    public static Thread threadLogin;
    public void GetInfo()
    {
        user = RMS.Read("Info", "user", "");
        pass = RMS.Read("Info", "pass", "");
        indexChar = int.Parse(RMS.Read("Info", "char", "0"));
    }
    public void login()
    {
        if (threadLogin == null || threadLogin.IsAlive == false)
        {
            threadLogin = new Thread(new ThreadStart(doLogin));
            threadLogin.Start();
        }
    }
    public void doLogin()
    {
        while (true)
        {
            Thread.Sleep(2000);
            if (SelectServerScr.isLoad == true && Code.isAutoLogin)
            {
                RMS.saveRMSString("acc", user);
                RMS.saveRMSString("pass", pass);
                GameCanvas.loginScr.tfUser.setText(user);
                GameCanvas.loginScr.tfPass.setText(pass);
                Thread.Sleep(1000);
                GameCanvas.selectsvScr.perform(1003, null);
                Thread.Sleep(2000);
                SelectCharScr.gI().indexSelect = indexChar;
                SelectCharScr.gI().perform(1000, null);
                GameCanvas.gameTick = 0;
                Thread.Sleep(2000);
            }
        }
    }
}
