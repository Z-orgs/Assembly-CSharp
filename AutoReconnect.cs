using System.Threading;

internal class AutoReconnect
{
    private static AutoReconnect instance;

    private static Thread threadLogin;
    private int indexChar;
    private string pass;
    private string user;

    public static AutoReconnect gI()
    {
        return instance ??= new AutoReconnect();
    }

    public void Login()
    {
        if (threadLogin != null && !Code.isAutoLogin) return;
        threadLogin = new Thread(DoLogin);
        threadLogin.Start();
    }

    private void DoLogin()
    {
        while (true)
        {
            Thread.Sleep(2000);
            if (!SelectServerScr.isLoad || !Code.isAutoLogin && Session_ME.gI().isConnected()) continue;
            var account = Storage.gI().getAccount();
            GameCanvas.loginScr.tfUser.setText(account.user);
            GameCanvas.loginScr.tfPass.setText(account.pass);
            Thread.Sleep(1000);
            GameCanvas.selectsvScr.perform(1003, null);
            Thread.Sleep(2000);
            SelectCharScr.gI().indexSelect = account.indexChar;
            SelectCharScr.gI().perform(1000, null);
            GameCanvas.gameTick = 0;
            Thread.Sleep(2000);
        }
    }
}