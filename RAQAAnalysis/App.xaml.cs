using RAQAAnalysis.ViewModels;
using RAQAAnalysis.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using VMS.DV.PD.Scripting;
using pd = VMS.DV.PD.Scripting;

namespace RAQAAnalysis
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application //!! 
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try 
            {
                using (var pdApp = pd.Application.CreateApplication()) 
                {
                    VMS.DV.PD.UI.Base.VTransientImageDataMgr.CreateInstance(true);// important pour lecriture 
                    Patient patient = pdApp.OpenPatientById("23232323");
                    PDPlanSetup plan = patient.PDPlanSetups.First();
                    MainView mainView = new MainView();
                    MainViewModel mainViewModel = new MainViewModel(patient, plan);
                    mainView.DataContext = mainViewModel;
                    mainView.ShowDialog();
                }

            }
            catch (Exception ex)
            { 

            }

        }
    }
}
