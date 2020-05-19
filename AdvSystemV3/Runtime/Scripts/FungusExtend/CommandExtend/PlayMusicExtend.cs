using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("Audio",
                 "Play Music (Extend)",
                 "Plays looping game music. If any game music is already playing, it is stopped. Game music will continue playing across scene loads.")]
    [AddComponentMenu("")]
    public class PlayMusicExtend : PlayMusic , ICommand
    {
        [SerializeField] protected int csvLine;
        [SerializeField] protected string csvCommandKey;
        public int CSVLine { get { return csvLine; } set { csvLine = value; }}
        public string CSVCommandKey { get { return csvCommandKey; } set { csvCommandKey = value; }}

        [SerializeField] protected bool RestartWhenTheBGMIsSame = false;

        public override void OnEnter()
        {
            var musicManager = FungusManager.Instance.MusicManager;

            float startTime = Mathf.Max(0, atTime);
            musicManager.PlayMusic(musicClip, loop, fadeDuration, startTime, RestartWhenTheBGMIsSame);
                
            Continue();
        }
        
        public void InitializeByParams(object[] param){
            CommandParam data = param[0] as CommandParam;
            
            AdvKeyContent ADVKeys = AdvKeyContent.GetCurrentInstance();
            if(ADVKeys != null){
                if(!string.IsNullOrEmpty(data.image)){
                    musicClip = ADVKeys.GetBGMByKey(data.image);
                }
            }
        }
    }
}

