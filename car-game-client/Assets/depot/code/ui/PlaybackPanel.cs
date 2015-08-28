using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.depot.code.ui
{
    public class PlaybackPanel : MonoBehaviour
    {
        public Button buttonStart;
        public Button buttonStop;
        public State state;
        public ActionPanel actionPanel;

        public enum State
        {
            Planning,
            Playing
        }

        void Start()
        {
            UpdateVisible();
        }

        private void UpdateVisible()
        {
            if (state == State.Planning)
            {
                buttonStart.gameObject.SetActive(true);
                buttonStop.gameObject.SetActive(false);
            }
            else
            {
                buttonStart.gameObject.SetActive(false);
                buttonStop.gameObject.SetActive(true);
            }
        }

        void Update()
        {
        }

        public void DoExecute()
        {
            if (state == State.Planning)
            {
                state = State.Playing;
                UpdateVisible();
            }
        }

        public void DoStop()
        {
            if (state == State.Playing)
            {
                Reset();
            }
        }

        private void Reset()
        {
            state = State.Planning;
            UpdateVisible();
        }
    }
}
