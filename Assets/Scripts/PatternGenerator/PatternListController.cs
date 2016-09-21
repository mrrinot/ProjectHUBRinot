using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;
using System.IO.IsolatedStorage;

public class PatternListController : MonoBehaviour {

    private RectTransform _rectTransform;
    private List<string> _descriptorsList = new List<string>();
    private List<GameObject> _prefabsList = new List<GameObject>();
    private PatternDescriptorData _selected = null;

    public GameObject display;
    public GameObject infosPanel;
    public static string DESCRIPTOR_PATH = "Assets/Resources/PatternDescriptors/";
    public GameObject filePrefab;

    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        LoadPatternDescriptors();
    }

    public void UpdatePattern(string oldName, PatternDescriptorData data)
    {
        if (oldName != null)
            File.Delete(DESCRIPTOR_PATH + oldName + ".xml");
        System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(PatternDescriptorData));
        FileStream fs = new FileStream(DESCRIPTOR_PATH + data.name + ".xml", FileMode.Create);
        writer.Serialize(fs, data);
        fs.Close();
        LoadPatternDescriptors();
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
        Debug.Log("creating new pattern");
        PatternDescriptorData data = new PatternDescriptorData();
        data.name = "default";
        data.rarity = 0;
        data.type = ePatternType.GENERIC;
        for (int i = 0; i < 25; ++i)
            data.patternDesign.Add("Empty");
        UpdatePattern(null, data);
        _selected = data;
        LoadPatternDescriptors();
    }

    public void SelectPattern(GameObject selected)
    {
        _selected = GetDataFromFile(selected.GetComponent<Text>().text);
        display.GetComponent<PatternDisplay>().LoadPattern(_selected);
        infosPanel.GetComponent<PatternInfosManager>().SetInfos(_selected.name, _selected.rarity, _selected.type);
    }
}
