// Only define one of the 3 approaches for triangle check. Default to dot product approach is none is defined
#define USE_BARYCENTRIC_COORDINATE_SYSTEM
//#define USE_PARAMETRIC_EQUATIONS_SYSTEM
//#define USE_DOT_PRODUCT

using UnityEngine;
using Unity.Mathematics;

namespace StrengthInNumber
{
    /// <summary>
    /// Utilities regarding checking point and triangle properties.
    /// Note #1: Main functions such as IsInCircumcircle and IsInTriangle only take 2 dimensional points
    /// Note #2: Need to ensure that no two points are too close to prevent degenerate cases
    /// </summary>
    public static class TriangleUtils
    {
        /// <summary>
        /// Returns determinant to determine whether a point on either half plane or on the line.
        /// Positive means same side of the normal (right / up), negative means opposite (left / down) and 0 means on the same line
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static float HalfPlaneCheck(Vector2 pt, Vector2 v1, Vector2 v2)
        {
            return (v2.x - v1.x) * (pt.y - v1.y) - (v2.y - v1.y) * (pt.x - v1.x);
        }

        /// <summary>
        /// Returns determinant to determine whether a point on either half plane or on the line.
        /// Positive means same side of the normal (right / up), negative means opposite (left / down) and 0 means on the same line
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static float HalfPlaneCheck(float2 pt, float2 v1, float2 v2)
        {
            return math.determinant(new float2x2(v2 - v1, pt - v1));
        }

        /// <summary>
        /// Use cross product to determine normal of the plane and then calculate dot product between the normal and vector from v1 to pt.
        /// Return the dot product.
        /// Positive means same side of the normal (right / up), negative means opposite (left / down) and 0 means on the same plane
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static float HalfPlaneCheck(Vector3 pt, Vector3 v1, Vector3 v2, Vector3 v3)
        {
            Vector3 cross = Vector3.Cross(v2 - v1, v3 - v1);
            float dot = Vector3.Dot(cross, pt - v1);
            return dot;
        }
        /// <summary>
        /// Use cross product to determine normal of the plane and then calculate dot product between the normal and vector from v1 to pt.
        /// Return the dot product.
        /// Positive means same side of the normal (right / up), negative means opposite (left / down) and 0 means on the same plane
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static float HalfPlaneCheck(float3 pt, float3 v1, float3 v2, float3 v3)
        {
            float3 cross = math.cross(v2 - v1, v3 - v1);
            float dot = math.dot(cross, pt - v1);
            return dot;
        }

        /// <summary>
        /// Check if 3 points are collinear by checking if cross product length is 0
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static bool IsCollinear(Vector2 v1, Vector2 v2, Vector2 v3)
        {
            return (v2.x - v1.x) * (v3.y - v1.y) - (v2.y - v1.y) * (v3.x - v1.x) == 0f;
        }

        /// <summary>
        /// Check if 3 points are collinear by checking if cross product length is 0
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static bool IsCollinear(float2 v1, float2 v2, float2 v3)
        {
            return (v2.x - v1.x) * (v3.y - v1.y) - (v2.y - v1.y) * (v3.x - v1.x) == 0f;
        }

        /// <summary>
        /// Check if 3 points are collinear by checking if cross product length is 0
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static bool IsCollinear(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            return Vector3.Cross(v2 - v1, v3 - v1).sqrMagnitude == 0f;
        }

        /// <summary>
        /// Check if 3 points are collinear by checking if cross product length is 0
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static bool IsCollinear(float3 v1, float3 v2, float3 v3)
        {
            return math.lengthsq(math.cross(v2 - v1, v3 - v1)) == 0f;
        }

