﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTutorial : MonoBehaviour {

    public CanvasGroup hudCanvas;
    public Animator animHolder;
    private GameObject enemyToHit;
    private float counter = 0;
    public float circleRange;

    private void Start() {
        StartCoroutine(ShowWASD());
    }

    private void Update() {
        if (animHolder.GetInteger("State") <= 1 && (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0 || Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0)) {
            counter += Time.deltaTime;
            if (counter >= 0.75f) animHolder.SetInteger("State", 2);
        }
        else if (animHolder.GetInteger("State") == 2) {
            RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, circleRange, Vector2.zero);
            foreach (RaycastHit2D hit in hits) if (hit.collider.tag == "Enemy") {
                    enemyToHit = hit.collider.gameObject;
                    hudCanvas.alpha = 1;
                    animHolder.SetInteger("State", 3);
                    Time.timeScale = 0;
                    break;
                }
        }
        else if (animHolder.GetInteger("State") == 3) {
            Vector3 v3 = Camera.main.ScreenToWorldPoint(Input.mousePosition) - enemyToHit.transform.position + new Vector3(0, 0, 10);
            if (Input.GetKeyDown(KeyCode.Mouse0) && v3.magnitude < 0.75f) {
                PlayerAtkList.instance.CastSkill(0);
                Time.timeScale = 1;
                animHolder.SetInteger("State", 4);
                PlayerAttack.instance.isAtking = false;
            }
        }
        else if (animHolder.GetInteger("State") == 4 && PlayerData.instance.leveling[1].lv > 0) animHolder.SetInteger("State", 5);
        else if (animHolder.GetInteger("State") == 5 && Input.GetKeyDown(KeyCode.Tab)) animHolder.SetInteger("State", 6);

        else if (animHolder.GetInteger("State") == 10) Destroy(gameObject);
    }

    IEnumerator ShowWASD() {
        PlayerAttack.instance.isAtking = true;
        hudCanvas.alpha = 0;

        yield return new WaitForSeconds(0.55f);

        if (counter < 0.5f) {
            counter = 0;
            animHolder.SetInteger("State", 1);
        }
    }


}
