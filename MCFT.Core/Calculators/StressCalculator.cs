using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace MCFT.Core.Calculators
{
    public static class StressCalculator
    {
        public static Vector<double> PrincipalSteelStressVector(Vector<double> globalStrainVector, double Es, double fyx, double fyy)
        {
            double ex = globalStrainVector[0];
            double ey = globalStrainVector[1];
            double fsx;
            double fsy;

            if (ex > 0) { fsx = Math.Min(Es * ex, fyx); }
            else { fsx = Math.Max(Es * ex, -fyx); }

            if (ey > 0) { fsy = Math.Min(Es * ey, fyy); }
            else { fsy = Math.Max(Es * ey, -fyy); }

            return Vector<double>.Build.DenseOfArray(new double[] { fsx, fsy, 0 });
        }

        public static Vector<double> PrincipalConcStressVector(double concE, double fcPrime, double fcr, double fyx, double fyy, double rhoX, double rhoY, double aggregateSize, double crackSpacingX, double crackSpacingY, double epsilonCr, double epsilonNought, Vector<double> concPrincipalStrainVector, Vector<double> steelPrincipalStressVector, double thetaDegrees)
        {
            double epsilon1 = concPrincipalStrainVector[0];
            double epsilon2 = concPrincipalStrainVector[1];
            double fsx = steelPrincipalStressVector[0];
            double fsy = steelPrincipalStressVector[1];
            
            double thetaRadians = Math.Abs(thetaDegrees) * (Math.PI / 180.0); //Absolute value avoids negative angles which can produce negative crack spacings, crack widths and negative vciMaxB
            double cosTheta = Math.Cos(thetaRadians);
            double sinTheta = Math.Sin(thetaRadians);
            double tanTheta = Math.Tan(thetaRadians);
            double crackSpacing = 1/(sinTheta/crackSpacingX + cosTheta/crackSpacingY);
            double crackWidth = crackSpacing * epsilon1;
            double f1cx = rhoX * (fyx - fsx);
            double f1cy = rhoY * (fyy - fsy);
            double vciMaxA = (0.18 * Math.Sqrt(fcPrime)) / (0.31 + (24 * crackWidth) / (aggregateSize + 16));
            double vciMaxB = Math.Abs(f1cx - f1cy) * cosTheta * sinTheta;
            double vci = Math.Min(vciMaxA, vciMaxB);

            double f1_a;
            if (epsilon1 >= epsilonCr) { f1_a = fcr / (1 + Math.Sqrt(500 * epsilon1)); }
            else { f1_a = epsilon1 * concE; }

            double f1_b = f1cx * sinTheta * sinTheta + f1cy * cosTheta * cosTheta;
            double f1_c = f1cx + vci / tanTheta;
            double f1_d = f1cy + vci * tanTheta;
            double fc1 = Math.Min(Math.Min(f1_a, f1_b), Math.Min(f1_c, f1_d));

            double fc2Max = -fcPrime * Math.Min(1 / (0.8 - 0.34 * epsilon1/(-epsilonNought)), 1);
            double fc2 = fc2Max * (2 * (epsilon2 / (-epsilonNought)) - Math.Pow((epsilon2 / (-epsilonNought)), 2));
            return Vector<double>.Build.DenseOfArray(new double[] { fc1, fc2, 0.0 });
        }

        public static Vector<double> StressStateGlobal(Vector<double> concretePrincipalStress, Vector<double> steelPrincipalStress, double thetaDegrees, double rhoX, double rhoY)
        {
            double fc1 = concretePrincipalStress[0];
            double fc2 = concretePrincipalStress[1];
            double fsx = steelPrincipalStress[0];
            double fsy = steelPrincipalStress[1];
            double thetaRadians = thetaDegrees * (Math.PI / 180.0);
            double sin2xTheta = Math.Sin(2*thetaRadians);
            double cos2xTheta = Math.Cos(2*thetaRadians);

            double rad = (fc1 - fc2) / 2;
            double cen = (fc1 + fc2) / 2;

            double fx = cen-rad*cos2xTheta + rhoX*fsx;
            double fy = cen+rad*cos2xTheta + rhoY*fsy;
            double vxy = rad*sin2xTheta;
            return Vector<double>.Build.DenseOfArray(new double[] { fx, fy, vxy });
        }
    }
}
