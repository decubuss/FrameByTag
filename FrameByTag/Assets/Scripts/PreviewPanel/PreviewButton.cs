using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PreviewButton : MonoBehaviour
{
    public GameObject Image;
    [HideInInspector]
    public int Index;
    private Toggle Toggle;
    private Image image;
    private Color NormalColor;

    void Awake()
    {
        Toggle = gameObject.GetComponent<Toggle>();
        image = Image.GetComponent<Image>();
        Toggle.onValueChanged.AddListener(OnToggleValueChanged);
        NormalColor = Toggle.colors.normalColor;
    }
    public void SetImage(int previewIndex)
    {
        ScreenshotHandler.TakeScreenshot_Static(160 * 4, 90 * 4, previewIndex);
        Index = previewIndex;
        Invoke("ApplyImage", 0.5f);

    }
    private void OnToggleValueChanged(bool isOn)
    {
        UIController.ReadPreview(Index);

        var colors = Toggle.colors;
        colors.selectedColor = isOn ? Color.green : NormalColor;
        Toggle.colors = colors;
    }
    
    private void ApplyImage()
    {
        var screenshotSprite = Sprite.Create(ScreenshotHandler.LastScreenshot, ScreenshotHandler.LastRect, new Vector2(0, 0));
        image.sprite = screenshotSprite;
    }

}
