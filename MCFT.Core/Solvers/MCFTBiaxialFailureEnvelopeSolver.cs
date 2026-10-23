
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

                Console.WriteLine($"Angle: {angle * 180 / Math.PI,5:F1}° , Unit Forces: ({currentInput.Fx,2:F3}, {currentInput.Fy,2:F3}) , Capacity: ({sigmaXAtCapacity,2:F3}, {sigmaYAtCapacity,2:F3})");

                biaxialFailureEnvelopeResult.BiaxialResultData.Add((sigmaXAtCapacity, sigmaYAtCapacity));
            }
            return biaxialFailureEnvelopeResult;
        }
    }
}
