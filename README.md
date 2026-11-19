# ImagesTracker — Multi-Marker AR 3D Visualization in Unity 6

An Android Augmented Reality application built with **Unity 6 (`6000.6.9f1`)**, **AR Foundation**, **Google ARCore**, and **Unity glTFast** that detects multiple physical reference targets simultaneously and anchors animated 3D models onto each marker in real time.

---

## Key Features
* **Plug-and-Play Multi-Marker Scalability:** Tracks up to 4 distinct physical targets concurrently (`AceMarker`, `QueenMarker`, `KingMarker`, `IdMarker`). Expanding the system requires zero changes to the C# codebase—simply register a new target image in `WatchImageLibrary` and map its corresponding prefab in the Inspector.
* **glTF 2.0 / `.glb` Asset Pipeline:** Uses `com.unity.cloud.gltfast` to import and render PBR-textured, animated `.glb` models alongside `.fbx` prefabs.
* **Dictionary-Backed Tracking Manager (`ARImageObjectTracker.cs`):** Maps reference image identifiers to individual prefabs, local placement offsets, and specific Animator states (`Take 001`, `allanims`) while preserving original prefab scales.
* **Occlusion Persistence:** Keeps spawned 3D models active during momentary tracking interruptions (`TrackingState.Limited`) and cleanly triggers animations upon tracking recovery.

---

## Reference Image to Prefab Mappings

| Reference Image | Target Image File (`TargetImages/`) | Mapped Prefab | Physical Width | Animation State | Application Domain |
| :--- | :--- | :--- | :--- | :--- | :--- |
| `AceMarker` | `Ace.jpeg` | `BlackHolePrefab` | `0.03 m` | `Take 001` | Interactive STEM & Space Education |
| `QueenMarker` | `Queen.jpeg` | `phoenixprefab` | `0.03 m` | `Take 001` | Immersive Children's Storytelling |
| `KingMarker` | `King.jpeg` | `coltprefab` | `0.03 m` | `Take 001` | Defense & Tactical Equipment Inspection |
| `IdMarker` | `id.jpeg` | `RifleLoad` | Configured in library | `allanims` | Real-Time Weapon Loading & Drill Training |

---

## Tech Stack & Dependencies
* **Engine:** Unity 6 (`6000.6.9f1`)
* **AR Framework:** `com.unity.xr.arfoundation` + `com.unity.xr.arcore`
* **3D Format Importer:** `com.unity.cloud.gltfast` (glTF 2.0 / `.glb`)
* **Target Platform:** Android (`OpenGLES3`, Linear Color Space, Google ARCore supported device)

---

## Repository Contents
```text
ARBasedAndroidImagesSpawner/
├── TargetImages/          # Reference marker images to scan with the app
├── ImagesTracker.apk      # Pre-built Android APK (tracked via Git LFS)
├── .gitattributes         # Git LFS configuration for APK binaries
└── README.md              # Project documentation and usage guide
