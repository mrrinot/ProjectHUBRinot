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
    public GameObject forbiddenPanel;
    public GameObject patternList;
    public GameObject lockButton;

    private PatternDisplay _display;
    private PatternForbiddenControler _forbidden;
    private PatternListController _list;
    private bool _locked = false;
    private Text _lockButtonText;
    private string _oldName;
    private PatternDescriptorData _data;

    void Start()
    {
        forbiddenPanel.SetActive(_locked);
        List<string> enumNames = new List<string>();
        foreach (string enumName in System.Enum.GetNames(typeof(ePatternType)))
            enumNames.Add(enumName);
        typeDrop.AddOptions(enumNames);
        _lockButtonText = lockButton.GetComponentInChildren<Text>();
        _list = patternList.GetComponent<PatternListController>();
        _forbidden = forbiddenPanel.GetComponent<PatternForbiddenControler>();
        _display = display.GetComponent<PatternDisplay>();
    }

    public void OnUpdate()
    {
        PatternDescriptorData data = new PatternDescriptorData();

        data.name = nameInput.text;
        data.rarity = System.Convert.ToInt32(rarityInput.text);
        data.type = (ePatternType)System.Enum.GetValues(typeof(ePatternType)).GetValue(typeDrop.value);
        data.patternDesign = _display.GetPatternList();
        List<List<string>> forbiddens = _forbidden.GetForbiddenList();
        data.upFor = forbiddens[0];
        data.downFor = forbiddens[1];
        data.leftFor = forbiddens[2];
        data.rightFor = forbiddens[3];
        _list.UpdatePattern(_oldName, data);
        SetInfos(data);
        _list.LoadPatternDescriptors();
    }

    public void SetInfos(PatternDescriptorData data)
    {
        _data = data;
        _oldName = data.name;
        nameInput.text = data.name;
        rarityInput.text = System.Convert.ToString(data.rarity);
        typeDrop.value = (byte)data.type;
    }

    public void LockPattern()
    {
        _locked = !_locked;
        _lockButtonText.text = !_locked ? "Lock pattern" : "Unlock pattern";
        forbiddenPanel.SetActive(_locked);
        _forbidden.LoadForbiddenLists(_data, _locked);
    }

    public string GetSelectedName()
    {
        return _oldName;
    }

    public bool isLocked()
    {
        return _locked;
    }
}
