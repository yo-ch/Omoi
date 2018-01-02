using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneController : MonoBehaviour {

    private Material mat;
    private Color laneColor;
    private Color pressedColor;
    public string key;

    // Use this for initialization
    void Start () {
        mat = GetComponent<Renderer>().material;
        laneColor = mat.color;
        pressedColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    }

    // Update is called once per frame
    void Update ()
    {
        if (Input.GetKey(key)) {
            mat.color = pressedColor;
        } else {
            mat.color = laneColor;
        }
    }
}
