#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class CombatUiPrefabGenerator
{
    private const string PrefabFolder = "Assets/Prefabs/UI/Combat";
    private const string CardViewPath = PrefabFolder + "/CardView.prefab";
    private const string EnemyViewPath = PrefabFolder + "/EnemyView.prefab";
    private const string CombatBoardViewPath = PrefabFolder + "/CombatBoardView.prefab";

    [MenuItem("Tools/Card Game/Generate Combat UI Prefabs")]
    public static void GeneratePrefabs()
    {
        EnsureFolder("Assets/Prefabs");
        EnsureFolder("Assets/Prefabs/UI");
        EnsureFolder(PrefabFolder);

        CombatCardView cardView = CombatCardView.CreateDefault(null);
        SavePrefab(cardView.gameObject, CardViewPath);

        CombatEnemyView enemyView = CombatEnemyView.CreateDefault(null);
        SavePrefab(enemyView.gameObject, EnemyViewPath);

        GameObject cardPrefabObject = AssetDatabase.LoadAssetAtPath<GameObject>(CardViewPath);
        GameObject enemyPrefabObject = AssetDatabase.LoadAssetAtPath<GameObject>(EnemyViewPath);
        CombatCardView cardPrefab = cardPrefabObject != null ? cardPrefabObject.GetComponent<CombatCardView>() : null;
        CombatEnemyView enemyPrefab = enemyPrefabObject != null ? enemyPrefabObject.GetComponent<CombatEnemyView>() : null;

        GameObject boardObject = new GameObject("CombatBoardView", typeof(RectTransform), typeof(Image), typeof(CombatBoardView));
        CombatBoardView boardView = boardObject.GetComponent<CombatBoardView>();
        boardView.SetPrefabReferences(cardPrefab, enemyPrefab);
        boardView.BuildIfNeeded();
        SavePrefab(boardObject, CombatBoardViewPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Generated combat UI prefabs in {PrefabFolder}.");
    }

    private static void SavePrefab(GameObject prefabRoot, string path)
    {
        PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
        Object.DestroyImmediate(prefabRoot);
    }

    private static void EnsureFolder(string folderPath)
    {
        if (AssetDatabase.IsValidFolder(folderPath))
        {
            return;
        }

        string parentPath = Path.GetDirectoryName(folderPath)?.Replace("\\", "/");
        string folderName = Path.GetFileName(folderPath);

        if (!string.IsNullOrEmpty(parentPath) && !AssetDatabase.IsValidFolder(parentPath))
        {
            EnsureFolder(parentPath);
        }

        AssetDatabase.CreateFolder(parentPath, folderName);
    }
}
#endif
