using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    public GameObject PreviewPanel;
    private HorizontalLayoutGroup Layout;
    private static Object PreviewButton;
    private static int _currentPreviewIndex;
    private static FrameSequenceController SequenceController;
    void Start()
    {
        PreviewButton = Resources.Load(@"PreviewToggle");
        Layout = PreviewPanel.GetComponent<HorizontalLayoutGroup>();
        SequenceController = FindObjectOfType<FrameSequenceController>();
    }
 
    public void AddPreview()
    {
        var button = (GameObject)Instantiate(PreviewButton, PreviewPanel.transform);
        var layoutLength = PreviewPanel.transform.childCount;
        button.transform.SetSiblingIndex(layoutLength - 2);
        button.name = (layoutLength - 2).ToString();
        _currentPreviewIndex = layoutLength - 2;
        button.GetComponent<PreviewButton>().SetImage(_currentPreviewIndex);

        SequenceController.CreateFrame();
    }
    public static void ReadPreview(int index)
    {
        SequenceController.ReadFrame(index);
        _currentPreviewIndex = index;
        Debug.Log(string.Format("{0} is ready", _currentPreviewIndex));
    }
    public void UpdatePreview()
    {
        SequenceController.UpdateFrame(_currentPreviewIndex);
        PreviewPanel.transform.GetChild(_currentPreviewIndex).GetComponent<PreviewButton>().SetImage(_currentPreviewIndex);
        Debug.Log(string.Format("{0} has changed", _currentPreviewIndex));
    }
    public void DeletePreview()
    {
        SequenceController.DeleteFrame(_currentPreviewIndex);
        Destroy(PreviewPanel.transform.GetChild(_currentPreviewIndex).gameObject);
        Debug.Log(string.Format("{0} is deleted now", _currentPreviewIndex));
    }

    
}
