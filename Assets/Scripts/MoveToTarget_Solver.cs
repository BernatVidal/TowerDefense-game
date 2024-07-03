
using UnityEngine;

/// <summary>
/// Class used for any object that needs to move to a specific position.
/// It will move the transform passed, and rotate it if desired.
/// Returns if Target is Reached or not.
/// </summary>
public static class MoveToTarget_Solver
{
    const float turnVelocity = 5;
    const float targetOffset = 0.2f;

    /// <summary>
    /// Moves to target at desired velocity, and returns true if target is reached
    /// </summary>
    public static bool MoveToTarget(Transform currentTransform, Vector3 targetPos, float velocity, bool aimToTarget = false)
    {
        currentTransform.position = Vector3.MoveTowards(currentTransform.position, targetPos, Time.deltaTime * velocity);

        if (aimToTarget)
        {
            if (!currentTransform.position.Equals(targetPos))
                currentTransform.rotation = Quaternion.Slerp(currentTransform.rotation, Quaternion.LookRotation(targetPos - currentTransform.position), Time.deltaTime * velocity * turnVelocity);
        }

        return TargetReached(currentTransform.position, targetPos);
    }

    /// <summary>
    /// Tracks if next point on the path is reached
    /// </summary>
    static bool TargetReached(Vector3 currentPos, Vector3 targetPos)
    {
        return Vector3.Distance(targetPos, currentPos) < targetOffset;
    }
}
