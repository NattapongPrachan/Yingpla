using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Sirenix.OdinInspector;
using System;
using System.Resources;
using Unity.VisualScripting;
[RequireComponent(typeof(BoxCollider2D))]
public class Fish : MonoBehaviour
{
    BoxCollider2D _boxCollider;
    [SerializeField]Transform _mouseTransform;
    [SerializeField] Bait _bait;
    
    public float Rand;
    public float Power;
    [Header("Stats & Data")]
    [SerializeField] FishStatsConfig FishStatsConfig;
    public FishStatsData StatsData = new FishStatsData();
    [ShowInInspector]public float SpeedTotal { get; private set; }
    [ShowInInspector] public float DiffSpeed { get; private set; }
    [ShowInInspector] public float DiffPower { get; private set; }
    [ShowInInspector] public float DiffPercent { get; private set; }
    [ShowInInspector] float _randomPercentPulling;

    Vector2 _stopPosition;

    //Observable
    IDisposable TimeCountdownPullingObservable;

    IDisposable TimeToStunningObservable;
    private void Start()
    {
        this.ObserveEveryValueChanged(_ => _.FishStatsConfig).Subscribe(fishStatsConfig =>
        {
            
            RefreshFishStats();
        }).AddTo(this);
        AddListener();
    }
    void AddListener()
    {
        this.StatsData.ObserveEveryValueChanged(_ =>_.Stamina).Where(stamina => stamina <= 0).Subscribe(stamina => {
            TimeCountdownPullingObservable?.Dispose();
            StatsData.IsPulling = false;
        }).AddTo(this);
        Observable.Interval(TimeSpan.FromSeconds(StatsData.RoundPullingDuration)).Subscribe(_ =>
        {
            CalculatePulling();
        }).AddTo(this);
    }
    void RefreshFishStats()
    {
        StatsData.Stamina = FishStatsConfig.FishStatsData.Stamina;
        StatsData.ConsumeStamina = FishStatsConfig.FishStatsData.ConsumeStamina;
        StatsData.RegenStamina = FishStatsConfig.FishStatsData.RegenStamina;
        StatsData.IsRandomStamina = FishStatsConfig.FishStatsData.IsRandomStamina;
        StatsData.MinRegenStamina = FishStatsConfig.FishStatsData.MinRegenStamina;
        StatsData.MaxRegenStamina = FishStatsConfig.FishStatsData.MaxRegenStamina;
       

        StatsData.Power = FishStatsConfig.FishStatsData.Power;
        StatsData.CurrentPower = FishStatsConfig.FishStatsData.CurrentPower;

        StatsData.TimeToKnockDown = FishStatsConfig.FishStatsData.TimeToKnockDown;
        StatsData.RegenRate = FishStatsConfig.FishStatsData.RegenRate;
        StatsData.State = FishStatsConfig.FishStatsData.State;
        StatsData.Speed = FishStatsConfig.FishStatsData.Speed;
        StatsData.Sprint = FishStatsConfig.FishStatsData.Sprint;

        StatsData.RoundPullingDuration = FishStatsConfig.FishStatsData.RoundPullingDuration;
        StatsData.PercentPulling = FishStatsConfig.FishStatsData.PercentPulling;
        StatsData.IsPulling = FishStatsConfig.FishStatsData.IsPulling;
        StatsData.IgnorePullingWhenStaminaLower = FishStatsConfig.FishStatsData.IgnorePullingWhenStaminaLower;

        StatsData.PullCountdown = FishStatsConfig.FishStatsData.PullCountdown;
        StatsData.PullCountdownMin = FishStatsConfig.FishStatsData.PullCountdownMin;
        StatsData.PullCountdownMax = FishStatsConfig.FishStatsData.PullCountdownMax;
    }
    public void CatchBait(Bait bait)
    {
        _bait = bait;
    }
    private void Update()
    {
        
        CalculatePower();
        if (_bait != null)
        {   
            switch(StatsData.State)
            {
                case FishState.Flee:
                    Fighting();
                    break;
                case FishState.Stunning:
                    Stunning();
                    break;
            }
        }
    }
    void Stunning()
    {
        DiffSpeed = 0;
        _bait.Rod.DiffSpeed = 0;
    }
    void Fighting()
    {
        DiffPower = StatsData.CurrentPower - _bait.Rod.Config.PullPower;
        DiffPercent = StatsData.CurrentPower / StatsData.Power;
        DiffSpeed = StatsData.Sprint * DiffPercent;
        if (StatsData.IsPulling)
            _bait.Rod.DiffSpeed = _bait.Rod.Config.DragSpeed * (1 - DiffPercent);
        else
            _bait.Rod.DiffSpeed = _bait.Rod.Config.DragSpeed;

        if (DiffPower < 0 || !StatsData.IsPulling || StatsData.State == FishState.Stuned)
        {
            transform.position = _bait.transform.position;
        }
        else
        {
            if (StatsData.IsPulling)
                _bait.transform.position = transform.position;
        }
        // transform.position = _bait.transform.position;
        //_bait.transform.position = transform.position;
        CalculateStamina();
    }
    void CalculatePulling()
    {
        if (StatsData.IsPulling) return;
        if (StatsData.Stamina <= StatsData.IgnorePullingWhenStaminaLower) return;
        _randomPercentPulling = UnityEngine.Random.Range(0, 1f);
        if(_randomPercentPulling > StatsData.PercentPulling)
        {
            
            StatsData.IsPulling = true;
            CreatePullingCountdown();
        }
    }
    void CreatePullingCountdown()
    {
        TimeCountdownPullingObservable?.Dispose();
        var timeRandom = UnityEngine.Random.Range(StatsData.PullCountdownMin, StatsData.PullCountdownMax);
        TimeCountdownPullingObservable = Observable.Timer(TimeSpan.FromSeconds(timeRandom)).Subscribe(_ =>
        {
            StatsData.IsPulling = false;
        }).AddTo(this);
    }
    void CalculatePower()
    {
        StatsData.CurrentPower = StatsData.Power * StatsData.Stamina * 0.01f;
    }
    private void CalculateStamina()
    {
        if(StatsData.IsPulling)
        {
            SpeedTotal = StatsData.Speed + (StatsData.Sprint * StatsData.Stamina);
            StatsData.Stamina -= StatsData.ConsumeStamina;
            if (StatsData.Stamina <= 0) StatsData.Stamina = 0;
        }
        else
        {
            if(StatsData.IsRandomStamina)
            {
                StatsData.RegenStamina = UnityEngine.Random.Range(StatsData.MinRegenStamina,StatsData.MaxRegenStamina);
            }
            StatsData.Stamina += StatsData.RegenStamina;
            if (StatsData.Stamina >= 100) StatsData.Stamina = 100;
        }
        
    }
    public void Shock()
    {
        TimeToStunningObservable?.Dispose();
        StatsData.IsPulling = false;
        StatsData.State = FishState.Stunning;
        _stopPosition = transform.position;
        TimeToStunningObservable = Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ =>
        {
            StatsData.State = FishState.Flee;
            StatsData.IsPulling = true;

        }).AddTo(this);
       
        
    }
    public void TakeDamage(int damage)
    {
        if (StatsData.State == FishState.Stunning) return;
        Shock();
        StatsData.Stamina -= damage;
        StartCoroutine(ResourcesLoaderManager.Instance.GetObject<TextPopup>(PathKey.DamagePopup, textPopup => {
            
            var popup = Instantiate(textPopup,GameObject.Find("Panel").transform);
            var text = popup.GetComponent<TextPopup>();
            text.SetRectPosition(transform.position);
            text.SetText(damage.ToString());
        }));
    }
    public void ReleaseBait()
    {
        GetComponent<RandomMovement>().enabled = true;
        GetComponent<FleeMovement>().enabled = false;
        RefreshFishStats();

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(TagKeys.Bait))
        {
            if(collision.gameObject.TryGetComponent(out Bait bait) && !bait.HasFish)
            {
                bait.GetFish(this);
                GetComponent<FleeMovement>().enabled = true;
                GetComponent<FleeMovement>().RodPosition = bait.BaitStart;
                GetComponent<RandomMovement>().Dispose();
                StatsData.State = FishState.Flee;
                CatchBait(bait);
            }
           
        }
       
    }
}


