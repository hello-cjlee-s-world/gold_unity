using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    public int nextMove;
    Animator anim;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capsuleCollider2D;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        Invoke("Think", 3);
    }

    void FixedUpdate()
    {   // Moving
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        // 지형 체크(낭떠러지)
        Vector2 frontVec = new Vector2(rigid.position.x + (nextMove*0.3f), rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
        if(rayHit.collider == null){
                Turn();
            }
    }

    // 재귀 함수
    void Think()
    {   
        // Set Next Active
        nextMove = Random.Range(-1, 2);
        float nextThinkTime = Random.Range(2f, 5f);

        // walk Animation
        anim.SetInteger("WalkSpeed", nextMove);

        // filp animation control
        if(nextMove != 0){
            spriteRenderer.flipX = nextMove == 1;
        }

        // Recursive
        Invoke("Think", nextThinkTime);
    }

    void Turn()
    {
        nextMove *= -1;
        CancelInvoke();
        spriteRenderer.flipX = nextMove == 1;
        Invoke("Think", 5);
    }

    // 적 데미지 받았을 경우
    public void Ondamaged(){
        // 색 변환
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //좌우 반전
        spriteRenderer.flipY = true;
        // 바닥 아래로 추락
        capsuleCollider2D.enabled = false;
        // 뛰어오르기
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        // 적 사라지기
        Invoke("DeActive", 5);
    }
    void DeActive(){
        Debug.Log("충돌 발생!");
        gameObject.SetActive(false);
    }
}
