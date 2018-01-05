using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;

public class NoteSpawner : MonoBehaviour {

    public string filePath;

    public SongData songData;
    public float speed;
    public float offset;

    public GameObject note;

    private NoteData noteData;
    private bool isInit = true;
    private float barTime;
    private float barExecutedTime = 0;
    private float arrowSpeed;
    private int barCount;
    private float distance = 188;
    private float songTimer = 0;
    private AudioSource audioSource;

    private LaneController[] lanes;

    // Use this for initialization
    void Start () {
        Debug.Log("init");
        songData = ParseFile();
        barCount = 0;
        barTime = 60.0f / songData.bpm * 4.0f;
        arrowSpeed = speed;
        audioSource = GameObject.Find("Song").GetComponent<AudioSource>();
        noteData = songData.chart;
        isInit = true;

        lanes = new LaneController[4];
        for (int i = 0; i < 4; i++)
        {
            lanes[i] = GameObject.Find("Lane " + (i + 1)).GetComponent<LaneController>();
        }
    }

    // Update is called once per frame
    void Update () {
        if (isInit && barCount < noteData.bars.Count)
        {
            float timeOffset = distance / arrowSpeed;
            songTimer = audioSource.time;

            if (songTimer - timeOffset - songData.offset + offset >= (barExecutedTime - barTime))
            {
                StartCoroutine(PlaceBar(noteData.bars[barCount], barCount++));
                barExecutedTime += barTime;
            }
        }
    }

    public struct SongData
    {
        public bool valid;

        public string title;
        public string subtitle;
        public string artist;

        // public string bannerPath;
        // public string backgroundPath;
        public string musicPath;

        //The offset that the song starts at compared to the step info
        public float offset;

        //The start and length of the sample that is played when selecting a song
        public float sampleStart;
        public float sampleLength;

        //The bpm the song is played at
        public float bpm;

        public NoteData chart;
    }

    public struct NoteData
    {
        public List<List<Notes> > bars;
    }

    public struct Notes
    {
        public bool left;
        public bool right;
        public bool up;
        public bool down;
    }

    private SongData ParseFile()
    {
        if (String.IsNullOrEmpty(filePath) || filePath.Trim().Length == 0)
        {
            SongData temp = new SongData();
            temp.valid = false;
            return temp;
        }

        bool parsingNotes = false;

        SongData songData = new SongData();
        songData.valid = true;

        List<string> fileData = File.ReadAllLines(filePath).ToList();
        string fileDir = Path.GetDirectoryName(filePath);
        if (!fileDir.EndsWith("\\"))
        {
            fileDir += "\\";
        }

        for (int i = 0; i < fileData.Count; i++)
        {
            string line = fileData[i].Trim();

            if (line.StartsWith("//")) //comment
            {
                continue;
            }
            else if (line.StartsWith("#"))           //metadata (title, bpm, etc)
            {
                string key = line.Substring(1, line.IndexOf(':') - 1);

                switch (key.ToUpper())
                {
                case "TITLE":
                    songData.title = line.Substring(line.IndexOf(':') + 1).Trim(';');
                    break;
                case "SUBTITLE":
                    songData.subtitle = line.Substring(line.IndexOf(':') + 1).Trim(';');
                    break;
                case "ARTIST":
                    songData.artist = line.Substring(line.IndexOf(':') + 1).Trim(';');
                    break;
                case "MUSIC":
                    songData.musicPath = fileDir + line.Substring(line.IndexOf(':') + 1).Trim(';');
                    if (!File.Exists(songData.musicPath))
                    {
                        songData.musicPath = null;
                        songData.valid = false;
                    }
                    break;
                case "OFFSET":
                    if (!float.TryParse(line.Substring(line.IndexOf(':') + 1).Trim(';'), out songData.offset))
                    {
                        songData.offset = 0.0f;
                    }
                    break;
                case "SAMPLESTART":
                    if (!float.TryParse(line.Substring(line.IndexOf(':') + 1).Trim(';'), out songData.sampleStart))
                    {
                        songData.sampleStart = 0.0f;
                    }
                    break;
                case "SAMPLELENGTH":
                    if (!float.TryParse(line.Substring(line.IndexOf(':') + 1).Trim(';'), out songData.sampleLength))
                    {
                        songData.sampleLength = 30.0f;
                    }
                    break;
                case "BPMS":
                    if (!float.TryParse(line.Substring(line.IndexOf(':') + 1).Trim(';'),
                                        out songData.bpm))
                    {
                        songData.valid = false;
                        songData.bpm = 0.0f;
                    }
                    break;
                case "NOTES":
                    parsingNotes = true;
                    break;
                default:
                    break;
                }
            }
            if (parsingNotes)
            {
                if (line.ToLower().Contains("beginner") ||
                    line.ToLower().Contains("easy") ||
                    line.ToLower().Contains("medium") ||
                    line.ToLower().Contains("hard") ||
                    line.ToLower().Contains("challenge"))
                {
                    List<string> noteChart = new List<string>();
                    for (int j = i + 1; j < fileData.Count; j++)
                    {

                        string noteLine = fileData[j].Trim();
                        if (noteLine.EndsWith(";"))
                        {
                            break;
                        }
                        else
                        {
                            noteChart.Add(noteLine);
                        }
                    }
                    songData.chart = ParseNotes(noteChart);
                    break;
                }
            }
        }

        return songData;
    }

