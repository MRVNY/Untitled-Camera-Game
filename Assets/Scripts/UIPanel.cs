using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanel : MonoBehaviour
{
    // Start is called before the first frame update
    protected void Start()
    {
        GetComponent<RectTransform>().localPosition = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
