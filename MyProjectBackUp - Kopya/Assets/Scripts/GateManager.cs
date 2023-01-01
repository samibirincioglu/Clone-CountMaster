using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GateManager : MonoBehaviour
{
    //multipler, multiplierX ve multiplierY ile kapinin carpan olup olmadigi ve istenilen deger araligi editorden belirlenir
    [SerializeField] public bool multiplier;
    [SerializeField] private int multiplier_1 = 0, multiplier_2 = 0;

    //increment degiskenleri ile editor ustunden kapinin kac karakter ekleyecegi belirlenir
    [SerializeField] private int increment_1 = 0, increment_2 = 0;
    [SerializeField] private TextMeshPro gateText;

    [HideInInspector] public int generatedNumber;
    private void Start()
    {
        //X kapisi ise
        if (multiplier)
        {
            int randomInt = Random.Range(multiplier_1, multiplier_2);
            gateText.text = "X" + randomInt;

            generatedNumber = randomInt;
        }
        //normal kapi ise
        else
        {
            int randomInt = Random.Range(increment_1, increment_2);

            //sayilarin cift olmasi gerekiyor
            while (randomInt % 10 != 0)
            {
               randomInt = Random.Range(increment_1, increment_2);
            }
            gateText.text = randomInt.ToString();

            generatedNumber = randomInt;
        }
    }
}
