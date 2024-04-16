using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class PlayerController : MonoBehaviour
{
    public GameObject chest;
    public GameObject cheeseText;
    public GameObject keyText;
    public GameObject timeText;

    public AudioClip jumpSound;
    public AudioClip splashSound;
    public AudioClip itemSound;

    public AudioSource audioSource;
    private Rigidbody rb;

    public float moveSpeed = 7f;
    public float jumpForce = 20.0f;

    public bool isKey = false;
    public int cheeseCount = 0;
    public float playTime = 30.0f;

    void Start()
    {
        chest = GameObject.Find("chest");
        cheeseText = GameObject.Find("CheeseText");
        keyText = GameObject.Find("KeyText");
        timeText = GameObject.Find("TimeText");

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // move character and rotation
        // if (Input.GetKeyDown(KeyCode.Escape))
        // {
        //    SceneManager.LoadScene("SampleScene");
        // }

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Vector3 moveDir = new Vector3(verticalInput, 0, horizontalInput).normalized;
        Vector3 moveDir = new Vector3(horizontalInput, 0, verticalInput).normalized;

        if (moveDir.magnitude > 0)
        {
            transform.forward = moveDir;
        }

        // character jump
        if (Input.GetKeyDown(KeyCode.Space) && rb.velocity.y == 0)
        {
            Jump();
            if (jumpSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(jumpSound);
            }
        }

        // gameover
        // 땅에서 떨어지거나 시간이 다되면 죽는다
        if (transform.position.y < 2.2f)
        {
            GetComponent<ParticleSystem>().Play();
            LoadNextScene("FailScene");
        }

            // 시간 제한이 있는 게임
            playTime -= Time.deltaTime;

        // 시간이 다되면
        if (playTime < 0)
        {
            Destroy(gameObject);
            LoadNextScene("FailScene");
        }

        keyText.GetComponent<TextMeshProUGUI>().text = (isKey == true ? "O" : "X");
        cheeseText.GetComponent<TextMeshProUGUI>().text = (cheeseCount * 100).ToString();
        timeText.GetComponent<TextMeshProUGUI>().text = playTime.ToString("F1") + "s"; 
    }

    void FixedUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Vector3 moveDir = new Vector3(horizontalInput, 0, verticalInput).normalized;
        Vector3 moveDir = new Vector3(-verticalInput, 0, horizontalInput).normalized;

        rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);

        Camera.main.transform.position = new Vector3(transform.position.x + 10, transform.position.y + 10, transform.position.z + 2);
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void LoadNextScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "cheese")
        {
            audioSource.PlayOneShot(itemSound);
            Debug.Log("cheese");
            cheeseCount++;
            Destroy(other.gameObject);
        }
        if(other.gameObject.tag == "key")
        {
            audioSource.PlayOneShot(itemSound);
            isKey = true;
            chest.GetComponent<BoxCollider>().isTrigger = true;
            Debug.Log("key");
            Destroy(other.gameObject);
        }
        if(other.gameObject.tag == "chest")
        {
            Debug.Log("chest");
            if (isKey)
            {
                LoadNextScene("clearScene");
                Destroy(other.gameObject);
            }
        }
    }  
}