#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class ThreatBoardSceneGenerator
{
    private const string ScenePath = "Assets/Scenes/ThreatBoardScene.unity";
    private const string DefaultConfigPath = "Assets/ThreatBoard/DefaultThreatBoardConfig.asset";
    private const string PaladinPath = "Assets/HeroClasses/Paladin.asset";
    private const string DevourerPath = "Assets/Patrons/The Devourer.asset";

    [MenuItem("Tools/Card Game/Create Threat Board Scene")]
    public static void CreateScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        GameObject cameraObject = new GameObject("Main Camera", typeof(Camera));
        cameraObject.tag = "MainCamera";
        Camera camera = cameraObject.GetComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.065f, 0.058f, 0.05f, 1f);
        camera.orthographic = true;

        GameObject eventSystemObject = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));

        GameObject canvasObject = new GameObject("ThreatBoardCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        GameObject viewObject = new GameObject("ThreatBoardView", typeof(RectTransform), typeof(Image), typeof(ThreatBoardView));
        viewObject.transform.SetParent(canvasObject.transform, false);
        ThreatBoardView view = viewObject.GetComponent<ThreatBoardView>();
        view.BuildIfNeeded();

        GameObject controllerObject = new GameObject("ThreatBoardController", typeof(ThreatBoardController));
        ThreatBoardController controller = controllerObject.GetComponent<ThreatBoardController>();

        SerializedObject serializedController = new SerializedObject(controller);
        serializedController.FindProperty("selectedClass").objectReferenceValue = AssetDatabase.LoadAssetAtPath<HeroClassData>(PaladinPath);
        serializedController.FindProperty("selectedPatron").objectReferenceValue = AssetDatabase.LoadAssetAtPath<PatronData>(DevourerPath);
        serializedController.FindProperty("boardConfig").objectReferenceValue = AssetDatabase.LoadAssetAtPath<ThreatBoardConfig>(DefaultConfigPath);
        serializedController.FindProperty("boardView").objectReferenceValue = view;
        serializedController.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.SaveScene(scene, ScenePath);
        AssetDatabase.Refresh();
        Debug.Log($"Created threat board scene at {ScenePath}.");
    }
}
#endif
