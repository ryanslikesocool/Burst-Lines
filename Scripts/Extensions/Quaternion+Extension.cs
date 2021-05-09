// Made with <3 by Ryan Boyer http://ryanjboyer.com

using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace BurstLines
{
    public static partial class Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 toEulerAngles(this quaternion q)
        {
            float3 angles;

            float sinr_cosp = 2 * (q.value.w * q.value.x + q.value.y * q.value.z);
            float cosr_cosp = 1 - 2 * (q.value.x * q.value.x + q.value.y * q.value.y);
            angles.x = math.atan2(sinr_cosp, cosr_cosp);

            float sinp = 2 * (q.value.w * q.value.y - q.value.z * q.value.x);
            if (math.abs(sinp) >= 1)
            {
                angles.y = math.abs(math.PI * 0.5f) * math.sign(sinp);
            }
            else
            {
                angles.y = math.asin(sinp);
            }

            float siny_cosp = 2 * (q.value.w * q.value.z + q.value.x * q.value.y);
            float cosy_cosp = 1 - 2 * (q.value.y * q.value.y + q.value.z * q.value.z);
            angles.z = math.atan2(siny_cosp, cosy_cosp);

            return angles;
        }
    }
}