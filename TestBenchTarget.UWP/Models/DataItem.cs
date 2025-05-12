using System;
using System.ComponentModel;

namespace TestBenchTarget.UWP.Models
{
    public class DataItem : INotifyPropertyChanged
    {
        private DateTime _dateColumnValue = DateTime.Now;
        private string _procedureColumnValue = string.Empty;
        private int _pointsColumnValue;
        private string _delegateColumnValue = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public DateTime DateColumnValue
        {
            get => _dateColumnValue;
            set
            {
                if (_dateColumnValue != value.Date)
                {
                    _dateColumnValue = value.Date;
                    OnPropertyChanged(nameof(DateColumnValue));
                }
            }
        }

        public string ProcedureColumnValue
        {
            get => _procedureColumnValue;
            set
            {
                if (_procedureColumnValue != value)
                {
                    _procedureColumnValue = value;
                    OnPropertyChanged(nameof(ProcedureColumnValue));
                }
            }
        }

        public int PointsColumnValue
        {
            get => _pointsColumnValue;
            set
            {
                if (_pointsColumnValue != value)
                {
                    _pointsColumnValue = value;
                    OnPropertyChanged(nameof(PointsColumnValue));
                }
            }
        }

        public string DelegateColumnValue
        {
            get => _delegateColumnValue;
            set
            {
                if (_delegateColumnValue != value)
                {
                    _delegateColumnValue = value;
                    OnPropertyChanged(nameof(DelegateColumnValue));
                }
            }
        }
    }
}
