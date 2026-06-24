using System;
using System.Collections.Generic;

[Serializable]
public class InputEvent
{
    public int lane;       // 0=Left 1=Up 2=Down 3=Right 4=Light 5=Medium 6=Heavy 7=DriveParry
    public int noteType;   // Directional lanes: always 0 — Attack lanes: 0=Punch 1=Kick
    public int startFrame;
    public int endFrame;
}

[Serializable]
public class ComboData
{
    public string comboName;
    public int fps = 60;
    public List<InputEvent> events = new List<InputEvent>();
}
