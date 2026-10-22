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
        // ============== INPUT PROPERTIES ==============
        [ObservableProperty]
        private MCFTInput input = new MCFTInput
        {
            FcPrime = 21.8,
            Ft = 1.541,
            EpsilonNought = 0.002,
            EpsilonCr = 0.000071,
            Ec = 21800,
            RhoX = 0.02195,
            RhoY = 0.0,
            Fyx = 402,
            Fyy = 402,
            Es = 200000,
            Fx = 3.1,
            Fy = 0,
            Vxy = 1.0,
            AggregateSize = 19,
            CrackSpacingX = 115,
            CrackSpacingY = 115,
            Tolerance = 0.001,
            MaxIterations = 1000,
            LoadFactorIncrement = 0.05,
            MaxLoadFactor = 10.0,
            Refinements = 8
        };

        // ============== RESULT PROPERTIES ==============
        [ObservableProperty]
        private MCFTStressStateResult currentResult = new MCFTStressStateResult();

        // ============== CHART MODELS ==============
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
                var currentStateResult = stressStateSolver.Solve(solverInput);
                CurrentResult = currentStateResult;

                // Second: Run MCFTStressStrainCurveSolver to generate complete failure path
                var stressStrainSolver = new MCFTStressStrainCurveSolver();
                var curveResult = stressStrainSolver.Solve(solverInput);

                // Update all charts from the curve results
                UpdateStressStrainCharts(curveResult);
                UpdateBiaxialEnvelopeChart(curveResult);
            }
            catch (Exception ex)
            {
                // Clear charts on error
                ResetAllCharts();
                CurrentResult = new MCFTStressStateResult();
                System.Diagnostics.Debug.WriteLine($"Analysis Error: {ex.Message}");
            }
        }

        [RelayCommand]
        private void LoadDefaults()
        {
            Input = new MCFTInput
            {
                FcPrime = 21.8,
                Ft = 1.541,
                EpsilonNought = 0.002,
                EpsilonCr = 0.000071,
                Ec = 21800,
                RhoX = 0.02195,
                RhoY = 0.0,
                Fyx = 402,
                Fyy = 402,
                Es = 200000,
                Fx = 3.1,
                Fy = 0,
                Vxy = 1.0,
                AggregateSize = 19,
                CrackSpacingX = 115,
                CrackSpacingY = 115,
                Tolerance = 0.001,
                MaxIterations = 1000,
                LoadFactorIncrement = 0.05,
                MaxLoadFactor = 10.0,
                Refinements = 8
            };
        }

        [RelayCommand]
        private void Save()
        {
            // Placeholder for save functionality
            System.Diagnostics.Debug.WriteLine("Save functionality not yet implemented");
        }

        // ============== PRIVATE METHODS ==============
        private void UpdateStressStrainCharts(MCFTStressStrainCurveResult curveResult)
        {
            var data = curveResult.CurveResultData;
            if (data.Count == 0) return;

            // σX vs εX
            PlotModel_SigmaXvsEpsilonX = CreatePlotModel("σX vs εX", "εX (mm/mm)", "σX (MPa)",
                data.Select(d => new DataPoint(d.EpsilonX, d.SigmaX)).ToList());

            // σY vs εY
            PlotModel_SigmaYvsEpsilonY = CreatePlotModel("σY vs εY", "εY (mm/mm)", "σY (MPa)",
                data.Select(d => new DataPoint(d.EpsilonY, d.SigmaY)).ToList());

            // τXY vs γXY
            PlotModel_TauXYvsGammaXY = CreatePlotModel("τXY vs γXY", "γXY (mm/mm)", "τXY (MPa)",
                data.Select(d => new DataPoint(d.GammaXY, d.TauXY)).ToList());

            // fc₁ vs ε₁
            PlotModel_Fc1vsEpsilon1 = CreatePlotModel("fc₁ vs ε₁", "ε₁ (mm/mm)", "fc₁ (MPa)",
                data.Select(d => new DataPoint(d.Epsilon1, d.Fc1)).ToList());

            // fc₂ vs ε₂
            PlotModel_Fc2vsEpsilon2 = CreatePlotModel("fc₂ vs ε₂", "ε₂ (mm/mm)", "fc₂ (MPa)",
                data.Select(d => new DataPoint(d.Epsilon2, d.Fc2)).ToList());

            // Vci vs ε₁
            PlotModel_VcivsEpsilon1 = CreatePlotModel("Vci vs ε₁", "ε₁ (mm/mm)", "Vci (MPa)",
                data.Select(d => new DataPoint(d.Epsilon1, d.Vci)).ToList());

            // fsx vs εX
            PlotModel_FsxvsEpsilonX = CreatePlotModel("fsx vs εX", "εX (mm/mm)", "fsx (MPa)",
                data.Select(d => new DataPoint(d.EpsilonX, d.Fsx)).ToList());

            // fsy vs εY
            PlotModel_FsyvsEpsilonY = CreatePlotModel("fsy vs εY", "εY (mm/mm)", "fsy (MPa)",
                data.Select(d => new DataPoint(d.EpsilonY, d.Fsy)).ToList());
        }

        private void UpdateBiaxialEnvelopeChart(MCFTStressStrainCurveResult curveResult)
        {
            var data = curveResult.CurveResultData;
            if (data.Count == 0) return;

            var plotModel = new PlotModel { Title = "Biaxial Failure Envelope" };
            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "σX (MPa)" });
            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "σY (MPa)" });

            var series = new LineSeries
            {
                Title = "Envelope",
                Color = OxyColors.Blue,
                StrokeThickness = 2
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
                MarkerSize = 3,
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