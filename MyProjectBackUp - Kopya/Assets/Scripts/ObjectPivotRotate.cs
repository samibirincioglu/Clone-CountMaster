using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ObjectPivotRotate : MonoBehaviour
{
    [SerializeField] Transform pivotPoint;
    private void Start()
    {
        //pivot noktasindan dondurebilmek icin pivot noktasini ayarliyorum
        transform.position = pivotPoint.position;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        //sonsuz donguye sokuyorum
        transform.DOLocalRotate(new Vector3(0, 0, 90f), 0.85f).SetLoops(-1,LoopType.Yoyo);
    }
}
