using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnosMods.StartingItemsPicker
{
    public class GUIClickController : Selectable, IPointerClickHandler
    {
        public ClickEvent onClick = new ClickEvent();
        public UnityEvent onLeft = new UnityEvent();
        public UnityEvent onRight = new UnityEvent();
        public UnityEvent onMiddle = new UnityEvent();

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                onLeft.Invoke();
            else if (eventData.button == PointerEventData.InputButton.Right)
                onRight.Invoke();
            else if (eventData.button == PointerEventData.InputButton.Middle)
                onMiddle.Invoke();
            onClick.Invoke(eventData.button);
        }
    }

    public class ClickEvent : UnityEvent<PointerEventData.InputButton> { }
}
