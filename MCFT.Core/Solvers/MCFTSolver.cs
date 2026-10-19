
using MCFT.Core.Calculators;
using MCFT.Core.Models;
using MCFT.Core.Utilities;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace MCFT.Core.Solvers

{
    public class MCFTSolver
    {
        public MCFTResult Solve(MCFTInput input)
        {
            var result = new MCFTResult();
            double error = double.MaxValue;
            int iteration = 0;
            double thetaDegrees = 0;
            double psiDegrees = 0;
            Vector<double> strainVectorGlobal = Vector<double>.Build.DenseOfArray(new double[] { 0.0, 0.0, 0.00001 });
            Vector<double> stressVectorGlobal = Vector<double>.Build.DenseOfArray(new double[] { input.Fx, input.Fy, input.Vxy });
            Vector<double> concPrincipalStrainVector = Vector<double>.Build.Dense(3);
            Vector<double> concPrincipalStressVector = Vector<double>.Build.Dense(3);
            Vector<double> steelPrincipalStressVector = Vector<double>.Build.Dense(3);
            Vector<double> newStressVectorGlobal = Vector<double>.Build.Dense(3);
            Matrix<double> concStiffnessMatrixLocal = Matrix<double>.Build.Dense(3, 3);
            Matrix<double> concStiffnessMatrixGlobal = Matrix<double>.Build.Dense(3, 3);
            Matrix<double> steelStiffnessMatrixGlobal = Matrix<double>.Build.Dense(3, 3);
            Matrix<double> combStiffnessMatrixGlobal = Matrix<double>.Build.Dense(3, 3);


            while (error > input.Tolerance)
            {
                iteration++;
                //Note theta is the mirrored angle to the compression strut angle
                thetaDegrees = StrainCalculator.PrincipalStrainAngleDegrees(strainVectorGlobal);
                psiDegrees = 180 - thetaDegrees;
                concStiffnessMatrixLocal = StiffnessCalculator.ConcreteStiffnessMatrixLocal(concPrincipalStressVector[0], concPrincipalStressVector[1], concPrincipalStrainVector[0], concPrincipalStrainVector[1], input.Ec);
                steelStiffnessMatrixGlobal = StiffnessCalculator.SteelStiffnessMatrixGlobal(input.Es, input.RhoX, input.RhoY, strainVectorGlobal[0], strainVectorGlobal[1], input.Fyx, input.Fyy);
                concStiffnessMatrixGlobal = MatrixUtilities.Rotate(concStiffnessMatrixLocal, psiDegrees);

                combStiffnessMatrixGlobal = concStiffnessMatrixGlobal + steelStiffnessMatrixGlobal;

                // Solve for new strains and stresses
                strainVectorGlobal = combStiffnessMatrixGlobal.Solve(stressVectorGlobal);

                concPrincipalStrainVector = StrainCalculator.PrincipalStrainVector(strainVectorGlobal);
                thetaDegrees = StrainCalculator.PrincipalStrainAngleDegrees(strainVectorGlobal);

                steelPrincipalStressVector = StressCalculator.PrincipalSteelStressVector(strainVectorGlobal, input.Es, input.Fyx, input.Fyy);
                concPrincipalStressVector = StressCalculator.PrincipalConcStressVector(input.Ec, input.FcPrime, input.Fcr, input.Fyx, input.Fyy, input.RhoX, input.RhoY, input.AggregateSize, input.CrackSpacingX, input.CrackSpacingY, input.EpsilonCr, input.EpsilonNought, concPrincipalStrainVector, steelPrincipalStressVector, thetaDegrees);
                newStressVectorGlobal = StressCalculator.StressStateGlobal(concPrincipalStressVector, steelPrincipalStressVector, thetaDegrees, input.RhoX, input.RhoY);

                error = (newStressVectorGlobal - stressVectorGlobal).AbsoluteMaximum();

                result.History.Add(new IterationResult
                {
                    Iteration = iteration,
                    Fx = newStressVectorGlobal[0],
                    Fy = newStressVectorGlobal[1],
                    Vxy = newStressVectorGlobal[2],
                    Error = error
                });

                if (iteration == input.MaxIterations) {break;}
            }

            result.Converged = error <= input.Tolerance;
            result.Iterations = iteration;
            result.EpsilonX = strainVectorGlobal[0];
            result.EpsilonY = strainVectorGlobal[1];
            result.GammaXY = strainVectorGlobal[2];

            return result;
        }
    }
}
