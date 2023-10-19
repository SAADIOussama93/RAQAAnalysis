using Prism.Commands;
using Prism.Mvvm;
using RAQAAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.DV.PD.Scripting;

namespace RAQAAnalysis.ViewModels
{
    public class MainViewModel:BindableBase
    {
        public List <SessionModel> Sessions { get; set; }
		private SessionModel _selectedSession;

		public SessionModel SelectedSession
		{
			get { return _selectedSession; }
			set 
			{ 
				SetProperty(ref _selectedSession, value);
			}
		}
		public DelegateCommand AnalyzeCommand { get; set; }
		public DelegateCommand PrintCommand { get; set;}
		// An Observablecollection is a list that notifies the view xhan something has been added 
		public ObservableCollection<AnalysisViewModel> AnalysisItems { get; set; }

        private Patient _patient;
        private PDPlanSetup _pdPlanSetup;

        public MainViewModel(Patient patient, PDPlanSetup pdPlanSetup)
        {
			Sessions = new List<SessionModel>();
			AnalysisItems = new ObservableCollection<AnalysisViewModel>();
			_patient=patient;
			_pdPlanSetup = pdPlanSetup;
			AnalyzeCommand = new DelegateCommand(OnAnalyze);
			SetSession();
        }

        private void SetSession()
        {
            foreach(var session in _pdPlanSetup.Sessions) 
			{
				SessionModel s = new SessionModel();
				s.Id = session.Id;
				s.CreationDate = session.SessionDate;
				s.SessionDetails = $"{s.Id} - {s.CreationDate.ToShortDateString()}";
				Sessions.Add(s);
			}
        }

        private void OnAnalyze()
        {
            var session = _pdPlanSetup.Sessions.First(x =>x.Id == SelectedSession.Id);
			// TODO check to see if multiple sessions can have th same ID ? 
			foreach(var image in session.PortalDoseImages) 
			{
				PDBeam beam = image.PDBeam; 
				if (beam.Id.Contains("Dosimetry"))
				{
					PortalDoseImage reference = session.PortalDoseImages.First(i => i.PDBeam.Id == "Dosimetry0");
					AnalysisItems.Add(new AnalysisViewModel(image, reference));
				}
				else if (beam.Id.Contains("PF"))
				{
                    PortalDoseImage reference = session.PortalDoseImages.First(i => i.PDBeam.Id == "PF0");
                    AnalysisItems.Add(new AnalysisViewModel(image, reference));

                }
			}
        }
    }
}
