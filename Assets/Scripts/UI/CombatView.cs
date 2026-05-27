using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatView
{
    private readonly Action<CardInstance> onCardClicked;
    private readonly Action<CardInstance, GameObject> onCardDropped;
    private readonly Action<GridEnemy> onEnemyClicked;
    private readonly Action onEndTurnClicked;
    private readonly Action onTakeRewardClicked;
    private readonly Action onSacrificeRewardClicked;
    private readonly Action onHealClicked;
    private readonly Action onUpgradeClicked;
    private readonly Action onCorruptClicked;
    private readonly Action<int> onDeckCardClicked;
    private readonly Action onRestartClicked;

    private Canvas canvas;
    private CombatBoardView boardView;

    public CombatView(
        Action<CardInstance> onCardClicked,
        Action<CardInstance, GameObject> onCardDropped,
        Action<GridEnemy> onEnemyClicked,
        Action onEndTurnClicked,
        Action onTakeRewardClicked,
        Action onSacrificeRewardClicked,
        Action onHealClicked,
        Action onUpgradeClicked,
        Action onCorruptClicked,
        Action<int> onDeckCardClicked,
        Action onRestartClicked)
    {
        this.onCardClicked = onCardClicked;
        this.onCardDropped = onCardDropped;
        this.onEnemyClicked = onEnemyClicked;
        this.onEndTurnClicked = onEndTurnClicked;
        this.onTakeRewardClicked = onTakeRewardClicked;
        this.onSacrificeRewardClicked = onSacrificeRewardClicked;
        this.onHealClicked = onHealClicked;
        this.onUpgradeClicked = onUpgradeClicked;
        this.onCorruptClicked = onCorruptClicked;
        this.onDeckCardClicked = onDeckCardClicked;
        this.onRestartClicked = onRestartClicked;
    }

    public void Build()
    {
        canvas = UnityEngine.Object.FindAnyObjectByType<Canvas>();

        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("CombatCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
        }

        boardView = UnityEngine.Object.FindAnyObjectByType<CombatBoardView>();

        if (boardView == null || boardView.GetComponentInParent<Canvas>() != canvas)
        {
            ClearChildren(canvas.transform);
            boardView = CombatBoardView.CreateRuntime(canvas);
        }

        boardView.Initialize(
            canvas,
            onCardClicked,
            onCardDropped,
            onEnemyClicked,
            onEndTurnClicked,
            onTakeRewardClicked,
            onSacrificeRewardClicked,
            onHealClicked,
            onUpgradeClicked,
            onCorruptClicked,
            onDeckCardClicked,
            onRestartClicked);
    }

    public void Refresh(
        CombatFlowState state,
        string heroName,
        string patronName,
        Unit player,
        EnemyFormation formation,
        DeckRuntime deck,
        int turnNumber,
        int currentEnergy,
        int maxEnergy,
        int patronInfluence,
        CardInstance selectedCard,
        int selectedDeckIndex,
        CardData currentRewardCard,
        IReadOnlyList<string> logLines,
        int shrineHealAmount,
        int patronCorruptCost,
        bool canUpgrade,
        bool canCorrupt)
    {
        boardView.Refresh(
            state,
            heroName,
            patronName,
            player,
            formation,
            deck,
            turnNumber,
            currentEnergy,
            maxEnergy,
            patronInfluence,
            selectedCard,
            selectedDeckIndex,
            currentRewardCard,
            logLines,
            shrineHealAmount,
            patronCorruptCost,
            canUpgrade,
            canCorrupt);
    }

    public void UpdateLog(IReadOnlyList<string> logLines)
    {
        boardView?.UpdateLog(logLines);
    }

    public void ResetTransientState()
    {
        boardView?.ResetTransientState();
    }

    private void ClearChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            UnityEngine.Object.Destroy(parent.GetChild(i).gameObject);
        }
    }
}
