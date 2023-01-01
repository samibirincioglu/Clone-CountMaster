using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Cinemachine;

public class PlayerManager : MonoBehaviour
{
    //temeller
    public static PlayerManager instance; 
    public Transform player;
    [SerializeField] private GameObject characterPrefab;
    [SerializeField] private TextMeshPro characterCounter_Txt;
    private int characterCount;

    //format
    [Range(0f,1f)][SerializeField] private float distance, radius;

    //kontrol
    [SerializeField] private Transform road;
    public bool touchMove, gameStarted;
    private Vector3 mouseStartPos, playerStartPos;
    public float playerSpeed,roadSpeed= 10f;
    private Camera cam;

    //enemy
    private Transform enemy;
    [HideInInspector]public bool attackStarted;

    //bitis cizgisi
    public GameObject finishCam;
    public bool finish, finishCamMove;
    public Transform characterHolder;
    public bool onceFinish;

    //chest
    public bool chest;

    //menu
    public GameObject finishMenu,restartMenu;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    void Start()
    {
        gameStarted = false;

        player = transform;
        cam = Camera.main;
        //Player'in ilk cocugu haric hepsi character olacagindan toplam sayi -1
        characterCount = transform.childCount - 1;
        characterCounter_Txt.text = characterCount.ToString();

    }
    private void Update()
    {
        if (attackStarted)
        {
        //karakterlerin dusmana dogru yonelecegi a��y� hesapliyorum
            var enemyPos = new Vector3(enemy.position.x, transform.position.y, enemy.position.z)- transform.position;

            //karakterleeri yoneltiyorum
            for (int i = 1; i < transform.childCount; i++)
            {
                transform.GetChild(i).rotation = Quaternion.Slerp(transform.GetChild(i).rotation, Quaternion.LookRotation(enemyPos, Vector3.up), Time.deltaTime * 3f);
            }

            if (enemy.GetChild(1).childCount > 1)
            {
                for (int i = 1; i < transform.childCount; i++)
                {
                    //2 grup arasindaki mesafeyi hesapliyorum 
                    var distanceBeetweenGroups = enemy.GetChild(1).GetChild(0).position - transform.GetChild(i).position;

                    if (distanceBeetweenGroups.magnitude < 6f)
                    {
                        //siradaki karakteri dusman grubun ustune gonderiyorum
                        transform.GetChild(i).position = Vector3.Lerp(transform.GetChild(i).position,
                            new Vector3(enemy.GetChild(1).GetChild(0).position.x, transform.GetChild(i).position.y,
                            enemy.GetChild(1).GetChild(0).position.z),
                            Time.deltaTime * 1f);
                    }
                }
            }
            else
            {
                attackStarted = false;
                roadSpeed = 10f;

                for (int i = 1; i < transform.childCount; i++)
                    transform.GetChild(i).rotation = Quaternion.identity;

                characterCount = transform.childCount - 1;
                characterCounter_Txt.text = characterCount.ToString();

                enemy.gameObject.SetActive(false);
                Format();
            }
            if (transform.childCount <2)
            {
                enemy.transform.GetChild(1).GetComponent<EnemyManager>().StopAttack();
                gameObject.SetActive(false);
            }
        }
        else if (finish)
        {

        }
        else
            HorizontalMove();

        if (transform.childCount == 1 )
        {
            gameStarted = false;
            restartMenu.SetActive(true);
        }

        if (finishCamMove && transform.childCount > 1)
        {
            var cinemachineTransposer = finishCam.GetComponent<CinemachineVirtualCamera>()
              .GetCinemachineComponent<CinemachineTransposer>();

            var cinemachineComposer = finishCam.GetComponent<CinemachineVirtualCamera>()
                .GetCinemachineComponent<CinemachineComposer>();

            cinemachineTransposer.m_FollowOffset = new Vector3(24.5f, Mathf.Lerp(cinemachineTransposer.m_FollowOffset.y,
                transform.GetChild(1).position.y + 2f, Time.deltaTime * 1f), -25f);

            cinemachineComposer.m_TrackedObjectOffset = new Vector3(-10f, Mathf.Lerp(cinemachineComposer.m_TrackedObjectOffset.y,
                15f, Time.deltaTime * 1f), 10f);

        }
        //yolu belirtilen hizda geriye dogru getir
        if (gameStarted)
        {
            road.Translate(-1 * road.forward * Time.deltaTime * roadSpeed);
        }

        if (chest)
        {
            gameStarted = false;
            finishMenu.SetActive(true);
        }
    }
    //Player Kontrolleri
    private void HorizontalMove()
    {
        //mouse basiliyorsa ve oyun basladiysa
        if (Input.GetMouseButtonDown(0) && gameStarted)
        {
            touchMove = true;
            //kameradan mouse'in oldugu pozisyona bir ray gonder 
            var plane = new Plane(Vector3.up, 0f);
            var ray = cam.ScreenPointToRay(Input.mousePosition);

            //ray'in degdigi noktayi ve player pozisyonunu tut 
            if (plane.Raycast(ray, out var _distance))
            {
                mouseStartPos = ray.GetPoint(_distance + 1f);
                playerStartPos = transform.position;
            }
        }
        //mouse birakildiysa hareketi durdur
        if (Input.GetMouseButtonUp(0))
        {
            touchMove = false;
        }
        //hareket basladiysa
        if (touchMove)
        {
            //kameradan mouse'in hareket ettigi noktaya bir ray gonder 
            var plane = new Plane(Vector3.up, 0f);
            var ray = cam.ScreenPointToRay(Input.mousePosition);

            if (plane.Raycast(ray, out var distance))
            {
                //bu pozisyonu mousePos ile tut
                var mousePos = ray.GetPoint(distance + 1f);
                //mouse startPosdan bu pozisyonu cikartarak Player'in gidecegi yonu belirle
                var move = mousePos - mouseStartPos;
                var control = playerStartPos + move;

                //playerin yatay hareketini limitle
                if (characterCount > 50)
                    control.x = Mathf.Clamp(control.x, -2f, 2f);
                else
                    control.x = Mathf.Clamp(control.x, -5.5f, 5.5f);


                //controlde tanimlanan yeni pozisyona Player'i hareket ettir
                transform.position = new Vector3(Mathf.Lerp(transform.position.x, control.x, Time.deltaTime * playerSpeed), 
                    transform.position.y,
                    transform.position.z);
            }
        }
 
    }
    public void Format()
    {
        //her karakter icin  
        for (int i = 1; i < player.childCount; i++)
        {
            //karakterlerin cember halinda toplanmasi icin gerekli hesaplamalar
            var x = distance * Mathf.Sqrt(i) * Mathf.Cos(i * radius);
            var z = distance * Mathf.Sqrt(i) * Mathf.Sin(i * radius);

            //buradaki y degeri karakterin worldspacedeki y degeri olacak
            var pos = new Vector3(x, 0.076f, z);

            //dagilma animasyonu
            player.transform.GetChild(i).GetComponent<Animator>().SetBool("run", true);
            player.transform.GetChild(i).DOLocalMove(pos, 1f).SetEase(Ease.OutBack);
        }
    }
    public void FinishFormat()
    {
        //her karakter icin  
        for (int i = 1; i < player.childCount; i++)
        {
            //karakterlerin cember halinda toplanmasi icin gerekli hesaplamalar
            var x = distance * Mathf.Sqrt(i) * Mathf.Cos(i * radius);
            var z = distance * Mathf.Sqrt(i) * Mathf.Sin(i * radius);

            //buradaki y degeri karakterin worldspacedeki y degeri olacak
            var pos = new Vector3(x, 21.91f, z);

            //dagilma animasyonu
            player.transform.GetChild(i).DOLocalMove(pos, 1f).SetEase(Ease.OutBack);
        }
    }
    public void CreateCharacter(int count)
    {
        
        for (int i = characterCount; i < count; i++)
        {
            var character= Instantiate(characterPrefab, transform.position, Quaternion.identity, transform);
        }
        characterCount = transform.childCount - 1;
        characterCounter_Txt.text = characterCount.ToString();

        Format();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gate"))
        {
            //2 eklemeyide almamasi icin ilk collisiondan sonra kapi colliderlarini kapatiyorum
            other.transform.parent.GetChild(0).GetComponent<BoxCollider>().enabled = false;
            other.transform.parent.GetChild(1).GetComponent<BoxCollider>().enabled = false;


            GateManager script = other.GetComponent<GateManager>();
            if (script.multiplier)
                CreateCharacter(characterCount * script.generatedNumber);
            else
                CreateCharacter(characterCount + script.generatedNumber);
        }
        if (other.CompareTag("Enemy"))
        {
            enemy = other.transform;
            attackStarted = true;
            roadSpeed = 2f;

            other.transform.GetChild(1).GetComponent<EnemyManager>().AttackStarted(transform);
        }
        if (other.CompareTag("Finish"))
        {
            finishCam.SetActive(true);
            finish = true;
            print("called");
            CreateTower.instance._CreateTower(transform.childCount - 1);
            transform.GetChild(0).gameObject.SetActive(false);
        }
        if (other.CompareTag("Player_StairFinished")&& !onceFinish)
        {
            StairsFinished();
        }
        if (other.CompareTag("Chest"))
        {
            chest = true;
        }
    }
    public void RefreshCounter()
    {
        characterCount = transform.childCount - 1;
        characterCounter_Txt.text = characterCount.ToString();
    }
    public void StairsFinished()
    {
        onceFinish = true;
        characterHolder = GameObject.FindGameObjectWithTag("ObjectPool").transform.GetChild(2);
        int limit = characterHolder.childCount;
        for (int i = 0; i < limit; i++)
        {
            characterHolder.GetChild(0).GetComponent<Rigidbody>().isKinematic = true;
            characterHolder.GetChild(0).parent = transform;
        }

        FinishFormat();
    }
    public void StartAnimations()
    {
        int length = transform.childCount;
        for (int i = 1; i < length; i++)
        {
            transform.GetChild(i).GetComponent<Animator>().SetBool("run", true);
        }
    }
}
