using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Sirenix.OdinInspector;
using System;
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

    [ShowInInspector] float percentPulling;

    //Observable
    IDisposable TimeCountdownPullingObservable;
    private void Start()
    {
        this.ObserveEveryValueChanged(_ => _.FishStatsConfig).Subscribe(fishStatsConfig =>
        {
            
            RefreshFishStats();
        }).AddTo(this);
    }
    void RefreshFishStats()
    {
        StatsData.Stamina = FishStatsConfig.FishStatsData.Stamina;
        StatsData.ConsumeStamina = FishStatsConfig.FishStatsData.ConsumeStamina;
        StatsData.Power = FishStatsConfig.FishStatsData.Power;
        StatsData.TimeToKnockDown = FishStatsConfig.FishStatsData.TimeToKnockDown;
        StatsData.RegenRate = FishStatsConfig.FishStatsData.RegenRate;
        StatsData.State = FishStatsConfig.FishStatsData.State;
        StatsData.Speed = FishStatsConfig.FishStatsData.Speed;
        StatsData.Sprint = FishStatsConfig.FishStatsData.Sprint;

        StatsData.PercentPulling = FishStatsConfig.FishStatsData.PercentPulling;
        StatsData.IsPulling = FishStatsConfig.FishStatsData.IsPulling;

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
        CalculatePulling();
        CalculatePower();
        if (_bait != null)
        {   
            if(StatsData.State == FishState.Flee)
            {

                //DiffSpeed = SpeedTotal-_bait.Rod.Config.PullPower;
                //_bait.Rod.DiffSpeed = DiffSpeed;
                //if(SpeedTotal < _bait.Rod.Config.PullPower)
                //{
                //    transform.position = _bait.transform.position;
                //}
                //else
                //{
                //    _bait.transform.position = transform.position;
                //}
                transform.position = _bait.transform.position;
                CalculateStamina();
            }
        }
    }
    void CalculatePulling()
    {
        if (StatsData.IsPulling) return;
        percentPulling = UnityEngine.Random.Range(0, 1f);
        if(percentPulling > StatsData.PercentPulling)
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

    }
    private void CalculateStamina()
    {
        SpeedTotal = StatsData.Speed + (StatsData.Sprint * StatsData.Stamina);
        StatsData.Stamina -= StatsData.ConsumeStamina;
        if (StatsData.Stamina <= 0) StatsData.Stamina = 0;
    }
    public void ReleaseBait()
    {
        GetComponent<RandomMovement>().Resume();
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