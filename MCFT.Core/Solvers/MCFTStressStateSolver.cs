using MCFT.Core.Calculators;
using MCFT.Core.Models;
using MCFT.Core.Utilities;
using MathNet.Numerics.LinearAlgebra;

namespace MCFT.Core.Solvers
{
    public class MCFTStressStateSolver
    {
        public MCFTStressStateResult Solve(MCFTInput input)
        {
            // Solves stress state using itirative secant stiffness method to solve nonlinear system
            var result = new MCFTStressStateResult();
            double error = double.MaxValue;
            int iteration = 0;
            Vector<double> stressVectorGlobal = Vector<double>.Build.DenseOfArray(new double[] { input.Fx, input.Fy, input.Vxy });
            Vector<double> strainVectorGlobal = Vector<double>.Build.DenseOfArray(new double[] { 0.0, 0.0, 0.00001 });
            Vector<double> concPrincipalStressVector = Vector<double>.Build.Dense(3);
            Vector<double> concPrincipalStrainVector = Vector<double>.Build.Dense(3);

            while (error > input.Tolerance)
            {
                iteration++;
                //Calc stiffness matrix, note theta is the mirrored angle to the compression strut angle
                double thetaDegrees = StrainCalculator.PrincipalStrainAngleDegrees(strainVectorGlobal);
                double psiDegrees = 180 - thetaDegrees;
                Matrix<double> concStiffnessMatrixLocal = StiffnessCalculator.ConcreteStiffnessMatrixLocal(concPrincipalStressVector[0], concPrincipalStressVector[1], concPrincipalStrainVector[0], concPrincipalStrainVector[1], input.Ec);
                Matrix<double> steelStiffnessMatrixGlobal = StiffnessCalculator.SteelStiffnessMatrixGlobal(input.Es, input.RhoX, input.RhoY, strainVectorGlobal[0], strainVectorGlobal[1], input.Fyx, input.Fyy);
                Matrix<double> concStiffnessMatrixGlobal = MatrixUtilities.Rotate(concStiffnessMatrixLocal, psiDegrees);
                Matrix<double> combStiffnessMatrixGlobal = concStiffnessMatrixGlobal + steelStiffnessMatrixGlobal;

                // Solve for new strains and stresses
                strainVectorGlobal = combStiffnessMatrixGlobal.Solve(stressVectorGlobal);
                concPrincipalStrainVector = StrainCalculator.PrincipalStrainVector(strainVectorGlobal);
                thetaDegrees = StrainCalculator.PrincipalStrainAngleDegrees(strainVectorGlobal);
                Vector<double> steelPrincipalStressVector = StressCalculator.PrincipalSteelStressVector(strainVectorGlobal, input.Es, input.Fyx, input.Fyy);
                concPrincipalStressVector = StressCalculator.PrincipalConcStressVector(input, concPrincipalStrainVector, steelPrincipalStressVector, thetaDegrees);
                Vector<double> newStressVectorGlobal = StressCalculator.StressStateGlobal(concPrincipalStressVector, steelPrincipalStressVector, thetaDegrees, input.RhoX, input.RhoY);
                
                error = (newStressVectorGlobal - stressVectorGlobal).AbsoluteMaximum();
                if (iteration == input.MaxIterations) {break;}
            }
            //Write final result
            result.Iterations = iteration;
            result.EpsilonX = strainVectorGlobal[0];
            result.EpsilonY = strainVectorGlobal[1];
            result.GammaXY = strainVectorGlobal[2];

            return result;
        }
    }
}
