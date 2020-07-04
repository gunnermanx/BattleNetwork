using UnityEditor;
using BattleNetwork.Battle.UI;

[CustomEditor(typeof(ChipDockUI))]
public class ChipDockUIEditor : Editor
{
    
    public void AddChip()
    {
        ChipDockUI chipDock = target as ChipDockUI;
        chipDock.InitializeChipDockUI(3);
    }

    //public void 
}