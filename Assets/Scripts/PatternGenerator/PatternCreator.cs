using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PatternCreator : MonoBehaviour {

    public GameObject patternList;
    public void OnClick()
    {
        patternList.GetComponent<PatternListController>().CreateNewPattern();
    }
}
