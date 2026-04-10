SKILL: Software Design Patterns

Use these patterns when appropriate:

Singleton
Used for global managers in Unity.
Example: GameManager, AudioManager.

Observer
Used for event-driven architecture.
Example: UI listening to StatManager changes.

Strategy
Used to swap behavior dynamically.
Example: different AI behaviors.

Factory
Used to create objects without exposing creation logic.
Example: enemy spawning systems.

Rules:
Prefer composition over inheritance.
Avoid excessive singletons.
Use dependency injection when possible.