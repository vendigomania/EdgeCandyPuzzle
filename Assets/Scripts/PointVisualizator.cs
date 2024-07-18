using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PointVisualizator : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private RectTransform root;
    [SerializeField] private RectTransform dirTransform;
    [SerializeField] private Image icon;
    [SerializeField] private Sprite[] sprites;

    public Vector2 Position => root.anchoredPosition;
    public static List<PointVisualizator> selected = new List<PointVisualizator>();
    public static System.Action OnSequenceFinish;
    public static int CurrentNumber;

    private int number = 1;
    public int Number
    {
        get => number;
        set
        {
            number = value;
            icon.sprite = sprites[number - 1];
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"Start Drag {gameObject.name}");
        if (!selected.Contains(this))
        {
            selected.Add(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
#if UNITY_EDITOR
        if (!Input.GetMouseButton(0)) return;
#else
        if (Input.touchCount == 0) return;
#endif

        Debug.Log($"Drag on {gameObject.name}");
        if(selected.Contains(this))
        {
            if(selected[selected.Count - 1] != this)
            {
                int index = selected.IndexOf(this);
                selected.RemoveRange(index, selected.Count - index);
            }
        }
        else if(selected.Count != 0 && number == selected[selected.Count - 1].number)
        {
            selected.Add(this);
        }

        foreach (var item in selected) item.SetDirection();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log($"Result selected by lenth {selected.Count}");

        if (selected.Count > 2) OnSequenceFinish?.Invoke();
        else
        {
            selected.ForEach((vis) => vis.SetDirection(false));
            selected.Clear();
        }
    }

    public void SetDirection(bool forceActive = true)
    {
        Debug.Log($"Set direction on {gameObject.transform.position}");

        if (forceActive && selected.Contains(this) && selected[selected.Count - 1] != this)
        {
            dirTransform.gameObject.SetActive(true);

            int index = selected.IndexOf(this);

            Vector2 dir = selected[index + 1].Position - Position;

            dirTransform.rotation = Quaternion.Euler(new Vector3(0, 0, Vector2.SignedAngle(Vector2.right, dir)));
        }
        else dirTransform.gameObject.SetActive(false);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"Result selected by lenth {selected.Count}");

        OnSequenceFinish?.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        
    }
}
