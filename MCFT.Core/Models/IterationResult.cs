
namespace MCFT.Core.Models
{
    public class IterationResult
    {
        public int Iteration { get; set; }
        public double Fx { get; set; }
        public double Fy { get; set; }
        public double Vxy { get; set; }
        public double Error { get; set; }
    }
}
