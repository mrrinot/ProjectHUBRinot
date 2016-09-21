using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PatternCreator : MonoBehaviour {

    public GameObject patternList;

    void Start()
    {

    }

    public void OnClick()
    {
        patternList.GetComponent<PatternListController>().CreateNewPattern();
    }
}
