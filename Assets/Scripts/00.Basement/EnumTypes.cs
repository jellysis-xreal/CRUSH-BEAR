/* [ Enum Types ]
* Global하게 사용되어야 하는 공통 데이터 타입 정의함
*/
namespace EnumTypes
{
    public enum GameState
    {
        Lobby,
        Waving,
        Ending,
        Tutorial
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

    public enum scoreType
    {
        Perfect,    // 정확하게 충돌+속도 60% 이상 = 150점
        Good,       // 정확한 방식+속도 20% 이상 = 100점
        Weak,       // 정확한 방식+속도 20% 미만 = 20점, 목숨 유지
        Bad,        // 부정확한 방식 = 0점, 목숨 1개 감소
        Miss        // 피격을 하지 않은 경우 = 0점, 목숨 1개 감소
    }
    
    public enum WaveType
    {
        Shooting,
        Punching,
        Hitting
    }
    public enum TutorialPunchType
    {
        Zap,
        Hook,
        UpperCut,
        Clear
    }

    public enum TutorialTennisType
    {
        LeftHand,
        RightHand,
        Clear
    }
    // XR origin의 두 controller 하위에 붙은
    // Hitter의 하위 자식과 동일하게 naming!
    public enum InteractionSide
    {
        Red, 
        Blue
    }
    public enum Controller
    {
        LeftController,
        RightController
    }
    public enum Motion
    {
        LeftZap, RightZap, LeftHook, RightHook, LeftUpperCut, RightUpperCut, LeftLowerCut, RightLowerCut, None
    }
}
