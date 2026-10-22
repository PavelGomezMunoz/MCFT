using MathNet.Numerics.LinearAlgebra;

namespace MCFT.Core.Calculators
{
    public static class InputCalcs
    {
        public static (double LoadFactorIncrement, double MaxLoadFactor) CurveSolverProperties(double sigmaX, double sigmaY, double tauXY, int numberOfStepsInUnitStressVector, int curveMaxNumberOfSteps)
        {
            Vector<double> stressVector = Vector<double>.Build.DenseOfArray(new double[] { sigmaX, sigmaY, tauXY });
            double stressVectorNorm = stressVector.L2Norm();
            Vector<double> stressUnitVector = stressVector / stressVectorNorm;
            double stressUnitVectorNorm = stressUnitVector.L2Norm();
            double stepSize = stressUnitVectorNorm / numberOfStepsInUnitStressVector;
            double LoadFactorIncrement = stepSize / stressVectorNorm;
            double MaxLoadFactor = curveMaxNumberOfSteps*stepSize / stressVectorNorm;
            return (LoadFactorIncrement, MaxLoadFactor);
        }
    }
}
