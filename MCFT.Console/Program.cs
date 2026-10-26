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

    Fx = 1.434,
    Fy = -2.868,
    Vxy = 0,

    AggregateSize = 19,
    CrackSpacingX = 110,
    CrackSpacingY = 125,

    Tolerance = 0.001,
    MaxIterations = 1000,

    NumberOfStepsInUnitStressVector = 500,
    CurveMaxNumberOfSteps = 50000,
    Refinements = 10,

    BiaxialEnvelopeNumAngles = 360
};


var stressStateSolver = new MCFTStressStateSolver();
var stressStateResult = stressStateSolver.Solve(input);
Console.WriteLine($"Converged: {stressStateResult.Converged}");
Console.WriteLine($"LoadFactor: {stressStateResult.LoadFactor}");
Console.WriteLine($"Iterations: {stressStateResult.Iterations}");
Console.WriteLine($"SigmaX: {stressStateResult.SigmaX}");
Console.WriteLine($"SigmaY: {stressStateResult.SigmaY}");
Console.WriteLine($"TauXY: {stressStateResult.TauXY}");
Console.WriteLine($"EpsilonX: {Math.Round(stressStateResult.EpsilonX*1000,3)} mm/m");
Console.WriteLine($"EpsilonY: {Math.Round(stressStateResult.EpsilonY*1000,3)} mm/m");
Console.WriteLine($"GammaXY: {Math.Round(stressStateResult.GammaXY*1000,3)} mm/m");
Console.WriteLine($"Epsilon1: {Math.Round(stressStateResult.Epsilon1 * 1000, 3)} mm/m");
Console.WriteLine($"Epsilon2: {Math.Round(stressStateResult.Epsilon2 * 1000, 3)} mm/m");
Console.WriteLine($"Theta: {Math.Round(stressStateResult.ThetaDegrees, 3)} deg");
Console.WriteLine($"Fc1: {Math.Round(stressStateResult.Fc1, 3)} MPa");
Console.WriteLine($"Fc2: {Math.Round(stressStateResult.Fc2, 3)} MPa");
Console.WriteLine($"Vci: {Math.Round(stressStateResult.Vci, 3)} MPa");
Console.WriteLine($"Fsx: {Math.Round(stressStateResult.Fsx, 3)} MPa");
Console.WriteLine($"Fsy: {Math.Round(stressStateResult.Fsy, 3)} MPa");


var stressStrainCurveSolver = new MCFTStressStrainCurveSolver();
var stressStrainCurveResult = stressStrainCurveSolver.Solve(input);
Console.WriteLine("Step  Converged LoadFactor Iterations SigmaX(MPa) SigmaY(MPa) Tau(MPa) EpsX(mm/m)  EpsY(mm/m) Gamma(mm/m) Epsilon1(mm/m) Epsilon2(mm/m) Theta(deg) Fc1(MPa) Fc2(MPa) Vci(MPa) Fsx(MPa) Fsy(MPa)");
for (int i = 0; i < stressStrainCurveResult.CurveResultData.Count; i++)
{
    var p = stressStrainCurveResult.CurveResultData[i];
    Console.WriteLine($"{i + 1,1} , {p.Converged,8:F3} , {p.LoadFactor,7:F3} , {p.Iterations,6}, {p.SigmaX,13:F3} , {p.SigmaY,9:F3} , {p.TauXY,7:F3} , {p.EpsilonX * 1000,7:F3} , {p.EpsilonY * 1000,9:F3} , {p.GammaXY * 1000,9:F3}, {p.Epsilon1 * 1000,10:F3} , {p.Epsilon2 * 1000,12:F3} , {p.ThetaDegrees,12:F3} , {p.Fc1,6:F3} , {p.Fc2,6:F3} , {p.Vci,6:F3} , {p.Fsx,6:F3} , {p.Fsy,6:F3}");
}


//var biaxialFailureEnvelopeSolver = new MCFTBiaxialFailureEnvelopeSolver();
//var biaxialFailureEnvelopeResult = biaxialFailureEnvelopeSolver.Solve(input);