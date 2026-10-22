
namespace MCFT.Core.Models
{
    public class MCFTBiaxialFailureEnvelopeResult
    {
        public List<(double SigmaX, double SigmaY)> BiaxialResultData { get; set; } = new();
    }
}
