using System;
using System.IO;
using UnityEngine;

public class SelectServerScr : mScreen, IActionListener
{
    private static LanguageScr gI;

    public static bool isFromLogin;

    public static string[] menu;

    public static string uname;

    public static string pass;

    public static string unameChange;

    public static string passChange;

    public static Command cmdChoiMoi;

    public static Command cmdDoiTaiKhoan;

    public static Command cmdChoiTiep;

    public static Command cmdChonServer;

    public static Command cmdUpdLinkSv;

    public static Command[][] cmd;

    public static int ipSelect;

    public static bool isLoad;

    private int indexRow = -1;

    public bool isFAQ;

    public string listFAQ = string.Empty;

    private int popupH;
    private int popupW;

    private int popupX;

    private int popupY;

    public string randomResuft;

    public string subtitleFAQ;

    public string titleFAQ;

    static SelectServerScr()
    {
        unameChange = string.Empty;
        passChange = string.Empty;
    }

    public void perform(int idAction, object p)
    {
        if (idAction == 12345)
        {
            var myVector = new MyVector();
            myVector.addElement(new Command("World", this, 123450, null));
            myVector.addElement(new Command("Việt", this, 123451, null));
            GameCanvas.menu.startAt(myVector, 3);
        }
        else if (idAction == 123450)
        {
            var myVector2 = new MyVector();
            var path = Application.persistentDataPath + "/Acc/World";
            if (Directory.Exists(path))
            {
                var flag = false;
                var files = new DirectoryInfo(path).GetFiles("*");
                foreach (var fileInfo in files)
                {
                    myVector2.addElement(new Command(fileInfo.Name, this, 123452, null));
                    flag = true;
                }

                if (!flag) GameCanvas.startOKDlg("Hiện không có tài khoản trong danh sách");
                GameCanvas.menu.startAt(myVector2, 3);
            }
            else
            {
                Directory.CreateDirectory(path);
                GameCanvas.startOKDlg("Đã tạo mới thư mục tài khoản");
            }
        }
        else if (idAction == 123452)
        {
            var text = Application.persistentDataPath + "/Acc/World";
            if (Directory.Exists(text))
            {
                try
                {
                    var files = new DirectoryInfo(text).GetFiles("*");
                    foreach (var fileInfo2 in files)
                        if (((Command)GameCanvas.menu.menuItems.elementAt(GameCanvas.menu.menuSelectedItem)).caption == fileInfo2.Name)
                        {
                            uname = File.ReadAllText(text + "/" + fileInfo2.Name).Split(',')[0];
                            pass = File.ReadAllText(text + "/" + fileInfo2.Name).Split(',')[1];
                            RMS.saveRMSString("acc", uname);
                            RMS.saveRMSString("pass", pass);
                            GameCanvas.selectsvScr.switchToMe();
                            return;
                        }
                }
                catch (Exception)
                {
                    GameCanvas.startYesNoDlg("Lỗi ! bạn có muốn xóa hết dữ liệu không ?", new Command("Có", this, 1, null), new Command("Không", this, 2, null));
                    return;
                }

                GameCanvas.startOKDlg("Không có thông tin");
            }
            else
            {
                Directory.CreateDirectory(text);
                GameCanvas.startOKDlg("Đã tạo mới thư mục tài khoản");
            }
        }
        else if (idAction <= 10001)
        {
            switch (idAction)
            {
                case 1000:
                    if (isVirtualAcc() && !uname.Equals(string.Empty))
                    {
                        GameCanvas.startYesNoDlg(mResources.NEW_ACC_ARLET, new Command(mResources.COUNTINUE_PLAY, this, 10001, null), new Command(mResources.NO, GameCanvas.instance, 8882, null));
                        break;
                    }

                    doViewFAQ();
                    Service.gI().login("-1", "12345", "1.8.0");
                    break;
                case 1001:
                    if (isVirtualAcc() && !uname.Equals(string.Empty) && unameChange.Equals(string.Empty))
                        GameCanvas.startYesNoDlg(mResources.NEW_ACC_ARLET, new Command(mResources.COUNTINUE, this, 10004, null), new Command(mResources.NO, GameCanvas.instance, 8882, null));
                    else
                        GameCanvas.loginScr.switchToMe();
                    break;
                case 1002:
                    doSelectServer();
                    break;
                case 1003:
                    doViewFAQ();
                    if (!unameChange.Equals(string.Empty))
                    {
                        uname = unameChange;
                        pass = passChange;
                        unameChange = string.Empty;
                        passChange = string.Empty;
                        RMS.saveRMSString("acc", uname);
                        RMS.saveRMSString("pass", pass);
                        Storage.gI().saveAccount(uname, pass);
                    }

                    Service.gI().login(uname, pass, "1.8.0");
                    break;
                case 10001:
                    doViewFAQ();
                    Service.gI().login("-1", "12345", "1.8.0");
                    if (!unameChange.Equals(string.Empty))
                    {
                        uname = unameChange;
                        pass = passChange;
                        unameChange = string.Empty;
                        passChange = string.Empty;
                        RMS.saveRMSString("acc", uname);
                        RMS.saveRMSString("pass", pass);
                        Storage.gI().saveAccount(uname, pass);
                    }

                    break;
            }
        }
        else if (idAction != 10004)
        {
            if (idAction - 20000 <= 19)
            {
                if (Session_ME.gI().isConnected()) Session_ME.gI().close();
                var num = idAction - 20000;
                GameCanvas.menu.showMenu = false;
                GameMidlet.IP = GameMidlet.ipList[num];
                GameMidlet.PORT = GameMidlet.portList[num];
                GameMidlet.serverLogin = GameMidlet.serverLoginList[num];
                saveIndexServer(num);
                GameCanvas.menu.menuSelectedItem = GameMidlet.serverST[num];
                GameCanvas.connect(7);
            }
            else if (idAction == 20100)
            {
                GameCanvas.startWaitDlg();
                GameMidlet.getStrSv();
                GameCanvas.endDlg();
            }
        }
        else
        {
            GameCanvas.currentDialog = null;
            GameCanvas.loginScr.switchToMe();
        }
    }

