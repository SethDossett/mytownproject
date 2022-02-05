using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CinematicBars : MonoBehaviour
{
    public static CinematicBars instance;
    [SerializeField] private Transform _topBar;
    [SerializeField] private Transform _bottomBar;
    [SerializeField] private float _cycleLegth = 2f;
   
    void Awake()
    {
        instance = this;
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
