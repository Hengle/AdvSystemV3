using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Fungus
{
    public class WriterExtend : Writer
    {
        [Header("Extend Variable")]
        [SerializeField] protected bool isAutoWrite = false;
        [SerializeField] protected float autoWriteWaitNext = 2.5f;
        
        protected IEnumerator DoWordsRunning;

        public bool IsAutoWrite
        {
            get { return isAutoWrite; }
            set
            {
                isAutoWrite = value;
                if (isAutoWrite == false)
                    AdvSignals.DoAdvStopAutoWrite();
            }
        }

        public bool SwitchAutoWrite() => isAutoWrite = !isAutoWrite;

        protected override void Start()
        {
            AdvUserSettingManager.Instance.OnNextTextShowTimeChange += OnNextTextShowTimeChange;
            AdvUserSettingManager.Instance.OnEveryLineEndWaitTimeChange += OnEveryLineEndWaitTimeChange;
            OnNextTextShowTimeChange(AdvUserSettingManager.Instance.NextTextShowTime);
            OnEveryLineEndWaitTimeChange(AdvUserSettingManager.Instance.EveryLineEndWaitTime);

            base.Start();
        }

        void OnDestroy()
        {
            AdvUserSettingManager.Instance.OnNextTextShowTimeChange -= OnNextTextShowTimeChange;
            AdvUserSettingManager.Instance.OnEveryLineEndWaitTimeChange -= OnEveryLineEndWaitTimeChange;
        }

        void OnNextTextShowTimeChange(float toValue)
        {
            writingSpeed = ConvertToWrittingSpeed(toValue);
            Debug.Log("Writting Speed to: " + writingSpeed);
        }

        void OnEveryLineEndWaitTimeChange(float toValue)
        {
            autoWriteWaitNext = ConvertToWaitNextTextTime(toValue);
            Debug.Log("Line Speed to: " + autoWriteWaitNext);
        }

        /// <summary>
        //Convert 0~1 to fungus Writing Speed
        /// </summary>
        public static float ConvertToWrittingSpeed(float src)
        {
            //speed = base + param
            //but unlimit = 0
            if (src == 1)
                return 0;

            // Range will be 0.1 second (1/10) ~ 0.016 second (1/60)
            return 10 + src * 50;
        }

        /// <summary>
        //Convert 0~1 to adv Wait Next Text Time
        /// </summary>
        public static float ConvertToWaitNextTextTime(float src)
        {
            // Range will be 0 ~ 5
            return 5 - (src * 5);
        }


        # region Override

        protected override IEnumerator DoWords(List<string> paramList, TokenType previousTokenType)
        {
            DoWordsRunning = base.DoWords(paramList, previousTokenType);
            yield return DoWordsRunning;
        }
        public void StopDoWord()
        {
            if (DoWordsRunning == null)
                return;

            int MoveLoop = 0;
            while (DoWordsRunning.MoveNext() && MoveLoop < 256)
                MoveLoop++;
        }


        /// <summary>
        /// 當一句話顯示完成時，等待使用者輸入
        /// </summary>
        protected override IEnumerator DoWaitForInput(bool clear)
        {
            NotifyPause();

            inputFlag = false;
            isWaitingForInput = true;

            float remainTime = autoWriteWaitNext + 0.01f;
            while (!inputFlag && !exitFlag && (remainTime > 0))
            {
                if (isAutoWrite)
                    remainTime -= Time.deltaTime;

                yield return null;
            }

            isWaitingForInput = false;
            inputFlag = false;

            if (clear)
            {
                textAdapter.Text = "";
            }

            NotifyResume();
        }

        #endregion



        #region backup code

        // protected virtual Sprite FindEmoji(string emojiName)
        // {
        //     return advCharacter.GetPortrait(emojiName);
        // }

        //Sora.add
        // protected override IEnumerator ProcessTokens(List<TextTagToken> tokens, bool stopAudio, Action onComplete)
        //     case TokenType.Emoji: 
        //     {
        //         Sprite emoji = null;
        //         if(CheckParamCount(token.paramList, 1))
        //         {
        //             emoji = FindEmoji(token.paramList[0]);
        //         }
        //         if(emoji != null)
        //         {
        //             //Sora.TODO , 可能發生 Writer 不在 SayDialog的情況 ?
        //             SayDialog dialog = GetComponent<SayDialog>();
        //             if(dialog != null){
        //                 dialog.SetCharacterImage(emoji);
        //             }
        //         }
        //     }
        //     break;

        #endregion

    }
}