    public static void loadIP()
    {
        ipSelect = loadIndexServer();
        if (ipSelect < 0 || ipSelect > GameMidlet.nameServer.Length - 1)
            loadInfoIp(GameMidlet.GetWorldIndex());
        else
            loadInfoIp(ipSelect);
    }

    public static void loadInfoIp(int index)
    {
        if (Session_ME.gI().isConnected()) Session_ME.gI().close();
        ipSelect = index;
        saveIndexServer(ipSelect);
        GameMidlet.IP = GameMidlet.ipList[ipSelect];
        GameMidlet.PORT = GameMidlet.portList[ipSelect];
        GameMidlet.serverLogin = GameMidlet.serverLoginList[ipSelect];
        mResources.loadLanguage(0);
        GameCanvas.menu.menuSelectedItem = GameMidlet.serverST[ipSelect];
        GameCanvas.connect(5);
    }

    public override void switchToMe()
    {
        Code.Stop();
        isLoad = true;
        if (RMS.loadRMSInt("isKiemduyet") == 0)
            GameCanvas.isKiemduyet = true;
        else
            GameCanvas.isKiemduyet = false;
        GameScr.gH = GameCanvas.h;
        if (GameCanvas.typeBg == 2)
            GameCanvas.loadBG(0);
        else
            GameCanvas.loadBG(TileMap.bgID);
        base.switchToMe();
        if (GameScr.instance != null) GameScr.instance = null;
        TileMap.bgID = (sbyte)(mSystem.currentTimeMillis() % 9);
        if (TileMap.bgID == 5 || TileMap.bgID == 6) TileMap.bgID = 4;
        GameScr.loadCamera(true);
        GameScr.cmx = 100;
        popupW = 170;
        popupH = 175;
        if (GameCanvas.w == 128 || GameCanvas.h <= 208)
        {
            popupW = 126;
            popupH = 160;
        }

        popupX = GameCanvas.w / 2 - popupW / 2;
        popupY = GameCanvas.h / 2 - popupH / 2;
        if (GameCanvas.h <= 250) popupY -= 10;
        left = new Command(mResources.LANGUAGE, GameCanvas.instance, 8886, null);
        right = new Command("Tài khoản", this, 12345, null);
        indexRow = -1;
        if (!GameCanvas.isTouch) indexRow = 0;
        if (cmdChoiMoi == null)
        {
            cmdChoiMoi = new Command(!GameCanvas.isTouch ? mResources.OK : string.Empty, this, 1000, null);
            cmdDoiTaiKhoan = new Command(!GameCanvas.isTouch ? mResources.OK : string.Empty, this, 1001, null);
            cmdChonServer = new Command(!GameCanvas.isTouch ? mResources.OK : string.Empty, this, 1002, null);
            cmdChoiTiep = new Command(!GameCanvas.isTouch ? mResources.OK : string.Empty, this, 1003, null);
            cmdUpdLinkSv = new Command(!GameCanvas.isTouch ? mResources.OK : string.Empty, this, 20100, null);
            cmd = new Command[2][]
            {
                new Command[4] { cmdChoiMoi, cmdDoiTaiKhoan, cmdChonServer, cmdUpdLinkSv },
                new Command[5] { cmdChoiTiep, cmdChoiMoi, cmdDoiTaiKhoan, cmdChonServer, cmdUpdLinkSv }
            };
        }

        uname = RMS.loadRMSString("acc");
        pass = RMS.loadRMSString("pass");
        if (uname == null) uname = string.Empty;
        if (pass == null) pass = string.Empty;
        if ((uname == null || uname.Equals(string.Empty)) && unameChange.Equals(string.Empty))
        {
            menu = new string[4]
            {
                mResources.NEW_PLAY,
                mResources.CHANGE_ACC,
                mResources.SERVER,
                mResources.UPDATE_LINKSV
            };
        }
        else
        {
            Storage.gI().saveAccount(uname, pass);
            menu = new string[5]
            {
                mResources.COUNTINUE_PLAY,
                mResources.NEW_PLAY,
                mResources.CHANGE_ACC,
                mResources.SERVER,
                mResources.UPDATE_LINKSV
            };
        }

        GameCanvas.menu.menuSelectedItem = GameMidlet.GetWorldIndex();
        GameMidlet.IP = GameMidlet.ipList[GameMidlet.GetWorldIndex()];
        if (loadIndexServer() > -1 && loadIndexServer() < GameMidlet.ipList.Length)
        {
            GameCanvas.menu.menuSelectedItem = loadIndexServer();
            GameMidlet.IP = GameMidlet.ipList[loadIndexServer()];
        }

        if (RMS.loadRMSString("random") == null) RMS.saveRMSString("random", randomNumberlist());
        if (LoginScr.imgTitle == null)
        {
            if (Main.isAppTeam)
                LoginScr.imgTitle = GameCanvas.loadImage("/tt1");
            else
                LoginScr.imgTitle = GameCanvas.loadImage("/tt");
        }
    }

