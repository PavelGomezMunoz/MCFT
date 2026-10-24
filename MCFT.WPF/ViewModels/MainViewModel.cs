using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MCFT.Core.Models;
using MCFT.Core.Solvers;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace MCFT.WPF.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        // Input Data Binded to the UI
        [ObservableProperty]
        private MCFTInput input = new MCFTInput
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

            Fx = 0.0,
            Fy = 2.867,
            Vxy = 0,

            AggregateSize = 19,
            CrackSpacingX = 110,
            CrackSpacingY = 125,

            Tolerance = 0.0001,
            MaxIterations = 1000,

            NumberOfStepsInUnitStressVector = 100,
            CurveMaxNumberOfSteps = 10000,
            Refinements = 10,

            BiaxialEnvelopeNumAngles = 360
        };

        // Result Data Binded to the UI
        [ObservableProperty]
        private MCFTStressStateResult currentResult = new MCFTStressStateResult();

        // Plots Binded to the UI
        [ObservableProperty]
        private PlotModel plotModel_SigmaXvsEpsilonX = new PlotModel { Title = "σX vs εX" };

        [ObservableProperty]
        private PlotModel plotModel_SigmaYvsEpsilonY = new PlotModel { Title = "σY vs εY" };

        [ObservableProperty]
        private PlotModel plotModel_TauXYvsGammaXY = new PlotModel { Title = "τXY vs γXY" };

        [ObservableProperty]
        private PlotModel plotModel_Fc1vsEpsilon1 = new PlotModel { Title = "fc₁ vs ε₁" };

        [ObservableProperty]
        private PlotModel plotModel_Fc2vsEpsilon2 = new PlotModel { Title = "fc₂ vs ε₂" };

        [ObservableProperty]
        private PlotModel plotModel_VcivsEpsilon1 = new PlotModel { Title = "Vci vs ε₁" };

        [ObservableProperty]
        private PlotModel plotModel_FsxvsEpsilonX = new PlotModel { Title = "fsx vs εX" };

        [ObservableProperty]
        private PlotModel plotModel_FsyvsEpsilonY = new PlotModel { Title = "fsy vs εY" };

        // Biaxial Envelope Result Data
        [ObservableProperty]
        private PlotModel plotModel_BiaxialEnvelope = new PlotModel { Title = "Biaxial Failure Envelope" };

        // ============== COMMANDS ==============
        [RelayCommand]
        private void RunAnalysis()
        {
            try
            {
                // Clone the input to preserve original
                var solverInput = Input.Clone();

                // First: Run MCFTStressStateSolver to get current stress state at LF=1.0
                var stressStateSolver = new MCFTStressStateSolver();
                var stressStateResult = stressStateSolver.Solve(solverInput);
                CurrentResult = stressStateResult;

                // Second: Run MCFTStressStrainCurveSolver to generate complete failure path
                var stressStrainCurveSolver = new MCFTStressStrainCurveSolver();
                var curveResult = stressStrainCurveSolver.Solve(solverInput);
                UpdateStressStrainCharts(curveResult);

                // Third: Run MCFTBiaxialFailureEnvelopeSolver to generate biaxial failure envelope
                var biaxialFailureEnvelopeSolver = new MCFTBiaxialFailureEnvelopeSolver();
                var biaxialResult = biaxialFailureEnvelopeSolver.Solve(solverInput);
                UpdateBiaxialEnvelopeChart(biaxialResult);
            }
            catch (Exception ex)
            {
                // Clear charts on error
                ResetAllCharts();
                CurrentResult = new MCFTStressStateResult();
                System.Diagnostics.Debug.WriteLine($"Analysis Run Error: {ex.Message}");
            }
        }



        // ============== PRIVATE METHODS ==============
        private void UpdateStressStrainCharts(MCFTStressStrainCurveResult curveResult)
        {
            var data = curveResult.CurveResultData;
            if (data.Count == 0) return;

            PlotModel_SigmaXvsEpsilonX = CreatePlotModel("σX vs εX", "εX (mm/mm)", "σX (MPa)",
                data.Select(d => new DataPoint(d.EpsilonX, d.SigmaX)).ToList());

            PlotModel_SigmaYvsEpsilonY = CreatePlotModel("σY vs εY", "εY (mm/mm)", "σY (MPa)",
                data.Select(d => new DataPoint(d.EpsilonY, d.SigmaY)).ToList());

            PlotModel_TauXYvsGammaXY = CreatePlotModel("τXY vs γXY", "γXY (mm/mm)", "τXY (MPa)",
                data.Select(d => new DataPoint(d.GammaXY, d.TauXY)).ToList());

            PlotModel_Fc1vsEpsilon1 = CreatePlotModel("fc₁ vs ε₁", "ε₁ (mm/mm)", "fc₁ (MPa)",
                data.Select(d => new DataPoint(d.Epsilon1, d.Fc1)).ToList());

            PlotModel_Fc2vsEpsilon2 = CreatePlotModel("fc₂ vs ε₂", "ε₂ (mm/mm)", "fc₂ (MPa)",
                data.Select(d => new DataPoint(d.Epsilon2, d.Fc2)).ToList());

            PlotModel_VcivsEpsilon1 = CreatePlotModel("Vci vs ε₁", "ε₁ (mm/mm)", "Vci (MPa)",
                data.Select(d => new DataPoint(d.Epsilon1, d.Vci)).ToList());

            PlotModel_FsxvsEpsilonX = CreatePlotModel("fsx vs εX", "εX (mm/mm)", "fsx (MPa)",
                data.Select(d => new DataPoint(d.EpsilonX, d.Fsx)).ToList());

            PlotModel_FsyvsEpsilonY = CreatePlotModel("fsy vs εY", "εY (mm/mm)", "fsy (MPa)",
                data.Select(d => new DataPoint(d.EpsilonY, d.Fsy)).ToList());
        }

        private void UpdateBiaxialEnvelopeChart(MCFTBiaxialFailureEnvelopeResult biaxialResult)
        {
            var data = biaxialResult.BiaxialResultData;
            if (data.Count == 0) return;

            var plotModel = new PlotModel { Title = "Biaxial Failure Envelope" , PlotType = PlotType.Cartesian, };
            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, PositionAtZeroCrossing = true, Title = "σX (MPa)" });
            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, PositionAtZeroCrossing = true, Title = "σY (MPa)" });

            var series = new LineSeries
            {
                Title = "Envelope",
                Color = OxyColors.Blue,
                StrokeThickness = 2,
                MarkerType = MarkerType.Circle,
                MarkerSize = 2,
                MarkerFill = OxyColors.Blue
            };

            foreach (var point in data)
            {
                series.Points.Add(new DataPoint(point.SigmaX, point.SigmaY));
            }

            plotModel.Series.Add(series);
            PlotModel_BiaxialEnvelope = plotModel;
        }

        private PlotModel CreatePlotModel(string title, string xAxisTitle, string yAxisTitle, List<DataPoint> points)
        {
            var plotModel = new PlotModel { Title = title };
            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = xAxisTitle });
            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = yAxisTitle });

            var series = new LineSeries
            {
                Color = OxyColors.Blue,
                StrokeThickness = 2,
                MarkerType = MarkerType.Circle,
                MarkerSize = 2,
                MarkerFill = OxyColors.Blue
            };

            foreach (var point in points)
            {
                series.Points.Add(point);
            }

            plotModel.Series.Add(series);
            return plotModel;
        }

        private void ResetAllCharts()
        {
            PlotModel_SigmaXvsEpsilonX = new PlotModel { Title = "σX vs εX" };
            PlotModel_SigmaYvsEpsilonY = new PlotModel { Title = "σY vs εY" };
            PlotModel_TauXYvsGammaXY = new PlotModel { Title = "τXY vs γXY" };
            PlotModel_Fc1vsEpsilon1 = new PlotModel { Title = "fc₁ vs ε₁" };
            PlotModel_Fc2vsEpsilon2 = new PlotModel { Title = "fc₂ vs ε₂" };
            PlotModel_VcivsEpsilon1 = new PlotModel { Title = "Vci vs ε₁" };
            PlotModel_FsxvsEpsilonX = new PlotModel { Title = "fsx vs εX" };
            PlotModel_FsyvsEpsilonY = new PlotModel { Title = "fsy vs εY" };
            PlotModel_BiaxialEnvelope = new PlotModel { Title = "Biaxial Failure Envelope" };
        }
    }
}