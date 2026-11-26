# Evidence Index: God Mode City Destroyer

This document logs all authenticated Evidence records, external network artifacts, engine logs, and inspection parameters gathered during Phase 1 and Phase 2.

---

## 1. REA MCP Evidence Records

| Evidence ID | Tool / Operation | Predicate Type | Confidence | Subject / Scope |
| :--- | :--- | :--- | :--- | :--- |
| `ev_3c90b996acec8977cd22e5dcc2f1a728dd81b10a4f380a71a36204a04cb7523d` | `list_browser_targets` | `rea.browser-target-list` | Observed | Target ID `3A2FD06887D595672AF2B1BDC0C91F82` (`https://www.crazygames.com/game/god-mode-city-destroyer`) |
| `ev_dff865cca2ed5b000b048e68b9878196509b3a6ff6c97eef89f07465cfa478f6` | `analyze_web_bundle` | `rea.web-bundle-analysis` | Observed | Static script chunk `scr_27cb14db816dfa60074c285ad8128de232425969bbc6008226f1d4b23aad6bd2` |
| `ev_474ac76a4ae3ada29af909f7738ce0ea1e626eb65014531fe516423baf8fdb98` | `observe_web_session` | `rea.web-observation-session` | Observed | 2000ms navigation timeline monitoring on target |
| `ev_073a4c233825bc4eeff0947f4a5a4328922bf714c146c3c3492fca464bc37b16` | `inspect_web_network_capture` | `rea.analysis-view` | Derived | HAR 1.2 capture record `game_load.har` (SHA-256 `0834bcdf9001b932efc43615c1501458c53742a7994fc426b701de86e9d90e10`) |

---

## 2. Public Network Artifacts

| Asset Identifier | MIME Type | Compression | Size (Bytes) | Role |
| :--- | :--- | :--- | :--- | :--- |
| `https://files.crazygames.com/god-mode-city-destroyer/17/Build/WebGL.loader.js` | `application/javascript` | None | 26,455 | Unity 6 WebGL loader |
| `https://files.crazygames.com/god-mode-city-destroyer/17/Build/WebGL.framework.js.br` | `application/javascript` | Brotli (`.br`) | 894,102 | Emscripten JS runtime framework |
| `https://files.crazygames.com/god-mode-city-destroyer/17/Build/WebGL.wasm.br` | `application/wasm` | Brotli (`.br`) | 18,204,910 | Compiled WebAssembly binary |
| `https://files.crazygames.com/god-mode-city-destroyer/17/Build/WebGL.data.br` | `application/octet-stream` | Brotli (`.br`) | 9,660,520 | Serialized Unity asset archive |
| `https://games.crazygames.com/en_US/god-mode-city-destroyer/index.html` | `text/html` | gzip/br | 13,120 | Host game container & metadata config |
| `https://videos.crazygames.com/god-mode-city-destroyer/fcff9fc2-d065-4a9a-ac62-43a8dc9c9f4c/god-mode-city-destroyer.mp4` | `video/mp4` | None | ~15 MB | Canonical gameplay video reference |

---

## 3. Verified Engine Diagnostics & Log Trace

The following literal log outputs were intercepted from the runtime console:

```text
[UnityMemory] Configuration Parameters - Can be set up in boot.config
    "memorysetup-allocator-temp-initial-block-size-main=262144"
    "memorysetup-allocator-temp-initial-block-size-worker=262144"
    "memorysetup-temp-allocator-size-audio-worker=65536"
    "memorysetup-remapper-allocator-initial-capacity=32768"
    "memorysetup-temp-allocator-size-background-worker=32768"
    "memorysetup-bucket-allocator-granularity=16"
    "memorysetup-bucket-allocator-bucket-count=8"
    "memorysetup-bucket-allocator-block-size=4194304"
    "memorysetup-bucket-allocator-block-count=1"
    "memorysetup-main-allocator-block-size=16777216"
    "memorysetup-thread-allocator-block-size=16777216"
    "memorysetup-gfx-main-allocator-block-size=16777216"
    "memorysetup-gfx-thread-allocator-block-size=16777216"
    "memorysetup-cache-allocator-block-size=4194304"
    "memorysetup-typetree-allocator-block-size=2097152"
    "memorysetup-profiler-bucket-allocator-granularity=16"
    "memorysetup-profiler-bucket-allocator-bucket-count=8"
    "memorysetup-profiler-bucket-allocator-block-size=4194304"
    "memorysetup-profiler-bucket-allocator-block-count=1"
    "memorysetup-profiler-allocator-block-size=16777216"
    "memorysetup-profiler-editor-allocator-block-size=1048576"
    "memorysetup-temp-allocator-size-main=4194304"
    "memorysetup-job-temp-allocator-block-size=2097152"
    "memorysetup-job-temp-allocator-block-size-background=1048576"
    "memorysetup-job-temp-allocator-reduction-small-platforms=262144"
Input System module state changed to: Initialized.
[Physics::Module] Initialized fallback backend.
[Physics::Module] Id: 0xdecafbad
Loading player data from data.unity3d
Initialize engine version: 6000.0.84f1 (78ab6fc243d5)
[Subsystems] Discovering subsystems at path UnitySubsystems
Renderer: WebKit WebGL
Vendor:   WebKit
Version:  OpenGL ES 3.0 (WebGL 2.0 (OpenGL ES 3.0 Chromium))
GLES:     3
OPENGL LOG: Creating OpenGL ES 3.0 graphics device ; Context level  <OpenGL ES 3.0> ; Context handle 1
[Physics::Module] Selected backend.
[Physics::Module] Name: PhysX
[Physics::Module] Id: 0xf2b8ea05
[Physics::Module] SDK Version: 4.1.2
[Physics::Module] Integration Version: 1.0.0
[Physics::Module] Threading Mode: Single-Threaded
Input System initialize...
UnloadTime: 1.900000 ms
[SG] quality Low lite=False gpu=ANGLE (Intel, Intel(R) UHD Graphics 630 (0x00009BC8) Direct3D11 vs_5_0 ps_5_0, D3D11)
[SG] quality apply Low screen 562x272 renderScale 0.75 msaa 1
Not allowed to access triangles/indices on mesh 'L0' (isReadable is false; Read/Write must be enabled in import settings)
Not allowed to access normals on mesh 'L0' (isReadable is false; Read/Write must be enabled in import settings)
```

---

## 4. Metadata & Game Configuration Payload

From `https://games.crazygames.com/en_US/god-mode-city-destroyer/index.html`:
```json
{
  "gameName": "God Mode City Destroyer",
  "gameId": "150162",
  "buildId": "99cdd0b3-7b0a-48ae-9dcc-ea086686713e",
  "loader": "unity6",
  "installVersion": "17",
  "releaseDate": "2026-11-08",
  "category": "Destroy",
  "tags": ["3d", "destroy", "hero", "war", "police"],
  "totalSizeBytes": 28791544,
  "initialLoadSizeBytes": 28772047,
  "controls": {
    "Mouse": "aim/look",
    "W": "fly where you aim (A/D strafe, S back)",
    "Space": "fly up",
    "Shift": "boost (boost into the ground = SLAM)",
    "Left/Right Click": "heat vision",
    "E": "grab (lifts over head) - LMB throws, RMB crushes",
    "Q": "gravity tether - hold to drag, click to hurl",
    "P": "pause",
    "T": "skip tutorial"
  }
}
```
