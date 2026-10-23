using MCFT.Core.Models;
using MCFT.Core.Solvers;

var input = new MCFTInput
{
    FcPrime = 21.7,
    Ft = 1.537,
    EpsilonNought = 0.002,
    EpsilonCr = 0.0000708,
    Ec = 21700,
    RhoX = 0.01785,
    RhoY = 0.00885,
    Fyx = 441,
    Fyy = 324,
    Es = 200000,

    Fx = 7.87,
    Fy = 2.87,
    Vxy = 0,

    AggregateSize = 19,
    CrackSpacingX = 110,
    CrackSpacingY = 125,

    Tolerance = 0.01,
    MaxIterations = 1000,

    NumberOfStepsInUnitStressVector = 5,
    CurveMaxNumberOfSteps = 1000,
    Refinements = 10,

    BiaxialEnvelopeNumAngles = 36
};

var stressStateSolver = new MCFTStressStateSolver();
var stressStateResult = stressStateSolver.Solve(input);
Console.WriteLine($"Iterations: {stressStateResult.Iterations}");
Console.WriteLine($"EpsilonX: {Math.Round(stressStateResult.EpsilonX*1000,3)} mm/m");
Console.WriteLine($"EpsilonY: {Math.Round(stressStateResult.EpsilonY*1000,3)} mm/m");
Console.WriteLine($"GammaXY: {Math.Round(stressStateResult.GammaXY*1000,3)} mm/m");

var stressStrainCurveSolver = new MCFTStressStrainCurveSolver();
var stressStrainCurveResult = stressStrainCurveSolver.Solve(input);
Console.WriteLine("Step  Converged LoadFactor Iterations SigmaX(MPa) SigmaY(MPa) Tau(MPa) EpsX(mm/m)  EpsY(mm/m) Gamma(mm/m) Epsilon1(mm/m) Epsilon2(mm/m) Theta(deg) Fc1(MPa) Fc2(MPa) Vci(MPa) Fsx(MPa) Fsy(MPa)");
for (int i = 0; i < stressStrainCurveResult.CurveResultData.Count; i++)
{
    var p = stressStrainCurveResult.CurveResultData[i];
    Console.WriteLine($"{i + 1,1} , {p.Converged,8:F3} , {p.LoadFactor,7:F3} , {p.Iterations,6}, {p.SigmaX,16:F3} , {p.SigmaY,9:F3} , {p.TauXY,9:F3} , {p.EpsilonX * 1000,9:F3} , {p.EpsilonY * 1000,9:F3} , {p.GammaXY * 1000,9:F3}, {p.Epsilon1 * 1000,9:F3} , {p.Epsilon2 * 1000,9:F3} , {p.ThetaDegrees,9:F3} , {p.Fc1,9:F3} , {p.Fc2,9:F3} , {p.Vci,9:F3} , {p.Fsx,9:F3} , {p.Fsy,9:F3}");
}
//Console.WriteLine($"Capacity load factor: {stressStrainCurveResult.CapacityLoadFactor:F3} (capacity reached: {stressStrainCurveResult.CapacityReached})");


//var biaxialFailureEnvelopeSolver = new MCFTBiaxialFailureEnvelopeSolver();
//var biaxialFailureEnvelopeResult = biaxialFailureEnvelopeSolver.Solve(input);