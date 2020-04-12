using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotHandler : MonoBehaviour
{
    private static ScreenshotHandler instance;

    private Camera myCamera;
    private bool takeScreenshotOnNextFrame;

    public static Texture2D LastScreenshot;
    public static Rect LastRect;
    private int ScreenshotNumber;
    private void Awake()
    {
        instance = this;
        myCamera = Camera.main;
    }

    private void OnPostRender()
    {
        if (takeScreenshotOnNextFrame)
        {
            takeScreenshotOnNextFrame = false;
            RenderTexture renderTexture = myCamera.targetTexture;
            Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
            renderResult.ReadPixels(rect, 0, 0);

            LastRect = rect;

            SaveScreenShot(renderResult, ScreenshotNumber, renderTexture.width, renderTexture.height);

            RenderTexture.ReleaseTemporary(renderTexture);
            myCamera.targetTexture = null;

        }
    }
    private void SaveScreenShot(Texture2D render, int screenshotIndex, int width, int height)
    {
        byte[] byteArray = render.EncodeToPNG();
        string path = Application.dataPath + string.Format(@"\Resources\Images\{0}.png", screenshotIndex);
        System.IO.File.WriteAllBytes(path, byteArray);

        LastScreenshot = new Texture2D(width, height);
        LastScreenshot.LoadImage(byteArray);

        Debug.Log(screenshotIndex + " is saved");
    }
    public void TakeScreenshot(int width, int height)
    {
        myCamera.targetTexture = RenderTexture.GetTemporary(width, height, 16);
        takeScreenshotOnNextFrame = true;
    }
    public static void TakeScreenshot_Static(int width, int height, int index)
    {
        instance.TakeScreenshot(width, height);
        instance.ScreenshotNumber = index;
    }
}
