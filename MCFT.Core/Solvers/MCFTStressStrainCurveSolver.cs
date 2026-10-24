using MCFT.Core.Models;
using MCFT.Core.Calculators;

namespace MCFT.Core.Solvers
{
    public class MCFTStressStrainCurveSolver
    {
        private readonly MCFTStressStateSolver _stressStateSolver = new();
        public MCFTStressStrainCurveResult Solve(MCFTInput input)
        {
            var (loadFactorIncrement, maxLoadFactor) = InputCalcs.CurveSolverProperties(input.Fx, input.Fy, input.Vxy, input.NumberOfStepsInUnitStressVector, input.CurveMaxNumberOfSteps);
            int refinements = input.Refinements;
            var stressStrainCurveResult = new MCFTStressStrainCurveResult();
            var state = new MCFTSolverState();
            double lastGoodLoadFactor = 0.0;

            while (lastGoodLoadFactor < maxLoadFactor)
            {
                double trialLoadFactor = Math.Min(lastGoodLoadFactor + loadFactorIncrement, maxLoadFactor);

                MCFTSolverState trialState = state.Clone();
                MCFTStressStateResult stressStrainResultAtPoint = _stressStateSolver.Solve(input, trialLoadFactor, trialState);

                if (stressStrainResultAtPoint.Converged)
                {
                    state = trialState;      // accept the step
                    lastGoodLoadFactor = trialLoadFactor;
                    stressStrainCurveResult.CurveResultData.Add(stressStrainResultAtPoint);
                    continue;
                }

                double low = lastGoodLoadFactor;
                double high = trialLoadFactor;
                for (int i = 0; i < refinements; i++) // If did not converge, refine the steps by bisection
                {
                    double mid = 0.5 * (low + high);
                    MCFTSolverState midState = state.Clone();
                    MCFTStressStateResult midPoint = _stressStateSolver.Solve(input, mid, midState);
                    if (midPoint.Converged)
                    {
                        state = midState;
                        lastGoodLoadFactor = mid;
                        stressStrainCurveResult.CurveResultData.Add(midPoint);
                        continue;
                    }
                }

                lastGoodLoadFactor = low;
                stressStrainCurveResult.CapacityReached = true;
                break;
            }

            stressStrainCurveResult.CapacityLoadFactor = lastGoodLoadFactor;
            stressStrainCurveResult.LoadSteps = stressStrainCurveResult.CurveResultData.Count;
            stressStrainCurveResult.Converged = stressStrainCurveResult.CurveResultData.Count > 0;
            return stressStrainCurveResult;
        }
    }
}
