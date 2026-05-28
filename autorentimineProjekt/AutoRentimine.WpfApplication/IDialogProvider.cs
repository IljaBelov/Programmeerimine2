namespace AutoRentimine.WpfApplication
{
    public interface IDialogProvider
    {
        bool Confirm(string message);
        void ShowError(string error);
    }
}