namespace EPOv2.ViewModels
{
    public class SupplierViewModel<T>
    {
        public T Id { get; set; }

        public string FullName { get; set; }

        public string Code { get; set; }
    }

    public class SupplierViewModel : SupplierViewModel<int>{}
}