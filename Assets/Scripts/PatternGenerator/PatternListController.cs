using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.IO;
using System.Text;
using System.IO.IsolatedStorage;

public class PatternListController : MonoBehaviour {

    private RectTransform _rectTransform;
    private List<string> _descriptorsList = new List<string>();
    private List<GameObject> _prefabsList = new List<GameObject>();
    private PatternDescriptorData _selected = null;
    private PatternInfosManager _infos;
    private PatternDisplay _display;
    private PatternForbiddenControler _forbidden;

    public GameObject display;
    public GameObject forbidden;
    public GameObject infosPanel;
    public static string DESCRIPTOR_PATH = "Assets/Resources/PatternDescriptors/";
    public GameObject filePrefab;

    void Start()
    {
        _infos = infosPanel.GetComponent<PatternInfosManager>();
        _display = display.GetComponent<PatternDisplay>();
        _forbidden = forbidden.GetComponent<PatternForbiddenControler>();
        _rectTransform = GetComponent<RectTransform>();
        LoadPatternDescriptors();
        SelectPattern(_prefabsList[0]);
    }

    void updateOldName(string oldName, string newName)
    {
        foreach (string name in _descriptorsList)
        {
            PatternDescriptorData data = GetDataFromFile(name);
            bool changed = false;
            if (data.upFor.FirstOrDefault(x => x == oldName) != null)
            {
                data.upFor = data.upFor.Select<string, string>(s => s == oldName ? newName : s).ToList();
                changed = true;
            }
            if (data.downFor.FirstOrDefault(x => x == oldName) != null)
            {
                data.downFor = data.downFor.Select<string, string>(s => s == oldName ? newName : s).ToList();
                changed = true;
            }
            if (data.leftFor.FirstOrDefault(x => x == oldName) != null)
            {
                data.leftFor = data.leftFor.Select<string, string>(s => s == oldName ? newName : s).ToList();
                changed = true;
            }
            if (data.rightFor.FirstOrDefault(x => x == oldName) != null)
            {
                data.rightFor = data.rightFor.Select<string, string>(s => s == oldName ? newName : s).ToList();
                changed = true;
            }
            if (changed)
                UpdatePattern(null, data);
        }
        LoadPatternDescriptors();
    }

    public void DeletePattern()
    {
        File.Delete(DESCRIPTOR_PATH + _selected.name + ".xml");
        LoadPatternDescriptors();
        if (_prefabsList.Count > 0)
            SelectPattern(_prefabsList[0]);
    }

    public void UpdatePattern(string oldName, PatternDescriptorData data)
    {
        if (oldName != null)
        {
            updateOldName(oldName, data.name);
            File.Delete(DESCRIPTOR_PATH + oldName + ".xml");
        }
        Debug.Log("UPDATE " + data.name);
        System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(PatternDescriptorData));
        FileStream fs = new FileStream(DESCRIPTOR_PATH + data.name + ".xml", FileMode.Create);
        writer.Serialize(fs, data);
        fs.Close();
    }

    public PatternDescriptorData GetDataFromFile(string name)
    {
        PatternDescriptorData data = new PatternDescriptorData();
        System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(PatternDescriptorData));
        FileStream fs = new FileStream(DESCRIPTOR_PATH + name + ".xml", FileMode.Open);
        try
        {
            data = reader.Deserialize(fs) as PatternDescriptorData;
        }
        catch
        {
            Debug.LogError("File " + DESCRIPTOR_PATH + name + ".xml" + " could not open !");
            fs.Close();
            return null;
        }
        fs.Close();
        return data;
    }

    public void LoadPatternDescriptors()
    {
        foreach (GameObject obj in _prefabsList)
            GameObject.Destroy(obj);
        _prefabsList.Clear();
        _descriptorsList.Clear();
        string[] fileEntries = Directory.GetFiles(DESCRIPTOR_PATH);
        foreach (string file in fileEntries)
        {
            if (!file.Contains(".meta"))
            {
                _descriptorsList.Add(file.Substring(DESCRIPTOR_PATH.Length, file.LastIndexOf(".") - DESCRIPTOR_PATH.Length));
            }
        }
        foreach (string file in _descriptorsList)
        {
            GameObject fileObj = Instantiate(filePrefab, Vector3.zero, Quaternion.identity) as GameObject;
            _prefabsList.Add(fileObj);
            fileObj.transform.parent = _rectTransform;
            fileObj.transform.localScale = new Vector3(1, 1, 1);
            fileObj.GetComponent<Text>().text = file;
        }
    }

    public void CreateNewPattern()
    {
        PatternDescriptorData data = new PatternDescriptorData();
        data.name = "default";
        data.rarity = 0;
        data.type = ePatternType.GENERIC;
        for (int i = 0; i < 25; ++i)
            data.patternDesign.Add("Empty");
        UpdatePattern(null, data);
        LoadPatternDescriptors();
        SelectPattern(data);
    }

    public void SelectPattern(PatternDescriptorData data)
    {
        if (!_infos.isLocked())
        {
            _selected = data;
            _display.LoadPattern(_selected);
            _infos.SetInfos(_selected);
        }
        else
        {
            _forbidden.LoadForbiddenDisplay(GetDataFromFile(data.name));
        }
    }

    public void SelectPattern(GameObject selected)
    {
        if (!_infos.isLocked())
        {
            _selected = GetDataFromFile(selected.GetComponent<Text>().text);
            _display.LoadPattern(_selected);
            _infos.SetInfos(_selected);
        }
        else
        {
            _forbidden.LoadForbiddenDisplay(GetDataFromFile(selected.GetComponent<Text>().text));
        }
    }
}
