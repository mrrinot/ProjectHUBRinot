using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PatternForbiddenList : MonoBehaviour {

    private List<string> _forbiddenList = new List<string>();
    private List<GameObject> _patternList = new List<GameObject>();
    public GameObject patternFile;

    public void AddFileName(string name)
    {
        if (_forbiddenList.FirstOrDefault(x => x == name) == null)
        {
            GameObject obj = GameObject.Instantiate(patternFile);
            _patternList.Add(obj);
            _forbiddenList.Add(name);
            obj.transform.parent = this.transform;
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.GetComponent<Text>().text = name;
        }
        else
        {
            foreach (GameObject obj in _patternList)
            {
                if (obj.GetComponent<Text>().text == name)
                {
                    _forbiddenList.Remove(_forbiddenList.FirstOrDefault(x => x == name));
                    _patternList.Remove(obj);
                    GameObject.Destroy(obj);
                    return;
                }
            }
        }
    }

    public void CleanList()
    {
        _forbiddenList.Clear();
        foreach (GameObject obj in _patternList)
        {
            GameObject.Destroy(obj);
        }
        _patternList.Clear();
    }

    public void LoadPatternForbidden(List<string> list)
    {
        foreach (string name in list)
        {
            GameObject obj = GameObject.Instantiate(patternFile);
            _patternList.Add(obj);
            _forbiddenList.Add(name);
            obj.transform.parent = this.transform;
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.GetComponent<Text>().text = name;
        }
    }

    public List<string> GetForbiddenList()
    {
        return _forbiddenList;
    }
}
