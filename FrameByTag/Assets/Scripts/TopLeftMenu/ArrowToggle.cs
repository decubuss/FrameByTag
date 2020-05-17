using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowToggle : MonoBehaviour
{
    [SerializeField]
    private Transform Checkmark;
    [SerializeField]
    private Toggle MyToggle;
    void Start()
    {
        Checkmark = gameObject.transform.FindDeepChild("Checkmark");
        MyToggle = gameObject.GetComponent<Toggle>();
        MyToggle.onValueChanged.AddListener(delegate { UpdateArrow(); });
        MyToggle.onValueChanged.AddListener(delegate { ShowButtonPanel(); });
    }
    private void UpdateArrow()
    {
        float scale = MyToggle.isOn ? 1 : -1;
        Checkmark.transform.localScale = new Vector3(1, scale, 1);
        //Checkmark.GetComponent<Image>().color = new Color(1, 1, 1, alpha);
    }
    private void ShowButtonPanel()
    {
        var buttonsPanel = gameObject.transform.FindDeepChild("ButtonsPanel");
        buttonsPanel.gameObject.SetActive(!MyToggle.isOn);
    }
}