        // Reference from http://totologic.blogspot.com/2014/01/accurate-point-in-triangle-test.html
        public static float EPSILON = 0.001f;
        public static float EPSILON_SQUARE = EPSILON * EPSILON;
        /// <summary>
        /// Check cases where point is outside bounding box, check if point is in triangle, handle edge cases where point is too close to edges
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static bool IsInsideTriangle(Vector2 pt, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            if (!IsInBoundingBox(pt, v1, v2, v3))
            {
                return false;
            }

            if (IsInsideTriangleNaive(pt, v1, v2, v3))
            {
                return true;
            }

            if (DistanceSquarePointToSegment(pt, v1, v2) <= EPSILON_SQUARE)
            {
                return true;
            }
            if (DistanceSquarePointToSegment(pt, v2, v3) <= EPSILON_SQUARE)
            {
                return true;
            }
            if (DistanceSquarePointToSegment(pt, v3, v1) <= EPSILON_SQUARE)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check cases where point is outside bounding box, check if point is in triangle, handle edge cases where point is too close to edges
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static bool IsInsideTriangle(float2 pt, float2 v1, float2 v2, float2 v3)
        {
            if (!IsInBoundingBox(pt, v1, v2, v3))
            {
                return false;
            }

            if (IsInsideTriangleNaive(pt, v1, v2, v3))
            {
                return true;
            }

            if (DistanceSquarePointToSegment(pt, v1, v2) <= EPSILON_SQUARE)
            {
                return true;
            }
            if (DistanceSquarePointToSegment(pt, v2, v3) <= EPSILON_SQUARE)
            {
                return true;
            }
            if (DistanceSquarePointToSegment(pt, v3, v1) <= EPSILON_SQUARE)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if a point is inside a triangle without handling edge cases where point is too close to edges
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static bool IsInsideTriangleNaive(Vector2 pt, Vector2 v1, Vector2 v2, Vector2 v3)
        {
#if USE_BARYCENTRIC_COORDINATE_SYSTEM
            float denominator = (v2.y - v3.y) * (v1.x - v3.x) + (v3.x - v2.x) * (v1.y - v3.y);
            float a = ((v2.y - v3.y) * (pt.x - v3.x) + (v3.x - v2.x) * (pt.y - v3.y)) / denominator;
            float b = ((v3.y - v1.y) * (pt.x - v3.x) + (v1.x - v3.x) * (pt.y - v3.y)) / denominator;
            float c = 1f - a - b;
            return 0f <= a && a <= 1f && 0f <= b && b <= 1f && 0f <= c && c <= 1f;
#elif USE_PARAMETRIC_EQUATIONS_SYSTEM
            float denominator = (v1.x * (v2.y - v3.y) + v1.y * (v3.x - v2.x) + v2.x * v3.y - v2.y * v3.x);
            float t1 = (pt.x * (v3.y - v1.y) + pt.y * (v1.x - v3.x) - v1.x * v3.y + v1.y * v3.x) / denominator;
            float t2 = (pt.x * (v2.y - v1.y) + pt.y * (v1.x - v2.x) - v1.x * v2.y + v1.y * v2.x) / -denominator;
            float s = t1 + t2;
            return 0f <= t1 && t1 <= 1f && 0f <= t2 && t2 <= 1f && s <= 1f;
#elif USE_DOT_PRODUCT
            bool checkSide1 = HalfPlaneCheck(pt, v1, v2) <= 0f;
            bool checkSide2 = HalfPlaneCheck(pt, v2, v3) <= 0f;
            bool checkSide3 = HalfPlaneCheck(pt, v3, v1) <= 0f;
            return checkSide1 && checkSide2 && checkSide3;
#else
            bool checkSide1 = HalfPlaneCheck(pt, v1, v2) <= 0f;
            bool checkSide2 = HalfPlaneCheck(pt, v2, v3) <= 0f;
            bool checkSide3 = HalfPlaneCheck(pt, v3, v1) <= 0f;
            return checkSide1 && checkSide2 && checkSide3;
#endif
        }

        /// <summary>
        /// Check if a point is inside a triangle without handling edge cases where point is too close to edges
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static bool IsInsideTriangleNaive(float2 pt, float2 v1, float2 v2, float2 v3)
        {
#if USE_BARYCENTRIC_COORDINATE_SYSTEM
            float denominator = (v2.y - v3.y) * (v1.x - v3.x) + (v3.x - v2.x) * (v1.y - v3.y);
            float a = ((v2.y - v3.y) * (pt.x - v3.x) + (v3.x - v2.x) * (pt.y - v3.y)) / denominator;
            float b = ((v3.y - v1.y) * (pt.x - v3.x) + (v1.x - v3.x) * (pt.y - v3.y)) / denominator;
            float c = 1f - a - b;
            return 0f <= a && a <= 1f && 0f <= b && b <= 1f && 0f <= c && c <= 1f;
#elif USE_PARAMETRIC_EQUATIONS_SYSTEM
            float denominator = (v1.x * (v2.y - v3.y) + v1.y * (v3.x - v2.x) + v2.x * v3.y - v2.y * v3.x);
            float t1 = (pt.x * (v3.y - v1.y) + pt.y * (v1.x - v3.x) - v1.x * v3.y + v1.y * v3.x) / denominator;
            float t2 = (pt.x * (v2.y - v1.y) + pt.y * (v1.x - v2.x) - v1.x * v2.y + v1.y * v2.x) / -denominator;
            float s = t1 + t2;
            return 0f <= t1 && t1 <= 1f && 0f <= t2 && t2 <= 1f && s <= 1f;
#elif USE_DOT_PRODUCT
            bool checkSide1 = HalfPlaneCheck(pt, v1, v2) <= 0f;
            bool checkSide2 = HalfPlaneCheck(pt, v2, v3) <= 0f;
            bool checkSide3 = HalfPlaneCheck(pt, v3, v1) <= 0f;
            return checkSide1 && checkSide2 && checkSide3;
#else
            bool checkSide1 = HalfPlaneCheck(pt, v1, v2) <= 0f;
            bool checkSide2 = HalfPlaneCheck(pt, v2, v3) <= 0f;
            bool checkSide3 = HalfPlaneCheck(pt, v3, v1) <= 0f;
            return checkSide1 && checkSide2 && checkSide3;
#endif
        }

        /// <summary>
        /// Calculate the squared distance of a point to a segment (NOT infinite line).
        /// Note: The function does not handle degenerate cases where two points overlaps, resulting in segment with length 0
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static float DistanceSquarePointToSegment(Vector2 pt, Vector2 v1, Vector2 v2)
        {
            float segmentSquareLength = (v2.x - v1.x) * (v2.x - v1.x) + (v2.y - v1.y) * (v2.y - v1.y);
            float dotProduct = ((pt.x - v1.x) * (v2.x - v1.x)) + ((pt.y - v1.y) * (v2.y - v1.y)) / segmentSquareLength;
            if (dotProduct < 0f)
            {
                return (pt.x - v1.x) * (pt.x - v1.x) + (pt.y - v1.y) * (pt.y - v1.y);
            }
            else if (dotProduct <= 1f)
            {
                float pointToV1SquareLength = (pt.x - v1.x) * (pt.x - v1.x) + (pt.y - v1.y) * (pt.y - v1.y);
                return pointToV1SquareLength - dotProduct * dotProduct * segmentSquareLength;
            }
            else
            {
                return (pt.x - v2.x) * (pt.x - v2.x) + (pt.y - v2.y) * (pt.y - v2.y);
            }
        }

        /// <summary>
        /// Calculate the squared distance of a point to a segment (NOT infinite line).
        /// Note: The function does not handle degenerate cases where two points overlaps, resulting in segment with length 0
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static float DistanceSquarePointToSegment(float2 pt, float2 v1, float2 v2)
        {
            float segmentSquareLength = (v2.x - v1.x) * (v2.x - v1.x) + (v2.y - v1.y) * (v2.y - v1.y);
            float dotProduct = ((pt.x - v1.x) * (v2.x - v1.x)) + ((pt.y - v1.y) * (v2.y - v1.y)) / segmentSquareLength;
            if (dotProduct < 0f)
            {
                return (pt.x - v1.x) * (pt.x - v1.x) + (pt.y - v1.y) * (pt.y - v1.y);
            }
            else if (dotProduct <= 1f)
            {
                float pointToV1SquareLength = (pt.x - v1.x) * (pt.x - v1.x) + (pt.y - v1.y) * (pt.y - v1.y);
                return pointToV1SquareLength - dotProduct * dotProduct * segmentSquareLength;
            }
            else
            {
                return (pt.x - v2.x) * (pt.x - v2.x) + (pt.y - v2.y) * (pt.y - v2.y);
            }
        }

        /// <summary>
        /// Check if a point is in the 2D bounding box from 3 corners of the triangle
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static bool IsInBoundingBox(Vector2 pt, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            float xMin = Mathf.Min(v1.x, v2.x, v3.x);
            float xMax = Mathf.Max(v1.x, v2.x, v3.x);
            float yMin = Mathf.Min(v1.y, v2.y, v3.y);
            float yMax = Mathf.Max(v1.y, v2.y, v3.y);

            if (pt.x < xMin || pt.x > xMax || pt.y < yMin || pt.y > yMax)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check if a point is in the 2D bounding box from 3 corners of the triangle
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static bool IsInBoundingBox(float2 pt, float2 v1, float2 v2, float2 v3)
        {
            float xMin = math.min(v1.x, math.min(v2.x, v3.x));
            float xMax = math.max(v1.x, math.max(v2.x, v3.x));
            float yMin = math.min(v1.y, math.min(v2.y, v3.y));
            float yMax = math.max(v1.y, math.max(v2.y, v3.y));

            if (pt.x < xMin || pt.x > xMax || pt.y < yMin || pt.y > yMax)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check if a point is in the 3D bounding box from 3 corners of the triangle
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static bool IsInBoundingBox(Vector3 pt, Vector3 v1, Vector3 v2, Vector3 v3)
        {
            float xMin = Mathf.Min(v1.x, v2.x, v3.x);
            float xMax = Mathf.Max(v1.x, v2.x, v3.x);
            float yMin = Mathf.Min(v1.y, v2.y, v3.y);
            float yMax = Mathf.Max(v1.y, v2.y, v3.y);
            float zMin = Mathf.Min(v1.z, v2.z, v3.z);
            float zMax = Mathf.Max(v1.z, v2.z, v3.z);

            if (pt.x < xMin || pt.x > xMax ||
                pt.y < yMin || pt.y > yMax ||
                pt.z < zMin || pt.z > zMax)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check if a point is in the 3D bounding box from 3 corners of the triangle
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static bool IsInBoundingBox(float3 pt, float3 v1, float3 v2, float3 v3)
        {
            float xMin = math.min(v1.x, math.min(v2.x, v3.x));
            float xMax = math.max(v1.x, math.max(v2.x, v3.x));
            float yMin = math.min(v1.y, math.min(v2.y, v3.y));
            float yMax = math.max(v1.y, math.max(v2.y, v3.y));
            float zMin = math.min(v1.z, math.min(v2.z, v3.z));
            float zMax = math.max(v1.z, math.max(v2.z, v3.z));

            if (pt.x < xMin || pt.x > xMax ||
                pt.y < yMin || pt.y > yMax ||
                pt.z < zMin || pt.z > zMax)
            {
                return false;
            }
            return true;
        }

        // Reference https://stackoverflow.com/questions/39984709/how-can-i-check-wether-a-point-is-inside-the-circumcircle-of-3-points
        /// <summary>
        /// Check if a point is in the circumcircle from 3 corners of the triangle
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static bool IsInCircumcircle(Vector2 pt, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            Vector2 _1 = v1 - pt;
            Vector2 _2 = v2 - pt;
            Vector2 _3 = v3 - pt;
            return (_1.sqrMagnitude * (_2.x * _3.y - _2.y * _3.x) -
                    _2.sqrMagnitude * (_1.x * _3.y - _1.y * _3.x) +
                    _3.sqrMagnitude * (_1.x * _2.y - _1.y * _2.x)) > 0f;
        }

        /// <summary>
        /// Check if a point is in the circumcircle from 3 corners of the triangle
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static bool IsInCircumcircle(float2 pt, float2 v1, float2 v2, float2 v3)
        {
            float2 _1 = v1 - pt;
            float2 _2 = v2 - pt;
            float2 _3 = v3 - pt;
            return (math.lengthsq(_1) * (_2.x * _3.y - _2.y * _3.x) -
                    math.lengthsq(_2) * (_1.x * _3.y - _1.y * _3.x) +
                    math.lengthsq(_3) * (_1.x * _2.y - _1.y * _2.x)) > 0f;
        }

        /// <summary>
        /// Check if 3 points are in counter clockwise order
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static bool IsCounterClockwise(Vector2 v1, Vector2 v2, Vector2 v3)
        {
            return v1.x * v2.y - v3.x * v2.y + v1.y * v3.x - v1.x * v3.y + v2.x * v3.y - v3.x * v2.y > 0f;
        }

        /// <summary>
        /// Check if 3 points are in counter clockwise order
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static bool IsCounterClockwise(float2 v1, float2 v2, float2 v3)
        {
            return v1.x * v2.y - v3.x * v2.y + v1.y * v3.x - v1.x * v3.y + v2.x * v3.y - v3.x * v2.y > 0f;
        }

        // Reference https://en.wikipedia.org/wiki/Circumcircle#Cartesian_coordinates_2
        /// <summary>
        /// Return coordinates of the circumcircle of the triangle made 3 points
        /// </summary>
        /// <returns></returns>
        public static Vector2 CircumcenterCoordinates(Vector2 v1, Vector2 v2, Vector2 v3)
        {
            float denominator = 2 * (v1.x * (v2.y - v3.y) + v2.x * (v3.y - v1.y) + v3.x * (v1.y - v2.y));
            float x = ((v1.x * v1.x + v1.y * v1.y) * (v2.y - v3.y) +
                       (v2.x * v2.x + v2.y * v2.y) * (v3.y - v1.y) +
                       (v3.x * v3.x + v3.y * v3.y) * (v1.y - v2.y)) / denominator;
            float y = ((v1.x * v1.x + v1.y * v1.y) * (v3.x - v2.x) +
                       (v2.x * v2.x + v2.y * v2.y) * (v1.x - v3.x) +
                       (v3.x * v3.x + v3.y * v3.y) * (v2.x - v1.x)) / denominator;

            return new Vector2(x, y);
        }

        /// <summary>
        /// Return coordinates of the circumcircle of the triangle made 3 points
        /// </summary>
        /// <returns></returns>
        public static float2 CircumcenterCoordinates(float2 v1, float2 v2, float2 v3)
        {
            float denominator = 2 * (v1.x * (v2.y - v3.y) + v2.x * (v3.y - v1.y) + v3.x * (v1.y - v2.y));
            float x = ((v1.x * v1.x + v1.y * v1.y) * (v2.y - v3.y) +
                       (v2.x * v2.x + v2.y * v2.y) * (v3.y - v1.y) +
                       (v3.x * v3.x + v3.y * v3.y) * (v1.y - v2.y)) / denominator;
            float y = ((v1.x * v1.x + v1.y * v1.y) * (v3.x - v2.x) +
                       (v2.x * v2.x + v2.y * v2.y) * (v1.x - v3.x) +
                       (v3.x * v3.x + v3.y * v3.y) * (v2.x - v1.x)) / denominator;

            return new float2(x, y);
        }

        /// <summary>
        /// Return coordinates of the circumcircle of the triangle made 3 points
        /// </summary>
        /// <returns></returns>
        public static Vector3 CircumcenterCoordinates(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            float denominator = 2 * Vector3.Cross(v1 - v2, v2 - v3).sqrMagnitude;
            float a = (v2 - v3).sqrMagnitude * Vector3.Dot(v1 - v2, v1 - v3) / denominator;
            float b = (v3 - v1).sqrMagnitude * Vector3.Dot(v2 - v1, v2 - v3) / denominator;
            float c = (v1 - v2).sqrMagnitude * Vector3.Dot(v3 - v1, v3 - v2) / denominator;

            return a * v1 + b * v2 + c * v3;
        }

        /// <summary>
        /// Return coordinates of the circumcircle of the triangle made 3 points
        /// </summary>
        /// <returns></returns>
        public static float3 CircumcenterCoordinates(float3 v1, float3 v2, float3 v3)
        {
            float denominator = 2 * math.lengthsq(math.cross(v1 - v2, v2 - v3));
            float a = math.lengthsq(v2 - v3) * math.dot(v1 - v2, v1 - v3) / denominator;
            float b = math.lengthsq(v3 - v1) * math.dot(v2 - v1, v2 - v3) / denominator;
            float c = math.lengthsq(v1 - v2) * math.dot(v3 - v1, v3 - v2) / denominator;

            return a * v1 + b * v2 + c * v3;
        }
    }
}
