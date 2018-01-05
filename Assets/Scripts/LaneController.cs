using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LaneController : MonoBehaviour {

    private Material mat;
    private Color laneColor;
    private Color pressedColor;
    public KeyCode key;
    public GameObject hitSprite;

    private Queue<GameObject> notes;
    private AudioSource audioSource;

    // Use this for initialization
    void Start () {
        mat = GetComponent<Renderer>().material;
        laneColor = mat.color;
        pressedColor = new Color(0.3f, 0.3f, 0.4f, 0.5f);
        notes = new Queue<GameObject>();
        audioSource = GameObject.Find("Song").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void OnGUI () {
        Event e = Event.current;

        if (e.isKey && e.keyCode == key && Input.GetKeyDown(key))
        {
            if (notes.Count > 0)
                Debug.Log((audioSource.time) + "@" + notes.Peek().GetComponent<Note>().hitTime);
            if (notes.Count > 0 && Math.Abs(audioSource.time - notes.Peek().GetComponent<Note>().hitTime) < 0.225f)
            {
                GameObject note = notes.Dequeue();
                Debug.Log("destroy");
                Destroy(note);
            }
        }
    }

    void Update ()
    {
        if (Input.GetKey(key))
        {
            mat.color = pressedColor;
        }
        else
        {
            mat.color = laneColor;
        }


        while (notes.Count > 0)
        {
            GameObject note = notes.Peek();
            if (note.transform.position.z < 23)
            {
                Debug.Log("too late");
                Destroy(notes.Dequeue());
            }
            else
            {
                break;
            }
        }

    }

    public void AddNote (GameObject note)
    {
        notes.Enqueue(note);
    }
}
