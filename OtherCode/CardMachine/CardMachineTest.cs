using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardMachineTest : MonoBehaviour
{
    public CardDataConfig cardDataConfig;
    public CardMachine cardMachine;

    void Start()
    {
        cardMachine = new CardMachine(cardDataConfig.cardDatas);
        //cardMachine.filterList.Add(new CardFilter_Noweapons());
        //cardMachine.AmplifierList.Add(new WeightAmplifier_SuperPotato());
    }


    private void Update()
    {

        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            var cards = cardMachine.GetCard_NotRepeatable(1, 0);
            foreach (var v in cards)
            {
                Debug.LogWarning(v.name);
            }
        }
    }
}
