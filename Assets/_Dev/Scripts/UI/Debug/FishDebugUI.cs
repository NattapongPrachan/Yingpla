using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;

public class FishDebugUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _stateTxt;
    [SerializeField] TextMeshProUGUI _staminaTxt;
    [SerializeField] TextMeshProUGUI _speedTxt;
    [SerializeField] Fish _fish;
    [SerializeField] Vector2 _offsetPosition;
    private void Start()
    {
        return;
        transform.parent = GameObject.Find("Panel").transform;
        _fish.ObserveEveryValueChanged(_ => _.StatsData.State).Subscribe(state =>
        {
            _stateTxt.text = "State : "+state.ToString();
        }).AddTo(this);
    }
    public void SetFish(Fish Fish)
    {
        _fish = Fish;
    }
    private void Update()
    {
        if (_fish == null) return;
        var toScreen = RectTransformUtility.WorldToScreenPoint(Camera.main, _fish.transform.position);
        transform.position = toScreen + _offsetPosition;
        _staminaTxt.text = "Stamina : "+_fish.StatsData.Stamina.ToString();
        _speedTxt.text = "Speed : "+_fish.DiffSpeed.ToString();
    }
}
