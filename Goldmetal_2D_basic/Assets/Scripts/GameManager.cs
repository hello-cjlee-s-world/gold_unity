using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public PlayerMove player;
    public GameObject[] Stages;
    public Image[] UIhealth;
    public Text UIPoint;
    public Text UIStage;
    public GameObject UIRestartBtn;

    void Update(){
        UIPoint.text = (totalPoint + stagePoint).ToString();
    }

    public void NextStage()
    {   
        // 스테이지 변경
        if(stageIndex < Stages.Length-1){
            Stages[stageIndex].SetActive(false);
            stageIndex ++;
            Stages[stageIndex].SetActive(true);
            PlayerReposition();
            UIStage.text = "STAGE "+ (stageIndex+1);
        } else { // 게임 클리어
            // 플레이어 사용 중지
            Time.timeScale = 0;
            // 결과 UI
            // 재시작 버튼
            Text btnText = UIRestartBtn.GetComponentInChildren<Text>();
            btnText.text = "Game Clear!";
            ViewBtn();
        }
        // 점수 계산
        totalPoint += stagePoint;
        stagePoint = 0;
    }

    public void HealthDown(){
        if(health > 1){
            health--;
            UIhealth[health].color = new Color(1, 0, 0, 0.4f);
        } else {
            UIhealth[0].color = new Color(1, 0, 0, 0.4f);
            player.onDie();
            // 재시작 버튼
            Invoke("ViewBtn", 3);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 떨어졌을 경우 플레이어 체력 감소
        if (collision.gameObject.tag == "Player"){
             // 플래이어 원위치
             if(health > 1){
                PlayerReposition();
            }
            HealthDown();
        }
    }
    void PlayerReposition(){
        player.transform.position = new Vector3(0, 0, -1);
        player.VelocityZero();
    }
    void ViewBtn(){
        UIRestartBtn.SetActive(true);
    }
    public void Restart(){
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
