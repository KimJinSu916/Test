using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterControl : MonoBehaviour
{
    [Header("캐릭터 이동 속도")]
    public float speed = 3;

    public InventoryControl script_InventoryControl;
    public SpriteRenderer characterRender;
    public Animator characterAnimator;
    public JoystickControl script_JoystickControl;
    public Image playerHPImage;

    bool characterFlipX = false;
    bool isInvincibility = false;
    public int jumpMaxNum = 2;
    public float jumpPower = 10;
    public float playerMaxHP = 100;
    public float invincibilityTime = 1;
    public int animState = 0;

    int jumpCount = 0;
    Rigidbody2D rb;
    
    float playerHP;
    public float PlayerHP
    {
        set
        {
            playerHP = value;

            if (playerHP > playerMaxHP)
            {
                playerHP = playerMaxHP;
            }
        }
        get
        {
            return playerHP;
        }
    }

    void Start()
    {
        PlayerHP = playerMaxHP;
        characterRender = GetComponent<SpriteRenderer>();
        characterAnimator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //Input Manager 설정 Edit -> ProjectSettings -> Input Manager 여기서 설정 할 수 있음
        //float moveVaule = Input.GetAxis("Horizontal");// 콘솔기기의 조이스틱 (0~1)
        /*float moveVaule = Input.GetAxisRaw("Horizontal");// 입력이 들어오면 1 없으면 0 (0,1) 
        if (moveVaule != 0)
        {
            if (moveVaule > 0)
            {
                characterFlipX = false;
            }
            else
            {
                characterFlipX = true;
            }
            animState = 1;
        }
        else
        {
            animState = 0;
        }
        transform.localPosition += new Vector3(moveVaule * Time.deltaTime * speed, 0, 0);*/
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UI_JumpButton();    
        }
        else if (rb.velocity.y == 0)
        {
            jumpCount = 0;
        }

        move();

        characterAnimator.SetInteger("AnimState", animState);
        characterAnimator.SetInteger("JumpCount", jumpCount);
        characterAnimator.SetBool("IsGround", GroundCheck());
        
        characterRender.flipX = characterFlipX;

        playerHPImage.fillAmount = (PlayerHP / playerMaxHP);

        if (PlayerHP <= 0)
        {
            Destroy(gameObject);
        }
    }

    void move()
    {
        float x = script_JoystickControl.GetJoystickVecValue.x;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            x = Input.GetAxisRaw("Horizontal");

        if (x != 0)
        {
            if (x > 0)
            {
                characterFlipX = false;
            }
            else
            {
                characterFlipX = true;
            }
            animState = 1;
        }
        else
        {
            animState = 0;
        }
        transform.localPosition += new Vector3(x * Time.deltaTime * speed, 0, 0);
    }
    public void UI_JumpButton()
    {
        if (jumpCount < jumpMaxNum)
        {
            if (jumpCount == 0)
                characterAnimator.SetTrigger("Jump");
            Jump(15f, false);
            jumpCount++;
        }
    }

    public void Jump(float jumpPower, bool isStopJump)
    {
        if (isStopJump)
        {
            jumpCount = 2;
            characterAnimator.SetTrigger("Trampoline");
        }
        //characterRig.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        rb.velocity = Vector2.up * jumpPower;
    }

    bool GroundCheck()
    {
        Debug.DrawRay(transform.position, Vector2.down * 0.5f, Color.red, 0);
        int layerMask = 1 << LayerMask.NameToLayer("Ground");
        return Physics2D.Raycast(transform.position, Vector2.down, 0.5f, layerMask);
    }

    IEnumerator Invincibility()
    {
        isInvincibility = true;

        float timer = invincibilityTime;
        float blinkingSpeed = 10;
        SpriteRenderer playerRenderer = GetComponent<SpriteRenderer>();
        while (true)
        {
            playerRenderer.color = new Color(1, 1, 1, Mathf.Abs(Mathf.Sin(timer * blinkingSpeed)));
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                break;
            }
            yield return null;
        }
        playerRenderer.color = new Color(1, 1, 1, 1);

        isInvincibility = false;
    }
    public void healpotion()
    {
        playerHP += 50;
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Item"))
        {
            script_InventoryControl.AddItemToInventory(col.GetComponent<SpriteRenderer>().sprite);
            Destroy(col.gameObject);
        }
        else if (col.CompareTag("Enemy") && !isInvincibility)
        {
            StartCoroutine(Invincibility());
            characterAnimator.SetTrigger("Hit");
            PlayerHP -= 20;
        }
    }
}