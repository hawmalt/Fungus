﻿using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fungus;
using Rotorz.ReorderableList;
using System.Linq;

[CustomPropertyDrawer (typeof(Variable))]
public class VariableDrawer : PropertyDrawer 
{	
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) 
	{
		SerializedProperty keyProp = property.FindPropertyRelative("key");
		SerializedProperty typeProp = property.FindPropertyRelative("type");

		// Draw the text field control GUI.
		EditorGUI.BeginChangeCheck();

		Rect keyRect = position;
		keyRect.width *= 0.5f;

		Rect typeRect = position;
		typeRect.x += keyRect.width;
		typeRect.width -= keyRect.width;

		string keyValue = EditorGUI.TextField(keyRect, label, keyProp.stringValue);
		int selectedEnum = (int)(VariableType)EditorGUI.EnumPopup(typeRect, (VariableType)typeProp.enumValueIndex);

		if (EditorGUI.EndChangeCheck ())
		{
			char[] arr = keyValue.Where(c => (char.IsLetterOrDigit(c) || c == '_')).ToArray(); 
			
			keyValue = new string(arr);

			keyProp.stringValue = keyValue;
			typeProp.enumValueIndex = selectedEnum;
		}
	}
}

[CustomEditor (typeof(SequenceController))]
public class SequenceControllerEditor : Editor 
{
	SerializedProperty globalVariablesProperty;
	SerializedProperty localVariablesProperty;

	void OnEnable() 
	{
		localVariablesProperty = serializedObject.FindProperty("variables");
	}

	public override void OnInspectorGUI() 
	{
		serializedObject.Update();

		SequenceController t = target as SequenceController;
	
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Open Fungus Editor"))
		{
			EditorWindow.GetWindow(typeof(FungusEditorWindow), false, "Fungus Editor");
		}
		
		if (GUILayout.Button("New Sequence"))
		{
			GameObject go = new GameObject("Sequence");
			go.transform.parent = t.transform;
			Sequence s = go.AddComponent<Sequence>();
			FungusEditorWindow fungusEditorWindow = EditorWindow.GetWindow(typeof(FungusEditorWindow), false, "Fungus Editor") as FungusEditorWindow;
			s.nodeRect.x = fungusEditorWindow.scrollPos.x;
			s.nodeRect.y = fungusEditorWindow.scrollPos.y;
			Undo.RegisterCreatedObjectUndo(go, "Sequence");
			Selection.activeGameObject = go;
		}
		
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		GUIContent stepTimeLabel = new GUIContent("Step Time", "Minimum time to execute each step");
		t.stepTime = EditorGUILayout.FloatField(stepTimeLabel, t.stepTime);

		GUIContent startSequenceLabel = new GUIContent("Start Sequence", "Sequence to be executed when controller starts.");
		t.startSequence = SequenceEditor.SequenceField(startSequenceLabel, t, t.startSequence);

		if (t.startSequence == null)
		{
			GUIStyle style = new GUIStyle(GUI.skin.label);
			style.normal.textColor = new Color(1,0,0);
			EditorGUILayout.LabelField(new GUIContent("Error: Please select a Start Sequence"), style);
		}

		ReorderableListGUI.Title("Variables");
		ReorderableListGUI.ListField(localVariablesProperty);

		serializedObject.ApplyModifiedProperties();
	}
}