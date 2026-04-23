SKILL: Unity Architecture

Guidelines:

Separate systems:
- GameController
- StatSystem
- InputManager
- CardManager


Gameplay architecture :

InputManager
     ↓
DecisionSystem
     ↓
StatSystem
     ↓
GameController