using System;
using System.ComponentModel;

namespace TestBenchTarget.UWP.Models
{
    public class DataItem : INotifyPropertyChanged
    {
        private DateTime dateColumnValue = DateTime.Now;
        private string procedureColumnValue = string.Empty;
        private int pointsColumnValue;
        private string delegateColumnValue = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public DateTime DateColumnValue
        {
            get => dateColumnValue;
            set
            {
                if (dateColumnValue != value.Date)
                {
                    dateColumnValue = value.Date;
                    OnPropertyChanged(nameof(DateColumnValue));
                }
            }
        }

        public string ProcedureColumnValue
        {
            get => procedureColumnValue;
            set
            {
                if (procedureColumnValue != value)
                {
                    procedureColumnValue = value;
                    OnPropertyChanged(nameof(ProcedureColumnValue));
                }
            }
        }

        public int PointsColumnValue
        {
            get => pointsColumnValue;
            set
            {
                if (pointsColumnValue != value)
                {
                    pointsColumnValue = value;
                    OnPropertyChanged(nameof(PointsColumnValue));
                }
            }
        }

        public string DelegateColumnValue
        {
            get => delegateColumnValue;
            set
            {
                if (delegateColumnValue != value)
                {
                    delegateColumnValue = value;
                    OnPropertyChanged(nameof(DelegateColumnValue));
                }
            }
        }
    }
}
