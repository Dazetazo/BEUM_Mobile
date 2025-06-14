using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform background;
    public RectTransform handle;

    private Vector2 inputVector;

    // español: Este script implementa un joystick virtual que se puede arrastrar en la pantalla.
    // english: This script implements a virtual joystick that can be dragged on the screen.
    public void OnDrag(PointerEventData eventData)
    {
        // español: Convierte la posición del puntero en coordenadas locales del fondo del joystick.
        // english: Converts the pointer position to local coordinates of the joystick background.
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(background, eventData.position, eventData.pressEventCamera, out pos);
        pos = Vector2.ClampMagnitude(pos, background.sizeDelta.x * 0.5f);
        handle.anchoredPosition = pos;
        // español: Calcula el vector de entrada normalizado basado en la posición del joystick. Ajusta sensibilidad.
        inputVector = pos / (background.sizeDelta.x * 0.5f);
    }

    public void OnPointerDown(PointerEventData eventData) => OnDrag(eventData);
    public void OnPointerUp(PointerEventData eventData)
    {
        // español: Resetea la posición del joystick y el vector de entrada cuando se suelta.
        // english: Resets the joystick position and input vector when released.
        inputVector = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }
    // español: Devuelve el vector de entrada normalizado del joystick.
    // english: Returns the normalized input vector of the joystick.
    public float Horizontal => inputVector.x;
    public float Vertical => inputVector.y;
    public Vector2 Direction => new Vector2(Horizontal, Vertical);
}
