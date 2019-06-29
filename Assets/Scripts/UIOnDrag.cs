using UnityEngine;
using UnityEngine.EventSystems;

public class UIOnDrag : MonoBehaviour, IDragHandler {

    void IDragHandler.OnDrag(PointerEventData eventData) {
        MainUI.OnDrag(eventData);
    }
}