/*

public class FishFight : MonoBehaviour
{
    public float fishStrength = 8f;       // ความแข็งแรงเริ่มต้นของปลา
    public float maxFishStrength = 10f;   // ความแข็งแรงสูงสุดของปลา
    public float fishFatigue = 0.1f;      // อัตราความเหนื่อยล้าของปลา
    public float fatigueIncreaseRate = 0.02f; // อัตราการเพิ่มความเหนื่อยของปลา
    public float playerPullForce = 6f;    // แรงดึงของผู้เล่น
    public float tension = 0f;            // แรงตึงในสายเบ็ด
    public float maxTension = 10f;        // แรงตึงสูงสุดก่อนสายขาด
    public float pullDifferenceThreshold = 0.5f; // ขีดจำกัดความต่างในการดึง (จะส่งผลเมื่อใด)

    private bool isFishPulling = false;   // ตรวจสอบว่าปลากำลังดึงอยู่หรือไม่
    private float fishPullTimer = 0f;     // ตัวจับเวลาสำหรับการดึงของปลา
    private float fishPullCooldown = 2f;  // ช่วงเวลาระหว่างการดึงของปลา

    void Update()
    {
        if (tension < maxTension)
        {
            HandleFishPulling();
            HandlePlayerPulling();
        }
        else
        {
            Debug.Log("สายขาด! ปลาหนีไปแล้ว!");
        }

        // ลดแรงตึงลงเมื่อไม่มีการดึง
        tension = Mathf.Max(0, tension - 0.5f * Time.deltaTime);
    }

    // การดึงของปลา
    void HandleFishPulling()
    {
        fishPullTimer += Time.deltaTime;

        if (fishPullTimer >= fishPullCooldown)
        {
            isFishPulling = Random.value > 0.5f; // โอกาส 50% ที่ปลาจะดึง
            fishPullTimer = 0f;

            if (isFishPulling)
            {
                float fishEffectiveStrength = fishStrength - (fishFatigue * fatigueIncreaseRate); // ลดความแข็งแรงเนื่องจากความเหนื่อยล้า
                Debug.Log("ปลากำลังดึง! แรงดึงปลา: " + fishEffectiveStrength);

                // คำนวณความต่างของแรงดึงระหว่างผู้เล่นและปลา
                float pullDifference = playerPullForce - fishEffectiveStrength;

                // ถ้าผู้เล่นดึงแรงกว่า
                if (pullDifference > pullDifferenceThreshold)
                {
                    Debug.Log("ผู้เล่นดึงปลามาได้!");
                    tension += Mathf.Abs(pullDifference) * Time.deltaTime; // แรงตึงเพิ่มขึ้นตามความต่าง
                }
                // ถ้าปลาดึงแรงกว่า
                else if (pullDifference < -pullDifferenceThreshold)
                {
                    Debug.Log("ปลาว่ายหนีไป!");
                    tension -= Mathf.Abs(pullDifference) * Time.deltaTime; // แรงตึงลดลงถ้าปลาชนะการดึง
                }
            }

            // เพิ่มความเหนื่อยล้าของปลาเมื่อเวลาผ่านไป
            fishFatigue += fatigueIncreaseRate;
        }
    }

    // การดึงของผู้เล่น
    void HandlePlayerPulling()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            float fishEffectiveStrength = fishStrength - (fishFatigue * fatigueIncreaseRate); // ความแข็งแรงของปลาหลังจากคิดความเหนื่อยล้า
            float pullDifference = playerPullForce - fishEffectiveStrength;

            // ถ้าผู้เล่นดึงแรงกว่าปลา
            if (pullDifference > pullDifferenceThreshold)
            {
                Debug.Log("ผู้เล่นดึงปลาเข้ามาได้! แรงตึงเพิ่มขึ้น");
                tension += Mathf.Abs(pullDifference) * Time.deltaTime; // แรงตึงเพิ่มขึ้นตามแรงดึง
            }
            else
            {
                Debug.Log("ผู้เล่นพยายามดึง แต่ปลายังว่ายหนีไป!");
            }
        }
    }
}

*/