using UnityEngine;
using UnityEngine.EventSystems;

public class Row : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private int x;
    [SerializeField] private int y;
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Click! "+x + ", " + y);
        GameManager.Instance.ClickedOnRow(x,y);
    }
}
