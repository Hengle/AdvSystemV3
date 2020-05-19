using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    [Serializable]
    public class LineExtend
    {
        [SerializeField] public string name;
        [SerializeField] public string text;

        [SerializeField] public string nameTerm;
        [SerializeField] public string textTerm;
        [SerializeField] public Sprite icon;
        [SerializeField] public AudioClip voice;
        
        [SerializeField] public bool showIcon;
    }

    [Serializable]
    public class NarrativeDataExtend
    {
        [SerializeField] public List<LineExtend> lines;

        public NarrativeDataExtend() {
            lines = new List<LineExtend>();
        }
    }

    public class NarrativeLogExtend : NarrativeLog
    {
        protected NarrativeDataExtend historyExtend;

        protected override void Awake()
        {
            base.Awake();
            historyExtend = new NarrativeDataExtend();
        }

        //Will be call for data log when say is finished , and it will call AdvNarrativeLog's method by DoNarrativeAdded() for UI display
        protected override void OnWriterState(Writer writer, WriterState writerState)
        {
            if (writerState == WriterState.End)
            {
                var sd = AdvManager.Instance.advSayDialog;
                var from = sd.NameText;
                var line = sd.StoryText;
                
                //Catch on screen variable to save log
                var nameTerm = sd.OnScreenNameTerm;
                var textTerm = sd.OnScreenTextTerm;
                var spt = sd.OnScreenSpeakerSprite;
                var voice = sd.OnScreenSpeakerVoice;
                
                var showIcon = sd.SayIconObject.activeSelf;

                AddLine(from, line, nameTerm, textTerm, spt, voice, showIcon);
            }
        }

        public void AddLine(string name, string text, string nameTerm, string textTerm, Sprite spt, AudioClip clip, bool showIcon)
        {
            LineExtend line = new LineExtend();
            line.name = name;
            line.text = text;

            line.nameTerm = nameTerm;
            line.textTerm = textTerm;
            line.icon = spt;
            line.voice = clip;
            
            line.showIcon = showIcon;

            historyExtend.lines.Add(line);
            DoNarrativeAdded();
        }

        public NarrativeDataExtend GetRawHistory(){
            return historyExtend;
        }

        public void ClearHistory(){
            historyExtend.lines.Clear();
        }
    }
}