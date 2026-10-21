using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCFT.Core.Models
{
    public class MCFTStressStrainCurveResult
    {
        public bool Converged { get; set; }
        public int LoadSteps { get; set; }
        public List<MCFTStressStateResult> Curve { get; } = new();
    }
}
