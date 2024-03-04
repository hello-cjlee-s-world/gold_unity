using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float maxSpeed;
    public float jumpPower;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    public GameManager gameManager;

    BoxCollider2D boxCollider2D;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    void Update()
    {   
        // Jump!
        if(Input.GetButtonDown("Jump") && !anim.GetBool("isJumping")){
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
        }

        // Set Stop Speed
        if(Input.GetButtonUp("Horizontal")){
            rigid.velocity = new Vector2(rigid.velocity.normalized.x*0.5f, rigid.velocity.y);
        }

        // Drirection Sprite
        if(Input.GetButton("Horizontal")){
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        // Walking Animation
        if(Mathf.Abs(rigid.velocity.x) < 0.3){
            anim.SetBool("isWalking", false);
        }
        else {
            anim.SetBool("isWalking", true);
        }
    }

    void FixedUpdate()
    {   // Move Speed
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        //Max Speed
        if(rigid.velocity.x > maxSpeed){ // Right Max Speed
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        } 
        else if(rigid.velocity.x < maxSpeed*(-1)){ // Left Max Speed
            rigid.velocity = new Vector2(maxSpeed*(-1), rigid.velocity.y);
        }

        // Landing Platform
        if(rigid.velocity.y < 0){
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            if(rayHit.collider != null){
                if(rayHit.distance < 0.5f){
                    anim.SetBool("isJumping", false);
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy"){
            // 머리 위에서 밟을 경우
            if(rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y){
                OnAttack(collision.transform);
            } else { // 플레이어가 데미지를 받을 경우
                OnDamaged(collision.transform.position);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Item"){
            // Point
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");
            if(isBronze) {
                gameManager.stagePoint+=50;
            } else if(isSilver){
                gameManager.stagePoint+=100;
            } else if(isGold){
                gameManager.stagePoint+=300;
            }
            // 아이템 사라지기
            collision.gameObject.SetActive(false);
        } else if(collision.gameObject.tag == "Finish"){
            // 다음 스테이지
            gameManager.NextStage();
        }
    }

    void OnAttack(Transform enemy)
    {
        // Point
        gameManager.stagePoint+=100;
        // Enemy Die
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        rigid.AddForce(Vector2.up * 11, ForceMode2D.Impulse);
        enemyMove.Ondamaged();
    }

    // 맞았을떄
    void OnDamaged(Vector2 targetPos)
    {
        // 체력 감소
        gameManager.HealthDown();
        // 레이어 변경
        gameObject.layer = 9;
        // 색깔 변경
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        // 밀려나기
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1)*9, ForceMode2D.Impulse);
        // Animation
        anim.SetTrigger("doDamaged");
        // 무적 풀기
        Invoke("OffDamaged", 3);     
    }

    void OffDamaged(){
        // 레이어 변경
        gameObject.layer = 8;

        // 색깔 변경
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void onDie(){
        // 색 변환
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //좌우 반전
        spriteRenderer.flipY = true;
        // 바닥 아래로 추락
        boxCollider2D.enabled = false;
        // 뛰어오르기
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
    }

    public void VelocityZero(){
        rigid.velocity = Vector2.zero;
    }
}
