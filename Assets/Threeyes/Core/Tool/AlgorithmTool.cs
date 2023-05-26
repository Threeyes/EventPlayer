using System;
using System.Collections;
using UnityEngine.Events;

public static class AlgorithmTool
{
    /// <summary>
    /// Recursive execute function
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="element"></param>
    /// <param name="getEnum"></param>
    /// <param name="action"></param>
    /// <param name="includeSelf"></param>
    /// <param name="maxDepth">null for endless seek&invoke (Warning: will get StackOverFlow if the seek never end! (eg: Nested self Classes)</param>
    public static void Recursive<T>(this T element, Func<T, IEnumerable> getEnum, UnityAction<T> action, bool includeSelf = true, int? maxDepth = null)
    {
        if (includeSelf)
            action.Execute(element);

        maxDepth--;
        if (maxDepth.HasValue && maxDepth == -1)
            return;

        if (getEnum == null)
            return;
        foreach (T subElement in getEnum.Invoke(element))
        {
            Recursive(subElement, getEnum, action, maxDepth: maxDepth.HasValue ? --maxDepth : null);//The child circle will be includeSelf=true so it will get set via action
        }
    }
}
