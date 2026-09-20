## File manager for ULTRAKILL

Press `LeftAlt`+`E` to toggle the file manager

<img alt="image" src="https://github.com/user-attachments/assets/c95af0dd-47e6-4756-a46e-6cfd88931b6e" />

Supported actions:
- Cut (`Ctrl`+`X`), Copy (`Ctrl`+`C`), Paste (`Ctrl`+`V`)
- Permanently delete (`Shift`+`Delete`)

Keybinds can be customized in the Thorn menu (`RightShift`)

## Issues & Feedback

Fireman is a simple file manager that integrates with
the game in some aspects and is convenient to use (as it's in-game).
It is not a full replacement for your OS's file manager, and
you will certainly be disappointed if you expect it to
"have the basic stuff of Windows Explorer".

Regardless, you can help make it better by shooting suggestions
at @end_4 on Discord or by opening an issue on GitHub.

## Developers

Fireman can be used as a file/folder picker:

```csharp
// Create a picker and get its MonoBehaviour
var comp = FileManager.CreatePicker(allowMultiSelection: false, isSelectionFolder: false);

// Place it somewhere
var picker = comp.gameObject;
var kanvas = SceneManager.GetActiveScene().GetRootGameObjects().FirstOrDefault(obj => obj.name == "Canvas");
picker.transform.SetParent(kanvas.transform, false);

// Listen for selection
comp.ItemsPicked += pathArr => {
    // Do stuff with picked file. pathArr is an array of string paths
};
```

## License

- Code: MIT
- Assets: CC BY-SA 4.0

In general I don't mind others reusing my creations.
The licenses are to make it explicit and legally stay in the clear.
