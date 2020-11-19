using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject Card;
    public Transform Parent;

    public int MinNumberCards = 4, MaxNumberCards = 6;
    public int MinValue = -2, MaxValue = 9;
    [Space]
    public float MoveDuration = 1.0f;
    public float TextDuration = 1.0f;
    [Space]
    public float ArcOfCard = 0.11f;
    public float ArcRadius = 20.0f;
    public float ArcShiftY = -23.0f;
    [Space]
    public float CardShift = 1f;

    private List<Card> _cards = new List<Card>();    
    private Vector3 _upShift, _downShift;
    private int _current;

    private void Start()
    {
        _upShift = new Vector3(0, CardShift, 0);
        _downShift = new Vector3(0, -CardShift, 0);
        
        int count = Random.Range(MinNumberCards, MaxNumberCards + 1);
        for(int i = 0; i < count; i++)
        {
            _cards.Add(Instantiate(Card, Parent).GetComponent<Card>());
            _cards[i].Dropped += OnDropped;
        }
        Reposition();
    }

    private void OnDropped(Card card)
    {
        int index = _cards.IndexOf(card);
        HideCard(index);
        if (_current >= _cards.Count) _current = 0;
        Reposition();
    }

    public void OnClick()
    {
        if (_cards.Count <= 0) return;
        var parameter = Random.Range(0, 3);
        var value = Random.Range(MinValue, MaxValue + 1);
        switch (parameter)
        {
            case 0:
                _cards[_current].ManaValue += value;
                _cards[_current].Mana.DOText(_cards[_current].ManaValue.ToString(), TextDuration, scrambleMode: ScrambleMode.Numerals);
                break;
            case 1:
                _cards[_current].AttackValue += value;
                _cards[_current].Attack.DOText(_cards[_current].AttackValue.ToString(), TextDuration, scrambleMode: ScrambleMode.Numerals);
                break;
            case 2:
                _cards[_current].HpValue += value;
                if (_cards[_current].HpValue < 1)
                {
                    HideCard(_current);
                    if (_current >= _cards.Count) _current = 0;
                    Reposition();
                    return;
                }
                else
                {
                    _cards[_current].Hp.DOText(_cards[_current].HpValue.ToString(), TextDuration, scrambleMode: ScrambleMode.Numerals);
                }
                break;
        }
        if (_cards.Count > 1)
        {
            _cards[_current].transform.DOComplete();
            _cards[_current].transform.DOMove(_cards[_current].transform.position + _downShift, MoveDuration);
            _current++;
            if (_current >= _cards.Count) _current = 0;
            _cards[_current].transform.DOComplete();
            _cards[_current].transform.DOMove(_cards[_current].transform.position + _upShift, MoveDuration);
            _cards[_current].transform.SetAsLastSibling();
        }
    }
    private void HideCard(int index)
    {
        _cards[index].gameObject.SetActive(false);
        _cards[index].gameObject.transform.DOKill();
        Destroy(_cards[index].gameObject);
        _cards.RemoveAt(index);
                
    }
    private void Reposition()
    {
        for (int i = 0; i < _cards.Count; i++)
        {
            GetCardPositionAndRotation(i, _cards.Count, out var position, out var rotation);
            if(i == _current)
            {
                _cards[i].transform.DOComplete();
                _cards[i].transform.DOMove(position + _upShift, MoveDuration);
                _cards[i].transform.SetAsLastSibling();
            }
            else
            {
                _cards[i].transform.DOComplete();
                _cards[i].transform.DOMove(position, MoveDuration);
            }
            
            _cards[i].transform.rotation = rotation;
        }
    }

    private void GetCardPositionAndRotation(int index, int count, out Vector3 position, out Quaternion rotation)
    {
        float begin = Mathf.PI * 0.5f - count * 0.5f * ArcOfCard;
        float end = Mathf.PI * 0.5f + count * 0.5f * ArcOfCard;

        var delta = (end - begin) / count;

        float phi = end - delta * index - delta * 0.5f;

        float x = ArcRadius * Mathf.Cos(phi);
        float y = ArcShiftY + ArcRadius * Mathf.Sin(phi);

        position = new Vector3(x, y, 0);
        rotation = Quaternion.AngleAxis(phi / Mathf.PI * 180 - 90, Vector3.forward);
    }
}
