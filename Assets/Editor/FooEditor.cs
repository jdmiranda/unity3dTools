using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (Foo))]
public class FooEditor : Editor {

	public override void OnInspectorGUI ()
	{

		Foo foo = target as Foo;

		if (DrawDefaultInspector ()) {
			foo.Bar ();
		}

		if (GUILayout.Button("Foo Bar")) {
			foo.Bar ();
		}


	}

}
