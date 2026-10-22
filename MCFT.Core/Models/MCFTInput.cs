
namespace MCFT.Core.Models
{
    public class MCFTInput
    {
        //Concrete, units MPa
        public double FcPrime { get; set; }
        public double Ft { get; set; }
        public double EpsilonNought { get; set; }
        public double EpsilonCr { get; set; }
        public double Ec {  get; set; }
        
        //Reinforcement, units MPa
        public double RhoX { get; set; }
        public double RhoY { get; set; }
        public double Fyx { get; set; }
        public double Fyy { get; set; }
        public double Es { get; set; }

        //Loads, units MPa
        public double Fx { get; set; }
        public double Fy { get; set; }
        public double Vxy { get; set; }

        //Additional Properties, units mm
        public double AggregateSize { get; set; }
        public double CrackSpacingX { get; set; }
        public double CrackSpacingY { get; set; }

        //Stress State Secant Solver Properties
        public double Tolerance { get; set; } = 0.001;
        public int MaxIterations { get; set; } = 1000;

        //Stress-Strain Curve Solver Properties
        public double LoadFactorIncrement { get; set; } = 0.05;
        public double MaxLoadFactor { get; set; } = 10.0;
        public int Refinements { get; set; } = 8;

        //Biaxial Failure Envelope Solver Properties




        public MCFTInput Clone() => (MCFTInput)MemberwiseClone();
    }
}
