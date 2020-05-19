// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;
using System.Reflection;
using System;
using System.Collections.Generic;

namespace Fungus.EditorUtils
{
    [CustomEditor(typeof(InvokeMethodExtend))]
    public class InvokeMethodExtendEditor : InvokeMethodEditor
    {
        static GameObject CacheObject;
        InvokeMethodExtend targetMethod;

        protected virtual void Awake(){
            targetMethod = target as InvokeMethodExtend;

            if (targetMethod == null)
                return;

            if (targetMethod.TargetObject != null)
                return;

            if (CacheObject != null){
                targetMethod.TargetObject = CacheObject;
                return;
            }

            // 快速預選 StaticMethod
            var results = AssetDatabase.FindAssets("StaticMethod");
            foreach (string guid in results)
            {
                GameObject temp = (GameObject)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(GameObject));
                targetMethod.TargetObject = temp;
                break;
            }
        }

        public override void DrawCommandGUI()
        {
            base.DrawCommandGUI();

            targetMethod = target as InvokeMethodExtend;
            
            if (targetMethod == null)
                return;
            
            if (targetMethod.TargetObject != null)
                CacheObject = targetMethod.TargetObject;

        }
    }
}