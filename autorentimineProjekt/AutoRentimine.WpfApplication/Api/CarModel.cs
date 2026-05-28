namespace AutoRentimine.WpfApplication.Api
{
    public class CarModel : NotifyPropertyChangedBase
    {
        private int _id;
        private string _mark = string.Empty;
        private string _model = string.Empty;
        private string _registrationNumber = string.Empty;
        private decimal _dailyRate;
        private string _status = string.Empty;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Mark
        {
            get => _mark;
            set => SetProperty(ref _mark, value);
        }

        public string Model
        {
            get => _model;
            set => SetProperty(ref _model, value);
        }

        public string RegistrationNumber
        {
            get => _registrationNumber;
            set => SetProperty(ref _registrationNumber, value);
        }

        public decimal DailyRate
        {
            get => _dailyRate;
            set => SetProperty(ref _dailyRate, value);
        }

        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }
    }
}