    private NoteData ParseNotes(List<string> notes)
    {
        NoteData noteData = new NoteData();
        noteData.bars = new List<List<Notes> >();

        List<Notes> bar = new List<Notes>();
        for (int i = 0; i < notes.Count; i++)
        {
            string line = notes[i].Trim();
            if (line.StartsWith(";"))
            {
                break;
            }

            if (line.StartsWith(","))
            {
                noteData.bars.Add(bar);
                bar = new List<Notes>();
            }
            else if (line.EndsWith(";"))
            {
                continue;
            }
            else if (line.Length >= 4)
            {
                Notes note = new Notes();
                note.left = false;
                note.down = false;
                note.up = false;
                note.right = false;

                //Add slider functionality later.
                if (line[0] != '0')
                {
                    note.left = true;
                }
                if (line[1] != '0')
                {
                    note.down = true;
                }
                if (line[2] != '0')
                {
                    note.up = true;
                }
                if (line[3] != '0')
                {
                    note.right = true;
                }

                bar.Add(note);
            }
        }

        return noteData;
    }

    IEnumerator PlaceBar(List<Notes> bar, int barCount)
    {
        for (int i = 0; i < bar.Count; i++)
        {
            if (bar[i].left)
            {
                GameObject obj = (GameObject)Instantiate(note, new Vector3(-9.0f, 0.1f, 225.0f), Quaternion.identity);
                Note noteScript = obj.GetComponent<Note>();
                noteScript.speed = arrowSpeed;
                noteScript.hitTime = ((barCount * 4.0f) + (((i + 1.0f) / bar.Count) * 4.0f)) / songData.bpm * 60;
                lanes[0].AddNote(obj);
            }
            if (bar[i].down)
            {
                GameObject obj = (GameObject)Instantiate(note, new Vector3(-3.0f, 0.1f, 225.0f), Quaternion.identity);
                Note noteScript = obj.GetComponent<Note>();
                noteScript.speed = arrowSpeed;
                noteScript.hitTime = ((barCount * 4.0f) + (((i + 1.0f) / bar.Count) * 4.0f)) / songData.bpm * 60;

                lanes[1].AddNote(obj);
            }
            if (bar[i].up)
            {
                GameObject obj = (GameObject)Instantiate(note, new Vector3(3.0f, 0.1f, 225.0f), Quaternion.identity);
                Note noteScript = obj.GetComponent<Note>();
                noteScript.speed = arrowSpeed;
                noteScript.hitTime = ((barCount * 4.0f) + (((i + 1.0f) / bar.Count) * 4.0f)) / songData.bpm * 60;

                lanes[2].AddNote(obj);
            }
            if (bar[i].right)
            {
                GameObject obj = (GameObject)Instantiate(note, new Vector3(9.0f, 0.1f, 225.0f), Quaternion.identity);
                Note noteScript = obj.GetComponent<Note>();
                noteScript.speed = arrowSpeed;
                noteScript.hitTime = ((barCount * 4.0f) + (((i + 1.0f) / bar.Count) * 4.0f)) / songData.bpm * 60;
                lanes[3].AddNote(obj);
            }
            yield return new WaitForSeconds((barTime / bar.Count) - Time.deltaTime);
        }
    }

}
