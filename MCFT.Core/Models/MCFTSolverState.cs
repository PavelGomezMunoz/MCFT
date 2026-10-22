using MathNet.Numerics.LinearAlgebra;

namespace MCFT.Core.Models
{
    // Everything the secant iteration remembers between iterations and between load steps when tracing a curve
    public class MCFTSolverState
    {
        public Vector<double> StrainVectorGlobal { get; set; }
        public Vector<double> ConcPrincipalStrainVector { get; set; }
        public double ThetaDegrees { get; set; }
        public Vector<double> ConcPrincipalStressVector { get; set; }
        public Vector<double> SteelPrincipalStressVector { get; set; }

        // Constructor - Cold start state for the secant iteration
        public MCFTSolverState()
        {
            StrainVectorGlobal = Vector<double>.Build.DenseOfArray(new double[] { 0.0, 0.0, 0.00001 });
            ConcPrincipalStrainVector = Vector<double>.Build.Dense(3);
            ThetaDegrees = 0.0;
            ConcPrincipalStressVector = Vector<double>.Build.Dense(3);
            SteelPrincipalStressVector = Vector<double>.Build.Dense(3);
        }

        // Independent copy such that a failed attempt will not damage the last good state
        public MCFTSolverState Clone()
        {
            return new MCFTSolverState
            {
                StrainVectorGlobal = StrainVectorGlobal.Clone(),
                ConcPrincipalStrainVector = ConcPrincipalStrainVector.Clone(),
                ThetaDegrees = ThetaDegrees,
                ConcPrincipalStressVector = ConcPrincipalStressVector.Clone(),
                SteelPrincipalStressVector = SteelPrincipalStressVector.Clone()
            };
        }

    }
}
