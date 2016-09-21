﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PatternFileEvent : MonoBehaviour {

    private GameObject _parentGrid;

	void Start()
    {
        _parentGrid = this.transform.parent.gameObject;
	}

    public void OnClick()
    {
        Debug.Log("LOAD " + gameObject.GetComponent<Text>().text);
        _parentGrid.GetComponent<PatternListController>().SelectPattern(this.gameObject);
    }
}
