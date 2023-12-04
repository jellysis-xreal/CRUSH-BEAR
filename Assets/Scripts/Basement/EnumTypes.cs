/* [ Enum Types ]
* Global하게 사용되어야 하는 공통 데이터 타입 정의함
*/


namespace EnumTypes
{

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
    
}
