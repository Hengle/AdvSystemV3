using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("Flow", 
                 "Wait (Extend)", 
                 "Waits for period of time before executing the next command in the block.")]
    [AddComponentMenu("")]
    public class WaitExtend : Wait , ICommand
    {
        [SerializeField] protected int csvLine;
        [SerializeField] protected string csvCommandKey;
        public int CSVLine { get { return csvLine; } set { csvLine = value; }}
        public string CSVCommandKey { get { return csvCommandKey; } set { csvCommandKey = value; }}
        
        public void InitializeByParams(object[] param)
        {
            CommandParam data = param[0] as CommandParam;

            if (float.TryParse(data.target, out float val)){
                _duration = new FloatData(val);
            }
        }
    }
}

