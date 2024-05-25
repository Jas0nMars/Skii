using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Flag : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshPro lvNumTxt;
    void Start()
    {
        UpdateLevelFlag();
    }

    public void UpdateLevelFlag()
    {
        lvNumTxt.text = GameManager.Instance.CurLevel + "";       
    }
}
