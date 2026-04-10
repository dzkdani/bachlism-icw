SKILL: Unity Architecture

Guidelines:

Separate systems:
GameManager
StatManager
InputManager
DeckManager

Avoid:
- logic inside Update
- heavy FindObjectOfType

Prefer:
- event-driven systems
- ScriptableObject data
- object pooling

Gameplay architecture example:

InputManager
     ↓
DecisionSystem
     ↓
StatManager
     ↓
GameManager