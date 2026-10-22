using MathNet.Numerics.LinearAlgebra;
using MCFT.Core.Models;

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

        public static Vector<double> PrincipalConcStressVector(MCFTInput input, Vector<double> concPrincipalStrainVector, Vector<double> steelPrincipalStressVector, double thetaDegrees)
        {
            double concE = input.Ec;
            double fcPrime = input.FcPrime;
            double ft = input.Ft;
            double fyx = input.Fyx;
            double fyy = input.Fyy;
            double rhoX = input.RhoX;
            double rhoY = input.RhoY;
            double aggregateSize = input.AggregateSize;
            double crackSpacingX = input.CrackSpacingX;
            double crackSpacingY = input.CrackSpacingY;
            double epsilonCr = input.EpsilonCr;
            double epsilonNought = Math.Abs(input.EpsilonNought);

            double epsilon1 = concPrincipalStrainVector[0];
            double epsilon2 = concPrincipalStrainVector[1];
            double fsx = steelPrincipalStressVector[0];
            double fsy = steelPrincipalStressVector[1];
            double tanAbsThetaLimitMax = Math.Tan(89 * Math.PI / 180); // limit for tan(theta) to avoid singularity in calcs
            double tanAbsThetaLimitMin = Math.Tan(1 * Math.PI / 180); // limit for tan(theta) to avoid singularity in calcs

            double thetaRadians = thetaDegrees * (Math.PI / 180.0); // possible theta range -90° < theta <= 90°
            double cosTheta = Math.Cos(thetaRadians);
            double sinTheta = Math.Sin(thetaRadians);
            double tanTheta = Math.Tan(thetaRadians);
            double sinAbsTheta = Math.Abs(sinTheta);
            double tanAbsTheta = Math.Abs(tanTheta);
            double fc1;
            double fc2;
            double vci = 0.0; // shear stress on the crack surface, stays 0 unless the concrete is cracked in tension-compression

            if (epsilon1 >= 0 && epsilon2 >= 0)
            {
                if (epsilon1 >= epsilonCr) { fc1 = ft / (1 + Math.Sqrt(500 * epsilon1)); }
                else { fc1 = epsilon1 * concE; }

                if (epsilon2 >= epsilonCr) { fc2 = ft / (1 + Math.Sqrt(500 * epsilon2)); }
                else { fc2 = epsilon2 * concE; }
            }

            else if (epsilon1 >= 0 && epsilon2 < 0)
            {
                // Crack geometry and reinforcement capacity at the crack, only needed in this branch
                double crackSpacing = 1 / (sinAbsTheta / crackSpacingX + cosTheta / crackSpacingY);
                double crackWidth = crackSpacing * epsilon1;
                double f1cx = rhoX * (fyx - fsx); // residual capacity of the x-dir reinf that crosses a crack
                double f1cy = rhoY * (fyy - fsy); // residual capacity of the y-dir reinf that crosses a crack

                // Most shear stress the crack surface can carry (aggregate interlock, no compression across the crack)
                double vciMaxA = (0.18 * Math.Sqrt(fcPrime)) / (0.31 + (24 * crackWidth) / (aggregateSize + 16));
                double vciMaxB = Math.Abs(f1cx - f1cy) * cosTheta * sinAbsTheta;
                double vciMax = Math.Min(vciMaxA, vciMaxB);

                double f1_a; // tensile capacity of concrete
                if (epsilon1 >= epsilonCr) { f1_a = ft / (1 + Math.Sqrt(500 * epsilon1)); }
                else { f1_a = epsilon1 * concE; }

                double f1_b = f1cx * sinTheta * sinTheta + f1cy * cosTheta * cosTheta; // reserve capacity to biaxial yield of reinforcement that crosses a crack

                double f1_c; // limit due to yield of the x-dir reinf in conjuction with vciMax
                double f1_d; // limit due to yield of the y-dir reinf in conjuction with vciMax
                if (tanAbsTheta < tanAbsThetaLimitMin)
                {
                    f1_c = f1cx + vciMax / tanAbsThetaLimitMin;
                    f1_d = f1cy;
                }
                else if (tanAbsTheta > tanAbsThetaLimitMax)
                {
                    f1_c = f1cx;
                    f1_d = f1cy + vciMax * tanAbsThetaLimitMax;
                }
                else
                {
                    f1_c = f1cx + vciMax / tanAbsTheta;
                    f1_d = f1cy + vciMax * tanAbsTheta;
                }

                fc1 = Math.Min(Math.Min(f1_a, f1_b), Math.Min(f1_c, f1_d));
                fc2 = (-fcPrime * Math.Min(1 / (0.8 - 0.34 * epsilon1 / (-epsilonNought)), 1)) * (2 * (epsilon2 / (-epsilonNought)) - Math.Pow((epsilon2 / (-epsilonNought)), 2));

                // Shear on the crack that equilibrium requires for this fc1
                double tanClamped = Math.Clamp(tanAbsTheta, tanAbsThetaLimitMin, tanAbsThetaLimitMax);
                double deltaY = fc1 - f1cy; // > 0: y-steel at the crack would exceed yield without help from vci
                double deltaX = fc1 - f1cx; // > 0: x-steel at the crack would exceed yield without help from vci
                double vciA=0.0;
                double vciB=0.0;
                if (deltaY > 0) { vciA = deltaY / tanClamped; }
                if (deltaX > 0) { vciB = deltaX * tanClamped; }
                vci = Math.Max(vciA, vciB);
                // otherwise vci stays 0: the reinforcement at the crack carries the tension alone
            }

            else
            {
                fc1 = (-fcPrime * Math.Min(1 / (0.8 - 0.34 * epsilon2 / (-epsilonNought)), 1)) * (2 * (epsilon1 / (-epsilonNought)) - Math.Pow((epsilon1 / (-epsilonNought)), 2));
                fc2 = (-fcPrime * Math.Min(1 / (0.8 - 0.34 * epsilon1 / (-epsilonNought)), 1)) * (2 * (epsilon2 / (-epsilonNought)) - Math.Pow((epsilon2 / (-epsilonNought)), 2));
            }

            return Vector<double>.Build.DenseOfArray(new double[] { fc1, fc2, vci });
        }

        public static Vector<double> StressStateGlobal(Vector<double> concretePrincipalStress, Vector<double> steelPrincipalStress, double thetaDegrees, double rhoX, double rhoY)
        {
            double fc1 = concretePrincipalStress[0];
            double fc2 = concretePrincipalStress[1];
            double fsx = steelPrincipalStress[0];
            double fsy = steelPrincipalStress[1];
            double thetaRadians = thetaDegrees * (Math.PI / 180.0);
            double sin2xTheta = Math.Sin(2 * thetaRadians);
            double cos2xTheta = Math.Cos(2 * thetaRadians);

            double rad = (fc1 - fc2) / 2;
            double cen = (fc1 + fc2) / 2;

            double fx = cen - rad * cos2xTheta + rhoX * fsx;
            double fy = cen + rad * cos2xTheta + rhoY * fsy;
            double vxy = rad * sin2xTheta;
            return Vector<double>.Build.DenseOfArray(new double[] { fx, fy, vxy });
        }
    }
}