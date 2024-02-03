/* [ Enum Types ]
* Global하게 사용되어야 하는 공통 데이터 타입 정의함
*/
namespace EnumTypes
{
    public enum GameState
    {
        Lobby,
        Waving,
        Ending
    }
    
    
    public enum InteractionType
    {
        Idle,
        Break,
        Tear,
        Scratch,
        Avoid
    }

    public enum MoveType
    {
        Walk,
        Throw,
        Shot,
        Steam,
        Press
    }

    public enum WaveType
    {
        Shooting,
        Punching,
        Hitting
    }
    
    // XR origin의 두 controller 하위에 붙은
    // Hitter의 하위 자식과 동일하게 naming!
    public enum InteractionSide
    {
        Red, 
        Green
    }
    public enum Controller
    {
        LeftController,
        RightController
    }
    public enum Motion
    {
        LeftZap, RightZap, LeftHook, RightHook, LeftUpperCut, RightUpperCut, None
    }
}
