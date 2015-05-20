using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


class NotificationPanel : MonoBehaviour 
{
    struct Note
    {
        public GameObject obj;
        public double lifetime;

        public Note(GameObject o)
        {
            obj = o;
            lifetime = Time.time + 5.0;
        }
    }

    private List<Note> m_active = new List<Note>();
    private Queue<String> m_overflow = new Queue<string>();
    private int m_maxSize = 5;

    public void AddNote(String msg)
    {
        if (m_active.Count >= m_maxSize)
        {
            m_overflow.Enqueue(msg);
        }
        else
        {
            ActivateNote(msg);
        }
    }

    private void ActivateNote(String msg)
    {
        var prefab = Resources.Load<RectTransform>("note_panel");
        var note = Instantiate<RectTransform>(prefab);
        var txt = note.GetComponentInChildren<Text>();
        txt.text = msg;
        note.SetParent(this.gameObject.transform);
        m_active.Add(new Note(note.gameObject));
    }

    void Start()
    { 
        // clear previous notes (if any)
        for(int i = 0; i < transform.childCount; ++i)
        {
            GameObject.DestroyImmediate(this.transform.GetChild(i).gameObject);
        }
    }

    void Update()
    {
        m_active.RemoveAll(n =>
        {
            if (Time.time > n.lifetime)
            {
                GameObject.Destroy(n.obj);
                return true;
            }
            return false;
        });

        int numToAdd = m_maxSize - m_active.Count;
        while (numToAdd > 0 && m_overflow.Count > 0)
        {
            ActivateNote(m_overflow.Dequeue());
            numToAdd--;
        }
    }
}
