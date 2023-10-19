using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.CA.Scripting;
using VMS.DV.PD.Scripting;

namespace RAQAAnalysis.ViewModels
{
    public class AnalysisViewModel : BindableBase
    {
        public PlotModel GammaPlot {  get; set; }
		private string _results;

		public string Results
		{
			get { return _results; }
			set { SetProperty (ref _results, value); }
		}
		private double _doseDifference;
		private double _distanceToAgreement;
        public AnalysisViewModel(PortalDoseImage beam, PortalDoseImage reference)
        {
            GammaPlot = new PlotModel();
            _doseDifference = Convert.ToDouble(ConfigurationManager.AppSettings["DD"]);
            _distanceToAgreement = Convert.ToDouble(ConfigurationManager.AppSettings["DTA"]);
            PDTemplate template = new PDTemplate(false,false,false,false, AnalysisMode.CU,NormalizationMethod.MaxEachDose ,// if not relative , normalize 
                                                  true, 0.05, ROIType.None,0, _doseDifference/100.0, _distanceToAgreement,false, 
                                                  new List<EvaluationTestDesc>
                                                  {
                                                      new EvaluationTestDesc(EvaluationTestKind.GammaAreaLessThanOne,0.0,0.95,true)
                                                  });
            var analysis = beam.CreateTransientAnalysis(template, reference);
            Results = $"Analysis for {beam.Id} compared to {reference.Id}" +
                $"\nGamma Value = { analysis.EvaluationTests.First().TestValue*100.0:F2}" + 
                $"\nGamma Criteria = [{_doseDifference}%/{_distanceToAgreement}mm";
            SetGammaImage(analysis.GammaImage);
        }

        private void SetGammaImage(ImageRT gammaImage)
        {
            var image = gammaImage.FramesRT.First();
            ushort[,] pixels = new ushort[image.XSize, image.YSize];
            double[,] plotPixels = new double[image.XSize,image.YSize];
            image.GetVoxels(0, pixels); // writes all pixels values to "pixels" 
            // write all ushort values to double
            for(int i=0; i<image.XSize; i++) 
            {
                for (int j = 0; j < image.YSize; j++) 
                {
                    // convert raw pixel value to gamma imaage value. (i.e 1 is passing threshold)
                    double value = image.VoxelToDisplayValue(pixels[i,j]);
                    plotPixels[i,image.YSize-1-j] = value <1 ? value : value*100.0; // pour au niveau des couleur soit gamma sup de 1 la valeur sera +100( rouge) ou -100 (blue)

                }
            }
            GammaPlot.Axes.Add(new LinearColorAxis
            {
                Palette = OxyPalettes.Plasma(100),
                IsAxisVisible = false
            });
            HeatMapSeries hms = new HeatMapSeries() 
            {
                X0 = 0,X1=image.XSize,
                Y0 = 0, Y1 = image.YSize,
                RenderMethod = HeatMapRenderMethod.Bitmap,
                Data = plotPixels
            };
            GammaPlot.Series.Add(hms);
        }
    }
}
