using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatternForbiddenControler : MonoBehaviour
{
    public GameObject upButton;
    public GameObject downButton;
    public GameObject leftButton;
    public GameObject rightButton;
    public GameObject forbiddenDisplay;

    private PatternDisplay _display;
    private string _forbiddenPatternName = null;
    private PatternForbiddenList _up;
    private PatternForbiddenList _down;
    private PatternForbiddenList _left;
    private PatternForbiddenList _right;
    
    void Awake() 
    {
        _display = forbiddenDisplay.GetComponent<PatternDisplay>();
        _up = upButton.GetComponentInChildren<PatternForbiddenList>();
        _down = downButton.GetComponentInChildren<PatternForbiddenList>();
        _left = leftButton.GetComponentInChildren<PatternForbiddenList>();
        _right = rightButton.GetComponentInChildren<PatternForbiddenList>();
    }

    public List<List<string>> GetForbiddenList()
    {
        List<List<string>> forbidden = new List<List<string>>();
        forbidden.Add(_up.GetForbiddenList());
        forbidden.Add(_down.GetForbiddenList());
        forbidden.Add(_left.GetForbiddenList());
        forbidden.Add(_right.GetForbiddenList());
        return forbidden;
    }

    public void LoadForbiddenLists(PatternDescriptorData data, bool locked)
    {
        if (locked)
        {
            _up.LoadPatternForbidden(data.upFor);
            _down.LoadPatternForbidden(data.downFor);
            _left.LoadPatternForbidden(data.leftFor);
            _right.LoadPatternForbidden(data.rightFor);
        }
        else
        {
            _up.CleanList();
            _down.CleanList();
            _left.CleanList();
            _right.CleanList();
        }
     }

    public void LoadForbiddenDisplay(PatternDescriptorData data)
    {
        _display.LoadPattern(data);
        _forbiddenPatternName = data.name;
    }

    public void OnClickButton(GameObject clicked)
    {
        if (_forbiddenPatternName != null)
            clicked.GetComponentInChildren<PatternForbiddenList>().AddFileName(_forbiddenPatternName);
    }
}
