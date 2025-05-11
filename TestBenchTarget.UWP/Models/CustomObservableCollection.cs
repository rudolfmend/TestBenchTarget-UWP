using System.Collections.ObjectModel;

namespace TestBenchTarget.UWP.Models
{
    public class CustomObservableCollection<T> : ObservableCollection<T>
    {
        // Zabezpečiť aby kolekcia nebola prázdna hneď na začiatku
        public CustomObservableCollection()
        {
            // Prázdny konštruktor
        }

        protected override void InsertItem(int index, T item)
        {
            try
            {
                // Vždy vložiť na začiatok namiesto zadaného indexu
                base.InsertItem(0, item);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in InsertItem: {ex.Message}");
                // Ak všetko zlyhá, pokúste sa vložiť na štandardný index
                base.InsertItem(index, item);
            }
        }
    }
}