    public override void paint(mGraphics g)
    {
        g.setColor(0);
        g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);
        GameCanvas.paintBGGameScr(g);
        g.drawImage(LoginScr.imgTitle, GameCanvas.hw - LoginScr.imgTitle.getWidth() / 2, popupY + 10 - LoginScr.imgTitle.getHeight() / 2, 0);
        if (!GameCanvas.isTouch && GameCanvas.menu.menuSelectedItem == -1) GameCanvas.menu.menuSelectedItem = 0;
        var num = popupY + 50;
        for (var i = 0; i < menu.Length; i++)
        {
            g.setColor(Paint.COLORDARK);
            g.fillRect(popupX + 10, num + i * 35, popupW - 20, 28);
            GameCanvas.paintz.paintFrameBorder(popupX + 10, num + i * 35, popupW - 20, 28, g);
            if (i == indexRow)
            {
                g.setColor(Paint.COLORLIGHT);
                g.fillRect(popupX + 10, num + i * 35, popupW - 20, 28);
                GameCanvas.paintz.paintFrameBorder(popupX + 10, num + i * 35, popupW - 20, 28, g);
            }

            if (i >= menu.Length) continue;
            if (uname.Equals(string.Empty) && unameChange.Equals(string.Empty))
            {
                if (i == 2)
                {
                    var text = GameMidlet.nameServer[loadIndexServer()];
                    mFont.tahoma_7b_white.drawString(g, menu[i] + text, popupX + popupW / 2, num + i * 35 + 8, 2);
                }
                else
                {
                    mFont.tahoma_7b_white.drawString(g, menu[i], popupX + popupW / 2, num + i * 35 + 6, 2);
                }

                continue;
            }

            switch (i)
            {
                case 0:
                    mFont.tahoma_7b_white.drawString(g, menu[i] + (!unameChange.Equals(string.Empty) ? ": " + unameChange : !uname.StartsWith("tmpusr") ? ": " + uname : string.Empty),
                        popupX + popupW / 2, num + i * 35 + 6, 2);
                    break;
                case 3:
                {
                    var text2 = GameMidlet.nameServer[loadIndexServer()];
                    mFont.tahoma_7b_white.drawString(g, menu[i] + text2, popupX + popupW / 2, num + i * 35 + 8, 2);
                    break;
                }
                default:
                    mFont.tahoma_7b_white.drawString(g, menu[i], popupX + popupW / 2, num + i * 35 + 6, 2);
                    break;
            }
        }

