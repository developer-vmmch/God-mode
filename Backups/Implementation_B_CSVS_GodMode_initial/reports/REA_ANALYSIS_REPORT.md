# REA v6.10.1 Analysis Report: God Mode City Destroyer

## 1. Executive Summary
This report documents the Phase 1 & Phase 2 reverse engineering and architecture investigation of the reference WebGL title **God Mode City Destroyer** (`https://www.crazygames.com/game/god-mode-city-destroyer`), conducted using the Reverse Engineer Anything (REA) framework v6.10.1 on Windows.

All observations were acquired passively via the Chrome DevTools Protocol (CDP), local browser session monitoring, static bundle graph analysis, and network HAR evaluation. In accordance with project authority guidelines, no proprietary assets or bytecodes were extracted or copied, and platform security boundaries were strictly respected.

---

## 2. Environment & Instrumentation Setup
- **REA Host Runtime**: Node.js v24.22.3, REA v6.10.1 (`rea-agents`)
- **Protocol & Adapter**: Production stdio adapter, MCP protocol version 2026-07-28
- **Browser Harness**: Google Chrome 158.0.8127.88 (`chrome.exe`) running with loopback remote debugging on port 9222
- **CDP Endpoint**: `http://127.0.0.1:9222`
- **Active REA MCP Tools Executed**:
  1. `list_browser_targets` — Discovered active page targets and metadata
  2. `inspect_web_page` — Captured DOM hierarchy, accessibility, and console telemetry
  3. `analyze_web_bundle` — Reconstructed static chunk graph and script origin inventory
  4. `observe_web_session` — Tracked navigation and session lifecycle timeline
  5. `inspect_web_network_capture` — Validated HAR 1.2 network ingestion and payload sizes

---

## 3. Browser Investigation (Phase 1 Findings)

### 3.1 Host & Frame Architecture
- **Primary Hosting Origin**: `https://www.crazygames.com`
- **Game Container Frame**: `https://games.crazygames.com/en_US/god-mode-city-destroyer/index.html`
- **Execution Iframe**: `https://god-mode-city-destroyer.game-files.crazygames.com/unity/unity2020/god-mode-city-destroyer.html`
- **Rendering Canvas**: `#unity-canvas` (`<canvas id="unity-canvas" style="position:fixed;top:0;left:0;width:100%;height:100%;outline:none"></canvas>`)
- **Canvas Scaling**: Full viewport fixed layout, dynamic resize handled via window resize events and Unity canvas buffer matching.

### 3.2 Asset Host & Delivery Pipeline
- **CDN Base URL**: `https://files.crazygames.com/god-mode-city-destroyer/17/`
- **Asset Packaging Format**: Brotli-compressed WebGL binary artifacts (`.br` extension).
- **Core Shipped Artifacts**:
  - `WebGL.loader.js` (26,455 bytes) — Unity 6 loader orchestrator
  - `WebGL.framework.js.br` (894,102 bytes compressed) — Emscripten / WebGL runtime framework
  - `WebGL.wasm.br` (18,204,910 bytes compressed) — Compiled Unity engine and game logic WebAssembly module
  - `WebGL.data.br` (9,660,520 bytes compressed) — Serialized asset pack and scene data
  - `StreamingAssets/` — Remote streaming asset directory

---

## 4. Game Architecture Verification (Phase 2 Findings)

### 4.1 Engine Identification & Build Version
From the captured initialization logs emitted by `WebGL.framework.js.br`:
- **Engine Version**: **Unity 6 (6000.0.84f1)** (`commit hash: 78ab6fc243d5`)
- **Build ID**: `99cdd0b3-7b0a-48ae-9dcc-ea086686713e`
- **Install Version**: `17`
- **Total Published Footprint**: 28,791,544 bytes (~27.4 MB)

### 4.2 Graphics & Render Pipeline
- **Graphics API**: WebGL 2.0 / OpenGL ES 3.0 (Chromium ANGLE D3D11 backend on Windows)
- **Render Pipeline**: **Universal Render Pipeline (URP)**
  - Confirmed via shader binding diagnostics:
    - `Hidden/TerrainEngine/Details/UniversalPipeline/BillboardWavingDoublePass`
    - `Hidden/Universal/HDRDebugView`
    - `Hidden/CoreSRP/CoreCopy`
- **Dynamic Quality & Resolution Scaling**:
  - Automatically profiles GPU via WebGL context (`ANGLE (Intel, Intel(R) UHD Graphics 630 Direct3D11)`).
  - Emits: `[SG] quality Low lite=False screen 562x272 renderScale 0.75 msaa 1`.
  - Confirms runtime implementation of URP render scale reduction and disabled MSAA on low-tier graphics devices.

### 4.3 Physics Architecture
- **Physics Backend**: **NVIDIA PhysX**
  - PhysX Backend Name: `PhysX`
  - PhysX Backend ID: `0xf2b8ea05`
  - PhysX SDK Version: `4.1.2`
  - Integration Version: `1.0.0`
  - Threading Mode: **Single-Threaded** (standard WebAssembly constraint)
- **Fallback Backend**: Initialized at `0xdecafbad` prior to PhysX selection.

### 4.4 Input System
- **Module**: Unity New Input System (`com.unity.inputsystem`)
- **Initialization Log**: `Input System module state changed to: Initialized.` followed by `Input System initialize...`.

### 4.5 Memory Allocators & Boot Configuration
The engine initializes specialized native memory blocks (configured via `boot.config` equivalents):
- `main-allocator-block-size`: 16 MB
- `thread-allocator-block-size`: 16 MB
- `gfx-main-allocator-block-size`: 16 MB
- `gfx-thread-allocator-block-size`: 16 MB
- `temp-allocator-size-main`: 4 MB
- `job-temp-allocator-block-size`: 2 MB
- `cache-allocator-block-size`: 4 MB
- `typetree-allocator-block-size`: 2 MB

### 4.6 Audio Lifecycle
- Web Audio `AudioContext` is gated by the browser's user-gesture policy.
- An iframe controller (`window.audioController`) intercepts gain node output for platform audio muting/unmuting during ads and tab switching.

---

## 5. Summary of Deliverables & Next Steps
The reference target has been completely mapped at the browser, loader, and engine architecture layers. The subsequent reports detail:
1. `GAME_ARCHITECTURE.md`: In-depth systems and subsystem architecture.
2. `EVIDENCE_INDEX.md`: Evidence provenance, digests, and raw log records.
3. `GAMEPLAY_MECHANICS.md`: Character controller, flight, heat vision, super strength, and chaos loop.
4. `PHYSICS_DESIGN.md`: Structural destruction, impulse propagation, and ragdoll/debris optimization.
5. `MISSING_DEPENDENCIES.md`: Status of offline disassemblers (WABT, IL2CPP) and environment recovery.
6. `DEVELOPMENT_ROADMAP.md`: Phased execution plan for building an original, clean-room Unity project.
