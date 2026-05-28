using System.Windows;

namespace AutoRentimine.WpfApplication
{
    public class DialogProvider : IDialogProvider
    {
        public bool Confirm(string message)
        {
            var result = MessageBox.Show(
                message,
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );
            return result == MessageBoxResult.Yes;
        }

        public void ShowError(string error)
        {
            MessageBox.Show(
                error,
                "Ошибка",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }
    }
}