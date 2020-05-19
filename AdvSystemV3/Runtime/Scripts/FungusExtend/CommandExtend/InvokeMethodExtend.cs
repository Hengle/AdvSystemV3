using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("Scripting", 
                 "Invoke Method (Extend)", 
                 "Invokes 的擴充, 可預選名為 StaticMethod 的 GameObject, 或是上一個已選的 GameObject")]
    public class InvokeMethodExtend : InvokeMethod
    {
        // Extend it with setter property
        public new virtual GameObject TargetObject { get { return targetObject; } set { targetObject = value; } }
    }
}