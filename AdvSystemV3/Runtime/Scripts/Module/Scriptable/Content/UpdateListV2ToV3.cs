using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FungusExt
{
    [CreateAssetMenu(menuName = "Adv Content/Update List V2 To V3")]
    public class UpdateListV2ToV3 : ScriptableObject
    {
        public List<ReplaceGuid> placeGuid;
    }

    [System.Serializable]
    public class ReplaceGuid
    {
        public TextAsset search;
        public TextAsset replace;
    }
}