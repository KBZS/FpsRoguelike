using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StealthUI : MonoBehaviour
{
    [SerializeField, Range(0, 1)] private float _alphaChannelBackground;
    [SerializeField] Color _worried;
    [SerializeField] Color _angry;
    [SerializeField] Stealth _stealth;

    [SerializeField] Image _stealthIcon;
    [SerializeField] Image _stealthIconBackground;
    [SerializeField] Image _stealthCircle;
    [SerializeField] Slider _slider;

    private Transform _sender;
    private float _woriedTimer = 0;
    private float _maxWoriedTime;

    private Coroutine _increaseAmountCoroutine;
    private Coroutine _decreaseAmountCoroutine;

    void Awake()
    {
        _stealthCircle.gameObject.SetActive(false);
        _stealthIcon.gameObject.SetActive(false);
        _stealthIconBackground.gameObject.SetActive(false);     
    }

    void Start()
    {
        _stealth.OnGetWorried.AddListener(OnGetWorried);
        _stealth.OnLoseTarget.AddListener(OnLoseTarget);
        _stealth.OnReact.AddListener(OnOnReact);
        _stealth.OnCalmDown.AddListener(OnOnCalmDown);
    }

    void Update()
    {
        if (_sender == null)
            return;

        Vector3 playerXZ = PlayerSingleton.Instance.forward;
        playerXZ.y = 0;
        Vector3 targetXZ = _sender.position - PlayerSingleton.Instance.position;
        targetXZ.y = 0;

        _stealthCircle.transform.rotation = Quaternion.Euler(Vector3.forward * Vector3.SignedAngle(playerXZ, targetXZ, Vector3.down));
    }

    void OnGetWorried(StealthEventArgs args)
    {
        _sender = args.Sender;
        _maxWoriedTime = args.ReactionTime;
        _stealthCircle.gameObject.SetActive(true);
        _stealthIcon.gameObject.SetActive(true);
        _stealthIconBackground.gameObject.SetActive(true);

        _stealthCircle.color = _worried;
        _stealthIcon.color = _worried;
        Color bg = _worried;
        bg.a *= _alphaChannelBackground;
        _stealthIconBackground.color = bg;

        if (_decreaseAmountCoroutine != null)
            StopCoroutine(_decreaseAmountCoroutine);
        _increaseAmountCoroutine = StartCoroutine(IncreaseAmount());
    }

    void OnLoseTarget(StealthEventArgs args)
    {
        if (_increaseAmountCoroutine != null)
            StopCoroutine(_increaseAmountCoroutine);
        _decreaseAmountCoroutine = StartCoroutine(DecreaseAmount());
    }

    void OnOnReact(StealthEventArgs args)
    {
        _stealthCircle.color = _angry;
        _stealthIcon.color = _angry;
        _stealthIconBackground.color = _angry;
    }

    void OnOnCalmDown(StealthEventArgs args)
    {
        _sender = null;
        _woriedTimer = 0;
        _stealthCircle.gameObject.SetActive(false);
        _stealthIcon.gameObject.SetActive(false);
        _stealthIconBackground.gameObject.SetActive(false);
    }

    IEnumerator IncreaseAmount()
    {
        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        while (_woriedTimer < _maxWoriedTime)
        {
            _woriedTimer += Time.deltaTime;
            _slider.value = _woriedTimer / _maxWoriedTime;
            yield return wait;
        }
    }

    IEnumerator DecreaseAmount()
    {
        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        while (_woriedTimer > 0)
        {
            _woriedTimer -= Time.deltaTime;
            _slider.value = _woriedTimer / _maxWoriedTime;
            yield return wait;
        }
    }

    void OnDisable()
    {
        _stealth.OnGetWorried.RemoveListener(OnGetWorried);
        _stealth.OnLoseTarget.RemoveListener(OnLoseTarget);
        _stealth.OnReact.RemoveListener(OnOnReact);
        _stealth.OnCalmDown.RemoveListener(OnOnCalmDown);
    }
}
