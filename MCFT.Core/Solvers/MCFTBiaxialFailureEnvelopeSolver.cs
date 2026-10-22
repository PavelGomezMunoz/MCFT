
using MCFT.Core.Models;

namespace MCFT.Core.Solvers
{
    public class MCFTBiaxialFailureEnvelopeSolver
    {
        private readonly MCFTStressStrainCurveSolver _stressStrainCurveSolver = new();
        public MCFTBiaxialFailureEnvelopeResult Solve(MCFTInput input)
        {
            var biaxialFailureEnvelopeResult = new MCFTBiaxialFailureEnvelopeResult();
            int numAngles = input.BiaxialEnvelopeNumAngles;
            
            for (int i = 0; i < (numAngles+1); i++)
            {
                double angle = i * (2*Math.PI / numAngles); // Angle in radians
                double unitVectorComponentSigmaX = Math.Cos(angle);
                double unitVectorComponentSigmaY = Math.Sin(angle);
                MCFTInput currentInput = input.Clone();
                currentInput.Fx = unitVectorComponentSigmaX;
                currentInput.Fy = unitVectorComponentSigmaY;
                currentInput.Vxy = 0.0;

                MCFTStressStrainCurveResult curveResultAtAngle = _stressStrainCurveSolver.Solve(currentInput);
                double sigmaXAtCapacity = curveResultAtAngle.CapacityReached ? curveResultAtAngle.CurveResultData.Last().SigmaX : double.NaN;
                double sigmaYAtCapacity = curveResultAtAngle.CapacityReached ? curveResultAtAngle.CurveResultData.Last().SigmaY : double.NaN;

                biaxialFailureEnvelopeResult.BiaxialResultData.Add((sigmaXAtCapacity, sigmaYAtCapacity));
            }
            return biaxialFailureEnvelopeResult;
        }
    }
}
