using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class AdvSignals
{
    public static event AdvStartHandler OnAdvStart;
    public delegate void AdvStartHandler(Flowchart src);
    public static void DoAdvStart(Flowchart src) { if (OnAdvStart != null) OnAdvStart(src); }

    public static event AdvStoppingHandler OnAdvStopping;
    public delegate void AdvStoppingHandler();
    public static void DoAdvStopping() { if (OnAdvStopping != null) OnAdvStopping(); }

    public static event AdvStoppedHandler OnAdvStopped;
    public delegate void AdvStoppedHandler();
    public static void DoAdvStopped() { if (OnAdvStopped != null) OnAdvStopped(); }

    public static event AdvStopAutoWriteHandler AdvStopAutoWrite;
    public delegate void AdvStopAutoWriteHandler();
    public static void DoAdvStopAutoWrite(){ if(AdvStopAutoWrite != null) AdvStopAutoWrite(); }

    public static event AdvStopAutoSkipHandler AdvStopAutoSkip;
    public delegate void AdvStopAutoSkipHandler();
    public static void DoAdvStopAutoSkip(){ if(AdvStopAutoSkip != null) AdvStopAutoSkip(); }

    public static AdvCheckFlowchartEndHandler AdvCheckFlowchartEnd;
    public delegate void AdvCheckFlowchartEndHandler();
    public static void DoAdvCheckFlowchartEnd(){ if(AdvCheckFlowchartEnd != null) AdvCheckFlowchartEnd(); }
}
