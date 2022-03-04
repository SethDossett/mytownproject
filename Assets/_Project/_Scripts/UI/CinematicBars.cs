using DG.Tweening;
using UnityEngine;
using MyTownProject.Events;

namespace MyTownProject.UI
{
    public class CinematicBars : MonoBehaviour
    {
        [SerializeField] UIEventChannelSO uIEventChannel;
        [SerializeField] private Transform _topBar;
        [SerializeField] private Transform _bottomBar;
        [SerializeField] private float _cycleLegth = 2f;

        private void OnEnable()
        {
            uIEventChannel.OnBarsOn += BarsOn;
            uIEventChannel.OnBarsOff += BarsOff;
        }
        private void OnDisable()
        {
            uIEventChannel.OnBarsOn -= BarsOn;
            uIEventChannel.OnBarsOff -= BarsOff;
        }

        public void BarsOn()
        {
            _topBar.DOLocalMoveY(245f, _cycleLegth).SetUpdate(true);
            _bottomBar.DOLocalMoveY(-245f, _cycleLegth).SetUpdate(true);

        }
        public void BarsOff()
        {
            _topBar.DOLocalMoveY(278f, _cycleLegth).SetUpdate(true);
            _bottomBar.DOLocalMoveY(-278f, _cycleLegth).SetUpdate(true);
        }
    }
}