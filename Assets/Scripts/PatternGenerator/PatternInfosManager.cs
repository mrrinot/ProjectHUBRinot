using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class PatternInfosManager : MonoBehaviour
{
    public InputField nameInput;
    public InputField rarityInput;
    public Dropdown typeDrop;
    public GameObject display;
    public GameObject patternList;

    private string _oldName;

    void Start()
    {
        List<string> enumNames = new List<string>();
        foreach (string enumName in System.Enum.GetNames(typeof(ePatternType)))
            enumNames.Add(enumName);
        typeDrop.AddOptions(enumNames);
    }

    public void OnUpdate()
    {
        PatternDescriptorData data = new PatternDescriptorData();

        data.name = nameInput.text;
        data.rarity = System.Convert.ToInt32(rarityInput.text);
        data.type = (ePatternType)System.Enum.GetValues(typeof(ePatternType)).GetValue(typeDrop.value);
        data.patternDesign = display.GetComponent<PatternDisplay>().GetPatternList();
        patternList.GetComponent<PatternListController>().UpdatePattern(_oldName, data);
        SetInfos(data.name, data.rarity, data.type);
    }

    public void SetInfos(string name, int rarity, ePatternType type)
    {
        _oldName = name;
        nameInput.text = name;
        rarityInput.text = System.Convert.ToString(rarity);
        typeDrop.value = (byte)type;
    }
}
