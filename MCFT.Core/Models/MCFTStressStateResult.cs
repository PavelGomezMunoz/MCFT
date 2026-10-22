
namespace MCFT.Core.Models
{
    public class MCFTStressStateResult
    {
        public bool Converged { get; set; }
        public double LoadFactor { get; set; }
        public int Iterations { get; set; }
        public double SigmaX { get; set; } // stresses
        public double SigmaY { get; set; }
        public double TauXY { get; set; }
        public double EpsilonX { get; set; } // strains
        public double EpsilonY { get; set; }
        public double GammaXY { get; set; }
        public double Epsilon1 { get; set; } // principal strains
        public double Epsilon2 { get; set; }
        public double ThetaDegrees { get; set; } // principal strain angle
        public double Fc1 { get; set; }   // concrete principal stresses
        public double Fc2 { get; set; }
        public double Vci { get; set; }
        public double Fsx { get; set; }   // steel stresses
        public double Fsy { get; set; }

    }
}
