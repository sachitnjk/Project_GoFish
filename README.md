## Controls

- **Mouse** — Navigate menus  
- **Space** — Cast line / start fishing  
- **Space (after casting)** — Cast again to refresh the cast
- **Space (after state changes to *"Bite"* )** — Trigger the reeling in 

---

## Architecture & Design Decisions

- **Main Menu UI**  
  Implemented using a *pushdown automata* pattern to manage layered UI states cleanly.

- **Fishing System**  
  Built on a *state machine* to handle transitions such as casting, waiting, bite, and reeling.

- **Fish Data Design**  
  Combines:
  - Static data from **Scriptable Objects**
  - Runtime-generated attributes derived from that data  

  Additional behavior:
  - Fish rarity is randomized  
  - Rarity directly influences **behavior difficulty** (implemented via 3 logic states)

- **Water Shader**  
  - Ripple effect reused from a previous project  
  - Shader normals sourced from reference images  

- **Audio System**  
  Managed using Unity’s **Audio Mixer** with three channels:
  - Master  
  - BGM  
  - SFX  

- **Fishing Mechanic Inspiration**  
  The reeling system is inspired by *Stardew Valley*, adapted to fit this project’s design.

---

## Assets & Credits

- **Fish visuals**: itch.io  
- **Audio**: Pixabay  
- **Water shader**: Reused from a previous project  
- **Water shader normals**: Google Images  

---

## Notes

- No generative AI or any other form of AI was used in the creation of this project.

---

## Future Improvements / Wishlist

Things that were planned but considered too time-consuming or out of scope for the current version:

- Adding visible fish in the water  
  - Would use a flocking system based on **Boids** logic  

- Adding UI animations  

- Creating a fully animated main menu  

- Implementing a more realistic casting arc  
  - Using a **Line Renderer** to simulate fishing line physics  

- Improving sound design with higher quality clips  

- Adding progression systems  
  - Bait upgrades  
  - Rod upgrades  
  - Shop system  
