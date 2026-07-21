using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CrawController : MonoBehaviour
{
    private Rigidbody2D rig;

    public Transform FruitSpot;

    [Tooltip("Prefabs de todas as frutas 'iniciais' que podem cair aleatoriamente (ex: Cereja, Morango, Uva, Maçã).")]
    public GameObject[] fruitPrefabs;

    [Tooltip("Velocidade de suavização do movimento (quanto maior, mais rápido a garra 'gruda' no dedo/mouse).")]
    public float speed;

    [Tooltip("Limite horizontal (em unidades do mundo) que a garra pode se mover para cada lado.")]
    public float clampX;

    private bool isDragging = false;
    private float offsetX = 0f;

    private GameObject currentFruit;
    private Rigidbody2D currentFruitRig;

    void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        // A garra deve ser Kinematic: ela é movida por código, não por física,
        // mas ainda pode empurrar/colidir com outros Rigidbody2D se precisar.
    }

    void Start()
    {
        SpawnFruitAtSpot();
    }

    void Update()
    {
        // No Editor/PC usa mouse (facilita testar); no build mobile usa touch.
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseInput();
#else
        HandleTouchInput();
#endif
    }

    void FixedUpdate()
    {
        if (isDragging)
        {
            float targetX = Mathf.Clamp(GetPointerWorldX() + offsetX, -clampX, clampX);
            Vector2 newPos = Vector2.Lerp(rig.position, new Vector2(targetX, rig.position.y), speed * Time.fixedDeltaTime);
            rig.MovePosition(newPos);
        }
    }

    // ---------- Mouse (Editor / testes no PC) ----------
    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            BeginDrag(ScreenToWorldX(Input.mousePosition));
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndDrag();
            ReleaseFruit();
        }
    }

    // ---------- Touch (Mobile) ----------
    void HandleTouchInput()
    {
        if (Input.touchCount == 0)
        {
            isDragging = false;
            return;
        }

        Touch touch = Input.GetTouch(0);
        float worldX = ScreenToWorldX(touch.position);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                BeginDrag(worldX);
                break;

            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                // offsetX já foi calculado no Began; a posição alvo é recalculada no FixedUpdate
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                EndDrag();
                ReleaseFruit();
                break;
        }
    }

    // ---------- Helpers ----------
    void BeginDrag(float pointerWorldX)
    {
        isDragging = true;
        // Guarda o deslocamento entre o dedo/mouse e a garra,
        // assim ela não "pula" para debaixo do dedo ao começar o arrasto.
        offsetX = rig.position.x - pointerWorldX;
    }

    void EndDrag()
    {
        isDragging = false;
    }

    float GetPointerWorldX()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        return ScreenToWorldX(Input.mousePosition);
#else
        return Input.touchCount > 0 ? ScreenToWorldX(Input.GetTouch(0).position) : rig.position.x;
#endif
    }

    float ScreenToWorldX(Vector2 screenPos)
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, -Camera.main.transform.position.z));
        return worldPoint.x;
    }

    // ---------- Spawn / Release de frutas ----------

    /// <summary>
    /// Instancia uma fruta aleatória no FruitSpot, como filha dele
    /// (assim ela acompanha a garra) e com a física desligada,
    /// pra não cair antes da hora.
    /// </summary>
    void SpawnFruitAtSpot()
    {
        if (fruitPrefabs == null || fruitPrefabs.Length == 0)
        {
            Debug.LogWarning("CrawCrontroller: nenhum prefab de fruta atribuído em fruitPrefabs.");
            return;
        }

        int index = Random.Range(0, fruitPrefabs.Length);
        currentFruit = Instantiate(fruitPrefabs[index], FruitSpot.position, Quaternion.identity, FruitSpot);

        currentFruitRig = currentFruit.GetComponent<Rigidbody2D>();
        if (currentFruitRig != null)
        {
            currentFruitRig.simulated = false; // sem física enquanto está "presa" na garra
        }
    }

    /// <summary>
    /// Solta a fruta atual (desvincula do FruitSpot e reativa a física)
    /// e instancia a próxima fruta aleatória no lugar.
    /// </summary>
    void ReleaseFruit()
    {
        if (currentFruit == null)
            return;

        currentFruit.transform.SetParent(null);

        if (currentFruitRig != null)
        {
            currentFruitRig.simulated = true; // agora cai normalmente
        }

        currentFruit = null;
        currentFruitRig = null;

        SpawnFruitAtSpot();
    }
}