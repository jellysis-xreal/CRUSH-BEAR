using UnityEngine;

public static class Utils
{
    /// <summary>
    /// aParent의 자식 개체에서 aName을 가진 transform을 반환합니다.
    /// </summary>
    /// <param name="aParent"></param>
    /// <param name="aName"></param>
    /// <returns></returns>
    public static Transform FindChildByRecursion(this Transform aParent, string aName)
    {
        if (aParent == null) return null;
        var result = aParent.Find(aName);
        if (result != null)
            return result;
        foreach (Transform child in aParent)
        {
            result = child.FindChildByRecursion(aName);
            if (result != null)
                return result;
        }

        return null;
    }
}
