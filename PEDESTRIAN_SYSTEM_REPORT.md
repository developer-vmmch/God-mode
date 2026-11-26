# PEDESTRIAN SYSTEM REPORT — LIVING STREET LIFE V0.2.2

## 1. Overview & Cultural Authenticity

The civilian pedestrian system (`KaraikalPedestrianSystem.cs`) injects living South Indian cultural atmosphere into the streets, bazaars, and beach promenade of Karaikal.

---

## 2. Demographic & Clothing Archetypes

| Attire Category | Visual Appearance | Color Variations | Cultural Context |
| :--- | :--- | :--- | :--- |
| **Traditional Veshti / Lungi** | White/cream cotton dhoti wrapped at waist with gold border (*Zari*); matching half-sleeve shirt. | Off-white, ivory, cream with maroon/gold borders. | Common elder and shopkeeper attire across Tamil Nadu and Puducherry. |
| **Traditional Saree** | Six-yard draped garment with pleated pallu across the shoulder. | Royal blue, deep crimson, peacock green, saffron, turmeric yellow. | Traditional women's attire worn on temple streets and markets. |
| **Casual Contemporary** | Tucked cotton collared shirt with formal trousers/chinos. | Light blue, khaki, grey, striped patterns. | Working professionals, youth, and office commuters. |
| **Auto-Rickshaw Driver** | Khaki uniform shirt and trousers with utility pockets. | Standard regulation Indian auto-rickshaw driver khaki. | Drivers resting near rickshaw stands or tea stalls. |

---

## 3. Sidewalk Navigation & AI Behavior

1. **Sidewalk Lane Following**: Pedestrians spawn along roadside walkways and pedestrian corridors, adhering to forward-moving flow vectors.
2. **Obstacle & Traffic Avoidance**: Forward raycasts ($2.0\text{ m}$ range) detect incoming vehicles and superhero ground impacts, triggering dynamic sidestep evasion.
3. **Panic / Reactivity States**:
   - *Normal Walking*: $1.2\text{ m/s}$ walking speed with rhythmic arm/leg oscillation.
   - *Nearby Destruction / Explosion*: Transitions into running flee mode ($4.5\text{ m/s}$), hands gesturing in panic, scattering away from ground zero.

---

## 4. Performance & Object Pooling

- **Active Pool Size**: 60 active pedestrian agents dynamically allocated within a $120\text{ m}$ player bubble.
- **Despawn & Recirculation**: Pedestrians exiting the camera frustum or player bubble are recycled with randomized clothing presets and repositioned ahead of the player.
- **CPU Budget**: Under $0.4\text{ ms}$ per frame on an Intel Core i3 quad-core CPU.
