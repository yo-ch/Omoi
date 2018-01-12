using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LaneController : MonoBehaviour {

    private Material mat;
    private Color laneColor;
    private Color pressedColor;
    public KeyCode key;
    public Text comboText;

    private List<GameObject> notes;
    private AudioSource audioSource;

    // Use this for initialization
    void Start () {
        mat = GetComponent<Renderer>().material;
        laneColor = mat.color;
        pressedColor = new Color(0.3f, 0.3f, 0.4f, 0.5f);
        notes = new List<GameObject>();
        comboText.text = "0";
        audioSource = GameObject.Find("Song").GetComponent<AudioSource>();
    }

    void OnGUI () {
        Event e = Event.current;

        if (e.isKey && e.keyCode == key && Input.GetKeyDown(key))
        {
            if (notes.Count > 0)
            {
                List<float> hitDifferences = new List<float>(notes.Select(n => Math.Abs(n.GetComponent<Note>().hitTime - audioSource.time)));

                if (hitDifferences.Min() <= 0.225) {
                    int closestNote = hitDifferences.IndexOf(hitDifferences.Min());

                    GameObject note = (GameObject) notes[closestNote];
                    Debug.Log((audioSource.time) + "@" + note.GetComponent<Note>().hitTime);
                    notes.RemoveAt(closestNote);
                    comboText.text = (Int32.Parse(comboText.text)+1).ToString();
                    Destroy(note);
                }
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

        foreach (var note in notes.Where(note =>
                                         note.GetComponent<Note>().hitTime - audioSource.time < -0.225))
        {
            Destroy((GameObject) note);
            comboText.text = "0";
        }
        notes.RemoveAll(note => note.GetComponent<Note>().hitTime - audioSource.time < -0.225);

    }

    public void AddNote (GameObject note)
    {
        notes.Add(note);
    }
}
