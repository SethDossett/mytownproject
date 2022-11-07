using DG.Tweening;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MyTownProject.UI
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] GameObject _pagesParent;
        [SerializeField] GameObject _currentMenuPage;
        [SerializeField] GameObject _prevMenuPage;
        int _currentMenuIndex;
        public bool InputDisabled;
        [SerializeField] List<GameObject> pages = new List<GameObject>();
        [SerializeField] MainMenuManager manager;

        [SerializeField] Image _triggerButtonL;
        [SerializeField] Image _triggerButtonR;


        private void OnEnable()
        {
            MainMenuManager.OnMenuStateChanged += StateChanged;
        }
        private void OnDisable()
        {
            MainMenuManager.OnMenuStateChanged -= StateChanged;
        }
        private void Start()
        {
            GetButtonSelection selection = _currentMenuPage.GetComponent<GetButtonSelection>();
            if (!selection.IgnoreGetter)
            {
                GameObject buttonSelection = selection.Button;
                StartCoroutine(SetFirstSelection(buttonSelection));
            }
        }
        void StateChanged(MainMenuState newState, MainMenuState prevState)
        {
            int prevStateIndex = (int)prevState;
            _currentMenuIndex = (int)newState;

            _prevMenuPage = pages[prevStateIndex];
            _currentMenuPage = pages[_currentMenuIndex];

        }

        public void MovePage(int direction)
        {
            EventSystem.current.currentInputModule.enabled = false;
            _currentMenuPage.SetActive(true);
            _currentMenuPage.transform.localPosition = new Vector2(1200f * direction, 0f);
            StartCoroutine(TweenPage(-direction, 0.75f));
        }

        IEnumerator TweenPage(int direction, float duration)
        {
            DisableInputs();
            Tween mytween = _prevMenuPage.transform.DOLocalMoveX((1200 * direction) + _prevMenuPage.transform.localPosition.x, duration).SetEase(Ease.InOutElastic).SetUpdate(true);
            _currentMenuPage.transform.DOLocalMoveX((1200 * direction) + _currentMenuPage.transform.localPosition.x, duration).SetEase(Ease.InOutElastic).SetUpdate(true);
            yield return mytween.WaitForCompletion();
            _prevMenuPage.SetActive(false);
            DisableInputs();
            yield break;
        }

        void DisableInputs()
        {
            if (!InputDisabled)
            {
                InputDisabled = true;
                _triggerButtonL.DOFade(0.3f, 0.3f);
                _triggerButtonR.DOFade(0.3f, 0.3f);
            }
            else
            {
                InputDisabled = false;
                _triggerButtonL.DOFade(1f, 0.3f);
                _triggerButtonR.DOFade(1f, 0.3f);
                GetButtonSelection selection = _currentMenuPage.GetComponent<GetButtonSelection>();
                if (!selection.IgnoreGetter)
                {
                    GameObject buttonSelection = selection.Button;
                    StartCoroutine(SetFirstSelection(buttonSelection));
                }
                EventSystem.current.currentInputModule.enabled = true;
            }
        }
        IEnumerator SetFirstSelection(GameObject selection)
        {
            EventSystem.current.SetSelectedGameObject(null);
            yield return new WaitForEndOfFrame();
            EventSystem.current.SetSelectedGameObject(selection);
            yield break;
        }
    }
}
