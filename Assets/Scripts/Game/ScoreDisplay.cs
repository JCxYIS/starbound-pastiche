using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ScoreDisplay : MonoBehaviour
{
    [Header("Bindings")]
    [SerializeField] Text _score;
    [SerializeField] Text _combo;
    [SerializeField] Text _lv;
    [SerializeField] Slider _lvSlider;
    [SerializeField] Text _debug;
    Image _lvSliderFillRect;

    [Header("Predefine")]
    [SerializeField] AnimationCurve _scaleCurve;
    [SerializeField] AnimationCurve _rotationCurve;
    [SerializeField] Color[] _colorsOfSlider;

    [Header("Channels")]
    [SerializeField] GameStartChannel _gameStartChannel;
    [SerializeField] ComboChangeChannel _comboChangeChannel;
    GameController gameController;


    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        _gameStartChannel.OnEventRaised += gc => gameController = gc;
        _comboChangeChannel.OnEventRaised += OnComboChange;
        OnComboChange(0);
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {        
        _comboChangeChannel.OnEventRaised -= OnComboChange;
    }


    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if(gameController)
        {
            _score.text = gameController.score.ToString("000000000");
            _lv.text = gameController.lv.ToString("");

            if(gameController.lv == 0)
                return;
            
            _lvSlider.value =  (gameController.LvAtCurrentSection + gameController.lvProgress) / 10;
            if(!_lvSliderFillRect)
                _lvSliderFillRect = _lvSlider.fillRect.GetComponent<Image>();
            _lvSliderFillRect.color = _colorsOfSlider[gameController.Section];
        }
        else
        {
            _score.text = "000000000";
            _lv.text = "";
            _lvSlider.value = 0;

        }
    }

    void OnComboChange(uint combo)
    {
        StopAllCoroutines();

        if(combo == 0)
        {
            _combo.enabled = false;
        }
        else
        {
            _combo.enabled = true;
            _combo.text = "x"+combo.ToString();
            StartCoroutine(ComboAnim());
        }
    }

    IEnumerator ComboAnim()
    {
        float duration = 0.5f;
        float time = 0;
        while(time < duration)
        {
            time += Time.deltaTime;
            float percent = time / duration;
            // print(_scaleCurve.Evaluate(percent));
            float scale = _scaleCurve.Evaluate(percent) + 1f;
            float rotationDeg = _rotationCurve.Evaluate(percent) * 30f;
            _combo.transform.localScale = new Vector3(scale ,scale, 1);
            _combo.transform.rotation = Quaternion.AngleAxis(rotationDeg, Vector3.forward);
            yield return 0;
        }
        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.identity;
    }

}