using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Card : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    
    public Image Art;
    public Image CardBackground;
    public Texture2D CardBackgroundBorder;
    [Space]
    public Text Mana;
    public Text Hp;
    public Text Attack;

    public int ManaValue { get; set; }
    public int AttackValue { get; set; }
    public int HpValue { get; set; }

    public event Action<Card> Dropped;

    private Vector3 _beginPosition;
    private GraphicRaycaster _graphicRaycaster;
    private float _moveDuration = 1.0f;
    private Sprite _default;
    private Sprite _drag;

    void Start()
    {
        _default = CardBackground.sprite;
        _drag = Sprite.Create(CardBackgroundBorder, new Rect(0, 0, CardBackgroundBorder.width, CardBackgroundBorder.height), new Vector2(0, 0));
        var canvas = transform.parent.GetComponentInParent<Canvas>();
        _graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
        StartCoroutine(GetText());

    }

    IEnumerator GetText()
    {
        var url = $"https://picsum.photos/{Art.rectTransform.rect.width}/{Art.rectTransform.rect.height}";
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                var texture = DownloadHandlerTexture.GetContent(uwr);
                Art.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var results = new List<RaycastResult>();
        _graphicRaycaster.Raycast(eventData, results);
        DropPanel dropPanel = null;
        foreach (var result in results)
        {
            dropPanel = result.gameObject.GetComponent<DropPanel>();
            if (dropPanel != null) break;
        }

        CardBackground.sprite = _default;
        if (dropPanel == null)
        {
            transform.DOMove(_beginPosition, _moveDuration);
            return;
        }
        transform.SetAsFirstSibling();
        Dropped(this);

    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.localPosition += (Vector3)eventData.delta;        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _beginPosition = transform.position;
        CardBackground.sprite = _drag;
    }
}
