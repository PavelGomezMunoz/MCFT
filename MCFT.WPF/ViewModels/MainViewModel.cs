using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MCFT.Core.Models;
using MCFT.Core.Solvers;

namespace MCFT.WPF.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        //Input
        [ObservableProperty]
        private double fcPrime = 20;
        [ObservableProperty]
        private double rhoX = 0.02;








        //Output
        [ObservableProperty]
        private double epsilonX;
        [ObservableProperty]
        private double epsilonY;

        [ObservableProperty]
        private double gammaXY;







        //Commands
        [RelayCommand]
        private void RunAnalysis()
        {
            var input = new MCFTInput
            {
                FcPrime = FcPrime,
                RhoX = RhoX
            };

            var stressStateSolver = new MCFTStressStateSolver();
            var stressStateResult = stressStateSolver.Solve(input);
            EpsilonX = stressStateResult.EpsilonX;
            EpsilonY = stressStateResult.EpsilonY;
            GammaXY = stressStateResult.GammaXY;
        }

    }
}
