# ImagesTracker — Multi-Marker AR 3D Visualization in Unity 6

An Android Augmented Reality application built with **Unity 6 (`6000.6.9f1`)**, **AR Foundation**, **Google ARCore**, and **Unity glTFast** that detects multiple physical reference targets simultaneously and anchors animated 3D models onto each marker in real time.

---

## Key Features
* **Plug-and-Play Multi-Marker Scalability:** Tracks up to 5 distinct physical targets concurrently (`AceMarker`, `QueenMarker`, `KingMarker`, `IdMarker`, `RubiksMarker`). Expanding the system requires zero changes to the C# codebase—simply register a new target image in `WatchImageLibrary` and map its corresponding prefab in the Inspector.
* **glTF 2.0 / `.glb` Asset Pipeline:** Uses `com.unity.cloud.gltfast` to import and render PBR-textured, animated `.glb` models alongside `.fbx` prefabs.
* **Dictionary-Backed Tracking Manager (`ARImageObjectTracker.cs`):** Maps reference image identifiers to individual prefabs, local placement offsets, and specific Animator states (`Take 001`, `allanims`) while preserving original prefab scales.
* **Occlusion Persistence:** Keeps spawned 3D models active during momentary tracking interruptions (`TrackingState.Limited`) and cleanly triggers animations upon tracking recovery.

---

## Reference Image to Prefab Mappings

| Reference Image | Physical Target | Mapped Prefab | Physical Width | Animation State | Application Domain |
| :--- | :--- | :--- | :--- | :--- | :--- |
| `AceMarker` | Ace of Spades Card | `BlackHolePrefab` | `0.03 m` | `Take 001` | Interactive STEM & Space Education |
| `QueenMarker` | Queen of Spades Card | `phoenixprefab` | `0.03 m` | `Take 001` | Immersive Children's Storytelling |
| `KingMarker` | King of Spades Card | `coltprefab` | `0.03 m` | `Take 001` | Defense & Tactical Equipment Inspection |
| `IdMarker` | ID Card | `RifleLoad` | Configured in library | `allanims` | Real-Time Weapon Loading & Drill Training |
| `RubiksMarker` | Rubik's Cube Face | `ARObject` | `0.50 m` | Default Layer 0 | 3D Object & Puzzle Visualization |

---

## Tech Stack & Dependencies
* **Engine:** Unity 6 (`6000.6.9f1`)
* **AR Framework:** `com.unity.xr.arfoundation` + `com.unity.xr.arcore`
* **3D Format Importer:** `com.unity.cloud.gltfast` (glTF 2.0 / `.glb`)
* **Target Platform:** Android (`OpenGLES3`, Linear Color Space, Google ARCore supported device)

---

## Repository Structure
```text
ImagesTracker-Unity6-AR/
├── Assets/
│   ├── ARImages/                 # Reference marker textures
│   ├── Models/                   # Imported .glb/.fbx 3D assets & Animator Controllers
│   ├── Prefab/                   # Configured 3D prefabs (BlackHolePrefab, phoenixprefab, coltprefab, RifleLoad, ARObject)
│   ├── Scenes/
│   │   └── WatchARScene.unity    # Main AR Foundation scene
│   ├── Script/
│   │   └── ARImageObjectTracker.cs # Multi-marker dictionary tracking & animation manager
│   ├── Settings/                 # URP pipeline settings
│   ├── XR/                       # XR Plug-in Management settings
│   └── WatchImageLibrary.asset   # XRReferenceImageLibrary with 5 physical markers
├── Packages/
│   ├── manifest.json             # Unity package dependencies (AR Foundation, ARCore, glTFast)
│   └── packages-lock.json
├── ProjectSettings/              # Android Player Settings, OpenGLES3 & XR configuration
├── .gitignore
└── README.md