        if (GameCanvas.currentDialog == null) GameCanvas.paintz.paintCmdBar(g, left, center, right);
        base.paint(g);
    }

    public override void update()
    {
        if (uname.Equals(string.Empty) && unameChange.Equals(string.Empty))
        {
            if (indexRow > -1 && indexRow < cmd[0].Length) center = cmd[0][indexRow];
        }
        else if (indexRow > -1 && indexRow < cmd[1].Length)
        {
            center = cmd[1][indexRow];
        }

        GameScr.cmx++;
        if (GameScr.cmx > GameCanvas.w * 3 + 100) GameScr.cmx = 100;
        base.update();
    }

    public override void updateKey()
    {
        if (GameCanvas.keyPressedz[2] || GameCanvas.keyPressedz[4])
        {
            indexRow--;
            if (indexRow < 0) indexRow = menu.Length - 1;
        }
        else if (GameCanvas.keyPressedz[8] || GameCanvas.keyPressedz[6])
        {
            indexRow++;
            if (indexRow > menu.Length - 1) indexRow = 0;
        }

        if (GameCanvas.isPointerJustRelease && GameCanvas.isPointerHoldIn(popupX + 10, popupY + 45, popupW - 10, 170))
        {
            if (GameCanvas.isPointerClick) indexRow = (GameCanvas.py - (popupY + 45)) / 35;
            if (uname.Equals(string.Empty) && unameChange.Equals(string.Empty))
            {
                if (indexRow > -1 && indexRow < cmd[0].Length) cmd[0][indexRow].performAction();
            }
            else if (indexRow > -1 && indexRow < cmd[1].Length)
            {
                cmd[1][indexRow].performAction();
            }
        }

        base.updateKey();
        GameCanvas.clearKeyPressed();
    }

    protected void doSelectServer()
    {
        var myVector = new MyVector();
        if (GameMidlet.indexClient == 0)
        {
            if (GameMidlet.isWorldver)
                for (var i = GameMidlet.GetLastIndex() + 1; i <= GameMidlet.language.Length - 1; i++)
                    myVector.addElement(new Command(GameMidlet.nameServer[i], this, 20000 + i, null));
            else
                for (var j = 0; j <= GameMidlet.GetLastIndex(); j++)
                    myVector.addElement(new Command(GameMidlet.nameServer[j], this, 20000 + j, null));
            GameCanvas.menu.startAt(myVector, 0);
            if (loadIndexServer() != -1) GameCanvas.menu.menuSelectedItem = GameMidlet.serverST[loadIndexServer()];
        }
        else
        {
            GameCanvas.menu.showMenu = false;
            GameMidlet.IP = GameMidlet.ipList[GameMidlet.GetWorldIndex()];
            saveIndexServer(GameMidlet.GetWorldIndex());
        }
    }

    public static void saveIndexServer(int index)
    {
        RMS.saveRMSInt("indServer", index);
        Storage.gI().saveServer(index);
    }

    public static int loadIndexServer()
    {
        var account = Storage.gI().getAccount();
        return account.indexServer;
        // return RMS.loadRMSInt("indServer");
    }

    public void doViewFAQ()
    {
        if (!listFAQ.Equals(string.Empty) || !listFAQ.Equals(string.Empty))
        {
        }

        if (!Session_ME.connected)
        {
            isFAQ = true;
            GameCanvas.connect(6);
        }

        GameCanvas.startWaitDlg();
    }

    public static bool isVirtualAcc()
    {
        if (uname != null && (uname.StartsWith("tmpusr") || uname.Equals(string.Empty))) return true;
        return false;
    }

    public static string randomNumberlist()
    {
        var text = string.Empty;
        for (var i = 0; i < 12; i++)
        {
            var text2 = Res.random(0, 9).ToString();
            text += text2;
        }

        return text;
    }
}