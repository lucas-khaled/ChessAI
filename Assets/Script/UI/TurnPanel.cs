using DG.Tweening;
using TMPro;
using UnityEngine;

public class TurnPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text turnText;
    [SerializeField] private RectTransform initialTextPosition;
    [SerializeField] private RectTransform finalTextPosition;
    [SerializeField] private float panelDuration = 3;

    private Sequence animationSequence;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = initialTextPosition.anchoredPosition;
    }

    public void ShowTurn(IPlayer player)
    {
        if (player is HumanPlayer)
            turnText.text = "It's your turn, pathetic human";
        else
            turnText.text = "It's my turn, and I'll show you how it's done!";

        AnimatePanel();
    }

    private void AnimatePanel()
    {
        if (animationSequence != null && animationSequence.IsPlaying())
            animationSequence.Kill();

        animationSequence = DOTween.Sequence();
        animationSequence.AppendInterval(0.1f);
        animationSequence.Append(rectTransform.DOAnchorPos(finalTextPosition.anchoredPosition, 0.7f).SetEase(Ease.InOutBack));
        animationSequence.AppendInterval(panelDuration);
        animationSequence.Append(rectTransform.DOAnchorPos(initialTextPosition.anchoredPosition, 0.7f).SetEase(Ease.InOutCubic));
    }
}
