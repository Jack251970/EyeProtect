namespace Project1.UI.Controls
{
    public static class ToastManager
    {
        private static Project1UIToast toast_;

        public static void Add(Project1UIToast toast)
        {
            toast_?.Hide();
            toast_ = toast;
        }

    }
}
