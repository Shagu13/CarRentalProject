using System.Resources;

namespace CarRentalPortal.Helpers
{
    public static class HelperService
    {
        public static string GetFullMessage(this Exception ex)
        {
            if (ex == null)
                return string.Empty;

            var messages = new List<string>();
            while (ex != null)
            {
                messages.Add($"[{ex.GetType().Name}] {ex.Message}");
                ex = ex.InnerException;
            }

            return string.Join(" => ", messages);
        }

        private static readonly ResourceManager _resourceManager =
            new ResourceManager("CarRentalPortal.Resource", typeof(HelperService).Assembly);

        // To get string messages from Resourse.resx
        public static string GetResourcevalue(string resourceKey, params object[] args) 

        {
            var value = _resourceManager.GetString(resourceKey);

            if (string.IsNullOrEmpty(value))
                return $"Missing resource for key: {resourceKey}";

            return string.Format(value, args);
        }
    }
}
