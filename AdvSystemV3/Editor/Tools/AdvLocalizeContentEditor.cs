using System.Collections;
using System.Collections.Generic;
using Ideafixxxer.CsvParser;
using UnityEngine;
using UnityEditor;
using System;
using System.Threading.Tasks;
using Sirenix.OdinInspector.Editor;

namespace FungusExt
{
    [CustomEditor(typeof(AdvLocalizeContent), true)]
    public class AdvLocalizeContentEditor : OdinEditor
    {
        [MenuItem("AdvLocalize/Update Localize")]
        public static void UpdateLocalize()
        {
            AdvLocalizeContent t = AdvLocalizeContent.Instance;
            DownloadNamesSheetPage(t);
            DownloadCSVToImport(t);
            UnityEditor.EditorUtility.SetDirty(t);
        }
        public override void OnInspectorGUI()
        {

            DrawDefaultInspector();

            GUILayout.Space(20);
            EditorGUILayout.LabelField("Spreadsheet Operate", EditorStyles.boldLabel);

            if (GUILayout.Button(new GUIContent("Download Google Spreadsheet Infos", "Download and fill up sheet's page infos"), GUILayout.Width(250)))
            {
                DownloadSpreadsheetInfos();
            }
            GUILayout.Space(20);
            EditorGUILayout.LabelField("Localize Operate", EditorStyles.boldLabel);
            if (GUILayout.Button(new GUIContent("Download Names Sheet Page", "Download names key data from sheet page"), GUILayout.Width(250)))
            {
                AdvLocalizeContent t = target as AdvLocalizeContent;
                DownloadNamesSheetPage(t);
                UnityEditor.EditorUtility.SetDirty(t);
                serializedObject.ApplyModifiedProperties();

            }
            GUILayout.Space(5);
            if (GUILayout.Button(new GUIContent("Import Google data to Localize Content", "Opens the Flowchart Window Extend"), GUILayout.Width(250)))
            {
                AdvLocalizeContent t = target as AdvLocalizeContent;
                DownloadCSVToImport(t);
                UnityEditor.EditorUtility.SetDirty(t);
            }
        }

        static async void DelayCall(System.Action callback, string name, int delay)
        {
            await Task.Delay(System.TimeSpan.FromSeconds(delay * 2));
            string log = string.Format("<color=green>Start download {0} : {1} </color>", name, delay);
            Debug.Log(log);
            callback();
        }

        void DownloadSpreadsheetInfos()
        {
            AdvLocalizeContent t = target as AdvLocalizeContent;
            foreach (var sheet in t.spreadsheets)
            {
                DownloadManager.GoogleGetPagesInfos((Result) =>
                {
                    CsvParser csvParser = new CsvParser();
                    string[][] csvTable = csvParser.Parse(Result);

                    sheet.infos = new List<PageData>();

                    for (int i = 0; i < csvTable.Length; i++)
                    {
                        sheet.infos.Add(new PageData() { gid = csvTable[i][1], description = csvTable[i][0] });
                    }
                }, t.webServices, sheet.sheet_id);
            }
            UnityEditor.EditorUtility.SetDirty(t);
        }

        static void DownloadNamesSheetPage(AdvLocalizeContent t)
        {
            DownloadManager.GoogleGetCSV((Result) =>
            {
                CsvParser csvParser = new CsvParser();
                string[][] csvTable = csvParser.Parse(Result);

                //搜尋開頭字串正不正確
                int id_key = Array.IndexOf(csvTable[0], "Keys");

                t.ActorNames = new List<LocalizeActorName>();

                for (int i = 1; i < csvTable.Length; i++)
                {
                    LocalizeActorName _actor = new LocalizeActorName() { key = csvTable[i][id_key] };
                    List<LocalizeText> _tags = new List<LocalizeText>();
                    for (int j = 0; j < csvTable[i].Length; j++)
                    {
                        if (j == id_key)
                            continue;

                        _tags.Add(new LocalizeText()
                        {
                            tag = csvTable[0][j],
                            content = csvTable[i][j]
                        });
                    }
                    _actor.names = _tags;
                    t.ActorNames.Add(_actor);
                }
            }, t.webServices, t.namesSheetPage.sheet_id, t.namesSheetPage.page_gid);
        }

        static void DownloadCSVToImport(AdvLocalizeContent t)
        {
            t.LocalizeNarratives = new List<NarrativeSheet>();

            int callOrder = 0;
            foreach (var sheet in t.spreadsheets)
            {
                NarrativeSheet newSheet = new NarrativeSheet();
                newSheet.sheetID = sheet.sheet_id;
                newSheet.pages = new List<NarrativePage>();
                foreach (var info in sheet.infos)
                {
                    DelayCall(() =>
                    {
                        DownloadManager.GoogleGetCSV((Result) =>
                        {
                            CsvParser csvParser = new CsvParser();
                            string[][] csvTable = csvParser.Parse(Result);

                            if (Array.IndexOf(csvTable[0], AdvUtility.TitleKeys) == -1)
                                return;

                            NarrativePage newPage = new NarrativePage();
                            newPage.pageID = info.gid;
                            newPage.cmds = new List<NarrativeCmd>();

                            for (int i = 1; i < csvTable.Length; i++)
                            {
                                int id_cmd = Array.IndexOf(csvTable[0], AdvUtility.TitleCommand);

                                if (csvTable[i][id_cmd] != "" && csvTable[i][id_cmd] != "Selection" && csvTable[i][id_cmd] != "Say")
                                    continue;

                                NarrativeCmd newCmd = new NarrativeCmd();
                                newCmd.cmd = csvTable[i][0];
                                newCmd.localizeTexts = new List<LocalizeText>();

                                for (int j = t.CSVTextContentStartAtColumn; j < csvTable[i].Length; j++)
                                {
                                    LocalizeText newText = new LocalizeText();
                                    newText.tag = csvTable[0][j];
                                    newText.content = csvTable[i][j];

                                    newCmd.localizeTexts.Add(newText);
                                }

                                newPage.cmds.Add(newCmd);
                            }

                            newSheet.pages.Add(newPage);

                        }, t.webServices, sheet.sheet_id, info.gid);
                    }, $"{sheet.description} - {info.description}", callOrder);

                    callOrder++;
                }
                t.LocalizeNarratives.Add(newSheet);
            }
        }
    }
}