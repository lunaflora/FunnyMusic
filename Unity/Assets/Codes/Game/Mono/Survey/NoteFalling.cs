using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;


namespace Codes
{
    public class NoteFalling : MonoBehaviour
    {
        public Transform BaseNote;
        public Transform[] NoteLine;
        public int SongBmp = 120;

        public float FallSpeed = 1;
        public float GroundedLine = -10.0f;

        private float secPerBeat;
        private List<Transform> notesList;
        
        private void Awake()
        {
            secPerBeat = 60f / SongBmp;
            notesList = new List<Transform>();
            SpawnNotes().Forget();
        }

        public void Update()
        {
            UpdateAllNotes();
        }


        async UniTask SpawnNotes()
        {
            for (int i = 0; i < 1000; i++)
            {
                await UniTask.Delay(Random.Range(200, 400));
                int index = Random.Range(0, NoteLine.Length);
                Transform spawnNote = Instantiate(BaseNote);
                spawnNote.position = NoteLine[index].position;
                notesList.Add(spawnNote);
            }
        }

        public void UpdateAllNotes()
        {
            foreach (var note in notesList)
            {
                Vector3 originPos = note.position;
                Vector3 purposePos = new Vector3(originPos.x, originPos.y - FallSpeed * Time.smoothDeltaTime);
                note.position = purposePos;
            }

            for (int i = notesList.Count - 1; i >= 0; i--)
            {
                if (notesList[i].position.y < GroundedLine)
                {
                    Destroy(notesList[i].gameObject);
                    notesList.RemoveAt(i);
                }
            }
  

        }
    }
}