using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class BattleScript : MonoBehaviourPun
{
    public Spinner spinnerScript;
    public Image spinSpeedBar_Image;
    public TextMeshProUGUI spinSpeedRatio_Text;

    public GameObject uI_3d_Gameobject;
    public GameObject deathPanelUIPrefab;
    private GameObject deathPanelUIGameobject;

    public float commonDamageCoefficient = 0.04f;

    public bool isAttacker;
    public bool isDefender;

    private bool isDead = false;

    [Header("Player Type Damage Coefficients")]
    public float doDamage_Coefficient_Attacker = 10f; // do more damage than defender - ADVANTAGE
    public float getDamage_Coefficient_Attacker = 1.2f; // gets more damage - DISADVANTAGE

    public float doDamage_Coefficient_Defender = 0.75f; // do less damage = DISADVANTAGE
    public float getDamage_Coefficient_Defender = 0.2f; // gets less damaage = ADVANTAGE

    private float startSpinSpeed;
    private float currentSpinSpeed;

    private Rigidbody rb;

    private void Awake()
    {
        startSpinSpeed = spinnerScript.spinSpeed;
        currentSpinSpeed = spinnerScript.spinSpeed;

        spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Comparing the speeds of the SPinnerTops
            float mySpeed = gameObject.GetComponent<Rigidbody>().velocity.magnitude;
            float otherPlayerSpeed = collision.collider.gameObject.GetComponent<Rigidbody>().velocity.magnitude;

            if (mySpeed > otherPlayerSpeed)
            {
                float defaultDamageAmount = gameObject.GetComponent<Rigidbody>().velocity.magnitude * 3600f * commonDamageCoefficient;
                if (isAttacker)
                {
                    defaultDamageAmount *= doDamage_Coefficient_Attacker;
                }
                else if (isDefender)
                {
                    defaultDamageAmount *= doDamage_Coefficient_Defender;
                }
                if (collision.collider.gameObject.GetComponent<PhotonView>().IsMine)
                {
                    // Apply Damage to the slower player
                    collision.collider.gameObject.GetComponent<PhotonView>().RPC("DoDamage", RpcTarget.AllBuffered, defaultDamageAmount);
                }
            }
        }
    }
    
    [PunRPC]
    public void DoDamage(float _damageAmount)
    {
        if (!isDead)
        {
            if (isAttacker)
            {
                _damageAmount *= getDamage_Coefficient_Attacker;
                if (_damageAmount > 1000)
                    _damageAmount = 400f;
            }
            else if (isDefender)
            {
                _damageAmount *= getDamage_Coefficient_Defender;
            }

            spinnerScript.spinSpeed -= _damageAmount;
            currentSpinSpeed = spinnerScript.spinSpeed;
            spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;
            spinSpeedRatio_Text.text = currentSpinSpeed.ToString("F0") + "/" + startSpinSpeed;

            if (currentSpinSpeed < 100)
            {
                // Die
                Die();
            }
        }
    }

    void Die()
    {
        isDead = true;

        GetComponent<MovementController>().enabled = false;
        rb.freezeRotation = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        spinnerScript.spinSpeed = 0f;
        uI_3d_Gameobject.SetActive(false);

        if (photonView.IsMine)
        {
            // countdown for respawn
            StartCoroutine(ReSpawn());
        }
    }

    
    

    IEnumerator ReSpawn()
    {
        GameObject canvasGameobject = GameObject.Find("Canvas");
        if (deathPanelUIGameobject == null)
        {
            deathPanelUIGameobject = Instantiate(deathPanelUIPrefab, canvasGameobject.transform);
        }
        else
        {
            deathPanelUIGameobject.SetActive(true);
        }

        Text respawnTimeText = deathPanelUIGameobject.transform.Find("RespawnTimeText").GetComponent<Text>();

        float respawnTime = 8.0f;

        respawnTimeText.text = respawnTime.ToString(".00");

        while (respawnTime > 0.0f)
        {
            yield return new WaitForSeconds(1.0f);
            respawnTime -= 1;
            respawnTimeText.text = respawnTime.ToString(".00");

            GetComponent<MovementController>().enabled = false;
        }

        deathPanelUIGameobject.SetActive(false);
        GetComponent<MovementController>().enabled = true;

        photonView.RPC("ReBorn", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void ReBorn()
    {
        spinnerScript.spinSpeed = startSpinSpeed;
        currentSpinSpeed = spinnerScript.spinSpeed;

        spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;
        spinSpeedRatio_Text.text = currentSpinSpeed.ToString("F0") + "/" + startSpinSpeed;

        rb.freezeRotation = true;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        uI_3d_Gameobject.SetActive(true);

        isDead = false;
    }

    private void CheckPlayerType()
    {
        if (gameObject.name.Contains("Attacker"))
        {
            isAttacker = true;
            isDefender = false;
        }
        else if (gameObject.name.Contains("Defender"))
        {
            isAttacker = false;
            isDefender = true;

            spinnerScript.spinSpeed = 4400;

            startSpinSpeed = spinnerScript.spinSpeed;
            currentSpinSpeed = spinnerScript.spinSpeed;

            spinSpeedRatio_Text.text = currentSpinSpeed + "/" + startSpinSpeed;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        CheckPlayerType();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
