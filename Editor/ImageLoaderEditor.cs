using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using VRC.SDKBase;
using System;
using VRC.Udon;

namespace iruca.ImageLoader
{
    [CustomEditor(typeof(ImageLoader))]
    public class ImageLoaderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            SerializedProperty defaultTexture = serializedObject.FindProperty("defaultTexture");
            SerializedProperty eventTargetUdon = serializedObject.FindProperty("udonBehaviour");
            using (new LabelWidthScope(300))
            {
                defaultTexture.objectReferenceValue = EditorGUILayout.ObjectField("Image displayed before loading (optional)", defaultTexture.objectReferenceValue, typeof(Texture), false);
            }
            EditorGUILayout.Space(10);
            eventTargetUdon.objectReferenceValue = EditorGUILayout.ObjectField("Udon Behavior (optional)", eventTargetUdon.objectReferenceValue, typeof(UdonBehaviour), true);
            EditorGUILayout.Space(20);
            reorderableList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        ReorderableList reorderableList;

        void OnEnable()
        {
            SerializedProperty prop = serializedObject.FindProperty("images");
            reorderableList = new ReorderableList(serializedObject, prop)
            {
                drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Image Download"),
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    SerializedProperty element = prop.GetArrayElementAtIndex(index);
                    rect.height -= 4;
                    rect.y += 2;
                    EditorGUI.PropertyField(rect, element);
                },
                elementHeightCallback = (index) =>
                {
                    return EditorGUI.GetPropertyHeight(prop.GetArrayElementAtIndex(index)) * 2.5f;
                },
            };
        }
    }

    [CustomPropertyDrawer(typeof(Images))]
    public class ImageLoaderEditorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //元は 1 つのプロパティーであることを示すために PropertyScope で囲む
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                //ラベル領域の幅を調整
                EditorGUIUtility.labelWidth = 150;

                position.height = EditorGUIUtility.singleLineHeight;

                //各プロパティーの Rect を求める
                Rect URLRect = new Rect(position)
                {
                    y = position.y
                };

                Rect MaterialRect = new Rect(URLRect)
                {
                    y = URLRect.y + EditorGUIUtility.singleLineHeight + 2
                };

                //各プロパティーの SerializedProperty を求める
                SerializedProperty URLProperty = property.FindPropertyRelative("imageUrl");
                SerializedProperty MaterialProperty = property.FindPropertyRelative("material");

                //各プロパティーの GUI を描画
                URLProperty.stringValue = EditorGUI.TextField(URLRect, "Image URL", URLProperty.stringValue);
                MaterialProperty.objectReferenceValue = EditorGUI.ObjectField(MaterialRect, "Material", MaterialProperty.objectReferenceValue, typeof(Material), false);
            }
        }
    }

    public sealed class LabelWidthScope : IDisposable
    {
        private readonly float m_oldLabelWidth;

        public LabelWidthScope(int labelWidth)
        {
            m_oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = labelWidth;
        }

        public void Dispose()
        {
            EditorGUIUtility.labelWidth = m_oldLabelWidth;
        }
    }
}
