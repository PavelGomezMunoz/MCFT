
namespace MCFT.Core.Models
{
    public class MCFTStressStateResult
    {
        public int Iterations { get; set; }
        public double SigmaX { get; set; }
        public double SigmaY { get; set; }
        public double TauXY { get; set; }
        public double EpsilonX { get; set; }
        public double EpsilonY { get; set; }
        public double GammaXY { get; set; }
    }
}
