using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rod))]
public class PlayerClient : Client
{
    [SerializeField]Rod _rod;
    [SerializeField]PlayerTouchScreen _playerTouchScreen;
    private void Start()
    {
        
    }

}
