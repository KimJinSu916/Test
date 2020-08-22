using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickControl : MonoBehaviour
{
    [Header ("조이스틱 설정")]
    public float m_fRadius = 50;

    public RectTransform rectBack;
    public RectTransform rectJoystick;
   
    public Vector2 GetJoystickVecValue
    {
        get
        {
            return (rectJoystick.localPosition).normalized;
        }
    }

    void OnTouch(Vector2 vecTouch)
    {
        Vector2 vec = new Vector2(vecTouch.x - rectBack.position.x, vecTouch.y - rectBack.position.y);
        //ClampMagnitude(백터, 최대 크기)
        vec = Vector2.ClampMagnitude(vec, m_fRadius);
        rectJoystick.localPosition = vec;
    }
    public void UI_OnPointerDown(BaseEventData eventData)
    {
        PointerEventData pointerEventData = eventData as PointerEventData;
        OnTouch(pointerEventData.position);
    }
    public void UI_OnPointerUp()
    {
        rectJoystick.localPosition = Vector2.zero;
    }
    public void UI_OnDrag(BaseEventData eventData)
    {
        PointerEventData pointerEventData = eventData as PointerEventData;
        OnTouch(pointerEventData.position);
    }
}
