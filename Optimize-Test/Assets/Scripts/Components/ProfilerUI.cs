using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfilerUI : MonoBehaviour
{
    [SerializeField] private Canvas _profilerCanvas;
    [SerializeField] private TextMeshProUGUI _fps;
    [SerializeField] private Button _button;

    private int _frameCount = 0;
    private float _deltaTimeSum = 0;
    private float _measureInterval = 0.1f;

    private void Awake()
    {
        _button.onClick.AddListener(ShowProfiler);
        _profilerCanvas.enabled = false;
    }

    private void ShowProfiler()
    {
        _profilerCanvas.enabled = true;
    }

    private void Update()
    {
        _deltaTimeSum += Time.deltaTime;
        _frameCount++;

        if (_deltaTimeSum >= _measureInterval)
        {
            var fps = (float)_frameCount / _deltaTimeSum;

            _fps.text = $"FPS: {Mathf.Ceil(fps)}";
            _frameCount = 0;
            _deltaTimeSum = 0;
        }
    }
}
