using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Movement : MonoBehaviour
{

    [SerializeField] private float Movement_Speed;
    private Rigidbody2D PlayerRb;
    private float Dir;
    [SerializeField] private float Jump_Power;

    [SerializeField] private LayerMask LayerMask;
    private Collider2D Col;

    [Header("Bash")]
    [SerializeField] private float Raduis;
    [SerializeField] GameObject BashAbleObj;
    private bool NearToBashAbleObj;
    private bool IsChosingDir;
    private bool IsBashing;
    [SerializeField] private float BashPower;
    [SerializeField] private float BashTime;
    [SerializeField] private GameObject Arrow;
    Vector3 BashDir;
    private float BashTimeReset;
    



     // Start is called before the first frame update
    void Start()
    {

        BashTimeReset = BashTime;
        PlayerRb = GetComponent<Rigidbody2D>();
        Col = GetComponent<BoxCollider2D>();
        


    }

    // Update is called once per frame
    void Update()
    {
        Dir = Input.GetAxis("Horizontal") * Movement_Speed;
        if (Dir > 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        Jump();
        Bash();
        
        
        
    }

    void FixedUpdate()
    {
        
            if(IsBashing == false)
            PlayerRb.velocity = new Vector2(Dir * Time.deltaTime, PlayerRb.velocity.y);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded())
        {
            PlayerRb.AddForce(transform.up * Jump_Power);
        }
    }
    bool isGrounded()
    {
        float ExtraHight = 0.03f;
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(Col.bounds.center, Col.bounds.size, 0f, Vector2.down, ExtraHight, LayerMask);
        return raycastHit2D.collider != null;
    }

    /////////////////////////////////----BASH

    void Bash()
    {
        RaycastHit2D[] Rays = Physics2D.CircleCastAll(transform.position, Raduis,Vector3.forward);
        foreach(RaycastHit2D ray in Rays)
        {

            NearToBashAbleObj = false;

            if(ray.collider.tag =="BashAble")
            {
                NearToBashAbleObj = true;
                BashAbleObj = ray.collider.transform.gameObject;
                break;
            }
        }
        if(NearToBashAbleObj)
        {
            BashAbleObj.GetComponent<SpriteRenderer>().color = Color.yellow;
            if(Input.GetKeyDown(KeyCode.Mouse1))
            {
                Time.timeScale = 0;
                BashAbleObj.transform.localScale = new Vector2(1.4f, 1.4f);
                Arrow.SetActive(true);
                Arrow.transform.position = BashAbleObj.transform.transform.position;
                IsChosingDir = true;
            }
            else if(IsChosingDir && Input.GetKeyUp(KeyCode.Mouse1))
            {
                Time.timeScale = 1f;
                BashAbleObj.transform.localScale = new Vector2(1, 1);
                IsChosingDir = false;
                IsBashing = true;
                PlayerRb.velocity = Vector2.zero;
                transform.position = BashAbleObj.transform.position;
                BashDir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                BashDir.z = 0;
                if(BashDir.x >0 )
                {
                    transform.eulerAngles = new Vector3(0, 0, 0);
                }
                else
                {
                    transform.eulerAngles = new Vector3(0, 180, 0);
                }
                BashDir = BashDir.normalized;
                BashAbleObj.GetComponent<Rigidbody2D>().AddForce(-BashDir * 50, ForceMode2D.Impulse);
                Arrow.SetActive(false);

            }
        }
        else if (BashAbleObj != null)
        {
            BashAbleObj.GetComponent<SpriteRenderer>().color = Color.white;
        }

        ////// Preform the bash
        ///
        if(IsBashing)
        {
            if(BashTime > 0 )
            {
                BashTime -= Time.deltaTime;
                PlayerRb.velocity = BashDir * BashPower * Time.deltaTime;
            }
            else
            {
                IsBashing = false;
                BashTime = BashTimeReset;
                PlayerRb.velocity = new Vector2(PlayerRb.velocity.x, 0);


            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, Raduis);
    }

   

}
    


     
   
   

