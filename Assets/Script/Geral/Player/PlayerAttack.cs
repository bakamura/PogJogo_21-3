using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {

    [HideInInspector] public static PlayerAttack instance;

    public SkillTreeCanvas treeCanvasScript;

    [Header("Atk General")]
    public bool canInput = true;
    [System.NonSerialized] public bool isAtking = false;
    [System.NonSerialized] public int currentAtk = 0;
    [HideInInspector] public float atkRemember;
    public float totalAtkRemember;

    public float[] atkTotalCDown = new float[3];
    [System.NonSerialized] public float[] atkCDown = new float[3];

    private void Awake() {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    private void Update() {
        Inputs();
    }

    private void Inputs() {
        if (canInput && !PlayerData.animPlayer.GetBool("Consuming")) {
            if (Input.GetButtonDown("Fire1")) {
                currentAtk = 1;
                atkRemember = totalAtkRemember;
                if (atkCDown[0] > totalAtkRemember) AudioManager.instance.Play("SkillCD");
            }
            if (Input.GetButtonDown("Fire2")) {
                currentAtk = 2;
                atkRemember = totalAtkRemember;
                if (atkCDown[1] > totalAtkRemember) AudioManager.instance.Play("SkillCD");
            }
            if (Input.GetKeyDown(KeyCode.E)) {
                currentAtk = 3;
                atkRemember = totalAtkRemember;
                if (atkCDown[2] > totalAtkRemember) AudioManager.instance.Play("SkillCD");
            }
            if (Input.GetKeyDown(KeyCode.F)) {  //Consume
                currentAtk = 4;
                atkRemember = totalAtkRemember;
            }
        }
    }

    private void FixedUpdate() {
        Attack();
        CoolDown();
    }

    private void Attack() {
        if (atkRemember > 0) {
            atkRemember -= Time.fixedDeltaTime;
            if (!isAtking) {
                switch (currentAtk) {
                    case 0:
                        atkRemember = 0;
                        break;
                    case 1:
                        if (atkCDown[0] <= 0) PlayerAtkList.instance.CastSkill(0);
                        break;
                    case 2:
                        if (atkCDown[1] <= 0) PlayerAtkList.instance.CastSkill(1);
                        break;
                    case 3:
                        if (atkCDown[2] <= 0) PlayerAtkList.instance.CastSkill(2);
                        break;
                    case 4:
                        if (!PlayerData.animPlayer.GetBool("Consuming")) CheckConsume();
                        break;
                }
            }
        }
    }

    private void CoolDown() {
        if (atkCDown[0] > 0) atkCDown[0] -= Time.fixedDeltaTime;
        else if (atkCDown[0] < 0) atkCDown[0] = 0;
        if (atkCDown[1] > 0) atkCDown[1] -= Time.fixedDeltaTime;
        else if (atkCDown[1] < 0) atkCDown[1] = 0;
        if (atkCDown[2] > 0) atkCDown[2] -= Time.fixedDeltaTime;
        else if (atkCDown[2] < 0) atkCDown[2] = 0;
    }

    private void CheckConsume() {
        RaycastHit2D[] corpses = Physics2D.CircleCastAll(transform.position, 0.45f, Vector2.zero);
        GameObject nearest = null;
        float minDist = 1;
        foreach (RaycastHit2D corpse in corpses) if (Vector2.Distance(transform.position, corpse.transform.position) < minDist && corpse.collider.GetComponent<EnemyBase>() != null) {
                if (corpse.collider.GetComponent<EnemyBase>().currentHealth <= 0) {
                    nearest = corpse.collider.gameObject;
                    minDist = Vector2.Distance(transform.position, corpse.transform.position);
                }
            }
        if (nearest != null) {
            PlayerData.animPlayer.SetBool("Consuming", true);
            GetComponent<SpriteRendererUpdater>().enabled = false;
            PlayerData.srPlayer.sortingOrder += 2;
            StartCoroutine(Consume(nearest.GetComponent<EnemyBase>()));
        }
    }

    private IEnumerator Consume(EnemyBase consumed) {
        PlayerData.animPlayer.SetBool("Consuming", true);
        PlayerMovement.instance.moveLock = true;
        PlayerData.rbPlayer.velocity = Vector2.zero;

        yield return new WaitForSeconds(1.25f);

        GetXP(consumed.xpType, consumed.xpAmount);
        PlayerData.instance.takeHealing(consumed.maxHealth / 5);

        PlayerData.animPlayer.SetBool("Consuming", false);
        GetComponent<SpriteRendererUpdater>().enabled = true;
        PlayerMovement.instance.moveLock = false;

        Destroy(consumed.gameObject);
    }

    private void GetXP(int type, float amount) {
        PlayerData.instance.leveling[type].xp += amount;

        FloatingText go = Instantiate(PlayerData.instance.floatingText, transform.position + new Vector3(Random.Range(-0.45f, 0.45f), Random.Range(-0.35f, 0.35f), 0), Quaternion.identity).GetComponent<FloatingText>();
        go.transform.SetParent(PlayerData.instance.textParent);
        go.color = type;
        go.type = 0;
        go.transform.localScale *= PlayerData.instance.leveling[type].lv == CheckLvUp(PlayerData.instance.leveling[type].xp)? 0.75f : 1.5f;
        go.text = PlayerData.instance.leveling[type].lv == CheckLvUp(PlayerData.instance.leveling[type].xp) ? amount.ToString("F1") + "xp" : "Lv Up!";
        if (PlayerData.instance.leveling[type].lv != CheckLvUp(PlayerData.instance.leveling[type].xp)) AudioManager.instance.Play("LevelUp");

        PlayerData.instance.leveling[type].lv = CheckLvUp(PlayerData.instance.leveling[type].xp);
    }


    private int CheckLvUp(float totalAmount) {
        return (int) (Mathf.Pow((totalAmount / 3), 1f / 3f));
    }

}
