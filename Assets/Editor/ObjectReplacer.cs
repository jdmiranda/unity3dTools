using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Text;

public class ObjectReplacer : ScriptableWizard 
{
	private Transform[] transforms = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.ExcludePrefab);

	private static string strShowDialogsKey = "ObjectReplacer.ShowDialogs";

	public bool ShowDialogs = true;
	public GameObject replacementPrefab;

	[MenuItem("Tools/Replace Selection", false, 1050)]
	static void CreateWizard()
	{
		ScriptableWizard.DisplayWizard("Object Replacer", typeof(ObjectReplacer), "Replace and Close", "Replace");
	}

	void OnWizardCreate()
	{
		DoReplaceAll();
	}

	void OnEnalbe()
	{
		ShowDialogs = EditorPrefs.GetBool(strShowDialogsKey);
	}

	void OnWizardOtherButton()
	{
		DoReplaceAll();
	}

	void OnWizardUpdate()
	{
		helpString = CheckForHelp();
		errorString = CheckForErrors();
		isValid = string.IsNullOrEmpty(errorString);
		EditorPrefs.SetBool(strShowDialogsKey, ShowDialogs);
	}

	void OnSelectionChange()
	{
		OnWizardUpdate();
	}

	string CheckForErrors()
	{
		var errorSb = new StringBuilder();
		if (replacementPrefab == null) {errorSb.Append("replacementPrefab is null\n");}
		if (transforms.Length.Equals(0)) {errorSb.Append("nothing selected for replacement\n");}
		return errorSb.ToString();
	}

	string CheckForHelp()
	{
		var helpSb = new StringBuilder();
		helpSb.Append(transforms.Length + " objects selected for replacement.");
		return helpSb.ToString();
	}

	void DoReplaceAll()
	{
		if (!string.IsNullOrEmpty(errorString)) return;

		if (ShowDialogs)
		{
			if (!EditorUtility.DisplayDialog
				(
					"Are you sure?",
					string.Format("Replace {0} objects with {1} ?", transforms.Length, replacementPrefab.name),
					"Yes",
					"Cancel"
				))
			{
				return;
			}
		}
		
		int replacedCount = 0;

		foreach ( Transform transform in transforms)
		{
			if (EditorUtility.DisplayCancelableProgressBar
				(
					"Working",
					string.Format("Replaceing {}", transform.name),
					replacedCount / (float) transforms.Length
				))
			{
				break;
			}

			ReplaceObject(transform);
			replacedCount++;
		}

		EditorUtility.ClearProgressBar();
		ShowNotification(new GUIContent(string.Format("{0} objects replaced", replacedCount)));

		if (ShowDialogs)
		{
			EditorUtility.DisplayDialog
			(
				"Done replacing", 
				string.Format("Done replacing {0} objects in scene.", transforms.Length),
				"Ok"
			);	
		}
	}

	void ReplaceObject(Transform transform)
	{
		GameObject newGameObject;
		newGameObject = PrefabUtility.InstantiatePrefab(replacementPrefab) as GameObject;
		newGameObject.transform.position = transform.position;
		newGameObject.transform.localScale = transform.localScale;
		newGameObject.transform.rotation = transform.rotation;
		newGameObject.transform.parent = transform.parent;
		Undo.RegisterCreatedObjectUndo(newGameObject, "Replaced gameObject");
		Undo.DestroyObjectImmediate(transform.gameObject);
	}
}
