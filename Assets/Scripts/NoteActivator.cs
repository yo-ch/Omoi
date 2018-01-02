using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteActivator : MonoBehaviour {

    public KeyCode key;

    private bool active = false;
    private GameObject note;

    // Use this for initialization
    void Start () {

    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(key) && active) {
            Destroy(note);
        }
    }

    void OnTriggerEnter(Collider col) {
        active = true;
        if (col.gameObject.CompareTag("Note")) {
            note = col.gameObject;
        }
    }

    void OnTriggerExit(Collider col) {
        active = false;
    }
}
