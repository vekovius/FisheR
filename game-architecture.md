# Fishr Game Architecture

Below is the UML class diagram for our fishing game:

```mermaid
classDiagram
    class StateInterface {
        <<interface>>
        +Enter()
        +Update()
        +Exit()
    }
    
    class StateController {
        -StateInterface currentState
        +GameObject inventoryPanel
        +GameObject mapPanel
        +GameObject settingsPanel
        +KeyCode inventoryKey
        +KeyCode mapKey
        +KeyCode settingsKey
        +float castSpeed
        +float maxCastSpeed
        +GameObject lurePrefab
        +Transform castOrigin
        +DirectionIndicator directionIndicator
        +KeyCode castKey
        +CastingMinigame castingMinigame
        +CameraController cameraController
        +Start()
        +ChangeState(StateInterface)
        +Update()
    }
    
    class PassiveState {
        -GameObject inventoryPanel
        -GameObject mapPanel
        -GameObject settingsPanel
        -GameObject playerObject
        -GameObject directionIndicator
        +KeyCode inventoryKey
        +KeyCode mapKey
        +KeyCode settingsKey
        +Enter()
        +Update()
        +Exit()
        -TogglePanel(GameObject)
    }
    
    class CastState {
        -float castSpeed
        -float maxCastSpeed
        -GameObject lurePrefab
        -Transform castOrigin
        -DirectionIndicator directionIndicator
        -GameObject currentLure
        -CastingMinigame castingMinigame
        +Enter()
        +Update()
        +Exit()
        -CastLure(Vector2)
    }
    
    class InAirState {
        +Enter()
        +Update()
        +Exit()
    }
    
    class CastingMinigame {
        +GameObject powerBarGameObject
        +Image PowerBarMask
        +float barChangeSpeed
        -float maxPowerBarValue
        +float currentPowerBarValue
        -bool powerIsIncreasing
        -bool PowerBarOn
        +OnEnable()
        +getCurrentPowerBarValue()
        +IEnumerator UpdatePowerBar()
    }
    
    class DirectionIndicator {
        +LineRenderer lineRenderer
        +float lineLength
        +int lineSegmentCount
        +float width
        +float angle
        +float angleChangeSpeed
        +Transform playerTransform
        +KeyCode increaseAnglekey
        +KeyCode decreaseAngleKey
        +KeyCode castKey
        -Vector2 currentDirection
        +Start()
        +Update()
        -UpdateDirection()
        -UpdateLine()
        +getCurrentDirection()
    }
    
    class CameraController {
        +Transform target
        +Vector3 offset
        +float smoothSpeed
        -LateUpdate()
    }
    
    class FishManager {
        +FishSpawner fishSpawner
        +Start()
    }
    
    class FishSpawner {
        +List~FishType~ fishTypes
        +float spawnFromCenterRad
        +List~FishAI~ SpawnSchool(FishType, SpawnRegion)
        +SpawnRegion GetSpawnRegionForSpecies(string)
    }
    
    class SpawnRegion {
        +Vector2 size
        +string speciesID
        +Color gizomoColor
        -OnDrawGizmosSelected()
        +Bounds GetBounds()
    }
    
    class FishType {
        +string typeName
        +GameObject prefab
        +string speciesID
        +float spawnWeight
        +SpawnRegion spawnRegion
        +float spawnDepth
        +int schoolSizeMin
        +int schoolSizeMax
        +float maxSpeed
        +float maxForce
        +float neighborRadius
        +float separationDistance
        +float alignmentWeight
        +float cohesionWeight
        +float separationWeight
        +float wanderWeight
        +float homeAttractionWeight
    }
    
    class FishAI {
        +FishType fishType
        +Transform currentLure
        +Vector2 velocity
        +Vector2 homePosition
        +float lureAttractionRadius
        -float maxSpeed
        -float neighborRadius
        -float separationDistance
        -float alignmentWeight
        -float cohesionWeight
        -float separationWeight
        -float wanderWeight
        -float homeAttractionWeight
        -Rigidbody2D rb
        -Start()
        -FixedUpdate()
        -Vector2 Flock()
        -Vector2 Wander()
        -Vector2 HomeAttraction()
        -Vector2 lureAttraction()
        -OnDrawGizmosSelected()
    }
    
    StateInterface <|.. PassiveState
    StateInterface <|.. CastState
    StateInterface <|.. InAirState
    StateController *-- PassiveState
    StateController *-- CastState
    StateController *-- InAirState
    StateController o-- DirectionIndicator
    StateController o-- CastingMinigame
    StateController o-- CameraController
    CastState o-- DirectionIndicator
    CastState o-- CastingMinigame
    CastState o-- CameraController
    FishManager o-- FishSpawner
    FishSpawner o-- FishType
    FishSpawner o-- SpawnRegion
    FishType o-- SpawnRegion
    FishAI o-- FishType
