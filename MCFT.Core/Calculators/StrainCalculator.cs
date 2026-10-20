using MathNet.Numerics.LinearAlgebra;

namespace MCFT.Core.Calculators
{
    public static class StrainCalculator
    {
        public static Vector<double> PrincipalStrainVector(Vector<double> strainVector)
        {
            double ex = strainVector[0];
            double ey = strainVector[1];
            double gammaXY = strainVector[2];

            double avg = (ex + ey) / 2.0;
            double radius = Math.Sqrt(Math.Pow((ex - ey) / 2.0, 2) + Math.Pow(gammaXY / 2.0, 2));
            double e1 = avg + radius;
            double e2 = avg - radius;

            return Vector<double>.Build.DenseOfArray(new double[] { e1, e2, 0 });
        }

        public static double PrincipalStrainAngleDegrees(Vector<double> strainVector)
        {
            double ex = strainVector[0];
            double ey = strainVector[1];
            double gammaXY = strainVector[2];
            double angle = 0.5 * Math.Atan2(gammaXY, ey - ex);
            return angle * (180.0 / Math.PI);
        }

        public static Vector<double> RotateStrainVector(Vector<double> strainVector, double angleDegrees)
        {
            double angleRadians = angleDegrees * (Math.PI / 180.0);
            double cosAngle = Math.Cos(angleRadians);
            double sinAngle = Math.Sin(angleRadians);
            double ex = strainVector[0];
            double ey = strainVector[1];
            double gammaXY = strainVector[2];
            double exPrime = ex * cosAngle * cosAngle + ey * sinAngle * sinAngle + gammaXY * sinAngle * cosAngle;
            double eyPrime = ex * sinAngle * sinAngle + ey * cosAngle * cosAngle - gammaXY * sinAngle * cosAngle;
            double gammaXYPrime = -2 * (ex - ey) * sinAngle * cosAngle + gammaXY * (cosAngle * cosAngle - sinAngle * sinAngle);
            return Vector<double>.Build.DenseOfArray(new double[] { exPrime, eyPrime, gammaXYPrime });
        }




    }
}
