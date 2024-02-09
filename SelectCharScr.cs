public class SelectCharScr : mScreen, IActionListener
{
    public static SelectCharScr instance;

    public static bool isPlayMusic;

    private readonly Command cmdSelect;

    public sbyte[] gender;

    public int h1char;

    public int indexSelect;

    public bool isNullChar = true;

    private bool isstarOpen;

    public int[] level;

    private int moveDow = GameCanvas.h / 2 + 2;

    private int moveUp = GameCanvas.h / 2 - 2;

    public string[] name;

    public int padchar;

    public int[] partbody;

    public int[] parthead;

    public int[] partleg;

    public int[] partWp;

    public string[] phai;

    public int w1char;

    private int waitToPerform;

    public int x;

    public int y;

    public SelectCharScr()
    {
        w1char = 48;
        h1char = 85;
        if (GameCanvas.w < 160) w1char = 32;
        padchar = 7;
        x = ((GameCanvas.w - 3 * w1char) >> 1) - 5;
        y = GameCanvas.hh - (h1char >> 1) + 10;
        if (GameCanvas.isTouch && GameCanvas.w > 200)
        {
            w1char = 74;
            padchar = 25;
            h1char = 110;
            x = ((GameCanvas.w - 3 * w1char) >> 1) - 20;
            y = GameCanvas.hh - (h1char >> 1);
            if (GameCanvas.w < 320)
            {
                padchar = 6;
                x = ((GameCanvas.w - 3 * w1char) >> 1) - 6;
            }
        }

        left = null;
        cmdSelect = new Command(mResources.SELECT, this, 1000, null);
        center = new Command(string.Empty, this, 1000, null);
        right = new Command(mResources.EXIT, this, 1001, null);
        left = cmdSelect;
        if (GameCanvas.isTouch && !Main.isPC)
        {
            center = null;
            left = null;
        }

        if (GameCanvas.isTouch && GameCanvas.w >= 320)
        {
            if (Main.isPC)
            {
                left.x = GameCanvas.w / 2 - 160;
                center.x = GameCanvas.w / 2 - 35;
                left.y = center.y = GameCanvas.h - 26;
            }

            right.x = GameCanvas.w / 2 + 88;
            right.y = GameCanvas.h - 26;
        }
    }

    public void perform(int idAction, object p)
    {
        switch (idAction)
        {
            case 1000:
                doSelect();
                break;
            case 1001:
                Session_ME.gI().close();
                GameCanvas.instance.resetToLoginScr();
                break;
        }
    }

    public static SelectCharScr gI()
    {
        if (instance == null) instance = new SelectCharScr();
        return instance;
    }

    public void initSelectChar()
    {
        name = new string[3];
        parthead = new int[3];
        partleg = new int[3];
        partbody = new int[3];
        partWp = new int[3];
        level = new int[3];
        phai = new string[3];
        gender = new sbyte[3];
        if (GameCanvas.isTouch)
        {
            indexSelect = !Main.isPC ? -1 : 0;
            Storage.gI().saveIndexChar(indexSelect);
        }
        else
        {
            indexSelect = 0;
            Storage.gI().saveIndexChar(indexSelect);
        }

        GameScr.gI().readPart();
        SmallImage.init();
    }

    protected void doSelect()
    {
        if (name[indexSelect] != null)
        {
            Service.gI().selectCharToPlay(name[indexSelect]);
            GameCanvas.isLoading = true;
        }
        else
        {
            CreateCharScr.gI().switchToMe();
        }
    }

    public override void updateKey()
    {
        base.updateKey();
        if (GameCanvas.currentDialog != null) return;
        if (GameCanvas.keyPressedz[Key.NUM6] || GameCanvas.keyPressedz[25] || GameCanvas.keyPressedz[22])
        {
            indexSelect++;
            if (indexSelect >= 3) indexSelect = 0;
            Out.println("update key");
            Storage.gI().saveIndexChar(indexSelect);
        }

        if (GameCanvas.keyPressedz[Key.NUM4] || GameCanvas.keyPressedz[25] || GameCanvas.keyPressedz[23])
        {
            indexSelect--;
            if (indexSelect < 0) indexSelect = 2;
            Storage.gI().saveIndexChar(indexSelect);
        }

        if (GameCanvas.isPointerDown && GameCanvas.isPointerHoldIn(x, y, 3 * (w1char + padchar), h1char))
        {
            var num = (GameCanvas.px - x) / (w1char + padchar);
            if (num > 2) num = 2;
            if (num < 0) num = 0;
            indexSelect = num;
            Storage.gI().saveIndexChar(indexSelect);
        }

        if (GameCanvas.isPointerJustRelease)
        {
            if (GameCanvas.isPointerHoldIn(x, y, 3 * (w1char + padchar), h1char))
            {
                waitToPerform = 5;
                Sound.play(Sound.MBClick, 0.4f);
            }
            else
            {
                indexSelect = -1;
                Storage.gI().saveIndexChar(indexSelect);
            }
        }

        GameCanvas.clearKeyHold();
        GameCanvas.clearKeyPressed();
    }

    public override void update()
    {
        GameScr.cmx++;
        if (GameScr.cmx > GameCanvas.w * 3 + 100) GameScr.cmx = 100;
        updateOpen();
        if (waitToPerform > 0)
        {
            waitToPerform--;
            if (waitToPerform == 0 && indexSelect >= 0) doSelect();
        }
    }

    public override void switchToMe()
    {
        Code.Stop();
        LoginScr.isAutoLogin = false;
        base.switchToMe();
        for (var i = 0; i < name.Length; i++)
            if (name[i] != null)
            {
                isNullChar = false;
                break;
            }

        if (isNullChar) CreateCharScr.gI().switchToMe();
    }

    public override void paint(mGraphics g)
    {
        GameCanvas.paintBGGameScr(g);
        for (var i = 0; i < 3; i++)
        {
            if (indexSelect == i)
                GameCanvas.paintz.paintFrameInsideSelected(x + i * (w1char + padchar), y, w1char, h1char, g);
            else
                GameCanvas.paintz.paintFrameInside(x + i * (w1char + padchar), y, w1char, h1char, g);
            GameCanvas.paintz.paintFrameBorder(x + i * (w1char + padchar), y, w1char, h1char, g);
        }

        for (var j = 0; j < 3; j++)
        {
            if (name[j] == null) continue;
            var part = GameScr.parts[parthead[j]];
            var part2 = GameScr.parts[partleg[j]];
            var part3 = GameScr.parts[partbody[j]];
            var part4 = GameScr.parts[partWp[j]];
            if (part.pi == null || part.pi.Length < 8)
                part = Char.getMyChar().getDefaultHead(gender[j]);
            else
                for (var k = 0; k < part.pi.Length; k++)
                    if (part.pi[k] == null || !SmallImage.isExitsImage(part.pi[k].id))
                    {
                        part = Char.getMyChar().getDefaultHead(gender[j]);
                        break;
                    }

            var num = x + j * (w1char + padchar) + w1char / 2;
            var num2 = 0;
            if (!GameCanvas.isTouch)
            {
                num2 = y + h1char / 2 + 16;
                SmallImage.drawSmallImage(g, part4.pi[Char.CharInfo[0][3][0]].id, num + Char.CharInfo[0][3][1] + part4.pi[Char.CharInfo[0][3][0]].dx,
                    num2 - Char.CharInfo[0][3][2] + part4.pi[Char.CharInfo[0][3][0]].dy, 0, 0);
                SmallImage.drawSmallImage(g, part2.pi[Char.CharInfo[0][1][0]].id, num + Char.CharInfo[0][1][1] + part2.pi[Char.CharInfo[0][1][0]].dx,
                    num2 - Char.CharInfo[0][1][2] + part2.pi[Char.CharInfo[0][1][0]].dy, 0, 0);
                SmallImage.drawSmallImage(g, part3.pi[Char.CharInfo[0][2][0]].id, num + Char.CharInfo[0][2][1] + part3.pi[Char.CharInfo[0][2][0]].dx,
                    num2 - Char.CharInfo[0][2][2] + part3.pi[Char.CharInfo[0][2][0]].dy, 0, 0);
                SmallImage.drawSmallImage(g, part.pi[Char.CharInfo[0][0][0]].id, num + Char.CharInfo[0][0][1] + part.pi[Char.CharInfo[0][0][0]].dx,
                    num2 - Char.CharInfo[0][0][2] + part.pi[Char.CharInfo[0][0][0]].dy, 0, 0);
                if (indexSelect == j)
                {
                    mFont.tahoma_8b.drawString(g, mResources.CHARINGFO[0] + ": " + name[j], GameCanvas.hw, y - 45, mFont.CENTER);
                    mFont.tahoma_7b_white.drawString(g, mResources.CHARINGFO[1] + ": " + level[j], GameCanvas.hw, y - 28, 2, mFont.tahoma_7b_blue);
                    mFont.tahoma_7b_white.drawString(g, phai[j], GameCanvas.hw, y - 16, 2, mFont.tahoma_7b_blue);
                }
            }
            else
            {
                num2 = y + h1char / 2 - 5;
                SmallImage.drawSmallImage(g, part4.pi[Char.CharInfo[0][3][0]].id, num + Char.CharInfo[0][3][1] + part4.pi[Char.CharInfo[0][3][0]].dx,
                    num2 - Char.CharInfo[0][3][2] + part4.pi[Char.CharInfo[0][3][0]].dy, 0, 0);
                SmallImage.drawSmallImage(g, part2.pi[Char.CharInfo[0][1][0]].id, num + Char.CharInfo[0][1][1] + part2.pi[Char.CharInfo[0][1][0]].dx,
                    num2 - Char.CharInfo[0][1][2] + part2.pi[Char.CharInfo[0][1][0]].dy, 0, 0);
                SmallImage.drawSmallImage(g, part3.pi[Char.CharInfo[0][2][0]].id, num + Char.CharInfo[0][2][1] + part3.pi[Char.CharInfo[0][2][0]].dx,
                    num2 - Char.CharInfo[0][2][2] + part3.pi[Char.CharInfo[0][2][0]].dy, 0, 0);
                SmallImage.drawSmallImage(g, part.pi[Char.CharInfo[0][0][0]].id, num + Char.CharInfo[0][0][1] + part.pi[Char.CharInfo[0][0][0]].dx,
                    num2 - Char.CharInfo[0][0][2] + part.pi[Char.CharInfo[0][0][0]].dy, 0, 0);
                mFont.tahoma_8b.drawString(g, name[j], num, y + h1char / 2 + 5, mFont.CENTER);
                mFont.tahoma_7b_white.drawString(g, mResources.CHARINGFO[1] + ": " + level[j], num, y + h1char / 2 + 22, 2);
                if (GameCanvas.w > 200) mFont.tahoma_7b_white.drawString(g, phai[j], num, y + h1char / 2 + 34, 2);
            }
        }

        base.paint(g);
    }

    public void updateOpen()
    {
        if (isstarOpen)
        {
            if (moveUp > -1) moveUp -= 4;
            if (moveDow < GameCanvas.h) moveDow += 4;
        }
    }
}