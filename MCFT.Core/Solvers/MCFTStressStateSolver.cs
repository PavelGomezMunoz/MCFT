using MCFT.Core.Calculators;
using MCFT.Core.Models;
using MCFT.Core.Utilities;
using MathNet.Numerics.LinearAlgebra;

namespace MCFT.Core.Solvers
{
    public class MCFTStressStateSolver
    {
        // Entry point for stress state solver, cold start and solves for 1.0 load factor
        public MCFTStressStateResult Solve(MCFTInput input)
        {
            return Solve(input, 1.0, new MCFTSolverState());
        }

        // Entry point for Stress-Strain Curve Solver. solves at (loadFactor x Loads in input) starting from "State"
        public MCFTStressStateResult Solve(MCFTInput input, double loadFactor, MCFTSolverState state)
        {
            // Solves stress state using iterative secant stiffness method to solve nonlinear system
            var stressStateResult = new MCFTStressStateResult();
            double error = double.MaxValue;
            int iteration = 0;
            Vector<double> stressVectorGlobal = loadFactor * Vector<double>.Build.DenseOfArray(new double[] { input.Fx, input.Fy, input.Vxy });

            while (error > input.Tolerance)
            {
                iteration++;
                //Calc stiffness matrix, note theta is the mirrored angle to the compression strut angle
                state.ThetaDegrees = StrainCalculator.PrincipalStrainAngleDegrees(state.StrainVectorGlobal);
                double psiDegrees = 180 - state.ThetaDegrees;
                Matrix<double> concStiffnessMatrixLocal = StiffnessCalculator.ConcreteStiffnessMatrixLocal(state.ConcPrincipalStressVector[0], state.ConcPrincipalStressVector[1], state.ConcPrincipalStrainVector[0], state.ConcPrincipalStrainVector[1], input.Ec);
                Matrix<double> steelStiffnessMatrixGlobal = StiffnessCalculator.SteelStiffnessMatrixGlobal(input.Es, input.RhoX, input.RhoY, state.StrainVectorGlobal[0], state.StrainVectorGlobal[1], input.Fyx, input.Fyy);
                Matrix<double> concStiffnessMatrixGlobal = MatrixUtilities.Rotate(concStiffnessMatrixLocal, psiDegrees);
                Matrix<double> combStiffnessMatrixGlobal = concStiffnessMatrixGlobal + steelStiffnessMatrixGlobal;

                // Solve for new strains and stresses
                state.StrainVectorGlobal = combStiffnessMatrixGlobal.Solve(stressVectorGlobal);
                state.ConcPrincipalStrainVector = StrainCalculator.PrincipalStrainVector(state.StrainVectorGlobal);
                state.ThetaDegrees = StrainCalculator.PrincipalStrainAngleDegrees(state.StrainVectorGlobal);
                state.SteelPrincipalStressVector = StressCalculator.PrincipalSteelStressVector(state.StrainVectorGlobal, input.Es, input.Fyx, input.Fyy);
                state.ConcPrincipalStressVector = StressCalculator.PrincipalConcStressVector(input, state.ConcPrincipalStrainVector, state.SteelPrincipalStressVector, state.ThetaDegrees);
                Vector<double> newStressVectorGlobal = StressCalculator.StressStateGlobal(state.ConcPrincipalStressVector, state.SteelPrincipalStressVector, state.ThetaDegrees, input.RhoX, input.RhoY);
                
                error = (newStressVectorGlobal - stressVectorGlobal).AbsoluteMaximum();
                if (double.IsNaN(error)) { break; }               // numerical blow-up: give up on this load
                if (iteration == input.MaxIterations) {break;}
            }
            //Write final result
            stressStateResult.Converged = error <= input.Tolerance;          // false if error is NaN
            stressStateResult.LoadFactor = loadFactor;
            stressStateResult.Iterations = iteration;
            stressStateResult.SigmaX = stressVectorGlobal[0];                // the applied stresses and resulting strains at this load level
            stressStateResult.SigmaY = stressVectorGlobal[1];
            stressStateResult.TauXY = stressVectorGlobal[2];
            stressStateResult.EpsilonX = state.StrainVectorGlobal[0];
            stressStateResult.EpsilonY = state.StrainVectorGlobal[1];
            stressStateResult.GammaXY = state.StrainVectorGlobal[2];
            stressStateResult.Epsilon1 = state.ConcPrincipalStrainVector[0];
            stressStateResult.Epsilon2 = state.ConcPrincipalStrainVector[1];
            stressStateResult.ThetaDegrees = state.ThetaDegrees;
            stressStateResult.Fc1 = state.ConcPrincipalStressVector[0];
            stressStateResult.Fc2 = state.ConcPrincipalStressVector[1];
            stressStateResult.Vci = state.ConcPrincipalStressVector[2];
            stressStateResult.Fsx = state.SteelPrincipalStressVector[0];
            stressStateResult.Fsy = state.SteelPrincipalStressVector[1];
            return stressStateResult;
        }
    }
}
