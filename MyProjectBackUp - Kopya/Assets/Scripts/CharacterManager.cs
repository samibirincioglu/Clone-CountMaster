using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    private bool once;
    private Animator charAnimator;
    private void Start()
    {
        charAnimator = GetComponent<Animator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyCharacter") && other.transform.parent.childCount > 0)
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
        if (other.CompareTag("Ramp"))
        {
            transform.DOJump(transform.position, 4f, 1, 0.7f).SetEase(Ease.Flash).OnComplete(PlayerManager.instance.Format);
        }
        if (other.CompareTag("Stair"))
        {
            transform.parent.parent = null; //  tower_0 instance i icin
            transform.parent = null; // karakter
            GetComponent<Rigidbody>().isKinematic = GetComponent<Collider>().isTrigger = false;
            GetComponent<Rigidbody>().freezeRotation = true;

            if (!PlayerManager.instance.finishCamMove)
                PlayerManager.instance.finishCamMove = true;

            charAnimator.SetBool("run", false);

            if (PlayerManager.instance.player.transform.childCount == 2)
            {
                other.GetComponent<Renderer>().material.DOColor(new Color(0.4f, 0.98f, 0.65f), 0.5f).SetLoops(1000, LoopType.Yoyo)
                    .SetEase(Ease.Flash);
            }

            //karakter merdivene degince merdivenin childi yap ve pozisyonunu sabitle
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            transform.parent = other.transform.parent;
        }
        if (other.CompareTag("StairFinished") && !once)
        {
            once = true;
            Transform objectPool = GameObject.FindGameObjectWithTag("ObjectPool").transform;
            transform.parent.parent = null;

            transform.parent = objectPool.GetChild(2);
        }
        if (other.CompareTag("Chest"))
        {
            charAnimator.SetBool("run", false);
        }
    }
}
