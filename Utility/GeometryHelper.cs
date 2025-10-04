using System.Numerics;
using System.Windows;

namespace Anime_Stiker.Utility;

public static class GeometryHelper
{
    // Calculate aBc angle
    public static double CalculateAngle(Point A, Point B, Point C)
    {
        var vectorBA = new Vector2((float)(A.X - B.X), (float)(A.Y - B.Y));
        var vectorBC = new Vector2((float)(C.X - B.X), (float)(C.Y - B.Y));

        // 1. Get the angle of each vector relative to the positive X-axis using Atan2.
        // Math.Atan2(y, x) returns angle in radians, from -PI to +PI.
        double angleBA = Math.Atan2(vectorBA.Y, vectorBA.X);
        double angleBC = Math.Atan2(vectorBC.Y, vectorBC.X);

        // 2. Calculate the difference (angle from BA to BC)
        double angleDiff = angleBA - angleBC;

        // 3. Normalize the result to the range (-PI, PI] to get the shortest signed angle
        if (angleDiff > Math.PI)
        {
            angleDiff -= 2 * Math.PI;
        }
        else if (angleDiff <= -Math.PI)
        {
            angleDiff += 2 * Math.PI;
        }

        // 4. Convert radians to degrees
        return angleDiff * (180.0 / Math.PI);
    }
}
