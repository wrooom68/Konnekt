using Firebase;
using Firebase.Unity.Editor;
using UnityEngine;

public class MyScript: MonoBehaviour {
	void Start() {
		// Set this before calling into the realtime database.
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://my-konnekt-demoirebaseio.com/");
             	}}
