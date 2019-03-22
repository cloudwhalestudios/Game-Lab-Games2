using UnityEngine;
using UnityEngine.EventSystems;

// TODO add point input
public class PointerInputController : IPointerDownHandler, IPointerClickHandler, IDragHandler //, InputController
{
    public float swipeThreshold = 20f;
    public float moveThreshold = 80f;

    Vector2 lastMoveCallback;
    bool isMoved;

    public void OnPointerDown(PointerEventData eventData)
    {
        isMoved = false;
        lastMoveCallback = eventData.position;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isMoved)
            return;

        float xDelta = eventData.position.x - eventData.pressPosition.x;
        /*if (xDelta > swipeThreshold)
            OnSecondary();
        else if (xDelta < -swipeThreshold)
            OnPrimary();*/
    }

    public void OnDrag(PointerEventData eventData)
    {
        /*
        if (eventData.position.x - lastMoveCallback.x >= moveThreshold)
            OnSecondary();
        else if (eventData.position.x - lastMoveCallback.x <= -moveThreshold)
            OnPrimary();
        else
            return;
            */
        lastMoveCallback = eventData.position;
        isMoved = true;
    }
}