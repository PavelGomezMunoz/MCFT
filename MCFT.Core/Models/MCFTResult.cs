
namespace MCFT.Core.Models
{
    public class MCFTResult
    {
        public bool Converged { get; set; }
        public int Iterations { get; set; }
        public double EpsilonX { get; set; }
        public double EpsilonY { get; set; }
        public double GammaXY { get; set; }
        public List<IterationResult> History { get; } = new();
    }
}
