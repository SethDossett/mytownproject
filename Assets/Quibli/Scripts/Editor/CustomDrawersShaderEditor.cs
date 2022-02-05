using System.Text.RegularExpressions;
using Quibli;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class CustomDrawersShaderEditor : ShaderGUI {
    private readonly MaterialGradientDrawer _gradientDrawer = new MaterialGradientDrawer();
    private readonly MaterialVector2Drawer _vectorDrawer = new MaterialVector2Drawer();

    public override void OnGUI(MaterialEditor editor, MaterialProperty[] properties) {
        foreach (var property in properties) {
            bool hideInInspector = (property.flags & MaterialProperty.PropFlags.HideInInspector) != 0;
            if (hideInInspector) {
                continue;
            }

            var tooltip = Tooltips.Get(editor, property.displayName);

            if (property.displayName.Contains("[Header]")) {
                DrawHeader(property, tooltip);
                continue;
            }

            if (property.displayName.Contains("[Space]")) {
                EditorGUILayout.Space();
                continue;
            }

            var displayName = property.displayName;
            displayName = HandleTabs(displayName);
            displayName = RemoveEverythingInBrackets(displayName);

            if (property.type == MaterialProperty.PropType.Texture && property.name.Contains("GradientTexture")) {
                EditorGUILayout.Space(18);
                _gradientDrawer.OnGUI(Rect.zero, property, property.displayName, editor, tooltip);
            } else if (property.type == MaterialProperty.PropType.Vector &&
                       property.displayName.Contains("[Vector2]")) {
                EditorGUILayout.Space(18);
                _vectorDrawer.OnGUI(Rect.zero, property, displayName, editor, tooltip);
            } else {
                var guiContent = new GUIContent(displayName, tooltip);
                editor.ShaderProperty(property, guiContent);
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        if (SupportedRenderingFeatures.active.editableMaterialRenderQueue) editor.RenderQueueField();
        editor.EnableInstancingField();
        editor.DoubleSidedGIField();
    }

    private string HandleTabs(string displayName) {
        while (displayName.Contains("[t]")) {
            displayName = displayName.Replace("[t]", "    ");
        }

        return displayName;
    }

    void DrawHeader(MaterialProperty property, string tooltip) {
        EditorGUILayout.Space();
        string displayName = RemoveEverythingInBrackets(property.displayName);
        var guiContent = new GUIContent(displayName, tooltip);
        EditorGUILayout.LabelField(guiContent);
    }

    private string RemoveEverythingInBrackets(string s) {
        s = Regex.Replace(s, @" ?\[.*?\]", string.Empty);
        s = Regex.Replace(s, @" ?\{.*?\}", string.Empty);
        return s;
    }
}