# Core Package

Last Updated: May 1, 2022 4:02 PM

# Scene Management

## Boot Loader

Boot Loader is responsible for managing which scenes are loaded and unloaded in the Unity scene. The Boot Loader should be placed in the first scene that is loaded in the Unity Scene. In the Boot Loader, there is an array of Scene Collections, in which you call the pass the index of the Scene Collection you want to load in. 

### Functions

To get the instance of the boot loader:

```csharp
CoreBootLoader.Instance;
```

To change the loaded Scene Collection:

```csharp
enum Scenes
{
    MainMenu = 0,
    Game = 1
};

CoreBootLoader.Instance.ChangeSceneCollection((int)eScenes.Game);
```

To add in a specific scene:

```csharp
CoreBootLoader.Instance.AddScene(_sceneIndex);
```

To remove a specific scene

```csharp
CoreBootLoader.Instance.RemoveScene(_sceneIndex);
```

### Actions

They’re are two Actions that are called during the scene loading process. These callbacks can be used to add in a loading scene and get information about how far the loading process is to display to the user.

| Action Name | Parameters | Description |
| --- | --- | --- |
| ShowLoadingScene | Bool  | A bool that is used to indicate if the loading scene should be shown or hidden. Will be called at the beginning and end of the scene loading process to show/hide UI. |
| UpdateLoadingPercentage | Float | A value between 0-1 that indicates the percentage to completion. |

## Scene Collections

Scenes Collections is a scriptable object that holds the reference of which scenes you want to be loaded in at one time. Scene Collections are helpful because it means you can mark all the scenes that are needed for being in a main menu, hub world or game world, and then switch to only showing those scenes at the right time.

## Base Scene Loader

Base Scene Loader is used as a way to know when the application is ready to move on from the scene loading process. Because we’re loading scenes asynchronously, the awake and start functions can run at different times, and so the base scene loader class can help to make sure every scene starts at the same time if need be. To make use of this functionality, you need to create a class that inherits from the Base Scene Loader. 

### Functions

```csharp
//Called as soon as all scenes are loaded
public override void OnSceneReady();

//Called a short time after all scenes are loader
public override void OnSceneStart();

//Called when scenes are changed and this scene hasn't unloaded
public override void OnSceneChange();
```

---

# Menu Manager

-Need to add this section

## Base Menu

---

# Audio Manager

Audio Manager is a partial class and has the basics set up for what any project would need audio functionality. Because it’s a partial class, it is easy to add more functionality to customise this for the exact project need.

## Properties

| Name | Type | Description  |
| --- | --- | --- |
| IsMusicMuted | Bool (get) | Is Music currently muted |
| MusicVolume | Float (get/set) | A value between 0-1. Returns the volume of the Music or sets the volume of the Music. |
| IsSFXMuted | Bool (get) | Is SFX muted |
| SFXVolume | Float (get/set) | A value between 0-1. Returns the volume of SFX or sets the volume of SFX. |

## Functions

To play music:

```csharp
AudioManager.Instance.PlayMusic(_audioClip);
```

If using the Audio Mixer component, you need to convert between decibels and liner values. 

```csharp
float _value;

_value.ConvertFlotToDb();
_value.ConvertDbToFloat();
```

## Callbacks

| Action Name | Parameter | Description |
| --- | --- | --- |
| OnMusicMuteChange | Bool | Is called when music muted is changed. True when muted, false when not. |
| OnMusicVolumeChange | Float | A value between 0-1. Is called when music volume is changed. |
| OnSFXMuteChange | Bool  | Is called when SFX mute is changed. True when muted, false when not. |
| OnSFXVolumeChange | Float | A value between 0-1. Is called when SFX is changed. |

---

# Object Pooler

The object pooler works but making use of an interface, which you can add to any class. The object pooler itself has an array of pooled objects which you need to fill, and these will become the objects that are pooled during gameplay. Please ensure a class that implements IPoolable is on the prefab to use.

To get a pooled object, use:

```csharp
ObjectPooler.Instance.GetObject(_poolID);
```

This returns an IPoolable for you to then use to move the object to the desired location.

To remove all objects of a specific ID :

```csharp
ObjectPooler.Instance.ClearPooledObject(_poolID);
```

