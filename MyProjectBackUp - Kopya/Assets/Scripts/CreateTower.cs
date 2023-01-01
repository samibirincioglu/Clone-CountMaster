using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CreateTower : MonoBehaviour
{
    private int characterAmount;
    [Range(5f, 10f)][SerializeField] private int maxCharPerRow;
    [Range(0f, 2f)][SerializeField] private float xGap;
    [Range(0f, 2f)][SerializeField] private float yGap;
    [Range(0f, 10f)][SerializeField] private float yOffset;

    [SerializeField] private List<int> towerCountList = new List<int>();
    [SerializeField] private List<GameObject> towerList = new List<GameObject>();
    public static CreateTower instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void _CreateTower(int characterNo)
    {
        //karakter sayisi belirlenir
        characterAmount = characterNo;
        //olusturalacak kule icin listeleme islemi yapilir
        FillTowerList();
        //kule insaa edilmeye baslanir
        StartCoroutine(BuildTowerCoroutine());
    }
    void FillTowerList()
    {
            int sum = 0;
            for (int i = 1; i < maxCharPerRow; i++)
            {
                sum += i;
            }
            print(sum);
            int addChar = (characterAmount - sum) % maxCharPerRow;


        for (int i = 1; i <= maxCharPerRow; i++)
        {
            if (characterAmount < i)
            {
                break;
            }
            characterAmount -= i;
            towerCountList.Add(i);
        }
        for (int i = maxCharPerRow; i > 0; i--)
        {
            if (characterAmount >= i)
            {
                characterAmount -= i;
                towerCountList.Add(i);
                i++;
            }
        }
        if (addChar!= 0 )
        {
            PlayerManager.instance.CreateCharacter(transform.childCount+(maxCharPerRow - addChar));
            towerCountList[towerCountList.Count - 1] += (maxCharPerRow - addChar);
        }
    }


    IEnumerator BuildTowerCoroutine()
    {
        var towerId = 0;
        //playeri ortaya getirir
        transform.DOMoveX(0f, 0.1f).SetEase(Ease.Flash);

        yield return new WaitForSecondsRealtime(0.35f);

        foreach (int towerCharCount in towerCountList)
        {
            foreach (GameObject child in towerList)
            {
                //satirlarin dikey pozisyonunu ayarla
                child.transform.DOLocalMove(child.transform.localPosition + new Vector3(0, yGap, 0), 0.15f).SetEase(Ease.OutQuad);
            }

            //her satir icin bir obje olustur
            var tower = new GameObject("Tower_Row_" + towerId);

            tower.transform.parent = transform;
            tower.transform.localPosition = new Vector3(0, 0, 0);

            towerList.Add(tower);

            var towerNewPos = Vector3.zero;
            float tempTowerCharCount = 0;

            for (int i = 1; i < transform.childCount; i++)
            {
                //her karakterin yatay pozisyonunu ayarliyorum
                Transform child = transform.GetChild(i);
                child.transform.parent = tower.transform;
                child.transform.localPosition = new Vector3(tempTowerCharCount * xGap, 0, 0);

                Vector3 childPos = child.transform.position;
                towerNewPos += childPos;
                tempTowerCharCount++;
                i--;
                if (tempTowerCharCount >= towerCharCount)
                {
                    break;
                }
            }

         
            tower.transform.position = new Vector3(-towerNewPos.x / towerCharCount, tower.transform.position.y - yOffset, tower.transform.position.z);
            

            towerId++;
            if (towerId >= towerCountList.Count)
            {
                transform.GetChild(1).gameObject.SetActive(false);
            }
            yield return new WaitForSecondsRealtime(0.15f);
            print(transform.GetChild(1).GetChild(1));
        }
    }

}
