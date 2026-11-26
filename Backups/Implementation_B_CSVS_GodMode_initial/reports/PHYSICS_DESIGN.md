# Physics Architecture & Destruction Design Blueprint

## 1. Physics Engine Setup & WebGL Constraints

### 1.1 Fixed Update & Solvers
- **Physics Backend**: PhysX 4.1.2 single-threaded.
- **Fixed Timestep**: `0.02s` (50 Hz). A 50 Hz physics step balances accurate trajectory integration with the single-core CPU budget of browser WebAssembly.
- **Solver Iterations**:
  - `Default Solver Iterations`: 6
  - `Default Solver Velocity Iterations`: 2
  - `Sleep Threshold`: `0.005` (Ensures settled debris quickly enters sleep state to avoid wasting CPU cycles).

---

## 2. Destructible Building System

### 2.1 Pre-Fractured Mesh Clustering (Voronoi)
Real-time boolean mesh slicing is computationally prohibitive in single-threaded WebGL. The recommended and industry-standard architecture uses pre-fractured Voronoi meshes:
1. **Intact Phase**:
   - The building is rendered as a single static mesh (or instanced floor sections) with a single box collider.
   - Zero active Rigidbody overhead.
2. **Fracture Trigger**:
   - When cumulative damage exceeds a threshold (e.g., from Ground Slam, Laser, or Hurled Object), the intact model is swapped for pre-fractured pieces.
3. **Hierarchy & Chunking**:
   - Skyscraper structures are partitioned into horizontal slices / floor chunks.
   - Breaching the foundation or lower 30% cuts support joints for upper chunks, causing gravitational collapse.

```
+--------------------------------------------------+
|               Intact Skyscraper                  |
|  - Static Box Collider (0 PhysX bodies)          |
|  - Health = 1000                                 |
+--------------------------------------------------+
                        |
              [Heavy Impact / Cut]
                        v
+--------------------------------------------------+
|           Segmented Fracture Slices              |
|  - Floor 3-10: Upper rigid compound body         |
|  - Floor 1-2: Shattered Voronoi chunks           |
|  - Active Rigidbodies with Convex MeshColliders  |
+--------------------------------------------------+
                        |
           [Chain Reaction Propagation]
                        v
+--------------------------------------------------+
|             Settled Debris & Dust                |
|  - Particle FX Volume (Dust & Fire)              |
|  - Sleep state -> Converted to static / pooled   |
+--------------------------------------------------+
```

### 2.2 Impulse & Force Transfer Equations
- **Kinetic Impact**:
  $$F_{\text{impact}} = m_{\text{projectile}} \cdot \frac{\Delta v}{\Delta t}$$
- **Radial Shockwave (Ground Slam)**:
  $$F_{\text{radial}}(r) = F_{\text{max}} \cdot \left(1 - \frac{r}{R_{\text{max}}}\right)^2 \quad \text{for } r \le R_{\text{max}}$$
  A quadratic falloff ensures violent destruction at the epicenter that rapidly diminishes near the perimeter.

---

## 3. Superpower Physics Mechanics

### 3.1 Flight Kinematics
- **Drag & Velocity Clamping**:
  $$v_{t+1} = v_t + (a_{\text{thrust}} - k_{\text{drag}} \cdot v_t) \cdot \Delta t$$
- **Terminal Velocities**:
  - Normal Flight: $35 \text{ m/s}$ (~126 km/h)
  - Supersonic Boost: $90 \text{ m/s}$ (~324 km/h)

### 3.2 Grab & Throw Physics
- **Attachment**: The grabbed vehicle or object is attached via a `ConfigurableJoint` or soft damped spring force:
  $$F_{\text{hold}} = -k_p (x_{\text{obj}} - x_{\text{target}}) - k_d (v_{\text{obj}} - v_{\text{target}})$$
- **Throw**: Joint is broken, and an instantaneous directional impulse is imparted:
  $$\vec{P}_{\text{throw}} = \vec{v}_{\text{aim}} \cdot M_{\text{throw}}$$

### 3.3 Gravity Tether Physics
- A persistent attractive spring force pulls the object toward the crosshair raycast anchor:
  $$\vec{F}_{\text{tether}} = k_{\text{tether}} \cdot (\vec{p}_{\text{ray}} - \vec{p}_{\text{obj}})$$
- High angular damping prevents erratic tumbling during mid-air tethering.

---

## 4. Debris Optimization & Performance Guardrails

1. **Active Rigid Body Budget**: Maximum **150–200 active dynamic bodies** simultaneously on screen.
2. **Debris Pool**: Small dust and concrete chunks use non-colliding GPU instanced particles rather than true PhysX bodies.
3. **Early Sleep**: Once a debris chunk's velocity falls below $0.1 \text{ m/s}$ for $>1.0\text{s}$, its `Rigidbody.isKinematic` is set to `true`, and its collider is moved to an inert static layer.
4. **Despawn Timer**: Debris chunks submerge slightly beneath the road surface or scale down to zero over 10–15 seconds before returning to the pool.
