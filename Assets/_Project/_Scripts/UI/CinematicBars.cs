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

        public void BarsOn(float speed)
        {
            _topBar.DOLocalMoveY(245f, speed).SetUpdate(true);
            _bottomBar.DOLocalMoveY(-245f, speed).SetUpdate(true);

        }
        public void BarsOff(float speed)
        {
            _topBar.DOLocalMoveY(278f, speed).SetUpdate(true);
            _bottomBar.DOLocalMoveY(-278f, speed).SetUpdate(true);
        }
    }
}