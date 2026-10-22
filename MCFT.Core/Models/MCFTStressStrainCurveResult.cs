
namespace MCFT.Core.Models
{
    public class MCFTStressStrainCurveResult
    {
        public bool Converged { get; set; }              // true if at least the first load step solved
        public int LoadSteps { get; set; }               // number of converged points in Curve
        public double CapacityLoadFactor { get; set; }   // largest lambda that converged
        public bool CapacityReached { get; set; }        // true = a step failed (capacity found), false = stopped at maxLoadFactor first
        public List<MCFTStressStateResult> CurveResultData { get; } = new();
    }
}