Or to remove all pooled objects:

```csharp
ObjectPooler.Instance.ClearAllObjects();
```

For this object pooler to work, you must implement some specific things in the IPoolable interface. 

## IPoolable

To implement the IPoolable interface

```csharp
public class ClassName : MonoBehaviour, IPoolable
{
}
```

There will then be several things you need to implement to satisfy the interface.

| Name | Type | Purpose |
| --- | --- | --- |
| PoolID | Int (variable) | A unique ID used to referance this pooled object |
| IsInScene | Bool (variable) | Used to know if the object is in the scene. Should just be implemented and left. |
| ReturnToPool() | Void (function) | Called when the object should be returned to the pool. |
| SetPosition() | Void (function) | Used to set the position of the object in the world. |

The interface should be implemented like below. You can add your own code around the code provided below but to make this work you need to include functions below.

```csharp
enum PoolID
{
	Coin,
	Gem
}

public int PoolID { get { return (int) PoolID.Coin; } }

public bool IsInScene { get; set; }

public void ReturnToPool()
{
	gameObject.SetActive(false);
  IsInScene = false;

	gameObject.transform.SetParent(ObjectPooler.Instance.transform);
}

public void SetPosition(Transform _newParent)
{
	SetPosition(_newParent, _newParent.position);
}

public void SetPosition(Transform _newParent, Vector3 _position)
{
	transform.SetParent(_newParent);
	transform.position = _position;

	gameObject.SetActive(true);
}
```

---

# Save File Helper

Save file helper is a static class that can be used to assist you saving and loading game data files. Currently it supports JSON and XML, and offers the ability to save data normally or securely using AES.

If you plan on using encrypted data, you need to set up a ‘Key’ first that is used to save your data. You can do this manually with:

```csharp
SaveDataFile.EncryptionKey = "SomeSuperSecretKey";
```

Or you can also let the Save File Helper create a key for you.

```csharp
SaveDataFile.CreateEncryptionKey();
```

You only need to set or create a key once. Once its been created, it saves it as a player pref and recalls it when its nee

## XML

To save data in a XML format

```csharp
string _path = "/SaveData.dat";
SomeDataClass _data = new SomeDataClass;

SaveDataFile.SaveDataXML(_path, _data);
```

Or to save it with encryption

```csharp
string _path = "/SaveData.dat";
SomeDataClass _data = new SomeDataClass;

SaveDataFile.SaveDataSecureXML(_path, _data);
```

To load data in XML

```csharp
string _path = "/SaveData.dat";
SomeDataClass _data = new SomeDataClass;

SaveDataFile.LoadDataXML(_path, out _data); //retur's a bool if successful
```

Or to load encrypted XML

 

```csharp
string _path = "/SaveData.dat";
SomeDataClass _data = new SomeDataClass;

SaveDataFile.LoadDataSecureXML(_path, out _data); //retur's a bool if successful
```

## JSON

To save data in JSON:

```csharp
string _path = "/SaveData.dat";
SomeDataClass _data = new SomeDataClass;

SaveDataFile.SaveDataJSON(_path, _data);
//Or
string _preCreatedJsonData;

SaveDataFile.SaveDataJson(_path, _preCreatedJsonData);
```

To encrypt save data in JSON:

```csharp
string _path = "/SaveData.dat";
SomeDataClass _data = new SomeDataClass;

SaveDataFile.SaveDataSecureJSON(_path, _data);
//Or
string _preCreatedJsonData;

SaveDataFile.SaveDataSecureJson(_path, _preCreatedJsonData);
```

To load JSON data:

```csharp
string _path = "/SaveData.dat";
SomeDataClass _data = new SomeDataClass;

SaveDataFile.LoadDataJSON(_path, out _data); //retur's a bool if successful
```

To load encrypted JSON

```csharp
string _path = "/SaveData.dat";
SomeDataClass _data = new SomeDataClass;

SaveDataFile.LoadDataSecureJSON(_path, out _data); //retur's a bool if successful
```

## Helpful Functions

If you want to check if a path is valid for a file:

```csharp
string _path = "/SaveData.dat";

SaveDataFile.DoesFileExist(_path); //returns if the file exists
```
