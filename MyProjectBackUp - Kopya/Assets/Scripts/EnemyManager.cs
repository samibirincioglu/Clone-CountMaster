using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
public class EnemyManager : MonoBehaviour
{
    //temeller
    [SerializeField] private TextMeshPro characterCounter_Txt;
    [SerializeField] private GameObject characterPrefab;
    [Range(0, 201)][SerializeField] private int minEnemyNumber, maxEnemyNumber;

    //dizilis
    [Range(0f, 1f)][SerializeField] private float distance, radius;

    //oyuncunun karakter grubu
    private Transform enemy;
    private bool attackStarted;
    private void Start()
    {
        for (int i = 0; i < Random.Range(minEnemyNumber, maxEnemyNumber); i++)
        {
            Instantiate(characterPrefab, transform.position, new Quaternion(0f, 100f, 0f, 1f), transform);
        }
        characterCounter_Txt.text = (transform.childCount - 1).ToString();

        Format();
    }
    private void Update()
    {
        if (attackStarted && transform.childCount > 1)
        {
            //karakterlerin dusmana dogru yonelecegi açýyý hesapliyorum
            var enemyDirection = enemy.position - transform.position;

            //karakterleeri yoneltiyorum
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).transform.rotation = Quaternion.Slerp(transform.GetChild(i).rotation, Quaternion.LookRotation(enemyDirection, Vector3.up), Time.deltaTime * 3f);
                if (enemy.childCount>1)
                {
                    var distanceBeetweenGroups = enemy.GetChild(1).position - transform.GetChild(i).position;
                        if (distanceBeetweenGroups.magnitude < 6f)
                        {
                            //siradaki karakteri dusman grubun ustune gonderiyorum
                            transform.GetChild(i).position = Vector3.Lerp(transform.GetChild(i).position,
                                enemy.GetChild(1).position,
                                Time.deltaTime * 2f);
                        }
                }
            }
        }
    }
    private void Format()
    {
        //her karakter icin  
        for (int i = 0; i < transform.childCount; i++)
        {
            //karakterlerin cember halinda toplanmasi icin gerekli hesaplamalar
            var x = distance * Mathf.Sqrt(i) * Mathf.Cos(i * radius);
            var z = distance * Mathf.Sqrt(i) * Mathf.Sin(i * radius);

            //buradaki y degeri karakterin worldspacedeki y degeri olacak
            var pos = new Vector3(x, 0.076f, z);

            //dagilma animasyonu
            transform.GetChild(i).localPosition = pos;
        }
    }

    public void AttackStarted(Transform enemyGroup)
    {
        enemy = enemyGroup;
        attackStarted = true;
    }
    public void StopAttack()
    {
        attackStarted = false;

        for (int i = 0; i < transform.childCount; i++)
        {

        }
        PlayerManager.instance.gameStarted = false;
    }
}
