using UnityEngine;

namespace BattleNetwork.Battle.UI
{
    public interface IDraggableUI
    {
        RectTransform GetRectTransform();
        void DragStarted();
        void DragEnded();
        GameObject GetGameObject();
    }
}
