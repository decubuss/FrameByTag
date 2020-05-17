using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInput : MonoBehaviour
{
    private InputField NameInput;
    private InputField AltnamesInput;
    private InputField ColorInput;
    private Button AcceptButton;

    private void Start()
    {
        NameInput = transform.FindDeepChild("NameInput").GetComponent<InputField>();
        AltnamesInput = transform.FindDeepChild("AltnamesInput").GetComponent<InputField>();
        ColorInput = transform.FindDeepChild("ColorInput").GetComponent<InputField>();
        AcceptButton = transform.FindDeepChild("AcceptButton").GetComponent<Button>();

        AcceptButton.onClick.AddListener(delegate { AcceptCharacter(); });
    }

    private void AcceptCharacter()
    {
        if(string.IsNullOrEmpty(NameInput.text)) { Debug.LogError("NameInput empty"); }
        if(string.IsNullOrEmpty(AltnamesInput.text)) { Debug.LogError("Altnames empty"); }
        if (string.IsNullOrEmpty(ColorInput.text)) { Debug.LogError("ColorInput empty"); }

        string name = NameInput.text.MakeCapitalLetter();
        string[] altnames= AltnamesInput.text.Split(',');
        Color color = ColorByColorCode(ColorInput.text);

        AvailableObjectsController.AddCharacter(name, altnames, color);
    }
    private Color ColorByColorCode(string colorcode)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(colorcode, out color))
            return color;
        return color;
    }
}
