using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FungusExt
{
    public partial class LocalizeManager
    {
        static LocalizeManager instance;
        public static LocalizeManager Instance {
            get {
                if(instance == null)
                    instance = new LocalizeManager();
                return instance;
            }
        }

        // private static readonly Dictionary<int, string> CSVLanguageMapping = new Dictionary<int, string>
        // {
        //     { (int) SystemLanguage.ChineseTraditional, "Content_zh-tw" },
        //     { (int) SystemLanguage.ChineseSimplified,  "Content_zh-cn"},
        //     { (int) SystemLanguage.English,            "Content_english|Content_en"},
        //     { (int) SystemLanguage.Japanese,           "Content_jp"},
        //     { (int) SystemLanguage.Korean,             "Content_kr"},
        // };

        public Dictionary<int, string> CSVLanguageMapping;
        public Dictionary<string, List<LocalizeText>> actorNames;
        public Dictionary<string, List<LocalizeText>> narrativeData;

        public LocalizeManager(){
            CSVLanguageMapping = new Dictionary<int, string>();
            if(AdvLocalizeContent.Instance.SupportLanguages != null)
                foreach (var item in AdvLocalizeContent.Instance.SupportLanguages)
                {
                    if(!CSVLanguageMapping.ContainsKey((int)item.langCode))
                        CSVLanguageMapping.Add((int)item.langCode, item.langTag);
                    else
                        Debug.Log($"Language list build error by duplicate code : {item.langCode}");
                }

            actorNames = new Dictionary<string, List<LocalizeText>>();
            if(AdvLocalizeContent.Instance.ActorNames != null)
                foreach (var item in AdvLocalizeContent.Instance.ActorNames)
                {
                    if(!actorNames.ContainsKey(item.key))
                        actorNames.Add(item.key, item.names);
                    else
                        Debug.Log($"Language list build error by duplicate code : {item.key}");
                }

            narrativeData = new Dictionary<string, List<LocalizeText>>();
            if(AdvLocalizeContent.Instance.LocalizeNarratives != null)
                foreach (var _sheet in AdvLocalizeContent.Instance.LocalizeNarratives)
                {
                    foreach (var _page in _sheet.pages)
                    {
                        foreach (var _cmd in _page.cmds)
                        {
                            string key = $"{_sheet.sheetID}.{_page.pageID}.{_cmd.cmd}";
                            if(!narrativeData.ContainsKey(key))
                                narrativeData.Add(key, _cmd.localizeTexts);
                            else
                                Debug.Log($"Narrative data build error by duplicate key : {key}");
                        }
                    }
                }
        }

        #region Static Method

        public static SystemLanguage GetExistLanguage()
        {
            //有語言時讀語言
            if (Instance.CSVLanguageMapping.ContainsKey((int)Application.systemLanguage))
            {
                return Application.systemLanguage;
            }
            //中文時讀繁體中文
            if (Application.systemLanguage == SystemLanguage.Chinese)
                return SystemLanguage.ChineseTraditional;

            //其餘英文
            return SystemLanguage.English;
        }

        public static string GetCSVLanguageTag(SystemLanguage systemLanguage)
        {
            if (Instance.CSVLanguageMapping.ContainsKey((int)systemLanguage))
            {
                return Instance.CSVLanguageMapping[(int)systemLanguage];
            }
            Debug.Log($"!! Get a null Language Tag of {systemLanguage} !!");

            return "";
        }

        //string targetText = AdvUtility.GetLocalizeText(localizeText, (SystemLanguage)AdvUserSettingManager.Instance.AdvLanguage);
        /// <summary>
        /// Input "googlePageID.cmdKey" , it will return localized text with current advLanguage setting
        /// </summary>
        public static string GetLocalizeText(string key)
        {
            if(string.IsNullOrEmpty(key)) Debug.LogError("AdvLocalizeManager >> key is null");
            SystemLanguage currentLanguage = (SystemLanguage)AdvUserSettingManager.Instance.AdvLanguage;
            List<LocalizeText> value;
            if (!Instance.narrativeData.TryGetValue(key, out value))
                Debug.LogError($"Cannot find localizeText with key : {key}");

            return GetLocalizeText(value, currentLanguage);
        }

        /// <summary>
        /// Input Name Term , it will return localized name with current advLanguage setting
        /// </summary>
        public static string GetLocalizeName(string nameKey){
            if(string.IsNullOrEmpty(nameKey)) Debug.LogError("AdvLocalizeManager >> key is null");
            SystemLanguage currentLanguage = (SystemLanguage)AdvUserSettingManager.Instance.AdvLanguage;
            List<LocalizeText> value;
            if (!Instance.actorNames.TryGetValue(nameKey, out value))
                Debug.LogError($"Cannot find localizeName with key : {nameKey}");

            return GetLocalizeText(value, currentLanguage);
        }

        public static string GetLocalizeTextByTag(List<LocalizeText> localizeTexts, string languageTag)
        {
            if (localizeTexts == null)
                return "";

            if (string.IsNullOrEmpty(languageTag))
                return "";

            string[] multiTag = languageTag.Split('|');

            foreach (LocalizeText item in localizeTexts)
            {
                foreach (var eachTag in multiTag)
                {
                    if (String.Equals(item.tag, eachTag, StringComparison.OrdinalIgnoreCase))
                        return item.content;
                }
            }
            return "";
        }

        public static string GetLocalizeText(List<LocalizeText> localizeTexts, SystemLanguage language)
        {
            string tag = GetCSVLanguageTag(language);
            string loc = GetLocalizeTextByTag(localizeTexts, tag);

            return loc;
        }

        // public static string GetTargetNameLanguane(LocalizedString localizedString)
        // {
        //     string overrideLang = I2Utils.GetLanguageName((SystemLanguage)AdvUserSettingManager.Instance.AdvLanguage);
        //     string transName = I2.Loc.LocalizationManager.GetTranslation(localizedString.mTerm, true, 0, true, false, null, overrideLang);

        //     if (string.IsNullOrEmpty(transName))
        //         return "";
        //     return transName;
        // }

        public static void SetLanguage(SystemLanguage language){

        }

        #endregion
    }

    [Serializable]
    public class LocalizeActorName
    {
        public string key;
        public List<LocalizeText> names;

        public string GetCurrentContent(SystemLanguage lang)
        {
            return LocalizeManager.GetLocalizeText(names, lang);
        }
    }

    [Serializable]
    public class NarrativeSheet
    {
        public string sheetID;
        public List<NarrativePage> pages;
    }

    [Serializable]
    public class NarrativePage
    {
        public string pageID;
        public List<NarrativeCmd> cmds;
    }

    [Serializable]
    public class NarrativeCmd
    {
        public string cmd;
        public List<LocalizeText> localizeTexts;
    }

    [Serializable]
    public class LocalizeText
    {
        public string tag;
        public string content;
    }


    [Serializable]
    public class SupportLanguages
    {
        public SystemLanguage langCode;
        public string langTag;
    }
}