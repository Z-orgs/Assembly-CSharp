using System.Threading;
using UnityEngine;

public class Main : MonoBehaviour
{
    public static Main main;

    public static GameCanvas canvas;

    public static mGraphics g;

    public static GameMidlet midlet;

    public static string res;

    public static string mainThreadName;

    public static bool started;

    public static bool isIpod;

    public static bool isAppTeam;

    public static bool isPC;

    public static bool isWp;

    public static bool IphoneVersionApp;

    public static string IMEI;

    public static int versionIp;

    public static int level;

    public static int a;

    public static bool isCompactDevice;

    public bool isWorldver;

    private Vector2 lastMousePos;

    private int paintCount;

    private int updateCount;

    static Main()
    {
        res = "res";
        a = 1;
        isCompactDevice = true;
    }

    private void Start()
    {
        if (!started)
        {
            level = RMS.loadRMSInt("levelScreenKN");
            if (level == 1)
                Screen.SetResolution(1024, 600, false);
            else
                Screen.SetResolution(1024, 600, false);
            if (Thread.CurrentThread.Name != "Main") Thread.CurrentThread.Name = "Main";
            mainThreadName = Thread.CurrentThread.Name;
            Storage.gI().saveAccount(RMS.loadRMSString("acc"), RMS.loadRMSString("pass"));
            AutoReconnect.gI().Login();
            if (iPhoneSettings.generation == iPhoneGeneration.iPodTouch4Gen) isIpod = true;
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            Application.runInBackground = true;
            Application.targetFrameRate = 30;
            useGUILayout = false;
            isCompactDevice = detectCompactDevice();
            if (main == null) main = this;
            started = true;
            ScaleGUI.initScaleGUI();
            IMEI = SystemInfo.deviceUniqueIdentifier;
            isPC = true;
            isWp = false;
            isAppTeam = false;
            IphoneVersionApp = false;
            if (isPC) Screen.fullScreen = false;
            g = new mGraphics();
            midlet = new GameMidlet();
            GameMidlet.isWorldver = isWorldver;
            canvas = new GameCanvas();
            TileMap.loadTileMapArr();
            SplashScr.gI().switchToMe();
            Sound.init();
        }
    }

    private void Update()
    {
        if (!isPC)
        {
            var num = 1 / a;
        }
    }

    private void FixedUpdate()
    {
        ipKeyboard.update();
        canvas.update();
        RMS.update();
        Image.update();
        DataInputStream.update();
        SMS.update();
        Net.update();
        updateCount++;
        Sound.update();
        Code.Fixed();
    }

    private void OnGUI()
    {
        checkInput();
        Session_ME.update();
        if (Event.current.type.Equals(EventType.Repaint) && paintCount <= updateCount)
        {
            canvas.paint(g);
            paintCount++;
            g.reset();
        }
    }

    private void OnApplicationQuit()
    {
        Code.T = null;
        Debug.LogWarning("APP QUIT");
        GameCanvas.bRun = false;
        Session_ME.gI().close();
        Code.Stop();
        if (isPC) Application.Quit();
    }

    public void doClearRMS()
    {
        if (isPC && RMS.loadRMSInt("lastZoomlevel") != mGraphics.zoomLevel)
        {
            RMS.clearRMS();
            RMS.saveRMSInt("lastZoomlevel", mGraphics.zoomLevel);
            RMS.saveRMSInt("levelScreenKN", level);
        }
    }

    private void checkInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mousePosition = Input.mousePosition;
            canvas.pointerPressed((int)(mousePosition.x / mGraphics.zoomLevel), (int)((Screen.height - mousePosition.y) / mGraphics.zoomLevel));
            lastMousePos.x = mousePosition.x / mGraphics.zoomLevel;
            lastMousePos.y = mousePosition.y / mGraphics.zoomLevel;
        }

        if (Input.GetMouseButton(0))
        {
            var mousePosition2 = Input.mousePosition;
            canvas.pointerDragged((int)(mousePosition2.x / mGraphics.zoomLevel), (int)((Screen.height - mousePosition2.y) / mGraphics.zoomLevel));
            lastMousePos.x = mousePosition2.x / mGraphics.zoomLevel;
            lastMousePos.y = mousePosition2.y / mGraphics.zoomLevel;
        }

        if (Input.GetMouseButtonUp(0))
        {
            var mousePosition3 = Input.mousePosition;
            lastMousePos.x = mousePosition3.x / mGraphics.zoomLevel;
            lastMousePos.y = mousePosition3.y / mGraphics.zoomLevel;
            canvas.pointerReleased((int)(mousePosition3.x / mGraphics.zoomLevel), (int)((Screen.height - mousePosition3.y) / mGraphics.zoomLevel));
        }

        if (Input.anyKeyDown && Event.current.type == EventType.KeyDown)
        {
            var num = MyKeyMap.map(Event.current.keyCode);
            if (num != 0) canvas.keyPressed(num);
        }

        if (Event.current.type == EventType.KeyUp)
        {
            var num2 = MyKeyMap.map(Event.current.keyCode);
            if (num2 != 0) canvas.keyReleased(num2);
        }
    }

    public static void exit()
    {
        if (isPC)
            main.OnApplicationQuit();
        else
            a = 0;
    }

    public static bool detectCompactDevice()
    {
        if (iPhoneSettings.generation == iPhoneGeneration.iPhone || iPhoneSettings.generation == iPhoneGeneration.iPhone3G || iPhoneSettings.generation == iPhoneGeneration.iPodTouch1Gen ||
            iPhoneSettings.generation == iPhoneGeneration.iPodTouch2Gen) return false;
        return true;
    }

    public static bool checkCanSendSMS()
    {
        if (iPhoneSettings.generation == iPhoneGeneration.iPhone3GS || iPhoneSettings.generation == iPhoneGeneration.iPhone4 || iPhoneSettings.generation > iPhoneGeneration.iPodTouch4Gen) return true;
        return false;
    }
}