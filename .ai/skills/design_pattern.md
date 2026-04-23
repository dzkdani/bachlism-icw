## Core Patterns Approaches
- Event-driven systems using event buses
- ScriptableObject data 
- state machines
- object pooling

## Avoid:
- logic inside Update
- heavy FindObjectOfType
- tight coupling between systems (e.g., GameManager accessing all other systems directly)
- monolithic classes that handle multiple responsibilities (e.g., a single class managing both game state and UI)
- hardcoded values instead of using ScriptableObjects or configuration files

## Prefer:
- event-driven systems using event buses to decouple systems and improve maintainability
- ScriptableObject data to store configuration and game data, allowing for easier tweaking and separation of
  data from logic
- state machines to manage complex game states and transitions in a clear and organized way
- object pooling to optimize performance by reusing objects instead of instantiating and destroying them frequently
- modular design with clear separation of concerns, where each system has a specific responsibility and interacts with others through well-defined interfaces or events

## Utils / Extention
- typewriter for text display
- tweening for smooth transitions and animations
- generic event bus implementation for decoupled communication between systems
- generic object pool implementation for efficient reuse of game objects