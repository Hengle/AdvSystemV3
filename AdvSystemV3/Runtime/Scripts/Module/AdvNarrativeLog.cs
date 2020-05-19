using UnityEngine;
using Fungus;

[RequireComponent(typeof(AdvLogLayout))]
public class AdvNarrativeLog : MonoBehaviour
{
    [HideInInspector] public AdvLogLayout advLayout;
    [HideInInspector] public RecyclingListView theList;
    [Adv.HelpBox] public string helpInfo = "This is a system Component for AdvLog UI require. \n Child object should have RecyclingListView";

    void Awake()
    {
        advLayout = GetComponent<AdvLogLayout>();
        theList = GetComponentInChildren<RecyclingListView>();
        theList.ChildPrefab = advLayout.LogContentPrefab;
        theList.ItemCallback = PopulateItem;
    }

    protected virtual void OnEnable()
    {
        NarrativeLog.OnNarrativeAdded += OnNarrativeAdded;
    }
            
    protected virtual void OnDisable()
    {
        NarrativeLog.OnNarrativeAdded -= OnNarrativeAdded;
    }

    void OnNarrativeAdded(){
        NarrativeDataExtend temp = AdvManager.Instance.fungusNarrativeLog.GetRawHistory();
        LineExtend lastLine = temp.lines[temp.lines.Count - 1];

        theList.RowCount = temp.lines.Count;
    }

    void PopulateItem(RecyclingListViewItem item, int rowIndex) {
        var newContent = item as AdvLogContentLayout;
        LineExtend lastLine = AdvManager.Instance.fungusNarrativeLog.GetRawHistory().lines[rowIndex];

        newContent.NameTerm = lastLine.nameTerm;
        newContent.TextTerm = lastLine.textTerm;
        newContent.AvatarSprite = lastLine.icon;
        newContent.AvatarVoice = lastLine.voice;
        
        newContent.showIcon = lastLine.showIcon;

        newContent.UpdateUIContent();
    }

    [ContextMenu("Refresh Log UI Content")]
    public void RefreshLogContentUI(SystemLanguage targetLanguage){
        if(theList.ChildItems == null){
            Debug.Log(">> not initialize for Log Content");
            return;
        }

        foreach (AdvLogContentLayout item in theList.ChildItems)
        {
            if(item == null)
                continue;

            item.UpdateUIContent();
        }
    }

    public void ClearNarrativeLog(){
        AdvManager.Instance.fungusNarrativeLog.ClearHistory();
        theList.Clear();
    }
}
